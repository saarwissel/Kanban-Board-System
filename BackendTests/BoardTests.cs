using Kanban.Backend.ServiceLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Kanban.BackendTests
{
    [TestClass]
    public class BoardTests
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
        public void LoadData_ShouldSucceed()
        {
            string result = factory.LoadData();
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void DeleteData_ShouldSucceed()
        {
            string result = factory.DeleteData();
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void CreateBoard_ValidData_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password1");
            string result = factory.CreateBoard("user@example.com", "MyBoard");
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void CreateBoard_EmptyName_ShouldFail()
        {
            factory.Register("user@example.com", "Password1");
            string result = factory.CreateBoard("user@example.com", "");
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void CreateBoard_DuplicateName_ShouldFail()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "MyBoard");
            string result = factory.CreateBoard("user@example.com", "MyBoard");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void DeleteBoard_ValidData_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "MyBoard");
            string result = factory.DeleteBoard("user@example.com", "MyBoard");
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void DeleteBoard_InvalidInput_ShouldFail()
        {
            string result = factory.DeleteBoard("", null);
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void LimitColumn_ValidLimit_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "MyBoard");
            string result = factory.LimitColumn("user@example.com", "MyBoard", 0, 5);
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void LimitColumn_InvalidOrdinal_ShouldFail()
        {
            string result = factory.LimitColumn("user@example.com", "MyBoard", -1, 5);
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void LimitColumn_InvalidLimit_ShouldFail()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "MyBoard");
            string result = factory.LimitColumn("user@example.com", "MyBoard", 0, -2);
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void GetColumnLimit_ValidData_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "MyBoard");
            factory.LimitColumn("user@example.com", "MyBoard", 0, 5);
            string result = factory.GetColumnLimit("user@example.com", "MyBoard", 0);
            Assert.IsTrue(result.Contains("5"));
        }

        [TestMethod]
        public void GetColumnLimit_InvalidData_ShouldFail()
        {
            string result = factory.GetColumnLimit("user@example.com", "MyBoard", 5);
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void GetColumnName_ValidOrdinal_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "MyBoard");
            string result = factory.GetColumnName("user@example.com", "MyBoard", 0);
            Assert.IsTrue(result.Contains("backlog"));
        }

        [TestMethod]
        public void GetColumnName_InvalidOrdinal_ShouldFail()
        {
            string result = factory.GetColumnName("user@example.com", "MyBoard", -1);
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void AddTask_ValidTask_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "Board1");
            string result = factory.AddTask("user@example.com", "Board1", "Title", "Description", DateTime.Now.AddDays(1));
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void AddTask_InvalidDueDate_ShouldFail()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "Board1");
            string result = factory.AddTask("user@example.com", "Board1", "Title", "Description", DateTime.Now.AddDays(-1));
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void AssignTask_ValidData_ShouldSucceed()
        {
            factory.Register("assignee@example.com", "Password1");
            factory.Register("owner@example.com", "Password1");
            factory.CreateBoard("owner@example.com", "BoardX");
            factory.JoinBoard("assignee@example.com", 0);
            factory.AddTask("owner@example.com", "BoardX", "Title", "Description", DateTime.Now.AddDays(1));
            string result = factory.AssignTask("owner@example.com", "BoardX", 0, 0, "assignee@example.com");
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void AssignTask_InvalidData_ShouldFail()
        {
            string result = factory.AssignTask("", "", -1, -1, "");
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void JoinBoard_ValidUser_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password1");
            factory.Register("owner@example.com", "Password1");
            factory.CreateBoard("owner@example.com", "SharedBoard");
            string result = factory.JoinBoard("user@example.com", 0);
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void JoinBoard_InvalidData_ShouldFail()
        {
            string result = factory.JoinBoard("", -1);
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }
        //new test for JoinBoard
        [TestMethod]
        public void JoinBoard_UserNotRegistered_ShouldFail()
        {
            string result = factory.JoinBoard("ghost@example.com", 0);
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void JoinBoard_UserNotLoggedIn_ShouldFail()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "TestBoard");
            factory.Logout("user@example.com"); // לוודא שיש לך את המתודה הזו

            string result = factory.JoinBoard("user@example.com", 0);
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void JoinBoard_EmailWithSpacesAndCaps_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password1");
            factory.Register("owner@example.com", "Password1");
            factory.CreateBoard("owner@example.com", "BoardX");

            string result = factory.JoinBoard("  USER@EXAMPLE.COM  ", 0);
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void JoinBoard_Twice_ShouldFail()
        {
            factory.Register("user@example.com", "Password1");
            factory.Register("owner@example.com", "Password1");
            factory.CreateBoard("owner@example.com", "BoardY");

            factory.JoinBoard("user@example.com", 0); // הצלחה ראשונה
            string result = factory.JoinBoard("user@example.com", 0); // ניסיון חוזר

            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }


        [TestMethod]
        public void JoinBoard_NonExistingBoard_ShouldFail()
        {
            factory.Register("user@example.com", "Password1");
            string result = factory.JoinBoard("user@example.com", 999); // לוח שלא קיים
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        //untill here

        [TestMethod]
        public void LeaveBoard_ValidUser_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password1");
            factory.Register("owner@example.com", "Password1");
            factory.CreateBoard("owner@example.com", "SharedBoard");
            factory.JoinBoard("user@example.com", 0);
            string result = factory.LeaveBoard("user@example.com", 0);
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void LeaveBoard_InvalidData_ShouldFail()
        {
            string result = factory.LeaveBoard("", -1);
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void GetBoardName_ValidId_ShouldReturnName()
        {
            factory.Register("user@example.com", "Password1");
            factory.CreateBoard("user@example.com", "MyBoard");
            string result = factory.GetBoardName(0);
            Assert.IsTrue(result.Contains("MyBoard"));
        }

        [TestMethod]
        public void GetBoardName_InvalidId_ShouldFail()
        {
            string result = factory.GetBoardName(-1);
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void TransferOwnership_ValidEmails_ShouldSucceed()
        {
            factory.Register("owner@example.com", "Password1");
            factory.Register("newowner@example.com", "Password1");
            factory.CreateBoard("owner@example.com", "Board1");
            factory.JoinBoard("newowner@example.com", 0);
            string result = factory.TransferOwnership("owner@example.com", "newowner@example.com", "Board1");
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void TransferOwnership_NullEmail_ShouldFail()
        {
            string result = factory.TransferOwnership(null, null, "Board1");
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }
    }
}