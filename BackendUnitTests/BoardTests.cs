using NUnit.Framework;
using Kanban.Backend.ServiceLayer;
using System;

namespace BackendUnitTests
{
    [TestFixture]
    public class BoardTests
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
        public void CreateBoard_ShouldSucceed()
        {
            factory.Register("board@example.com", "Password123");
            var result = factory.CreateBoard("board@example.com", "MyBoard");
            Assert.IsTrue(result.Contains("null"));
        }

        [Test]
        public void JoinBoard_ShouldSucceed()
        {
            factory.Register("owner@example.com", "Password123");
            factory.Register("member@example.com", "Password123");
            factory.CreateBoard("owner@example.com", "Board");

            var result = factory.JoinBoard("member@example.com", 0);
            Assert.IsTrue(result.Contains("null"));
        }

        [Test]
        public void LeaveBoard_ShouldRemoveMember()
        {
            factory.Register("owner@example.com", "Password123");
            factory.Register("member@example.com", "Password123");
            factory.CreateBoard("owner@example.com", "Board");
            factory.JoinBoard("member@example.com", 0);

            var result = factory.LeaveBoard("member@example.com", 0);
            Assert.IsTrue(result.Contains("null"));
        }

        [Test]
        public void GetBoardName_ShouldReturnCorrectName()
        {
            factory.Register("user@example.com", "Password123");
            factory.CreateBoard("user@example.com", "SpecialBoard");

            var name = factory.GetBoardName(0);
            Assert.IsTrue(name.Contains("SpecialBoard"));
        }

        [Test]
        public void DeleteBoard_ShouldSucceed()
        {
            factory.Register("user@example.com", "Password123");
            factory.CreateBoard("user@example.com", "TempBoard");

            var result = factory.DeleteBoard("user@example.com", "TempBoard");
            Assert.IsTrue(result.Contains("null"));
        }
    }
}
