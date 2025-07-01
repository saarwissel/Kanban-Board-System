using Kanban.Backend.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class TaskFd
    {
        public string Title { get; set; }
        public string Assignee { get; set; }
        public string Column { get; set; }
        public TaskFd(TaskBL task)
        {
            Title = task.Title;
            Assignee = task.emailAssignee;
            Column = task.currPosition;
        }
    }
}
