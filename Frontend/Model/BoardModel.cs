using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Frontend.Model
{
    public class BoardModel
    {
        public string Name { get; }
        public string Owner { get; }

        public BoardModel(string name, string owner)
        {
            Name = name;
            Owner = owner;
        }
    }
}
