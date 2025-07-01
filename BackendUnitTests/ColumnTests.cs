using NUnit.Framework;
using Kanban.Backend.ServiceLayer;
using System;

namespace BackendUnitTests
{
    [TestFixture]
    public class ColumnTests
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
        public void LimitColumn_ShouldSucceed()
        {
            factory.Register("col@example.com", "Password123");
            factory.CreateBoard("col@example.com", "Board");

            var result = factory.LimitColumn("col@example.com", "Board", 2, 5); // Column: Done
            Assert.IsTrue(result.Contains("null"));
        }

        [Test]
        public void GetColumnName_ShouldReturnCorrect()
        {
            factory.Register("colname@example.com", "Password123");
            factory.CreateBoard("colname@example.com", "Board");

            var name = factory.GetColumnName("colname@example.com", "Board", 0);
            Assert.IsTrue(name.Contains("backlog", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void GetColumnLimit_ShouldReturnDefaultLimit()
        {
            factory.Register("limitcheck@example.com", "Password123");
            factory.CreateBoard("limitcheck@example.com", "Board");

            var limit = factory.GetColumnLimit("limitcheck@example.com", "Board", 1); // In progress
            Assert.IsTrue(limit.Contains("-1")); // ברירת מחדל
        }

        [Test]
        public void GetColumn_ShouldReturnTasks()
        {
            factory.Register("getcol@example.com", "Password123");
            factory.CreateBoard("getcol@example.com", "Board");
            factory.AddTask("getcol@example.com", "Board", "Task", "Desc", DateTime.Now.AddDays(2));

            var col = factory.GetColumn("getcol@example.com", "Board", 0);
            Assert.IsTrue(col.Contains("Task"));
        }
    }
}
