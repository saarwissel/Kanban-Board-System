using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using log4net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kanban.Backend.BusinessLayer;
using System.Runtime.Intrinsics.X86;
using Kanban.Backend.DataAccessLayer;
namespace Kanban.Backend.BusinessLayer
{
    public class BoardBL
    {
        public string BoardName;
        private column backlog;
        public column Backlog
        {
            get { return backlog; }
            set { backlog = value; }
        }
        private column inProgress;
        public column InProgress
        {
            get { return inProgress; }
            set { inProgress = value; }
        }
        private column done;
        public column Done
        {
            get { return done; }
            set { done = value; }
        }

        public int backlogCapacity;
        public int inProgressCapacity;
        public int doneCapacity;
        public string ownerEmail;
        private UserBL user;
        private string errorMessage;
        public int BoardID;
        private List<UserBL> Members;
        private int erorrLimit = -1;
        public int TaskID;
        private int errorID = 1;
        public ColumnController ColumnController { get; set; }


        private BoardDAL boardDAL;
        public BoardDAL BoardDAL
        {
            get { return boardDAL; }
        }

        public BoardBL(string email, string name, string msg, int boardID)
        {
            this.BoardName = name;
            this.backlogCapacity = -1;
            this.inProgressCapacity = -1;
            this.doneCapacity = -1;
            this.ownerEmail = email;
            this.backlog = new column("backlog", -1);
            this.inProgress = new column("in progress", -1);
            this.done = new column("done", -1);
            this.errorMessage = msg;
            this.BoardID = boardID;
            this.Members = new List<UserBL>();
            this.TaskID = 0;
            this.boardDAL = new BoardDAL(boardID, name, email);
            this.ColumnController = new ColumnController();
            if (!this.boardDAL.Presist())
                throw new Exception("Failed to presist board");
        }
        public BoardBL(string email, string name, string msg, int boardID, string nulL)
        {
            this.BoardName = name;
            this.backlogCapacity = -1;
            this.inProgressCapacity = -1;
            this.doneCapacity = -1;
            this.ownerEmail = email;
            this.backlog = new column("backlog", -1);
            this.inProgress = new column("in progress", -1);
            this.done = new column("done", -1);
            this.errorMessage = msg;
            this.BoardID = boardID;
            this.Members = new List<UserBL>();
            this.TaskID = 0;
            this.boardDAL = new BoardDAL(boardID, name, email, "");
            this.ColumnController = new ColumnController();
        }


        public BoardBL(UserBL user, string boardName)
        {
            this.user = user;
            this.BoardName = boardName;
        }

        public string GetName()
        {
            return this.BoardName;
        }
        public int getBoardID()
        {
            return BoardID;
        }
        public bool isEmailOwner(string email)
        {
            return ownerEmail.Equals(email);
        }




        public void SetColumnLimit(int columnOrdinal, int limit)
        {
            if (columnOrdinal < 0 || columnOrdinal > 2)
            {
                throw new Exception("Invalid column ordinal");
            }
            switch (columnOrdinal)
            {
                case 0:
                    this.BoardDAL.UpdateColumnLimit("backlog", limit);
                    backlog.ColumnLimit = limit;
                    break;
                case 1:
                    this.BoardDAL.UpdateColumnLimit("in progress", limit);
                    inProgress.ColumnLimit = limit;
                    break;
                case 2:
                    this.BoardDAL.UpdateColumnLimit("done", limit);
                    done.ColumnLimit = limit;
                    break;
            }
        }
        public int GetColumnLimit(int columnOrdinal)
        {
            switch (columnOrdinal)
            {
                case 0:
                    return backlog.ColumnLimit;
                case 1:
                    return inProgress.ColumnLimit;
                case 2:
                    return done.ColumnLimit;
                default:
                    return erorrLimit;
            }
        }
        public string GetColumnName(int columnOrdinal)
        {
            switch (columnOrdinal)
            {
                case 0:
                    return "backlog";
                case 1:
                    return "in progress";
                case 2:
                    return "done";
                default:
                    return "";
            }
        }

