using Kanban.Backend.ServiceLayer;
using NUnit.Framework;
using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Kanban.Backend.DataAccessLayer;
using System.Linq;

namespace BackendUnitTests
{
    [TestFixture]
    public class UserTests
    {
        private ServiceFactory factory;

        [SetUp]
        public void Setup()
        {
            TestDatabaseInitializer.Initialize();
            factory = new ServiceFactory();
        }

        [TearDown]
        public void Cleanup() => factory.DeleteData();

        [Test]
        public void Register_ValidUser_ShouldSucceed()
        {
            var result = factory.Register("test@example.com", "Password123");
            Assert.IsTrue(result.Contains("null"));
        }

        [Test]
        public void Register_NullEmail_ShouldFail()
        {
            var result = factory.Register(null, "Password123");
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [Test]
        public void Register_DuplicateEmail_ShouldFail()
        {
            factory.Register("dup@example.com", "Password123");
            var result = factory.Register("dup@example.com", "Password456");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [Test]
        public void Login_ShouldReturnEmailAsEntered()
        {
            string email = "MiXeDCaSe@Example.Com";
            factory.Register(email, "Password123");
            factory.Logout(email);
            var result = factory.Login(email, "Password123");
            Assert.IsTrue(result.Contains(email));
        }

        [Test]
        public void Login_ValidCredentials_ShouldSucceed()
        {
            factory.Register("login@example.com", "Password123");
            factory.Logout("login@example.com");
            var result = factory.Login("login@example.com", "Password123");
            Assert.IsTrue(result.Contains("login@example.com"));
        }

        [Test]
        public void Login_InvalidPassword_ShouldFail()
        {
            factory.Register("badlogin@example.com", "Password123");
            factory.Logout("badlogin@example.com");
            var result = factory.Login("badlogin@example.com", "WrongPassword");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [Test]
        public void Login_NonExistentUser_ShouldFail()
        {
            var result = factory.Login("nouser@example.com", "AnyPassword");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [Test]
        public void Login_EmptyEmail_ShouldFail()
        {
            var result = factory.Login("", "Password123");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [Test]
        public void Login_EmptyPassword_ShouldFail()
        {
            factory.Register("emptyPass@example.com", "Password123");
            var result = factory.Login("emptyPass@example.com", "");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [Test]
        public void Login_SameUserTwice_ShouldFail()
        {
            factory.Register("repeat@example.com", "Password123");
            factory.Login("repeat@example.com", "Password123");
            var result = factory.Login("repeat@example.com", "Password123");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [Test]
        public void Logout_NonExistentUser_ShouldFail()
        {
            var result = factory.Logout("nouser@example.com");
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [Test]
        public void Logout_ValidUser_ShouldSucceed()
        {
            factory.Register("logout@example.com", "Password123");
            factory.Login("logout@example.com", "Password123");
            var result = factory.Logout("logout@example.com");
            Assert.IsTrue(result.Contains("null"));
        }

        [Test]
        public void Logout_UserNotLoggedIn_ShouldFail()
        {
            factory.Register("notlogged@example.com", "Password123");
            var result = factory.Logout("notlogged@example.com");
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [Test]
        public void GetUserBoards_UserWithNoBoards_ShouldReturnEmptyList()
        {
            factory.Register("user@example.com", "Password1");
            var result = factory.GetUserBoards("user@example.com");
            Assert.IsTrue(result.Contains("[]"));
        }

        [Test]
        public void GetUserBoards_UserNotLoggedIn_ShouldFail()
        {
            factory.Register("user@example.com", "Password123");
            factory.Logout("user@example.com");
            var result = factory.GetUserBoards("user@example.com");
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [Test]
        public void GetUserBoards_ValidUser_ShouldReturnBoards()
        {
            factory.Register("boards@example.com", "Password123");
            factory.Login("boards@example.com", "Password123");
            factory.CreateBoard("boards@example.com", "MyBoard");
            var result = factory.GetUserBoards("boards@example.com");
            Assert.IsTrue(result.Contains("0"));
        }

        [Test]
        public void GetUserBoards_InvalidUser_ShouldFail()
        {
            var result = factory.GetUserBoards("ghost@example.com");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [Test]
        public void LoadData_ShouldRestoreDataAfterSimulatedCrash()
        {
            string email = "restore@example.com", password = "Password123", boardName = "RestoredBoard";
            Assert.IsTrue(factory.Register(email, password).Contains("null"));
            Assert.IsTrue(factory.CreateBoard(email, boardName).Contains("null"));

            var newFactory = new ServiceFactory();
            Assert.IsTrue(newFactory.LoadData().Contains("null"));
            Assert.IsTrue(newFactory.Login(email, password).Contains(email));
        }

        [Test]
        public void LoadData_ShouldRestoreJoinedBoardsForUser()
        {
            string ownerEmail = "owner@example.com", memberEmail = "member@example.com", password = "Password123";

            factory.Register(ownerEmail, password);
            factory.Register(memberEmail, password);

            for (int i = 1; i <= 5; i++)
                Assert.IsTrue(factory.CreateBoard(ownerEmail, $"Board{i}").Contains("null"));

            for (int boardId = 0; boardId < 5; boardId++)
                Assert.IsTrue(factory.JoinBoard(memberEmail, boardId).Contains("null"));

            var newFactory = new ServiceFactory();
            Assert.IsTrue(newFactory.LoadData().Contains("null"));
            Assert.IsTrue(newFactory.Login(ownerEmail, password).Contains(ownerEmail));
            Assert.IsTrue(newFactory.Login(memberEmail, password).Contains(memberEmail));

            var boardsResult = newFactory.GetUserBoards(memberEmail);
            Assert.IsTrue(boardsResult.Contains("0") && boardsResult.Contains("4"));
        }

        [Test]
        public void LeaveBoard_ShouldReflectCorrectlyAfterLoadData()
        {
            string owner = "owner@example.com", member = "member@example.com", pass = "Password123";
            factory.Register(owner, pass);
            factory.Register(member, pass);

            for (int i = 1; i <= 5; i++)
                Assert.IsTrue(factory.CreateBoard(owner, $"Board{i}").Contains("null"));

            for (int boardId = 0; boardId < 4; boardId++)
                Assert.IsTrue(factory.JoinBoard(member, boardId).Contains("null"));

            Assert.IsTrue(factory.LeaveBoard(member, 2).Contains("null"));

            var newFactory = new ServiceFactory();
            Assert.IsTrue(newFactory.LoadData().Contains("null"));
            Assert.IsTrue(newFactory.Login(owner, pass).Contains(owner));
            Assert.IsTrue(newFactory.Login(member, pass).Contains(member));

            string memberBoards = newFactory.GetUserBoards(member);
            Assert.IsTrue(memberBoards.Contains("0"));
            Assert.IsTrue(memberBoards.Contains("1"));
            Assert.IsTrue(memberBoards.Contains("3"));
            Assert.IsFalse(memberBoards.Contains("2"));
        }

        [Test]
        public void TransferOwnership_NonExistentBoard_ShouldFail()
        {
            factory.Register("owner@example.com", "Password123");
            factory.Register("newowner@example.com", "Password123");

            var result = factory.TransferOwnership("owner@example.com", "newowner@example.com", "NonExistentBoard");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [Test]
        public void TransferOwnership_UserNotOwner_ShouldFail()
        {
            factory.Register("owner@example.com", "Password123");
            factory.Register("user@example.com", "Password123");
            factory.CreateBoard("owner@example.com", "MyBoard");

            var result = factory.TransferOwnership("user@example.com", "owner@example.com", "MyBoard");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [Test]
        public void TransferOwnership_TargetUserNotMember_ShouldFail()
        {
            factory.Register("owner@example.com", "Password123");
            factory.Register("stranger@example.com", "Password123");
            factory.CreateBoard("owner@example.com", "MyBoard");

            var result = factory.TransferOwnership("owner@example.com", "stranger@example.com", "MyBoard");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [Test]
        public void TransferOwnership_SameUser_ShouldFail()
        {
            factory.Register("owner@example.com", "Password123");
            factory.CreateBoard("owner@example.com", "MyBoard");

            var result = factory.TransferOwnership("owner@example.com", "owner@example.com", "MyBoard");
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [Test]
        public void TransferOwnership_InvalidEmail_ShouldFail()
        {
            factory.Register("owner@example.com", "Password123");
            factory.CreateBoard("owner@example.com", "MyBoard");

            var result = factory.TransferOwnership("owner@example.com", "", "MyBoard");
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        [Test]
        public void TransferOwnership_NotLoggedIn_ShouldFail()
        {
            factory.Register("owner@example.com", "Password123");
            factory.Register("newowner@example.com", "Password123");
            factory.CreateBoard("owner@example.com", "MyBoard");
            factory.Logout("owner@example.com");

            var result = factory.TransferOwnership("owner@example.com", "newowner@example.com", "MyBoard");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }
    }
}
