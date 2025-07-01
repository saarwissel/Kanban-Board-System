using Kanban.Backend.BusinessLayer;
using Kanban.Backend.ServiceLayer;
using log4net;
using log4net.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Kanban.Backend.DataAccessLayer;
using System.Reflection.Metadata;
namespace Kanban.Backend.BusinessLayer
{

    public class BoardFacade
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int BoardID;
        public Dictionary<int, BoardBL> Boards_dict;// boardID, BoardBL
        public int taskID;
        public AuthenticationFacade authFacade;
        public UserFacade uf;
        public BoardController boardController;
        public ColumnController ColumnController;
        public TaskController TaskController;
        public BoardsUsersController BoardsUsersController;



        public BoardFacade(AuthenticationFacade af, UserFacade userFacade)
        {
            this.Boards_dict = new Dictionary<int, BoardBL>();
            this.uf = userFacade;
            this.authFacade = af;
            this.taskID = 0;
            this.BoardID = -1;
            this.boardController = new BoardController();
            this.ColumnController = new ColumnController();
            this.TaskController = new TaskController();
            this.BoardsUsersController = new BoardsUsersController();
        }


        /// <summary>
        /// Loads all board-related data from the database, including users, boards, columns, tasks, and board members.
        /// </summary>
        /// <returns>True if loading is successful, otherwise throws exception.</returns>
        public bool LoadData()
        {
            try
            {
                Log.Info("Starting data load from the database...");

                // Step 1: load demo data
                


                // Step 2: Load users from database
                List<UserDAL> usersDAL = this.uf.LoadData();
                if (usersDAL == null)
                {
                    Log.Warn("No users found in the database.");
                    throw new Exception("No users found in the database.");
                }


                if (usersDAL.Count == 0)
                {
                    return true;
                }

                // Step 3: For each user, load their boards and related data
                foreach (var userDAL in usersDAL)
                {
                    List<BoardDAL> boardDALs = boardController.SelectBoardsByUser(userDAL.Email);

                    foreach (var boardDAL in boardDALs)
                    {
                        if (!Boards_dict.ContainsKey(boardDAL.ID))
                        {
                            // Create board
                            var board = new BoardBL(boardDAL.OwnerEmail, boardDAL.Name, "", boardDAL.ID, "");
                            Boards_dict[boardDAL.ID] = board;
                            Log.Info($"Loaded board {boardDAL.ID} for user {boardDAL.OwnerEmail}.");

                            // Add board to owner's list
                            if (uf.Users.ContainsKey(boardDAL.OwnerEmail))
                                uf.Users[boardDAL.OwnerEmail].addBoardToList(board);

                            // Step 4: Load column limits
                            List<ColmunsDAL> columns = ColumnController.Select(boardDAL.ID);
                            foreach (var col in columns)
                            {
                                int limit = (col.LimitCol == null || col.LimitCol < 0) ? -1 : (int)col.LimitCol;

                                if (col.ColumnID == "backlog")
                                    board.Backlog.ColumnLimit = limit;
                                else if (col.ColumnID == "in progress")
                                    board.InProgress.ColumnLimit = limit;
                                else
                                    board.Done.ColumnLimit = limit;
                            }


                            // Step 5: Load tasks
                            var tasks = TaskController.SelectTasksByBoard(boardDAL.ID);
                            foreach (var taskDAL in tasks)
                            {
                                var taskBL = new TaskBL(taskDAL.Id, taskDAL.TaskStatus, DateTime.Parse(taskDAL.DueDate),
                                                        taskDAL.Title, taskDAL.Description, boardDAL.ID, "", taskDAL.Assignee, "");
                                taskBL.taskDAL = taskDAL;

                                if (taskDAL.TaskStatus == "backlog")
                                    board.Backlog.Tasks.Add(taskBL);
                                else if (taskDAL.TaskStatus == "in progress")
                                    board.InProgress.Tasks.Add(taskBL);
                                else if (taskDAL.TaskStatus == "done")
                                {
                                    taskBL.isDone = true;
                                    board.Done.Tasks.Add(taskBL);
                                }

                                board.TaskID = Math.Max(board.TaskID, taskBL.taskId + 1);
                            }

                            // Step 6: Load board members
                            List<string> boardMembers = BoardsUsersController.SelectUsersInBoard(boardDAL.ID);
                            foreach (var userEmail in boardMembers)
                            {
                                if (uf.Users.ContainsKey(userEmail))
                                {
                                    var user = uf.Users[userEmail];
                                    board.addMem(user);
                                    user.addBoardToList(board);
                                }
                            }

                            Log.Info($"Finished loading data for board ID {boardDAL.ID}.");
                        }
                    }
                }

                //load the correct boardID
                if (Boards_dict.Keys.Any())
                    this.BoardID = Boards_dict.Keys.Max();
                else
                    this.BoardID = -1;

                Log.Info("Data loaded successfully from the database.");
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Error loading boards data: " + e.Message);
                throw new Exception("Error loading boards data: " + e.Message);
            }
        }

