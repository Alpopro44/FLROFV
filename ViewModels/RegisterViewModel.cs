using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using TodoListApp.Commands;
using TodoListApp.Data;
using TodoListApp.Models;
using TodoListApp.Views;

namespace TodoListApp.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly DatabaseContext _dbContext;
        private string _username;
        private string _password;
        private string _email;
        private string _errorMessage;
        private bool _hasError;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                HasError = !string.IsNullOrEmpty(value);
            }
        }

        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                OnPropertyChanged(nameof(HasError));
            }
        }

        public ICommand RegisterCommand { get; }
        public ICommand BackToLoginCommand { get; }

        public RegisterViewModel(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            RegisterCommand = new RelayCommand<PasswordBox>(Register);
            BackToLoginCommand = new RelayCommand(BackToLogin);
        }

        private void Register(PasswordBox passwordBox)
        {
            try
            {
                ErrorMessage = null;

                if (string.IsNullOrWhiteSpace(Username))
                {
                    ErrorMessage = "Lütfen kullanıcı adı giriniz.";
                    return;
                }

                if (passwordBox == null || string.IsNullOrWhiteSpace(passwordBox.Password))
                {
                    ErrorMessage = "Lütfen şifre giriniz.";
                    return;
                }

                var confirmPasswordBox = GetConfirmPasswordBox();
                if (confirmPasswordBox == null || passwordBox.Password != confirmPasswordBox.Password)
                {
                    ErrorMessage = "Şifreler eşleşmiyor!";
                    return;
                }

                if (_dbContext.Users.Any(u => u.Username == Username))
                {
                    ErrorMessage = "Bu kullanıcı adı zaten kullanılıyor!";
                    return;
                }

                var user = new User
                {
                    Username = Username,
                    Password = passwordBox.Password,
                    Email = Email
                };

                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();

                MessageBox.Show("Kayıt başarıyla tamamlandı!", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
                BackToLogin();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Kayıt olurken bir hata oluştu: {ex.Message}";
            }
        }

        private PasswordBox GetConfirmPasswordBox()
        {
            if (Application.Current.Windows[0] is RegisterWindow registerWindow)
            {
                return registerWindow.FindName("ConfirmPasswordBox") as PasswordBox;
            }
            return null;
        }

        private void BackToLogin()
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            if (Application.Current.Windows[0] is Window currentWindow)
            {
                currentWindow.Close();
            }
        }
    }
} 