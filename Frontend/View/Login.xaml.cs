using IntroSE.Kanban.Frontend.Model;
using IntroSE.Kanban.Frontend.View;
using IntroSE.Kanban.Frontend.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace IntroSE.Kanban.Frontend.View
{
    public partial class Login : Window
  
    {
        private LoginVM vm;

        public Login()
        {
            InitializeComponent();
            vm = new LoginVM();
            vm.RequestClose += OnLoginClose;
            this.DataContext = vm;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is LoginVM viewModel)
                viewModel.Password = ((PasswordBox)sender).Password;
        }

        private void OnLoginClose()
        {
            if (!string.IsNullOrEmpty(vm.LoggedInEmail))
            {
                var mainWindow = new MainWindow(new UserModel(vm.LoggedInEmail));
                mainWindow.Show();
            }

            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
