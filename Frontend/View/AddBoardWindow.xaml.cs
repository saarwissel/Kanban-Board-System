using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IntroSE.Kanban.Frontend.View
{
    /// <summary>
    /// Interaction logic for AddBoardWindow.xaml
    /// </summary>
    public partial class AddBoardWindow : Window
    {
        public string BoardName { get; private set; } = string.Empty;
        public AddBoardWindow()
        {
            InitializeComponent();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            BoardName = BoardNameBox.Text.Trim();

            if (string.IsNullOrEmpty(BoardName))
            {
                MessageBox.Show("Board name cannot be empty.");
                return;
            }

            this.DialogResult = true;
            this.Close();
        }
    }

}








