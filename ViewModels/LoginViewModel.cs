using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using TodoListApp.Commands;
using TodoListApp.Data;
using TodoListApp.Models;
using TodoListApp.Views;
using TodoListApp.Services;

namespace TodoListApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly DatabaseContext _dbContext;
        private readonly LogService _logService;
        private string _username;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _logService = new LogService(dbContext);
            LoginCommand = new RelayCommand<PasswordBox>(Login);
            ExitCommand = new RelayCommand(Exit);
            RegisterCommand = new RelayCommand(OpenRegisterWindow);
        }

        private void OpenRegisterWindow()
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Show();
            
            // Mevcut pencereyi kapat
            if (Application.Current.Windows[0] is Window currentWindow)
            {
                currentWindow.Close();
            }
        }

        private void Login(PasswordBox passwordBox)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username) || passwordBox == null || string.IsNullOrWhiteSpace(passwordBox.Password))
                {
                    MessageBox.Show("Lütfen kullanıcı adı ve şifre giriniz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var user = _dbContext.Users.FirstOrDefault(u => 
                    u.Username == Username && u.Password == passwordBox.Password);

                if (user == null)
                {
                    _logService.AddLog(
                        LogType.Login, 
                        $"Başarısız giriş denemesi. Kullanıcı adı: {Username}"
                    );
                    MessageBox.Show("Kullanıcı adı veya şifre hatalı!", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Başarılı giriş logla
                _logService.AddLog(
                    LogType.Login, 
                    $"Başarılı giriş. Kullanıcı adı: {user.Username}", 
                    user.Id
                );

                var mainWindow = new MainWindow(user.Id, user.Username, user.Role);
                mainWindow.Show();

                // Mevcut pencereyi kapat
                if (Application.Current.Windows[0] is Window currentWindow)
                {
                    currentWindow.Close();
                }
            }
            catch (Exception ex)
            {
                _logService.AddLog(
                    LogType.SystemError, 
                    $"Giriş yapılırken hata: {ex.Message}"
                );
                MessageBox.Show($"Giriş yapılırken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }
    }
} 