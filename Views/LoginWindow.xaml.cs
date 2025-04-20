using System.Windows;
using System.Windows.Input;
using TodoListApp.ViewModels;
using TodoListApp.Data;

namespace TodoListApp.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _viewModel;
        
        public LoginWindow()
        {
            InitializeComponent();
            var dbContext = new DatabaseContext();
            _viewModel = new LoginViewModel(dbContext);
            DataContext = _viewModel;
        }

        // Pencereyi sürüklemek için
        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                DragMove();
            }
        }

        // Pencereyi kapatmak için
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
} 