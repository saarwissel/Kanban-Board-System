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
using System.Threading;
using Kanban.Backend.DataAccessLayer;
namespace Kanban.Backend.BusinessLayer
{
    public class TaskBL
    {
        public int taskId;

        public string title;
        public string Title
        {
            get { return title; }
        }

        public string description;

        public DateTime dueDate;
        public bool isDone { get; set; }

        public string currPosition { get; set; }

        public int BoardId { get; set; }

        public readonly DateTime creationTime;
        private string errorMessage;

        public string emailAssignee { get; set; }
        public TaskDAL taskDAL { get; set; }



        public TaskBL(int id, string currpos, DateTime dueDate, string title, string description, int boardID, string msg, string eamil)
        {
            this.taskId = id;
            this.dueDate = dueDate;
            this.title = title;
            this.description = description;
            this.isDone = false;
            this.creationTime = DateTime.Now;
            this.BoardId = boardID;
            this.currPosition = currpos;
            this.errorMessage = msg;
            emailAssignee = eamil;
            taskDAL = new TaskDAL(id, creationTime, dueDate, title, description, eamil, boardID);
        }
        public TaskBL(int id, string currpos, DateTime dueDate, string title, string description, int boardID, string msg, string eamil, string nulL)
        {
            this.taskId = id;
            this.dueDate = dueDate;
            this.title = title;
            this.description = description;
            this.isDone = false;
            this.creationTime = DateTime.Now;
            this.BoardId = boardID;
            this.currPosition = currpos;
            this.errorMessage = msg;
            emailAssignee = eamil;
        }

        public void UpdateDueDate(DateTime dueDate)
        {
            this.dueDate = dueDate;
        }
        public void UpdateTaskTitle(string title)
        {
            this.title = title;
        }
        public void UpdateTaskDescription(string description)
        {
            this.description = description;
        }
        public void setPoss(string poss)
        {
            this.currPosition = poss;
            taskDAL.UpdatePosition(poss);
        }
        public int getId()
        {
            return this.taskId;
        }
        public string getErrorMessage()
        {
            return this.errorMessage;
        }
        public string getTitle()
        {
            return this.title;
        }
        public string getDescription()
        {
            return this.description;
        }
        public DateTime getDueDate()
        {
            return this.dueDate;
        }
        public string getCurrPosition()
        {
            return this.currPosition;
        }

        public bool getIsDone()
        {
            return this.isDone;
        }
        public DateTime getcreationTime()
        {
            return this.creationTime;
        }
        public void isAssignUser(string email)
        {
            if (this.emailAssignee==null)
            {
                return;
            }
            else if (!this.emailAssignee.Equals(email))
            {
                throw new Exception("not the assigne user");
            }
            else
            {
                return;
            }

        }
        public string getEmailAssignee()
        {
            return this.emailAssignee;
        }
        public void setDueDate(DateTime date)
        {
            if (this.dueDate <= date)
            {
                throw new Exception("Due date cannot be set to a past date");
            }
            this.dueDate = date;
            this.taskDAL.UpdateDueDate(date);
        }
        public void setTitle(string tit)
        {
            if (string.IsNullOrWhiteSpace(tit))
            {
                throw new Exception("invalid title");
            }
            this.title = tit;
            this.taskDAL.UpdateTitle(tit);
        }
        public void setDescription(string des)
        {
            if (string.IsNullOrWhiteSpace(des))
            {
                throw new Exception("invalid description");
            }
            this.description = des;
            this.taskDAL.UpdateDescription(des);
        }
        public void setAssignee(String email)
        {
            this.emailAssignee = email;
            this.taskDAL.UpdateAssignee(email);
        }

        public bool UpdateTaskAssignee(string emailAssignee)
        {
            this.emailAssignee = emailAssignee;
            if (taskDAL != null && !taskDAL.UpdateAssignee(emailAssignee))
                throw new InvalidOperationException("Task assignee update failed");
            return true;

        }
        public int getBoardId()
        {
            return this.BoardId;
        }
    }
}
