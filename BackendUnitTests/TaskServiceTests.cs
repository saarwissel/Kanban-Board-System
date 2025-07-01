using NUnit.Framework;
using Kanban.Backend.ServiceLayer;
using System;

namespace BackendUnitTests
{
    [TestFixture]
    public class TaskServiceTests
    {
        private ServiceFactory factory;

        [SetUp]
        public void Setup()
        {
            TestDatabaseInitializer.Initialize();
            factory = new ServiceFactory();
        }

        [TearDown]
        public void Cleanup()
        {
            factory.DeleteData();
        }

        [Test]
        public void UpdateTaskTitle_ValidInput_ShouldSucceed()
        {
            factory.Register("title@example.com", "Password123");
            factory.CreateBoard("title@example.com", "Board");
            factory.AddTask("title@example.com", "Board", "Old", "desc", DateTime.Now.AddDays(3));

            var result = factory.UpdateTaskTitle("title@example.com", "Board", 0, 0, "New");
            Assert.IsTrue(result.Contains("null"));
        }

        [Test]
        public void AdvanceTask_ShouldSucceed()
        {
            factory.Register("adv@example.com", "Password123");
            factory.CreateBoard("adv@example.com", "Board");
            factory.AddTask("adv@example.com", "Board", "Task", "desc", DateTime.Now.AddDays(3));

            var result = factory.AdvanceTask("adv@example.com", "Board", 0, 0);
            Assert.IsTrue(result.Contains("null"));
        }

        [Test]
        public void InProgressTasks_ShouldReturnOnlyInProgress()
        {
            factory.Register("inprog@example.com", "Password123");
            factory.Login("inprog@example.com", "Password123");
            factory.CreateBoard("inprog@example.com", "Board");
            factory.AddTask("inprog@example.com", "Board", "T1", "desc", DateTime.Now.AddDays(3));
            factory.AdvanceTask("inprog@example.com", "Board", 0, 0); // to InProgress

            var result = factory.InProgressTasks("inprog@example.com");
            Assert.IsTrue(result.Contains("T1"));
        }

        [Test]
        public void UpdateTaskDueDate_ShouldFail_WhenTaskNotExist()
        {
            factory.Register("fail@example.com", "Password123");
            factory.CreateBoard("fail@example.com", "Board");

            var result = factory.UpdateTaskDueDate("fail@example.com", "Board", 0, 5, DateTime.Now);
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }
    }
}
