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
namespace Kanban.Backend.BusinessLayer
{
    public class Logger
    {
        public string message { get; set; }
        public string severity { get; set; }

        public Logger()
        {
            this.severity = "Error";
            this.message = string.Empty;
        }
        public void LogInfo(string msg)
        {
            LogEvent(msg, "Info");
        }

        public void LogError(string msg)
        {
            LogEvent(msg, "Error");
        }

        public void LogEvent(string msg, string svt)
        {
            this.message = msg;
            this.severity = svt;

            Console.WriteLine($"[{DateTime.Now}] [{severity}] {message}");


        }
    }
}
