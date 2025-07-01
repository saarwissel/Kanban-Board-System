using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using log4net;
using Kanban.Backend.BusinessLayer;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Kanban.Backend.ServiceLayer{
    public class TaskSL
{
    public int Id { get; set; }
    public DateTime CreationTime { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string  Email { get; set; }
    public int BoardId { get; set; }

        public TaskSL(int id, DateTime creationTime, string title, string description, DateTime dueDate, int boardId, string email)

        {
            this.Id = id;
            this.CreationTime = creationTime;
            this.Title = title;
            this.Description = description;
            this.DueDate = dueDate;
            this.Email = email;
            this.BoardId = boardId;
            
        }
}

}

