using IntroSE.Kanban.Frontend.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IntroSE.Kanban.Frontend.ViewModel
{
    internal class LoginVM : INotifyPropertyChanged
    {
        private string email;
        private string password;
        public event Action? RequestClose;

        public string Email
        {
            get => email;
            set { email = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => password;
            set { password = value; OnPropertyChanged(); }
        }

        public string? LoggedInEmail { get; private set; }

        public ICommand LoginCommand { get; }
        public ICommand CancelCommand { get; }

        public LoginVM()
        {
            LoginCommand = new RelayCommand(_ => Login());
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke());
            Email = string.Empty;
            Password = string.Empty;
        }

        private void Login()
        {
            try
            {
                ControllerFactory.Instance.UserController.Login(Email, Password);
                LoggedInEmail = Email;
                MessageBox.Show($"Welcome back {LoggedInEmail}!");
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}");
                LoggedInEmail = null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
