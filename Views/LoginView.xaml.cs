using System.Windows;
using TodoListApp.ViewModels;
using TodoListApp.Data;

namespace TodoListApp.Views
{
    public partial class LoginView : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginView()
        {
            InitializeComponent();
            var dbContext = new DatabaseContext();
            _viewModel = new LoginViewModel(dbContext);
            DataContext = _viewModel;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox != null)
            {
                _viewModel.LoginCommand.Execute(PasswordBox);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterView();
            registerWindow.Show();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ExitCommand.Execute(null);
        }
    }
} 