using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using Kanban.Backend.DataAccessLayer;
using System.Linq.Expressions;
using log4net;

using SQLitePCL;

namespace Kanban.Backend.DataAccessLayer;

public class BoardDAL
{

    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public int ID { get; private set; }
    public string Name { get; set; }
    public string OwnerEmail { get; set; }
    private BoardController boardController;
    private BoardsUsersController boardsUsersController;
    private ColmunsDAL backlogColumnsDAL;
    private ColmunsDAL inProgressColumnsDAL;
    private ColmunsDAL doneColumnsDAL;


    public string IDColumnName = "ID";
    public string NameColumnName = "Name";
    public string OwnerEmailColumnName = "OwnerEmail";


    public BoardDAL(int id, string name, string ownerEmail, string nulLL)
    {
        this.ID = id;
        this.Name = name;
        this.OwnerEmail = ownerEmail;
        this.backlogColumnsDAL = new ColmunsDAL(id, "backlog", -1); // Assuming 1 is the ID for Backlog column
        this.inProgressColumnsDAL = new ColmunsDAL(id, "in progress", -1); // Assuming 2 is the ID for In Progress column
        this.doneColumnsDAL = new ColmunsDAL(id, "done", -1); // Assuming 3 is the ID for Done column
        this.boardController = new BoardController();
        this.boardsUsersController = new BoardsUsersController();
    }








    public BoardDAL(int id, string name, string ownerEmail)
    {
        this.ID = id;
        this.Name = name;
        this.OwnerEmail = ownerEmail;
        this.backlogColumnsDAL = new ColmunsDAL(id, "backlog", -1); // Assuming 1 is the ID for Backlog column
        this.inProgressColumnsDAL = new ColmunsDAL(id, "in progress", -1); // Assuming 2 is the ID for In Progress column
        this.doneColumnsDAL = new ColmunsDAL(id, "done", -1); // Assuming 3 is the ID for Done column
        this.boardController = new BoardController();
        this.boardsUsersController = new BoardsUsersController();
        if (!backlogColumnsDAL.insert() || !inProgressColumnsDAL.insert() || !doneColumnsDAL.insert())
        {
            throw new Exception("Failed to presist colmuns");
        }
        if (!boardsUsersController.JoinUser(this.ID, this.OwnerEmail, "Owner"))
        {
            throw new Exception("Failed to join owner to board");
        }


    }
    public bool deleteBoard()
    {
        bool success = backlogColumnsDAL.DeleteColumnsByBoardID(this.ID);
        if (!success &&
                backlogColumnsDAL.ColumnController.HasColumnsForBoard(this.ID))
        {
            log.Error("Failed to delete columns for board.");
            return false;
        }

        bool deleteSuccess = boardsUsersController.DeleteBoard(this.ID);
        if (!deleteSuccess && boardsUsersController.HasUsersForBoard(this.ID))
        {
            log.Error("Failed to delete board-user relationships.");
            return false;
        }
        if (!boardController.Delete(this.ID))
        {
            log.Error("Failed to delete the board itself.");
            return false;
        }

        return true;
    }



    public bool JoinUser(string userEmail)
    {
        if (boardsUsersController.JoinUser(this.ID, userEmail, "Member"))
        {
            return true;
        }
        else
        {
            Console.WriteLine("Failed to join user to board.");
            return false;
        }
    }
    public bool LeaveUser(string userEmail)
    {
        if (boardsUsersController.LeaveUser(this.ID, userEmail))
        {
            return true;
        }
        else
        {
            Console.WriteLine("Failed to leave user from board.");
            return false;
        }
    }

    public bool UpdateOwner(string newEmail)
    {
        if (boardController.UpdateOwner(newEmail, this.ID))
        {
            this.OwnerEmail = newEmail;
            return true;
        }
        throw new Exception("Failed to update owner email");
    }

    public bool Presist()
    {
        return boardController.Presist(this);
    }
    public bool insertColumns()
    {
        if (backlogColumnsDAL.insert() && inProgressColumnsDAL.insert() && doneColumnsDAL.insert())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool UpdateColumnLimit(string columnName, int limit)
    {
        if (columnName == "backlog")
        {
            backlogColumnsDAL.LimitCol = limit;
            return backlogColumnsDAL.UpdateColumnLimit(this.ID, columnName, limit);
        }
        else if (columnName == "in progress")
        {
            inProgressColumnsDAL.LimitCol = limit;
            return inProgressColumnsDAL.UpdateColumnLimit(this.ID, columnName, limit);
        }
        else
        {
            doneColumnsDAL.LimitCol = limit;
            return doneColumnsDAL.UpdateColumnLimit(this.ID, columnName, limit);
        }
    }
    

    public bool TransferOwnership(int boardID, string currentOwnerEmail, string newOwnerEmail)
    {
        if (boardsUsersController.TransferOwnership(boardID, currentOwnerEmail, newOwnerEmail))
        {
            return true;
        }
        else
        {
            throw new Exception("Failed to transfer ownership");
        }
    }

}