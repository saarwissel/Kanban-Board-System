using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using log4net;
using Kanban.Backend.BusinessLayer;
using System.Threading.Tasks;





namespace Kanban.Backend.BusinessLayer
{
    public class column

    {
        private string name;
        private int Limit;

        public string ColumnName
        {
            get { return name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    log.Info("Column name cannot be null or empty.");
                    throw new ArgumentException("Column name cannot be null or empty.");
                }
                name = value;
            }
        }


        public int ColumnLimit
        {
            get { return Limit; }
            set
            {
                Limit = value;
            }
        }


        public List<TaskBL> Tasks { get; set; }

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public column(string columnName, int columnLimit)
        {
            this.name = columnName;
            this.Limit = columnLimit;
            this.Tasks = new List<TaskBL>();

        }
        public TaskBL AssignTask(string email, int columnOrdinal, int taskID, string emailAssignee)
        {
            TaskBL task = Tasks.FirstOrDefault(t => t.taskId == taskID);
            if (task == null)
                throw new InvalidOperationException("Task not found");
            if (task.emailAssignee != null && !task.emailAssignee.Equals(email))
                throw new InvalidOperationException("Only the Assignee can make changes");
            if (task.emailAssignee == emailAssignee)
                throw new InvalidOperationException("Assignee can not assign to himself");
            task.UpdateTaskAssignee(emailAssignee);
            return task;
        }
        public int GetColumnLimit()
        {
            return this.ColumnLimit;
        }
        public List<TaskBL> GetTasks()
        {
            return this.Tasks;
        }
        public TaskBL GetTaskFromCol(int taskId)
        {
            return this.Tasks.FirstOrDefault(task => task.taskId == taskId);
        }
        
    }

    
}