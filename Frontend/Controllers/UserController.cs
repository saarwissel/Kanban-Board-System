using IntroSE.Kanban.Frontend.Model;
using Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Frontend.Controllers
{
    internal class UserController
    {
        private readonly UserService userService;

        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        public UserModel Register(string email, string password)
        {
            var json = userService.Register(email, password);
            var response = JsonSerializer.Deserialize<Response>(json);

            if (response == null)
                throw new Exception("Unexpected null response from service");

            if (!string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response.ErrorMessage); 

            return new UserModel(email);
        }

        public void Login(string email, string password)
        {
            var json = userService.Login(email, password);
            var response = JsonSerializer.Deserialize<Response>(json);

            if (response == null || !string.IsNullOrEmpty(response.ErrorMessage))
                throw new Exception(response?.ErrorMessage ?? "Unknown login error");
        }
        


        private class Response
        {
            public bool ErrorOccured { get; set; }
            public string? ErrorMessage { get; set; }
            public JsonElement? ReturnValue { get; set; }
        }
    }
}
