using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kanban.Backend.ServiceLayer;

namespace IntroSE.Kanban.Frontend.Controllers
{
    internal class ControllerFactory
    {
        public static ControllerFactory Instance { get; } = new ControllerFactory();

        public readonly UserController UserController;
        public readonly BoardController BoardController;


        private ControllerFactory()
        {
            var serviceFactory = new ServiceFactory();
            UserController = new UserController(serviceFactory.us);
            BoardController =new BoardController(serviceFactory.bs,serviceFactory.ts);
        }

    }
}
