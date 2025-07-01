using Kanban.Backend.BusinessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Kanban.BackendTests
{
    [TestClass]
    public class ColumnTests
    {
        private column col;

        [TestInitialize]
        public void Setup()
        {
            col = new column("backlog", 5);
        }

        [TestMethod]
        public void Column_Creation_ShouldSucceed()
        {
            Assert.AreEqual("backlog", col.ColumnName);
            Assert.AreEqual(5, col.ColumnLimit);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Column_SetInvalidName_ShouldFail()
        {
            col.ColumnName = "";
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Column_SetNegativeLimit_ShouldFail()
        {
            col.ColumnLimit = -10;
        }

        [TestMethod]
        public void GetColumnLimit_ShouldReturnCorrectValue()
        {
            int limit = col.GetColumnLimit();
            Assert.AreEqual(5, limit);
        }

        [TestMethod]
        public void GetTasks_InitiallyEmpty_ShouldSucceed()
        {
            var tasks = col.GetTasks();
            Assert.AreEqual(0, tasks.Count);
        }

        [TestMethod]
        public void AssignTask_ValidInput_ShouldSucceed()
        {
            var task = new TaskBL(0, "backlog", DateTime.Now.AddDays(1), "title", "desc", 1, null, null);
            col.Tasks.Add(task);
            string assigner = null;
            string newAssignee = "user@example.com";
            var result = col.AssignTask(assigner, 0, 0, newAssignee);
            Assert.AreEqual(newAssignee, result.emailAssignee);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AssignTask_TaskNotFound_ShouldFail()
        {
            col.AssignTask("user@example.com", 0, 999, "assignee@example.com");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AssignTask_NotCurrentAssignee_ShouldFail()
        {
            var task = new TaskBL(0, "backlog", DateTime.Now.AddDays(1), "title", "desc", 1, "original@example.com", null);
            col.Tasks.Add(task);
            col.AssignTask("someoneelse@example.com", 0, 0, "assignee@example.com");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AssignTask_AssigneeToSelf_ShouldFail()
        {
            var task = new TaskBL(0, "backlog", DateTime.Now.AddDays(1), "title", "desc", 1, "user@example.com", null);
            col.Tasks.Add(task);
            col.AssignTask("user@example.com", 0, 0, "user@example.com");
        }

        [TestMethod]
        public void GetTaskFromCol_ExistingTask_ShouldReturnTask()
        {
            var task = new TaskBL(1, "backlog", DateTime.Now.AddDays(1), "task", "desc", 1, null, null);
            col.Tasks.Add(task);
            var found = col.GetTaskFromCol(1);
            Assert.AreEqual(task, found);
        }

        [TestMethod]
        public void GetTaskFromCol_NonExistingTask_ShouldReturnNull()
        {
            var found = col.GetTaskFromCol(123);
            Assert.IsNull(found);
        }
    }
}
