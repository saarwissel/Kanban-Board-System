using Kanban.Backend.ServiceLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using Kanban.Backend.DataAccessLayer;
using System.Linq;

using System.IO;


namespace Kanban.BackendTests
{
    [TestClass]
    public class UserTests
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
        public void Register_ValidUser_ShouldSucceed()
        {
            string result = factory.Register("test@example.com", "Password123");
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void Register_NullEmail_ShouldFail()
        {
            string result = factory.Register(null, "Password123");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void Register_DuplicateEmail_ShouldFail()
        {
            factory.Register("dup@example.com", "Password123");
            string result = factory.Register("dup@example.com", "Password456");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void Login_ShouldReturnEmailAsEntered()
        {
            string originalEmail = "MiXeDCaSe@Example.Com";
            factory.Register(originalEmail, "Password123");
            factory.Logout(originalEmail); // Ensure user is logged out before login test
            string result = factory.Login(originalEmail, "Password123");
            Console.WriteLine("Returned login result: " + result);

            Assert.IsTrue(result.Contains(originalEmail), "Expected email to be returned exactly as entered.");
        }

        [TestMethod]
        public void Login_ValidCredentials_ShouldSucceed()
        {
            factory.Register("login@example.com", "Password123");
            factory.Logout("login@example.com"); // Ensure user is logged out before login test
            string result = factory.Login("login@example.com", "Password123");
            Console.WriteLine("LOGIN RESULT: " + result);
            Assert.IsTrue(result.Contains("login@example.com"));
        }

        [TestMethod]
        public void Login_InvalidPassword_ShouldFail()
        {
            factory.Register("badlogin@example.com", "Password123");
            factory.Logout("badlogin@example.com"); // Ensure user is logged out before login test
            string result = factory.Login("badlogin@example.com", "WrongPassword");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void Login_NonExistentUser_ShouldFail()
        {
            string result = factory.Login("nouser@example.com", "AnyPassword");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }
        [TestMethod]
        public void Login_EmptyEmail_ShouldFail()
        {
            string result = factory.Login("", "Password123");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }
        [TestMethod]
        public void Login_EmptyPassword_ShouldFail()
        {
            factory.Register("emptyPass@example.com", "Password123");
            string result = factory.Login("emptyPass@example.com", "");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }


        [TestMethod]
        public void Login_SameUserTwice_ShouldFail()
        {
            factory.Register("repeat@example.com", "Password123");
            factory.Login("repeat@example.com", "Password123");
            string result = factory.Login("repeat@example.com", "Password123");
            Console.WriteLine("LOGIN (Duplicate) RESULT: " + result); // ← תוסיף את זה
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }

        [TestMethod]
        public void Logout_NonExistentUser_ShouldFail()
        {
            string result = factory.Logout("nouser@example.com");
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }




        [TestMethod]
        public void Logout_ValidUser_ShouldSucceed()
        {
            factory.Register("logout@example.com", "Password123");
            factory.Login("logout@example.com", "Password123");
            string result = factory.Logout("logout@example.com");
            Assert.IsTrue(result.Contains("null"));
        }

        [TestMethod]
        public void Logout_UserNotLoggedIn_ShouldFail()
        {
            factory.Register("notlogged@example.com", "Password123");
            string result = factory.Logout("notlogged@example.com");
            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"));
        }

        //new tests for GetUserBoards
        [TestMethod]
        public void GetUserBoards_UserWithNoBoards_ShouldReturnEmptyList()
        {
            factory.Register("user@example.com", "Password1");
            string result = factory.GetUserBoards("user@example.com");
            Assert.IsTrue(result.Contains("[]"));
        }


        [TestMethod]
        public void GetUserBoards_UserNotLoggedIn_ShouldFail()
        {
            factory.Register("user@example.com", "Password123");
            factory.Logout("user@example.com");

            string result = factory.GetUserBoards("user@example.com");
            Console.WriteLine("TEST RESULT (UserNotLoggedIn): " + result); // ← תראה בקונסול

            Assert.IsTrue(result.Contains("Error") || result.Contains("Exception"), "Expected failure when user is not logged in.");
        }
        //untill here

        [TestMethod]
        public void GetUserBoards_ValidUser_ShouldReturnBoards()
        {
            factory.Register("boards@example.com", "Password123");
            factory.Login("boards@example.com", "Password123");
            factory.CreateBoard("boards@example.com", "MyBoard");
            string result = factory.GetUserBoards("boards@example.com");
            Assert.IsTrue(result.Contains("0"));
        }

        [TestMethod]
        public void GetUserBoards_InvalidUser_ShouldFail()
        {
            string result = factory.GetUserBoards("ghost@example.com");
            Assert.IsTrue(result.Contains("error") || result.Contains("Exception"));
        }



        private void PrintTable(SqliteConnection connection, string tableName, params string[] columns)
        {
            string query = $"SELECT {string.Join(", ", columns)} FROM {tableName}";
            using (var command = new Microsoft.Data.Sqlite.SqliteCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                Console.WriteLine(new string('-', 70));
                Console.WriteLine(string.Join(" | ", columns));
                Console.WriteLine(new string('-', 70));
                while (reader.Read())
                {
                    List<string> values = new List<string>();
                    foreach (var col in columns)
                        values.Add(reader[col].ToString());
                    Console.WriteLine(string.Join(" | ", values));
                }
                Console.WriteLine(new string('-', 70));
            }
        }

        [TestMethod]
        public void LoadData_ShouldRestoreDataAfterSimulatedCrash()
        {
            string email = "restore@example.com";
            string password = "Password123";
            string boardName = "RestoredBoard";

            // Step 1: Register + Create Board
            string regResult = factory.Register(email, password);
            Console.WriteLine("Register: " + regResult);
            Assert.IsTrue(regResult.Contains("null"));

            string boardResult = factory.CreateBoard(email, boardName);
            Console.WriteLine("CreateBoard: " + boardResult);
            Assert.IsTrue(boardResult.Contains("null"));

            // Step 2: Simulate crash
            var newFactory = new ServiceFactory();

            // Step 3: Load data
            string loadResult = newFactory.LoadData();
            Console.WriteLine("LoadData: " + loadResult);
            Assert.IsTrue(loadResult.Contains("null"), "LoadData failed");

            // Step 4: Try login
            string loginResult = newFactory.Login(email, password);
            Console.WriteLine("Login after Load: " + loginResult);
            Assert.IsTrue(loginResult.Contains(email), "User was not restored from DB after LoadData.");
        }



[TestMethod]
public void LimitColumn_ShouldUpdateLimitInDatabaseProperly()
{
    string email = "limitcheck@example.com";
    string password = "Pass123!";
    string boardName = "MyBoard";
    int newLimit = 7;
    int columnOrdinal = 2; // done

    // Step 1: Register user
    string regResult = factory.Register(email, password);
    Console.WriteLine("Register: " + regResult);
    Assert.IsTrue(regResult.Contains("null"));

    // Step 2: Create board
    string createBoardResult = factory.CreateBoard(email, boardName);
    Console.WriteLine("CreateBoard: " + createBoardResult);
    Assert.IsTrue(createBoardResult.Contains("null"));

    // Step 3: Apply LimitColumn
    string limitResult = factory.LimitColumn(email, boardName, columnOrdinal, newLimit);
    Console.WriteLine("LimitColumn: " + limitResult);
    Assert.IsTrue(limitResult.Contains("null"));

    // Step 4: Verify directly from database
    var boardController = new Kanban.Backend.DataAccessLayer.BoardController();
    int boardId = boardController.SelectBoardsByUser(email)[0].ID;

    var columnController = new Kanban.Backend.DataAccessLayer.ColumnController();
    var columns = columnController.Select(boardId);

    int? actualLimit = columns
        .Where(c => c.ColumnID == "done")
        .Select(c => c.LimitCol)
        .FirstOrDefault();

    Console.WriteLine($"Expected Limit: {newLimit}, Actual Limit: {actualLimit}");
    Assert.AreEqual(newLimit, actualLimit);
}





        
        
        





    }
}