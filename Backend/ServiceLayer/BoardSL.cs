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
namespace Kanban.Backend.ServiceLayer{
    
    public class BoardSL
    {
        string BoardName;
        int backlogCapacity;
        int inProgressCapacity; 
        int doneCapacity;
        public BoardSL(string name)
        {
            this.BoardName = name;
            this.backlogCapacity = int.MaxValue;
            this.inProgressCapacity = int.MaxValue;
            this.doneCapacity = int.MaxValue;

        }

        public BoardSL(BoardBL other)
        {
            this.BoardName = other.BoardName;
            this.backlogCapacity = other.backlogCapacity;
            this.inProgressCapacity = other.inProgressCapacity;
            this.doneCapacity = other.doneCapacity;
        }
        public string GetName()
        {
            return BoardName.ToString();
        }
        public void setCapacity(int lim)
        {
             backlogCapacity = lim;
             inProgressCapacity = lim;
             doneCapacity = lim;
        }
    }


}