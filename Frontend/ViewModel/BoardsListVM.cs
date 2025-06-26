using IntroSE.Kanban.Frontend.Controllers;
using IntroSE.Kanban.Frontend.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IntroSE.Kanban.Frontend.ViewModel
{
    internal class BoardsListVM
    {
        public string Email => user.Email;
        public ObservableCollection<BoardModel> Boards { get; set; }
        public ICommand DeleteBoardCommand { get; }
        public ICommand OpenBoardCommand { get; }



        public ICommand AddBoardCommand { get; }

        private readonly UserModel user;

        public BoardsListVM(UserModel user)
        {
            this.user = user;
            Boards = new ObservableCollection<BoardModel>();
            LoadBoards();
            AddBoardCommand = new RelayCommand(_ => AddBoard());
            DeleteBoardCommand = new RelayCommand(board => DeleteBoard((BoardModel)board));
            OpenBoardCommand = new RelayCommand(board => OpenBoard((BoardModel)board));


        }

        private void LoadBoards()
        {
            Boards.Clear();

            try
            {
                var boardNames = ControllerFactory.Instance.BoardController.GetUserBoardNames(Email);

                foreach (var name in boardNames.Distinct()) 
                {
                    Boards.Add(new BoardModel(name, Email));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load boards: {ex.Message}");
            }
        }




        private void AddBoard()
        {
            var window = new View.AddBoardWindow();
            bool? result = window.ShowDialog();

            if (result == true && !string.IsNullOrWhiteSpace(window.BoardName))
            {
                try
                {
                    // קריאה לבאקאנד דרך הקונטרולר
                    ControllerFactory.Instance.BoardController.CreateBoard(Email, window.BoardName);

                    // טען מחדש מהדאטהבייס כדי למנוע כפילויות
                    LoadBoards();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to create board: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void DeleteBoard(BoardModel board)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete the board '{board.Name}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    ControllerFactory.Instance.BoardController.DeleteBoard(board.Owner, board.Name);
                    Boards.Remove(board);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete board: {ex.Message}");
                }
            }
        }
        private void OpenBoard(BoardModel board)
        {
            var window = new View.BoardView(board, user); // שלח גם את המשתמש
            window.ShowDialog();
        }




    }
}
