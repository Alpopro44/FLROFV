using System.Windows;
using TodoListApp.ViewModels;
using TodoListApp.Data;
using TodoListApp.Models;
using TodoListApp.Services;
using System.Collections.Generic;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace TodoListApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly LogService _logService;
        private readonly int _userId;
        private readonly string _username;

        public MainWindow(int userId = 0, string username = "Misafir", UserRole role = UserRole.Standard)
        {
            InitializeComponent();
            var dbContext = new DatabaseContext();
            _logService = new LogService(dbContext);
            _userId = userId;
            _username = username;
            
            _viewModel = new MainViewModel(dbContext, userId, username, role);
            DataContext = _viewModel;
            
            // Admin kullanıcıları için yönetim menüsünü göster
            if (role != UserRole.Admin)
            {
                adminMenuItem.Visibility = Visibility.Collapsed;
            }
        }

        private void UserManagement_Click(object sender, RoutedEventArgs e)
        {
            var userManagementWindow = new UserManagementWindow();
            userManagementWindow.ShowDialog();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ayarlar yakında eklenecek!", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Yardım dokümantasyonu yakında eklenecek!", "Bilgi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Uygulamadan çıkmak istediğinize emin misiniz?", "Çıkış", 
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Çıkışı logla (eğer misafir değilse)
                if (_userId > 0)
                {
                    _logService.AddLog(
                        LogType.Logout, 
                        $"Kullanıcı çıkış yaptı: {_username}", 
                        _userId
                    );
                }
                
                Application.Current.Shutdown();
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is Todo todo)
            {
                try
                {
                    // İşaretlenme durumunu al
                    bool isChecked = checkBox.IsChecked ?? false;
                    
                    // Todo nesnesinin durumunu doğrudan güncelle
                    todo.IsCompleted = isChecked;
                    
                    // ViewModel'deki komutu çalıştır
                    if (DataContext is MainViewModel viewModel)
                    {
                        Console.WriteLine($"Ana görev işaretlendi - ID: {todo.Id}, Yeni durum: {isChecked}");
                        viewModel.CompleteTodoCommand.Execute(todo);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Görev durumu güncellenirken hata oluştu: {ex.Message}", "Hata", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    
                    // Hata durumunda checkbox'ı eski durumuna getir
                    checkBox.IsChecked = !checkBox.IsChecked;
                }
            }
        }
        
        // Alt görev tamamlama checkbox'ı için click olayı
        private void SubTaskCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is SubTask subTask)
            {
                try
                {
                    // ViewModel'e ulaş
                    if (DataContext is MainViewModel viewModel)
                    {
                        // Önce UI'daki seçilen durumu al
                        bool isChecked = checkBox.IsChecked ?? false;
                        
                        // SubTask nesnesinin durumunu manuel olarak güncelle
                        subTask.IsCompleted = isChecked;
                        
                        Console.WriteLine($"Alt görev checkbox tıklandı - ID: {subTask.Id}, Yeni durum: {isChecked}");
                        
                        // ViewModel üzerinden alt görevin durumunu kaydet
                        viewModel.UpdateSubTaskStatus(subTask, isChecked);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Alt görev durumu güncellenirken hata oluştu: {ex.Message}", "Hata", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    
                    // Hata durumunda checkbox durumunu geri al
                    checkBox.IsChecked = !checkBox.IsChecked;
                }
            }
        }
        
        private void LogsView_Click(object sender, RoutedEventArgs e)
        {
            var logsWindow = new LogsWindow();
            logsWindow.ShowDialog();
        }
        
        private void ReportsView_Click(object sender, RoutedEventArgs e)
        {
            var reportsWindow = new ReportsWindow();
            reportsWindow.ShowDialog();
        }
         
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var todo = button.CommandParameter as Todo;
                if (todo != null)
                {
                    var viewModel = (MainViewModel)DataContext;
                    viewModel.StartEditTodoCommand.Execute(todo);
                }
            }
        }
        
        private void TagInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var viewModel = (MainViewModel)DataContext;
                if (!string.IsNullOrWhiteSpace(viewModel.NewTagInput))
                {
                    viewModel.AddTagCommand.Execute(viewModel.NewTagInput);
                    e.Handled = true;
                }
            }
        }
        
        private void TagSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var viewModel = (MainViewModel)DataContext;
                if (!string.IsNullOrWhiteSpace(viewModel.SearchTagText))
                {
                    viewModel.FilterByTagText();
                    e.Handled = true;
                }
            }
        }
    }
} 