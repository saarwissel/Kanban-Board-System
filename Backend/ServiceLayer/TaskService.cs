using System;
using System.Collections.Generic;
using log4net;
using Kanban.Backend.BusinessLayer;
using System.Text.Json;

namespace Kanban.Backend.ServiceLayer
{
    public class TaskService
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private BoardFacade boardFacade;

        public TaskService(BoardFacade boardFacade)
        {
            this.boardFacade = boardFacade;
        }

        /// <summary>
        /// Updates the due date of a task.
        /// </summary>
        public string UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)//check about the date time improper
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(boardName) || columnOrdinal < 0 || taskId < 0 || columnOrdinal > 2)
                return new Response("null input.").ToJson();

            try
            {
                boardFacade.UpdateTaskDueDate(email.Trim().ToLower(), boardName, columnOrdinal, taskId, dueDate);
                return new Response(null, null).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in UpdateTaskDueDate: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Updates the title of a task.
        /// </summary>
        public string UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(boardName) || string.IsNullOrWhiteSpace(title) || title.Length > 50 || columnOrdinal < 0 || columnOrdinal > 2 || taskId < 0)
                return new Response("null input.").ToJson();

            try
            {
                boardFacade.UpdateTaskTitle(email.Trim().ToLower(), boardName, columnOrdinal, taskId, title);
                return new Response(null, null).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in UpdateTaskTitle: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Updates the description of a task.
        /// </summary>
        public string UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string description)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(boardName) || string.IsNullOrWhiteSpace(description) || description.Length > 300 || columnOrdinal < 0 || columnOrdinal > 2 || taskId < 0)
                return new Response("null input.").ToJson();

            try
            {
                boardFacade.UpdateTaskDescription(email.Trim().ToLower(), boardName, columnOrdinal, taskId, description);
                return new Response(null, null).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in UpdateTaskDescription: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Advances a task to the next column.
        /// </summary>
        public string AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(boardName) || columnOrdinal < 0 || columnOrdinal > 2 || taskId < 0)
                return new Response("null input.").ToJson();

            try
            {
                var task = boardFacade.AdvanceTask(email.Trim().ToLower(), boardName, columnOrdinal, taskId);
                return new Response(null, task).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in AdvanceTask: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }

        /// <summary>
        /// Returns all in-progress tasks of a user.
        /// </summary>
        public string InProgressTasks(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return new Response("Null email.").ToJson();

            try
            {
                TaskBL[] tasks = boardFacade.InProgressTasks(email.Trim().ToLower());
                if (tasks == null)
                    return new Response("not success", null).ToJson();

                List<TaskSL> taskSLs = new List<TaskSL>();
                foreach (var task in tasks)
                {
                    taskSLs.Add(new TaskSL(task.getId(), task.getcreationTime(), task.getTitle(), task.getDescription(), task.getDueDate(),task.getBoardId(), task.getEmailAssignee()));
                }
                return new Response(null, taskSLs.ToArray()).ToJson();
            }
            catch (Exception e)
            {
                log.Error("Error in InProgressTasks: " + e.Message);
                return new Response("Exception: " + e.Message).ToJson();
            }
        }
    }
}
