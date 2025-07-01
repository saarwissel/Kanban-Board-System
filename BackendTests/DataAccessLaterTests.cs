using System;
using System.Collections.Generic;
using Kanban.Backend.DataAccessLayer;

namespace Kanban.BackendTests
{
    public class DataAccessLaterTests
    {
        private UserController userController = new UserController();
        private BoardController boardController = new BoardController();

        public void RunAllTests()
        {
            Console.WriteLine("== Running Data Access Layer Tests ==");

            InitializeData();

            // UserController tests
            if (CheckUserController())
                Console.WriteLine("UserController tests completed successfully.\n");
            if (TestUserControllerEdgeCases())
                Console.WriteLine("UserController edge cases passed.\n");

            // BoardController tests
            Console.WriteLine("== Running BoardController Tests ==");
            if (CheckBoardController())
                Console.WriteLine("BoardController tests completed successfully.\n");
            if (TestBoardControllerEdgeCases())
                Console.WriteLine("BoardController edge cases passed.\n");
        }

        private void InitializeData()
        {
            // clear any existing data before each run
            userController.DeleteAll();
            boardController.DeleteAll();
        }

        /// <summary>
        /// Happy-path tests for UserController
        /// </summary>
        private bool CheckUserController()
        {
            bool success = true;

            // 1. initially empty
            var initial = userController.AllUsers();
            if (initial == null || initial.Count != 0)
            {
                Console.WriteLine($"[User] Expected 0 users but got {initial?.Count}");
                success = false;
            }

            // 2. persist a valid user
            var validUser = new UserDAL("valid@test.com", "Pass123");
            if (!userController.Persist(validUser))
            {
                Console.WriteLine("[User] Persist(valid user) failed.");
                success = false;
            }

            // 3. retrieve
            var listAfter = userController.AllUsers();
            if (listAfter == null || listAfter.Count != 1 || listAfter[0].Email != "valid@test.com")
            {
                Console.WriteLine("[User] Retrieved user list does not match persisted.");
                success = false;
            }

            // 4. HasRecords should now be true
            if (!userController.HasRecords())
            {
                Console.WriteLine("[User] HasRecords returned false after persisting.");
                success = false;
            }

            // 5. delete all
            if (!userController.DeleteAll())
            {
                Console.WriteLine("[User] DeleteAll failed.");
                success = false;
            }

            // 6. verify empty again
            var listFinal = userController.AllUsers();
            if (listFinal == null || listFinal.Count != 0 || userController.HasRecords())
            {
                Console.WriteLine($"[User] Expected 0 after delete but got {listFinal?.Count}");
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Edge-case tests for UserController
        /// </summary>
        private bool TestUserControllerEdgeCases()
        {
            bool passed = true;

            // Persist null
            try
            {
                bool result = userController.Persist(null);
                if (result)
                {
                    Console.WriteLine("[User] Persist(null) should return false.");
                    passed = false;
                }
            }
            catch
            {
                Console.WriteLine("[User] Persist(null) threw an exception.");
                passed = false;
            }

            // DeleteAll when empty
            userController.DeleteAll(); // ensure empty
            if (!userController.DeleteAll())
            {
                Console.WriteLine("[User] DeleteAll on empty should return true.");
                passed = false;
            }

            // HasRecords when empty
            if (userController.HasRecords())
            {
                Console.WriteLine("[User] HasRecords on empty should return false.");
                passed = false;
            }

            // AllUsers when empty
            var emptyList = userController.AllUsers();
            if (emptyList == null)
            {
                Console.WriteLine("[User] AllUsers on empty should not return null.");
                passed = false;
            }

            return passed;
        }

        /// <summary>
        /// Happy-path tests for BoardController
        /// </summary>
        private bool CheckBoardController()
        {
            bool success = true;
            const string owner = "owner@test.com";

            // 1. initially no boards for this user
            var initial = boardController.SelectBoardsByUser(owner);
            if (initial == null || initial.Count != 0)
            {
                Console.WriteLine($"[Board] Expected 0 boards but got {initial?.Count}");
                success = false;
            }

            // 2. persist a valid board
            var board = new BoardDAL(1, "Test Board", owner);
            if (!boardController.Presist(board))
            {
                Console.WriteLine("[Board] Persist(valid board) failed.");
                success = false;
            }

            // 3. retrieve
            var listAfter = boardController.SelectBoardsByUser(owner);
            if (listAfter == null || listAfter.Count != 1 ||
                listAfter[0].Name != "Test Board" ||
                listAfter[0].OwnerEmail != owner)
            {
                Console.WriteLine("[Board] Retrieved boards do not match persisted.");
                success = false;
            }

            // 4. HasRecords should now be true
            if (!boardController.HasRecords())
            {
                Console.WriteLine("[Board] HasRecords returned false after persisting.");
                success = false;
            }

            // 5. delete all
            if (!boardController.DeleteAll())
            {
                Console.WriteLine("[Board] DeleteAll failed.");
                success = false;
            }

            // 6. verify empty again
            var listFinal = boardController.SelectBoardsByUser(owner);
            if (listFinal == null || listFinal.Count != 0 || boardController.HasRecords())
            {
                Console.WriteLine($"[Board] Expected 0 after delete but got {listFinal?.Count}");
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Edge-case tests for BoardController
        /// </summary>
        private bool TestBoardControllerEdgeCases()
        {
            bool passed = true;

            // Persist null
            try
            {
                bool result = boardController.Insert(null);
                if (result)
                {
                    Console.WriteLine("[Board] Insert(null) should return false.");
                    passed = false;
                }
            }
            catch
            {
                Console.WriteLine("[Board] Insert(null) threw an exception.");
                passed = false;
            }

            // DeleteAll when empty
            boardController.DeleteAll(); // ensure empty
            if (!boardController.DeleteAll())
            {
                Console.WriteLine("[Board] DeleteAll on empty should return true.");
                passed = false;
            }

            // HasRecords when empty
            if (boardController.HasRecords())
            {
                Console.WriteLine("[Board] HasRecords on empty should return false.");
                passed = false;
            }

            // SelectBoardsByUser when empty
            var emptyList = boardController.SelectBoardsByUser("nobody@test.com");
            if (emptyList == null)
            {
                Console.WriteLine("[Board] SelectBoardsByUser on empty should not return null.");
                passed = false;
            }

            return passed;
        }
    }
}
