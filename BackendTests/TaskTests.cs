using Kanban.Backend.ServiceLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Kanban.BackendTests
{
    [TestClass]
    public class TaskServiceTests
    {
        private ServiceFactory factory;

        [TestInitialize]
        public void Setup()
        {
            factory = new ServiceFactory();
        }

        [TestCleanup]
        public void Cleanup()
        {
            factory.DeleteData();
        }

        [TestMethod]
        public void UpdateTaskDueDate_Valid_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "Board1");
            factory.AddTask("user@example.com", "Board1", "Task1", "desc", DateTime.Now.AddDays(2));
            string result = factory.UpdateTaskDueDate("user@example.com", "Board1", 0, 0, DateTime.Now.AddDays(5));
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void UpdateTaskDueDate_Invalid_ShouldFail()
        {
            string result = factory.UpdateTaskDueDate("", "", -1, -1, DateTime.Now);
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void UpdateTaskTitle_Valid_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "Board1");
            factory.AddTask("user@example.com", "Board1", "Task1", "desc", DateTime.Now.AddDays(2));
            string result = factory.UpdateTaskTitle("user@example.com", "Board1", 0, 0, "Updated Title");
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void UpdateTaskTitle_Invalid_ShouldFail()
        {
            string result = factory.UpdateTaskTitle("", "", -1, -1, new string('A', 51));
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void UpdateTaskDescription_Valid_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "Board1");
            factory.AddTask("user@example.com", "Board1", "Task1", "desc", DateTime.Now.AddDays(2));
            string result = factory.UpdateTaskDescription("user@example.com", "Board1", 0, 0, "New Description");
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void UpdateTaskDescription_Invalid_ShouldFail()
        {
            string result = factory.UpdateTaskDescription("", "", -1, -1, new string('B', 301));
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void AdvanceTask_Valid_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "Board1");
            factory.AddTask("user@example.com", "Board1", "Task1", "desc", DateTime.Now.AddDays(2));
            factory.AssignTask("user@example.com", "Board1", 0, 0, "user@example.com");
            string result = factory.AdvanceTask("user@example.com", "Board1", 0, 0);
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void AdvanceTask_Invalid_ShouldFail()
        {
            string result = factory.AdvanceTask("", "", -1, -1);
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void InProgressTasks_ValidUser_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "Board1");
            factory.AddTask("user@example.com", "Board1", "Task1", "desc", DateTime.Now.AddDays(2));
            factory.AssignTask("user@example.com", "Board1", 0, 0, "user@example.com");
            factory.AdvanceTask("user@example.com", "Board1", 0, 0);
            string result = factory.InProgressTasks("user@example.com");
            Assert.IsTrue(result.Contains("Task1") || result.Contains("desc"));
        }

        [TestMethod]
        public void InProgressTasks_NullEmail_ShouldFail()
        {
            string result = factory.InProgressTasks(null);
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }
    }
}