        /*public TaskBL AddTask(string title, string description, DateTime dueDate)
        {

            TaskBL task = new TaskBL(this.TaskID, "backlog", dueDate, title, description, this.GetName(), "", null);
            TaskDAL taskDAL = new TaskDAL(TaskID, task.getcreationTime(), dueDate, "backlog", description, null, this.BoardID);
            if (backlog.ColumnLimit == -1)
            {
                try
                {
                    taskDAL.InsertTask();

                }
                catch (Exception ex)
                {
                    errorMessage = "Failed to insert task into the database: " + ex.Message;
                    throw new Exception(errorMessage);
                }
                backlog.Tasks.Add(task);
                this.TaskID++;
                return task;
            }
            else if (backlog.Tasks.Count + 1 >= backlog.ColumnLimit)
            {
                errorMessage = "backlog is full";
                throw new Exception("backlog is full");
            }
            else
            {
                backlog.Tasks.Add(task);
                try
                {
                    taskDAL.InsertTask();
                }
                catch (Exception ex)
                {
                    errorMessage = "Failed to insert task into the database: " + ex.Message;
                    throw new Exception(errorMessage);
                }
                this.TaskID++;
                return task;
            }
        }*/

        public TaskBL AddTask(string title, string description, DateTime dueDate)
        {
            if (title.Length > 50)
                throw new Exception("Invalid title");

            if (description.Length > 300 && description.Length > 300)
                throw new Exception("Description too long");

            if (dueDate <= DateTime.Now)
                throw new Exception("Due date must be in the future");

            if (backlog.Tasks.Count >= backlog.ColumnLimit && backlog.ColumnLimit != -1)
                throw new Exception("Backlog is full");

            var task = new TaskBL(this.TaskID, "backlog", dueDate, title, description, this.BoardID, "", null);

            backlog.Tasks.Add(task);

            try
            {
                task.taskDAL.InsertTask();
            }
            catch (Exception ex)
            {
                errorMessage = "Failed to insert task into the database: " + ex.Message;
                throw new Exception(errorMessage);
            }

            this.TaskID++;
            return task;
        }

