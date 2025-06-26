using IntroSE.Kanban.Frontend.Controllers;
using IntroSE.Kanban.Frontend.Model;
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
    internal class RegisterVM : INotifyPropertyChanged
    {
        private string email;
        private string password;
        public event Action? RequestClose;
        public string? RegisteredEmail { get; private set; }

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

        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }

        public RegisterVM()
        {
            RegisterCommand = new RelayCommand(_ => Register());
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke());
            Email = string.Empty;
            Password = string.Empty;
        }

        public UserModel? RegisteredUser { get; private set; }

        private void Register()
        {
            try
            {
                ControllerFactory.Instance.UserController.Register(Email, Password);
                RegisteredEmail = Email; 
                MessageBox.Show($"Welcome {RegisteredEmail}!");
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Registration failed: {ex.Message}");
                RegisteredEmail = null; 
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> execute;
        private readonly Predicate<object?>? canExecute;

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => execute(parameter);
        public event EventHandler? CanExecuteChanged;
    }
}