        /// <summary>
        /// Deletes all in-memory and persistent board-related data, including users, boards, tasks, columns, and board-user associations.
        /// </summary>
        /// <returns>True if deletion was successful; otherwise, throws an exception.</returns>
        public bool DeleteData()
        {
            try
            {
                Log.Info("Starting deletion of all board-related data...");

                // Step 1: Delete user data from memory and DB
                uf.DeleteData();
                Log.Info("User data deleted successfully.");

                // Step 2: Clear list of logged-in users
                authFacade.ClearLoggedInUsers();
                Log.Info("Logged-in users cleared.");

                // Step 3: Clear in-memory board data
                Boards_dict = new Dictionary<int, BoardBL>();
                BoardID = -1;
                Log.Info("In-memory board dictionary and ID reset.");

                // Step 4: Delete persistent board data
                boardController.DeleteAll();
                Log.Info("All boards deleted from database.");

                // Step 5: Delete persistent tasks
                bool deleteTask = TaskController.DeleteAll();
                Log.Info("All tasks deleted from database.");

                // Step 6: Delete persistent column definitions
                bool deleteColumn = ColumnController.DeleteAll();
                Log.Info("All columns deleted from database.");

                // Step 7: Delete persistent board-user relations
                bool deleteBoardsUsers = BoardsUsersController.DeleteAll();
                Log.Info("All board-user relations deleted from database.");

                Log.Info("All board-related data deleted successfully.");
                return true;
            }
            catch (Exception e)
            {
                Log.Error("Error deleting boards data: " + e.Message);
                throw new Exception("Error deleting boards data: " + e.Message);
            }
        }

        /// <summary>
        /// Creates a new board for a logged-in user.
        /// </summary>
        /// <param name="email">Owner's email</param>
        /// <param name="boardName">Name of the new board</param>
        public void CreateBoard(string email, string boardName)//mabe need to add colmuns adding data base 
        {
            try
            {
                Log.Info($"Attempting to create board '{boardName}' for user '{email}'.");

                // Step 1: Validate user exists and is logged in
                UserExistOrLoggedIn(email);
                var user = uf.Users[email];

                // Step 2: Check if user already has a board with the same name
                user.isBoardNameExists(boardName);

                // Step 3: Generate new board ID
                this.BoardID++;

                // Step 4: Create board object and update data structures
                BoardBL board = new BoardBL(email, boardName, "", BoardID);
                user.addBoardToList(board);
                Boards_dict.Add(BoardID, board);
                board.GetMembers().Add(user); // Add owner as the first member
                //board.addMember(user); // Also adds to BoardsUsersController

                Log.Info($"Board '{boardName}' (ID: {BoardID}) created successfully for user '{email}'.");
            }
            catch (Exception e)
            {
                Log.Error($"Error creating board '{boardName}' for user '{email}': {e.Message}");
                throw new Exception($"Error creating board: {e.Message}");
            }
        }

