using System.Windows.Controls;
using TodoListApp.ViewModels;
using TodoListApp.Data;

namespace TodoListApp.Views
{
    public partial class MainView : UserControl
    {
        private readonly MainViewModel _viewModel;

        public MainView()
        {
            InitializeComponent();
            var dbContext = new DatabaseContext();
            _viewModel = new MainViewModel(dbContext, 0, "Misafir");
            DataContext = _viewModel;
        }
    }
} 