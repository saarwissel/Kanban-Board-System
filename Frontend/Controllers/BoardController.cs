using IntroSE.Kanban.Frontend.Model;
using Kanban.Backend.DataAccessLayer;
using Kanban.Backend.ServiceLayer;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Frontend.Controllers
{
    internal class BoardController
    {
        private readonly BoardService bs;
        private readonly TaskService ts;
        public BoardController(BoardService boardS,TaskService TaskS)
        {
            this.bs = boardS;
            this.ts = TaskS;


        }
        public void LoadData() => bs.LoadData();

        public void CreateBoard(string email, string boardName)
        {
            var json = bs.CreateBoard(email, boardName);
            var response = JsonSerializer.Deserialize<Response>(json);

            if (response == null || !string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response?.ErrorMessage ?? "Unknown error while creating board");
        }

        public List<string> GetUserBoardNames(string email)
        {
            var json = bs.GetUserBoardsDetailed(email);
            var response = JsonSerializer.Deserialize<Response>(json);

            if (response == null || !string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response?.ErrorMessage ?? "Unknown error");

            var rawList = response.ReturnValue?.ToString();
            if (rawList == null)
                return new List<string>();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var boards = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(rawList, options);

            return boards?.Select(b => b["Name"].ToString()!).ToList() ?? new List<string>();
        }

        public void DeleteBoard(string email, string boardName)
        {
            var json = bs.DeleteBoard(email, boardName);
            var response = JsonSerializer.Deserialize<Response>(json);

            if (response == null || !string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response?.ErrorMessage ?? "Unknown error deleting board");
        }


        public List<string> GetBoardMembers(string email, string boardName)
        {
            var json = bs.GetBoardMembers(email, boardName);
            var response = JsonSerializer.Deserialize<Response>(json);
            if (response == null || response.ErrorMessage != null)
                throw new System.Exception(response?.ErrorMessage ?? "Unknown error");
            return JsonSerializer.Deserialize<List<string>>(response.ReturnValue.ToString());
        }

        public List<TaskModel> GetColumnTasks(string email, string boardName, int columnOrdinal)
        {
            var json = bs.GetBoardTasks(email, boardName, columnOrdinal);
            var response = JsonSerializer.Deserialize<Response>(json);

            if (response == null || response.ErrorMessage != null)
                throw new Exception(response?.ErrorMessage ?? "Unknown error");

            var taskDictList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(
                response.ReturnValue?.ToString() ?? "[]",
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var result = new List<TaskModel>();

            foreach (var task in taskDictList)
            {
                string debugJson = JsonSerializer.Serialize(task);
                System.Diagnostics.Debug.WriteLine($"📦 Raw Task JSON: {debugJson}");

                string title = task.ContainsKey("Title") ? task["Title"]?.ToString() ?? "Untitled" : "Untitled";
                string assignee = task.ContainsKey("Assignee") ? task["Assignee"]?.ToString() ?? "Unassigned" : "Unassigned";
                string status = task.ContainsKey("Column") ? task["Column"]?.ToString()?.ToLower() ?? "unknown" : "unknown";

                var taskModel = new TaskModel(title, assignee, status);
                result.Add(taskModel);

                System.Diagnostics.Debug.WriteLine($"✅ Parsed TaskModel: {taskModel}");
            }


            return result;
        }


        
        






        private class Response
        {
            public string? ErrorMessage { get; set; }
            public object? ReturnValue { get; set; }
        }

    }
}
