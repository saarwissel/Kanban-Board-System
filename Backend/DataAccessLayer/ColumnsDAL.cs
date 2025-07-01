
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;
using System.Data.SQLite;

using Kanban.Backend.DataAccessLayer;


namespace Kanban.Backend.DataAccessLayer
{
    public class ColmunsDAL
    {
        public int BoardID { get; set; }
        public string ColumnID { get; set; }
        public int? LimitCol { get; set; }
        private ColumnController columnController;
        public ColumnController ColumnController
        {
            get { return columnController; }
            set { columnController = value; }
        }

        public string BoardIDColumnName = "BoardID";
        public string ColumnIDColumnName = "ColumnName";
        public string LimitColumnName = "TaskLimit";

        public ColmunsDAL(int boardID, string columnID, int? limitCol)
        {
            this.BoardID = boardID;
            this.ColumnID = columnID;
            this.LimitCol = limitCol;
            this.columnController = new ColumnController();
        }

        public bool insert()
        {
            if (columnController.Insert(this))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Failed to insert column.");
                return false;
            }
        }
        public bool DeleteColumnsByBoardID(int boardID)
        {
            if (columnController.DeleteColumnsByBoardID(boardID))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Failed to delete columns for the board.");
                return true;
            }

        }

        public bool UpdateColumnLimit(int boardID, string columnID, int limitCol)
        {
            if (columnController.UpdateColumnLimit(boardID, columnID, limitCol))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Failed to update column limit.");
                return false;
            }
        }

    }
}