using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TodoListApp.Data;
using TodoListApp.Models;
using TodoListApp.Services;

namespace TodoListApp.Views
{
    public partial class AddTodoWindow : Window
    {
        private readonly DatabaseContext _dbContext;
        private readonly LogService _logService;
        private readonly int _userId;
        private readonly bool _isAdmin;
        
        public string TodoTitle { get; private set; }
        
        public AddTodoWindow(int userId, bool isAdmin = false)
        {
            InitializeComponent();
            
            _dbContext = new DatabaseContext();
            _logService = new LogService(_dbContext);
            _userId = userId;
            _isAdmin = isAdmin;
            
            // Admin ise kullanıcı atama panelini göster ve kullanıcıları yükle
            if (_isAdmin)
            {
                AssignTaskPanel.Visibility = Visibility.Visible;
                LoadUsers();
            }
        }
        
        private void LoadUsers()
        {
            try
            {
                // Tüm kullanıcıları getir (giriş yapan admin hariç)
                var users = _dbContext.Users
                    .Where(u => u.Id != _userId)
                    .OrderBy(u => u.Username)
                    .ToList();
                
                AssignToUserComboBox.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcılar yüklenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                {
                    MessageBox.Show("Lütfen başlık alanını doldurunuz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Öncelik değerini al
                var priorityTag = (PriorityComboBox.SelectedItem as ComboBoxItem)?.Tag.ToString() ?? "Medium";
                var priority = Priority.Medium; // Varsayılan
                
                switch (priorityTag)
                {
                    case "Low":
                        priority = Priority.Low;
                        break;
                    case "High":
                        priority = Priority.High;
                        break;
                }
                
                // Kategori değerini al
                var category = (CategoryComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Diğer";
                
                // Yeni görevi oluştur
                var todo = new Todo
                {
                    Title = TitleTextBox.Text,
                    Description = DescriptionTextBox.Text,
                    Priority = priority,
                    Category = category,
                    UserId = _userId,
                    CreatedAt = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    IsCompleted = false
                };
                
                // Başlangıç ve bitiş tarih/saatlerini ayarla
                if (StartDatePicker.SelectedDate.HasValue)
                {
                    DateTime startDate = StartDatePicker.SelectedDate.Value;
                    TimeSpan? startTime = StartTimePicker.SelectedTime?.TimeOfDay;
                    
                    if (startTime.HasValue)
                    {
                        todo.TaskStartTime = startDate.Date.Add(startTime.Value);
                    }
                    else
                    {
                        todo.TaskStartTime = startDate.Date;
                    }
                }
                
                if (EndDatePicker.SelectedDate.HasValue)
                {
                    DateTime endDate = EndDatePicker.SelectedDate.Value;
                    TimeSpan? endTime = EndTimePicker.SelectedTime?.TimeOfDay;
                    
                    if (endTime.HasValue)
                    {
                        todo.TaskEndTime = endDate.Date.Add(endTime.Value);
                    }
                    else
                    {
                        todo.TaskEndTime = endDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }
                    
                    // Bitiş tarihi başlangıç tarihinden önce olmamalı
                    if (todo.TaskStartTime.HasValue && todo.TaskEndTime.HasValue && 
                        todo.TaskEndTime.Value < todo.TaskStartTime.Value)
                    {
                        MessageBox.Show("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.", "Uyarı", 
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                
                // Admin ise ve kullanıcı seçilmişse görevi ata
                if (_isAdmin && AssignToUserComboBox.SelectedItem != null)
                {
                    var selectedUser = AssignToUserComboBox.SelectedItem as User;
                    if (selectedUser != null)
                    {
                        todo.AssignedToUserId = selectedUser.Id;
                        
                        // Görevi atama işlemini logla
                        _logService.AddLog(
                            LogType.TodoAssigned,
                            $"Görev atandı: {todo.Title} -> {selectedUser.Username}",
                            _userId
                        );
                    }
                }
                
                // Veritabanına kaydet
                _dbContext.Todos.Add(todo);
                _dbContext.SaveChanges();
                
                // İşlemi logla
                _logService.AddLog(
                    LogType.TodoCreated,
                    $"Yeni görev oluşturuldu: {todo.Title}",
                    _userId
                );
                
                // İşlem başarılı, başlığı sakla ve pencereyi kapat
                TodoTitle = todo.Title;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Görev kaydedilirken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                
                // Hatayı logla
                _logService.AddLog(
                    LogType.SystemError,
                    $"Görev eklenirken hata: {ex.Message}",
                    _userId
                );
            }
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 