using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kanban.Backend.BusinessLayer;
using Kanban.Backend.ServiceLayer;
namespace Kanban.Backend.ServiceLayer
    {
    public class Response
    {
        public string ErrorMessage { get; set; }
        public object ReturnValue { get; set; }

        public Response() { }

        public Response(string errorMessage)
        {
            ErrorMessage = errorMessage;
            ReturnValue = null;
        }

        public Response(string errorMessage, object returnValue)
        {
            ErrorMessage = errorMessage;
            ReturnValue = returnValue;
        }

        public string ToJson(){
            return JsonSerializer.Serialize(this);
        }
    }
}

