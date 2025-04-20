using System.Windows;
using TodoListApp.ViewModels;
using TodoListApp.Data;

namespace TodoListApp.Views
{
    public partial class UserManagementWindow : Window
    {
        private readonly UserManagementViewModel _viewModel;

        public UserManagementWindow()
        {
            InitializeComponent();
            var dbContext = new DatabaseContext();
            _viewModel = new UserManagementViewModel(dbContext);
            DataContext = _viewModel;
        }
    }
} 