        /// <summary>
        /// Deletes a board owned by the specified user, along with its tasks and DB entries.
        /// </summary>
        /// <param name="email">Owner's email</param>
        /// <param name="boardName">Name of the board to delete</param>
        public void DeleteBoard(string email, string boardName)
        {
            try
            {
                Log.Info($"Attempting to delete board '{boardName}' for user '{email}'.");

                // Step 1: Validate user is logged in
                UserExistOrLoggedIn(email);
                var user = uf.Users[email];

                // Step 2: Get the board and verify ownership
                BoardBL board = user.getBoardByName(boardName);
                board.isOwner(email);

                // Step 3: Delete tasks from DB
                bool deleteSuccess = TaskController.deleteTasksFromBoard(board.BoardID);
                if (!deleteSuccess && TaskController.HasTasksForBoard(board.BoardID))
                {
                    Log.Error("Failed to delete tasks from the board.");
                    throw new Exception("Failed to delete tasks from the board.");
                }

                // Step 4: Delete board from DB
                BoardDAL boardDAL = new BoardDAL(board.BoardID, boardName, email, "");
                if (!boardDAL.deleteBoard())
                {
                    Log.Error("Failed to delete board from the database.");
                    throw new Exception("Failed to delete board from the database.");
                }

                // Step 5: Remove board from in-memory structures
                user.removeBoardFromList(board);
                Boards_dict.Remove(board.getBoardID());
                board.deleteMe(); // Clean references if needed

                Log.Info($"Board '{boardName}' deleted successfully.");
            }
            catch (Exception e)
            {
                Log.Error($"Error deleting board '{boardName}' for user '{email}': {e.Message}");
                throw new Exception($"Error deleting board: {e.Message}");
            }
        }

        /// <summary>
        /// Sets a task limit for a specific column in a board.
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="boardName">Board name</param>
        /// <param name="columnOrdinal">Ordinal index of the column (0-2)</param>
        /// <param name="limit">Limit to set</param>
        /// <returns>The updated BoardBL object</returns>
        public BoardBL LimitColumn(string email, string boardName, int columnOrdinal, int limit)
        {
            try
            {
                Log.Info($"Attempting to limit column {columnOrdinal} to {limit} tasks in board '{boardName}' for user '{email}'.");

                // Step 1: Validate user
                UserExistOrLoggedIn(email);
                var user = uf.Users[email];

                // Step 2: Get board and apply limit
                var board = user.getBoardByName(boardName);
                board.SetColumnLimit(columnOrdinal, limit);

                Log.Info($"Successfully set column limit to {limit} for column {columnOrdinal} in board '{boardName}'.");

                return board;
            }
            catch (Exception e)
            {
                Log.Error($"Error limiting column {columnOrdinal} in board '{boardName}': {e.Message}");
                throw new Exception($"Error limiting column: {e.Message}");
            }
        }

