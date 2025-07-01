using System;

namespace Kanban.Backend.DataAccessLayer
{
    public class TaskDAL
    {
        public int BoardID { get; set; }
        public int Id { get; set; }
        public string CreationTime { get; set; }
        public string DueDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Assignee { get; set; }
        public string TaskStatus { get; set; }
        private TaskController taskController;

        public string BoardIDColumnName = "BoardID";
        public string IDColumnName = "ID";
        public string CreationTimeColumnName = "CreationTime";
        public string DueDateColumnName = "DueDate";
        public string TitleColumnName = "Title";
        public string DescriptionColumnName = "Description";
        public string AssigneeColumnName = "Assignee";
        public string TaskStatusColumnName = "TaskStatus";

        public TaskDAL() { }

        public TaskDAL(int id, DateTime creationTime, DateTime dueDate, string title, string description, string? assignee, int boardID)
        {
            this.Id = id;
            this.CreationTime = creationTime.ToString("yyyy-MM-dd HH:mm:ss");
            this.DueDate = dueDate.ToString("yyyy-MM-dd HH:mm:ss");
            this.Title = title;
            this.Description = description;
            this.Assignee = assignee;
            this.TaskStatus = "backlog";
            this.BoardID = boardID;
            this.taskController = new TaskController();
        }
        public bool InsertTask()
        {
            if (taskController.Insert(this))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Failed to insert task.");
                throw new Exception("Failed to insert task.");
            }
        }

        public bool UpdateAssignee(string assignee)
        {
            if (taskController.UpdateAssignee(assignee, this.Id, this.BoardID))
            {
                this.Assignee = assignee;
                return true;
            }
            return false;
        }
        public bool UpdatePosition(string position)
        {
            if (taskController.UpdateTaskStatus(position, this.Id, this.BoardID))
            {
                this.TaskStatus = position;
                return true;
            }
            return false;
        }
        public bool UpdateDueDate(DateTime dueDate)
        {
            if (taskController.UpdateDueDate(dueDate, this.Id, this.BoardID))
            {
                this.DueDate = dueDate.ToString("yyyy-MM-dd HH:mm:ss");
                return true;
            }
            return false;
        }
        public bool UpdateTitle(string title)
        {
            if (taskController.UpdateTitle(title, this.Id, this.BoardID))
            {
                this.Title = title;
                return true;
            }
            return false;
        }
        public bool UpdateDescription(string description)
        {
            if (taskController.UpdateDescription(description, this.Id, this.BoardID))
            {
                this.Description = description;
                return true;
            }
            return false;
        }


    }

}