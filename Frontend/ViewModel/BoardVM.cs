using IntroSE.Kanban.Frontend.Controllers;
using IntroSE.Kanban.Frontend.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IntroSE.Kanban.Frontend.ViewModel
{
    public class BoardVM
    {
        public string BoardName { get; }
        public string OwnerEmail { get; }
        public ObservableCollection<string> Members { get; }
        public ObservableCollection<string> BacklogTasks { get; }
        public ObservableCollection<string> InProgressTasks { get; }
        public ObservableCollection<string> DoneTasks { get; }

        public BoardVM(BoardModel board, UserModel user)
        {
            BoardName = board.Name;
            OwnerEmail = board.Owner;

            try
            {
                var membersList = ControllerFactory.Instance.BoardController.GetBoardMembers(user.Email, board.Name);
                Members = new ObservableCollection<string>(membersList);
            }
            catch (Exception ex)
            {
                Members = new ObservableCollection<string> { "Failed to load members: " + ex.Message };
            }

            BacklogTasks = new ObservableCollection<string>();
            InProgressTasks = new ObservableCollection<string>();
            DoneTasks = new ObservableCollection<string>();

            LoadTasksInto(BacklogTasks, OwnerEmail, BoardName, 0);
            LoadTasksInto(InProgressTasks, OwnerEmail, BoardName, 1);
            LoadTasksInto(DoneTasks, OwnerEmail, BoardName, 2);
        }

        private void LoadTasksInto(ObservableCollection<string> collection, string email, string boardName, int columnOrdinal)
        {
            try
            {
                var tasks = ControllerFactory.Instance.BoardController.GetColumnTasks(email, boardName, columnOrdinal);

                collection.Clear();
                foreach (var task in tasks)
                {

                    string formatted = $"[{task.Status}] {task.Title} (Assignee: {task.Assignee})";
                    collection.Add(formatted);
                    System.Diagnostics.Debug.WriteLine($"🟩 Loaded Task into column {columnOrdinal}: {formatted}");
                }

                System.Diagnostics.Debug.WriteLine($"✅ Loaded {tasks.Count} tasks into column {columnOrdinal}");
            }
            catch (Exception ex)
            {
                collection.Clear();
                collection.Add($"Error loading tasks: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ Failed to load tasks into column {columnOrdinal}: {ex.Message}");
            }
        }
    }
}