        public void AdvanceTask(TaskBL task, int columnOrdinal)
        {
            column from = columnOrdinal == 0 ? backlog : inProgress;
            column to = columnOrdinal == 0 ? inProgress : done;
            string newStatus = columnOrdinal == 0 ? "in progress" : "done";

            if (!from.GetTasks().Contains(task))
                throw new Exception("Task not found in the current column.");

            if (to.ColumnLimit != -1 && to.GetTasks().Count >= to.ColumnLimit)
                throw new Exception($"Target column '{newStatus}' is at full capacity.");

            from.GetTasks().Remove(task);
            to.GetTasks().Add(task);
            task.setPoss(newStatus);

            if (newStatus == "done")
                task.isDone = true;
        }
        public List<TaskBL> GetColumn(int columnOrdinal)
        {
            if (columnOrdinal == 0)
            {
                return backlog.GetTasks();
            }
            else if (columnOrdinal == 1)
            {
                return inProgress.GetTasks();
            }
            else
            {
                return done.GetTasks();
            }
        }
        public string GetErrorMessage()
        {
            return errorMessage;
        }
        public void isOwner(string email)
        {
            if (!this.ownerEmail.Equals(email.Trim().ToLower(), StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("User is not the owner of the board");
            }
        }
        public void addMem(UserBL user)
        {
            if (this.Members.Contains(user))// maybe not needed
            {
                throw new Exception("User is already a member of the board");
            }
            this.Members.Add(user);
        }
        public void addMember(UserBL user)
        {
            if (this.Members.Contains(user))// maybe not needed
            {
                throw new Exception("User is already a member of the board");
            }
            if (!this.boardDAL.JoinUser(user.Email))
            {
                throw new Exception("Failed to add user to the board in the database");
            }
            this.Members.Add(user);
        }
        public List<UserBL> getMembers()
        {
            return this.Members;
        }
        public void isMember(UserBL user)
        {
            if (!Members.Contains(user))
            {
                throw new Exception("this user not assined to this board");
            }
            return;
        }
        public TaskBL GetTask(int columnOrdinal, int Id)
        {
            switch (columnOrdinal)
            {
                case 0:
                    return backlog.GetTaskFromCol(Id) ?? throw new Exception("Task not found in backlog.");
                case 1:
                    return inProgress.GetTaskFromCol(Id) ?? throw new Exception("Task not found in progress.");
                case 2:
                    return done.GetTaskFromCol(Id) ?? throw new Exception("Task not found in done.");
                default:
                    throw new Exception("Invalid column ordinal.");
            }
        }
        public void leaveBoard(UserBL user)
        {
            if (user == null)
            {
                throw new Exception("User is null");
            }
            if (!Members.Contains(user))
            {
                throw new Exception("User is not a member of the board");
            }
            if (!boardDAL.LeaveUser(user.Email))
            {
                throw new Exception("Failed to remove user from the board in the database");
            }
            Members.Remove(user);
            leaveTasks(user);
        }
        public void leaveTasks(UserBL user)
        {
            foreach (TaskBL task in backlog.GetTasks())
            {
                if (task.getEmailAssignee().Equals(user.Email))
                {
                    task.setAssignee(null);
                }
            }
            foreach (TaskBL task in inProgress.GetTasks())
            {
                if (task.getEmailAssignee().Equals(user.Email))
                {
                    task.setAssignee(null);
                }
            }
            foreach (TaskBL task in done.GetTasks())
            {
                if (task.getEmailAssignee().Equals(user.Email))
                {
                    task.setAssignee(null);
                }

            }
        }

        public bool TransferOwnership(string newEmail)
        {
            try
            {
                var updateOwnew = boardDAL.UpdateOwner(newEmail);
                if(!boardDAL.TransferOwnership(this.BoardID,ownerEmail, newEmail))
                {
                    throw new Exception("Failed to transfer ownership in the database");
                }
                ownerEmail = newEmail;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public bool AssignTask(string email, int columnOrdinal, int taskID, string emailAssignee)
        {
            try
            {
                if (columnOrdinal < 0 || columnOrdinal > 2)
                    throw new Exception("Invalid column number");

                if (columnOrdinal == 2)
                    throw new Exception("Can not assign done task");
                TaskBL task;

                if (columnOrdinal == 0)
                    task = backlog.AssignTask(email, columnOrdinal, taskID, emailAssignee);
                else if (columnOrdinal == 1)
                    task = inProgress.AssignTask(email, columnOrdinal, taskID, emailAssignee);
                else
                    throw new Exception("Invalid columnOrdinal");


                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


        }
        public void UnassignUserTasks(string email)
        {
            foreach (var task in backlog.Tasks)
            {
                if (task.emailAssignee == email && !task.isDone)
                    task.setAssignee(null);
            }
            foreach (var task in inProgress.Tasks)
            {
                if (task.emailAssignee == email && !task.isDone)
                    task.setAssignee(null);
            }
        }
        public List<UserBL> GetMembers()
        {
            return Members;
        }



        public void deleteMe()
        {
            this.backlog.Tasks.Clear();
            this.inProgress.Tasks.Clear();
            this.done.Tasks.Clear();
            this.Members.Clear();
            this.BoardName = null;
            this.ownerEmail = null;
            this.backlog = null;
            this.inProgress = null;
            this.done = null;
            this.errorMessage = null;
            this.user = null;
            this.boardDAL = null;
            this.ColumnController = null;
            this.Members = null;

        }
        
        public void userExist(string email)
        {
            foreach (UserBL user in Members)
            {
                if (user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
                else
                {
                    throw new Exception("User does not exist in the board");
                }
            }
        }
        
        

     }

}
