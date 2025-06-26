using IntroSE.Kanban.Frontend.View;
using IntroSE.Kanban.Frontend.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace IntroSE.Kanban.Frontend.View
{
    public partial class Register : Window
    {
        private RegisterVM vm;

        public Register()
        {
            InitializeComponent();
            vm = new RegisterVM();
            vm.RequestClose += OnRegisterClose; // ← מתחבר לאירוע מה־ViewModel
            this.DataContext = vm;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RegisterVM viewModel)
                viewModel.Password = ((PasswordBox)sender).Password;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // או כל פעולה אחרת שתרצה בביטול
        }

        private void OnRegisterClose()
        {
            if (!string.IsNullOrEmpty(vm.RegisteredEmail))
            {
                var mainWindow = new MainWindow(new Model.UserModel(vm.Email)); 
                mainWindow.Show();
            }

            this.Close();
        }

    }
}
