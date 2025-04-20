using System.Windows;
using System.Windows.Input;
using TodoListApp.Data;
using TodoListApp.ViewModels;

namespace TodoListApp.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            DataContext = new RegisterViewModel(new DatabaseContext());
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