using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Frontend.Model
{
    public class TaskModel
    {
        public string Title { get; set; }
        public string Assignee { get; set; }
        public string Status { get; set; }


        public TaskModel(string title, string assignee, string status)
        {
            Title = title;
            Assignee = assignee;
            Status = status;
        }

        public override string ToString()
        {
            return $"{Title} [{Status}] (Assigned to: {Assignee})";
        }
    }



}




