using System;
using TodoListApp.Data;

namespace TodoListApp.ViewModels
{
    public class ViewModelLocator
    {
        private static DatabaseContext _dbContext;
        
        public ViewModelLocator()
        {
            _dbContext = new DatabaseContext();
        }
        
        public MainViewModel MainViewModel
        {
            get
            {
                // Geçici bir varsayılan kullanıcı ID ve adı
                return new MainViewModel(_dbContext, 1, "Kullanıcı", Models.UserRole.Standard);
            }
        }
        
        public static void Cleanup()
        {
            _dbContext?.Dispose();
        }
    }
} 