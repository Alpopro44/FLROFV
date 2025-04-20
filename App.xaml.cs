using System;
using System.Windows;
using TodoListApp.Data;

namespace TodoListApp
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                // Veritabanı şemasını güncelleyelim
                using (var dbContext = new DatabaseContext())
                {
                    dbContext.EnsureRoleColumnExists();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Uygulama başlatılırken bir hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 