        /// <summary>
        /// Retrieves the limit of a specific column in a board.
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="boardName">Board name</param>
        /// <param name="columnOrdinal">Column index (0-2)</param>
        /// <returns>Column limit</returns>
        public int GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            try
            {
                Log.Info($"Getting column limit for column {columnOrdinal} in board '{boardName}'.");

                // Step 1: Validate user
                UserExistOrLoggedIn(email);
                var user = uf.Users[email];

                // Step 2: Retrieve board and get limit
                var board = user.getBoardByName(boardName);
                int limit = board.GetColumnLimit(columnOrdinal);

                Log.Info($"Column {columnOrdinal} in board '{boardName}' has limit {limit}.");

                return limit;
            }
            catch (Exception e)
            {
                Log.Error($"Error getting column limit for column {columnOrdinal} in board '{boardName}': {e.Message}");
                throw new Exception($"Error getting column limit: {e.Message}");
            }
        }
        /// <summary>
        /// Retrieves the name of a column given its ordinal index.
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="boardName">Board name</param>
        /// <param name="columnOrdinal">Ordinal index of the column (0-2)</param>
        /// <returns>Column name</returns>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        {
            try
            {
                Log.Info($"Getting column name for column {columnOrdinal} in board '{boardName}'.");

                // Step 1: Validate user
                UserExistOrLoggedIn(email);
                var user = uf.Users[email];

                // Step 2: Retrieve board and column name
                var board = user.getBoardByName(boardName);
                string name = board.GetColumnName(columnOrdinal);

                if (string.IsNullOrEmpty(name))
                {
                    Log.Warn($"Column name for column {columnOrdinal} in board '{boardName}' is empty.");
                    throw new Exception("Column name is not set.");
                }

                Log.Info($"Column name for column {columnOrdinal} in board '{boardName}' is '{name}'.");

                return name;
            }
            catch (Exception e)
            {
                Log.Error($"Error getting column name for column {columnOrdinal} in board '{boardName}': {e.Message}");
                throw new Exception($"Error getting column name: {e.Message}");
            }
        }

        /// <summary>
        /// Adds a new task to the backlog column of a board.
        /// </summary>
        public TaskBL AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            try
            {
                // Step 1: Validate inputs
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(boardName) || string.IsNullOrWhiteSpace(title))
                {
                    Log.Warn("AddTask failed: Email, board name or title is null or empty.");
                    throw new Exception("Email, board name or title is null or empty.");
                }

                // Step 2: Check user session
                UserExistOrLoggedIn(email);

                // Step 3: Retrieve board and add task
                var user = uf.Users[email];
                var board = user.getBoardByName(boardName);
                var task = board.AddTask(title, description, dueDate);

                Log.Info($"Task added successfully to board '{boardName}' by user '{email}'.");

                return task;
            }
            catch (Exception e)
            {
                Log.Error($"Error in AddTask for board '{boardName}': {e.Message}");
                throw new Exception($"Error adding task: {e.Message}");
            }
        }

        /// <summary>
        /// Updates the due date of a task in a board.
        /// </summary>
        public void UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            try
            {
                Log.Info($"Updating due date for task {taskId} in board '{boardName}', column {columnOrdinal} by '{email}'.");

                // Step 1: Validate user and get board
                UserExistOrLoggedIn(email);
                var user = uf.Users[email];
                var board = user.getBoardByName(boardName);

                // Step 2: Get the task and update due date
                var task = board.GetTask(columnOrdinal, taskId);
                if (task.isDone)
                {
                    throw new Exception("task in done cannot update");
                }
                task.isAssignUser(email);
                task.setDueDate(dueDate);

                Log.Info($"Due date updated for task {taskId} to {dueDate:yyyy-MM-dd}.");
            }
            catch (Exception e)
            {
                Log.Error($"Error updating due date for task {taskId}: {e.Message}");
                throw new Exception($"Error updating due date: {e.Message}");
            }
        }

        /// <summary>
        /// Updates the title of a task.
        /// </summary>
        public void UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title)
        {
            try
            {
                Log.Info($"Updating title for task {taskId} in board '{boardName}', column {columnOrdinal}.");

                // Step 1: Validate session
                UserExistOrLoggedIn(email);
                var user = uf.Users[email];
                var board = user.getBoardByName(boardName);

                // Step 2: Update title
                var task = board.GetTask(columnOrdinal, taskId);
                if (task.isDone)
                {
                    throw new Exception("task in done cannot update");
                }
                task.isAssignUser(email);
                task.setTitle(title);

                Log.Info($"Task {taskId} title updated to '{title}'.");
            }
            catch (Exception e)
            {
                Log.Error($"Error updating title for task {taskId}: {e.Message}");
                throw new Exception($"Error updating task title: {e.Message}");
            }
        }

        /// <summary>
        /// Updates the description of a task.
        /// </summary>
        public void UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string description)
        {
            try
            {
                Log.Info($"Updating description for task {taskId} in board '{boardName}', column {columnOrdinal}.");

                // Step 1: Authenticate and get board
                UserExistOrLoggedIn(email);
                var user = uf.Users[email];
                var board = user.getBoardByName(boardName);

                // Step 2: Update task description
                var task = board.GetTask(columnOrdinal, taskId);
                if (task.isDone)
                {
                    throw new Exception("task in done cannot update");
                }
                task.isAssignUser(email);
                task.setDescription(description);

                Log.Info($"Task {taskId} description updated successfully.");
            }
            catch (Exception e)
            {
                Log.Error($"Error updating description for task {taskId}: {e.Message}");
                throw new Exception($"Error updating task description: {e.Message}");
            }
        }

        /// <summary>
        /// Advances a task to the next column if the user is the assignee or no assignee is set.
        /// </summary>
        public TaskBL AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            try
            {
                Log.Info($"User '{email}' attempts to advance task {taskId} in board '{boardName}' from column {columnOrdinal}");

                // Step 1: Validate user is logged in
                UserExistOrLoggedIn(email);

                // Step 2: Get board and task
                var user = uf.Users[email];
                var board = user.getBoardByName(boardName);

                // Step 3: Check if columnOrdinal is valid for advancing
                if (columnOrdinal < 0 || columnOrdinal > 1)
                    throw new Exception("Cannot advance task from this column.");

                var task = board.GetTask(columnOrdinal, taskId);
                if (task.isDone)
                {
                    throw new Exception("task in done cannot update");
                }
                task.isAssignUser(email);

                // Step 4: Validate assignee
                if (task.emailAssignee == null || task.emailAssignee.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    board.AdvanceTask(task, columnOrdinal);
                    return task;
                    Log.Info($"Task {taskId} successfully advanced by '{email}'");
                }
                else
                {
                    throw new Exception("Only the assignee can advance this task.");
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error advancing task {taskId}: {e.Message}");
                throw new Exception($"Error advancing task: {e.Message}");
            }
        }

        /// <summary>
        /// Retrieves all tasks in a specific column.
        /// </summary>
        public TaskBL[] GetColumn(string email, string boardName, int columnOrdinal)
        {
            try
            {
                Log.Info($"Fetching tasks for user '{email}' in board '{boardName}', column {columnOrdinal}");

                // Step 1: Authenticate user
                UserExistOrLoggedIn(email);

                // Step 2: Get board and tasks
                var user = uf.Users[email];
                var board = user.getBoardByName(boardName);
                List<TaskBL> tasks = board.GetColumn(columnOrdinal);

                Log.Info($"Retrieved {tasks.Count} tasks from column {columnOrdinal} of board '{boardName}'");
                return tasks.ToArray();
            }
            catch (Exception e)
            {
                Log.Error($"Error fetching tasks from column {columnOrdinal}: {e.Message}");
                throw new Exception($"Error fetching column: {e.Message}");
            }
        }

        /// <summary>
        /// Returns all tasks that are currently in 'In Progress' state across all boards of a user.
        /// </summary>
        public TaskBL[] InProgressTasks(string email)
        {
            try
            {
                Log.Info($"Fetching in-progress tasks for user '{email}'");

                // Step 1: Authenticate user
                UserExistOrLoggedIn(email);

                var user = uf.Users[email];
                List<TaskBL> tasks = new List<TaskBL>();

                // Step 2: Collect all tasks from 'In Progress' column (ordinal 1) in each board
                foreach (BoardBL board in user.Boards)
                {
                    var inProgressTasks = board.GetColumn(1);
                    tasks.AddRange(inProgressTasks);
                }

                Log.Info($"User '{email}' has {tasks.Count} tasks in progress.");
                return tasks.ToArray();
            }
            catch (Exception e)
            {
                Log.Error($"Error retrieving in-progress tasks for user '{email}': {e.Message}");
                throw new Exception($"Error retrieving in-progress tasks: {e.Message}");
            }
        }

        /// <summary>
        /// Allows a user to join an existing board by ID.
        /// </summary>
        public void JoinBoard(string email, int boardID)
        {
            try
            {
                Log.Info($"User '{email}' attempting to join board with ID {boardID}.");

                // Step 1: Validate user and board existence
                UserExistOrLoggedIn(email);
                isBoardIdExistsOrValid(boardID);

                var user = uf.Users[email];
                user.isBoardIdExists(boardID); // Validates user's permission
                var board = Boards_dict[boardID];


                // Step 2: Check if user is already a member of the board
                if (user.hasNameBoard(board.GetName()))
                {
                    Log.Warn($"User '{email}' is already a member of board {boardID}.");
                    throw new Exception("User is already a member of this board.");
                }


                // Step 2: Add user to the board and update user record
                board.addMember(user);
                user.addBoardToList(board);

                Log.Info($"User '{email}' successfully joined board {boardID}.");
            }
            catch (Exception e)
            {
                Log.Error($"Error joining board {boardID} by user '{email}': {e.Message}");
                throw new Exception($"Error joining board: {e.Message}");
            }
        }

        /// <summary>
        /// Allows a user to leave a board they are a member of, unless they are the board's owner.
        /// </summary>
        public void LeaveBoard(string email, int boardID)
        {
            try
            {
                Log.Info($"User '{email}' attempting to leave board with ID {boardID}.");

                // Step 1: Input validation
                if (string.IsNullOrWhiteSpace(email) || boardID < 0)
                {
                    Log.Error("Email or board ID is null or invalid.");
                    throw new Exception("Email or board ID is null or invalid.");
                }

                // Step 2: Validate user and board
                UserExistOrLoggedIn(email);
                isBoardIdExistsOrValid(boardID);

                var user = uf.Users[email];
                var board = Boards_dict[boardID];

                // Step 3: Check ownership
                board.isMember(user);
                if (board.isEmailOwner(email))
                {
                    Log.Warn($"Owner '{email}' tried to leave their own board (ID: {boardID}).");
                    throw new Exception("Owner cannot leave the board. Please delete the board instead.");
                }

                // Step 4: Perform removal
                board.leaveBoard(user);
                board.UnassignUserTasks(email);
                user.removeBoardFromList(board);

                Log.Info($"User '{email}' successfully left board {boardID}.");
            }
            catch (Exception e)
            {
                Log.Error($"Error leaving board {boardID} by user '{email}': {e.Message}");
                throw new Exception($"Error leaving board: {e.Message}");
            }
        }

        public string getBoardName(int boardId)
        {
            try
            {
                isBoardIdExistsOrValid(boardId);
                return Boards_dict[boardId].GetName();
            }
            catch (Exception e)
            {
                Log.Error("Error getting board name: " + e.Message);
                throw new Exception("Error getting board name: " + e.Message);
            }
        }
        public bool IsValidEmail(string email)
        {
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
        public void UserExistOrLoggedIn(string email)
        {

            if (!uf.Users.ContainsKey(email))
            {
                throw new Exception("User does not exist.");
            }
            if (!authFacade.IsLoggedIn(email))
            {
                throw new Exception("User is not logged in.");
            }

        }
        public void isBoardIdExistsOrValid(int boardId)
        {
            if (boardId < 0)
            {
                throw new Exception("Invalid board ID");
            }
            if (!Boards_dict.ContainsKey(boardId))
            {
                throw new Exception("Board not found");
            }
        }
        public void isValidInput(string email, string boardName, string title, string description)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(boardName) || string.IsNullOrWhiteSpace(title))
            {
                throw new Exception("Email, board name or title is null or empty.");
            }
            if (title.Length > 50 || description.Length > 300)
            {
                throw new Exception("title or description is too long.");
            }
            if (!IsValidEmail(email))
            {
                throw new Exception("Invalid email format.");
            }
        }

        /// <summary>
        /// Transfers ownership of a board from the current owner to another user.
        /// </summary>
        /// <param name="currEmail">Current owner's email</param>
        /// <param name="newEmail">New owner's email</param>
        /// <param name="boardName">Board name</param>
        public void TransferOwnership(string currEmail, string newEmail, string boardName)
        {
            try
            {
                Log.Info($"Attempting to transfer ownership of board '{boardName}' from '{currEmail}' to '{newEmail}'.");

                // Step 1: Validate current user
                UserExistOrLoggedIn(currEmail);

                // Step 2: Validate new user exists
                if (!uf.Users.ContainsKey(newEmail))
                {
                    Log.Warn($"Transfer failed: New user '{newEmail}' does not exist.");
                    throw new Exception("The new user does not exist.");
                }

                // Step 3: Get current board and verify ownership
                var oldUser = uf.Users[currEmail];
                var boardFromOldUser = oldUser.getBoardByName(boardName);
                boardFromOldUser.isOwner(currEmail);

                // Step 4: Prevent transfer to same user
                if (newEmail.Equals(currEmail.Trim().ToLower(), StringComparison.OrdinalIgnoreCase))
                {
                    Log.Warn("Transfer failed: Attempted to transfer ownership to the same user.");
                    throw new Exception("Invalid transfer: cannot transfer board ownership to yourself.");
                }

                // Step 5: Get board from new user's list and perform the transfer
                var newUser = uf.Users[newEmail];
                var boardFromNewUser = newUser.getBoardByName(boardName);
                boardFromNewUser.TransferOwnership(newEmail);

                Log.Info($"Ownership of board '{boardName}' successfully transferred from '{currEmail}' to '{newEmail}'.");
            }
            catch (Exception ex)
            {
                Log.Error($"Error in TransferOwnership from '{currEmail}' to '{newEmail}' for board '{boardName}': {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Assigns a task in a board to a specific user, if the caller is the current assignee or no one is assigned yet.
        /// </summary>
        /// <param name="email">The email of the user attempting the assignment</param>
        /// <param name="boardName">The board name</param>
        /// <param name="columnOrdinal">The column index (0 - Backlog, 1 - In Progress, etc.)</param>
        /// <param name="taskID">The task ID to assign</param>
        /// <param name="emailAssignee">The email of the user to be assigned</param>
        public void AssignTask(string email, string boardName, int columnOrdinal, int taskID, string emailAssignee)
        {
            try
            {
                Log.Info($"User '{email}' attempting to assign task {taskID} in column {columnOrdinal} on board '{boardName}' to '{emailAssignee}'.");

                // Step 1: Validate acting user
                UserExistOrLoggedIn(email);
                var user = uf.Users[email];

                // Step 2: Get board and task
                var board = user.getBoardByName(boardName);
                var task = board.GetTask(columnOrdinal, taskID);
                if (task.isDone)
                {
                    throw new Exception("task in done cannot update");
                }

                // Step 3: Validate assignee user
                if (!uf.Users.ContainsKey(emailAssignee))
                {
                    Log.Warn($"Assignment failed: User '{emailAssignee}' does not exist.");
                    throw new Exception("User does not exist.");
                }

                var assigneeUser = uf.Users[emailAssignee];
                board.isMember(assigneeUser);

                // Step 4: Only the current assignee (or unassigned task) can reassign the task
                bool isAssignable = task.emailAssignee == null || task.emailAssignee.Equals(email.Trim().ToLower(), StringComparison.OrdinalIgnoreCase);
                if (isAssignable)
                {
                    task.setAssignee(emailAssignee);
                    Log.Info($"Task {taskID} on board '{boardName}' successfully assigned to '{emailAssignee}'.");
                }
                else
                {
                    Log.Warn($"Assignment failed: Task is already assigned to someone else.");
                    throw new Exception("Task not assigned to you, cannot assign to another user.");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error in AssignTask by '{email}' for task {taskID} in board '{boardName}': {ex.Message}");
                throw new Exception(ex.Message);
            }
        }
        public List<BoardBL>GetBoardsUS(string email)
        {
            try
            {
                UserExistOrLoggedIn(email);
                var user = uf.Users[email];
                return user.Boards;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public List<object> GetUserBoardsDetailed(string email)
        {
            try
            {
                UserExistOrLoggedIn(email);
                var user = uf.Users[email];
                var result = new List<object>();
                foreach (var board in user.Boards)
                {
                    result.Add(new
                    {
                        BoardId = board.getBoardID(),
                        Name = board.GetName(),
                        OwnerEmail = board.ownerEmail,
                        TaskCount = board.GetColumn(0).Count + board.GetColumn(1).Count + board.GetColumn(2).Count,
                        InProgressTasks = board.GetColumn(1).Count,
                        DoneTasks = board.GetColumn(2).Count
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); 
            }   
        }


        // change to boardID because BoardNmae not uniqe
        public List<string> GetBoardMembers(string email, string boardName)
        {
            try
            {
                UserExistOrLoggedIn(email);
                var user = uf.users[email];
                var board =user.getBoardByName(boardName);
                return board.GetMembers().Select(user => user.Email).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message); 
            }
        }


        //same to here
        public List<TaskBL> GetBoardTasks(string email, string boardName, int columnOrdinal)
        {
            try
            {
                UserExistOrLoggedIn(email);
                var board = uf.Users[email].getBoardByName(boardName);
                return board.GetColumn(columnOrdinal);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }






    }
}
    

