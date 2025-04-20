using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using TodoListApp.Data;
using TodoListApp.Models;
using TodoListApp.Services;
using System.Windows.Input;
using TodoListApp.Commands;

namespace TodoListApp.Views
{
    public partial class ReportsWindow : Window
    {
        private readonly DatabaseContext _dbContext;
        private int _currentUserId;
        private Func<ChartPoint, string> _pointLabel;
        private ICommand _viewTaskDetailsCommand;

        public ReportsWindow()
        {
            InitializeComponent();
            
            try
            {
                _dbContext = new DatabaseContext();
                
                // Grafik düğüm noktalarının etiketlerini hazırla
                _pointLabel = chartPoint => string.Format("{0} ({1:P1})", 
                    chartPoint.Y, chartPoint.Participation);
                
                // PointLabel özelliğine atama yap
                DataContext = this;
                TaskPieChart.Series[0].LabelPoint = _pointLabel;
                TaskPieChart.Series[1].LabelPoint = _pointLabel;
                
                // Kullanıcıları yükle
                LoadUsers();
                
                // Varsayılan değerleri ayarla
                UserPerformanceFilterComboBox.SelectedIndex = 0;
                
                // İlk analizleri hazırla
                LoadCategoryAnalysis();
                LoadUserPerformanceReport();
                
                // Varsayılan olarak tüm kullanıcıları göster
                ShowAllUsersReport();
                
                // Varsayılan olarak Genel Raporlar sekmesini seç
                ActivatePanel(GeneralReportsPanel);
                SetActiveButton(GeneralReportsButton);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Rapor sayfası yüklenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void LoadUsers()
        {
            try
            {
                var users = _dbContext.Users.OrderBy(u => u.Username).ToList();
                UserSelectionComboBox.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcılar yüklenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void UserSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (UserSelectionComboBox.SelectedItem is User selectedUser)
                {
                    _currentUserId = selectedUser.Id;
                    ShowUserReport(selectedUser);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı değiştirilirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void AllUsers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserSelectionComboBox.SelectedIndex = -1;
                ShowAllUsersReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tüm kullanıcı raporu gösterilirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void SearchUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchTerm = UserSearchTextBox.Text.Trim().ToLower();
                
                if (string.IsNullOrEmpty(searchTerm))
                {
                    MessageBox.Show("Lütfen arama terimi girin", "Uyarı", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                var user = _dbContext.Users.FirstOrDefault(u => 
                    u.Username.ToLower().Contains(searchTerm) || 
                    u.Email.ToLower().Contains(searchTerm));
                
                if (user != null)
                {
                    UserSelectionComboBox.SelectedItem = user;
                }
                else
                {
                    MessageBox.Show("Aradığınız kullanıcı bulunamadı", "Bilgi", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı aranırken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ShowUserReport(User user)
        {
            try
            {
                // Başlığı güncelle
                ReportTitleTextBlock.Text = $"{user.Username} - Görev İstatistikleri";
                
                // Kullanıcının görevlerini getir
                var userTasks = _dbContext.Todos.Where(t => t.UserId == user.Id).ToList();
                
                // İstatistikleri hesapla
                int totalTasks = userTasks.Count;
                int completedTasks = userTasks.Count(t => t.IsCompleted);
                int pendingTasks = totalTasks - completedTasks;
                
                // Alt görev sayısını hesapla
                int subTaskCount = _dbContext.SubTasks.Count(st => 
                    userTasks.Select(t => t.Id).Contains(st.TodoId));
                
                // İstatistik alanlarını güncelle
                TotalTasksTextBlock.Text = totalTasks.ToString();
                CompletedTasksTextBlock.Text = completedTasks.ToString();
                PendingTasksTextBlock.Text = pendingTasks.ToString();
                SubTasksTextBlock.Text = subTaskCount.ToString();
                
                // Pasta grafiğini güncelle
                UpdatePieChart(completedTasks, pendingTasks);
                
                // Görev listesini göster
                UserTasksListView.ItemsSource = userTasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı raporu oluşturulurken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ShowAllUsersReport()
        {
            try
            {
                // Başlığı güncelle
                ReportTitleTextBlock.Text = "Tüm Kullanıcılar - Görev İstatistikleri";
                
                // Tüm görevleri getir
                var allTasks = _dbContext.Todos.ToList();
                
                // İstatistikleri hesapla
                int totalTasks = allTasks.Count;
                int completedTasks = allTasks.Count(t => t.IsCompleted);
                int pendingTasks = totalTasks - completedTasks;
                
                // Alt görev sayısını hesapla
                int subTaskCount = _dbContext.SubTasks.Count();
                
                // İstatistik alanlarını güncelle
                TotalTasksTextBlock.Text = totalTasks.ToString();
                CompletedTasksTextBlock.Text = completedTasks.ToString();
                PendingTasksTextBlock.Text = pendingTasks.ToString();
                SubTasksTextBlock.Text = subTaskCount.ToString();
                
                // Pasta grafiğini güncelle
                UpdatePieChart(completedTasks, pendingTasks);
                
                // Görev listesini göster (son 20 görev)
                var orderedTasks = allTasks.OrderByDescending(t => t.ModifiedDate).Take(20).ToList();
                UserTasksListView.ItemsSource = orderedTasks;
                TaskListCountTextBlock.Text = $"(Son {orderedTasks.Count} görev)";
                
                // Varsayılan sıralama seçeneğini ayarla
                if (TaskSortingComboBox != null)
                {
                    TaskSortingComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tüm kullanıcı raporu oluşturulurken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void UpdatePieChart(int completedTasks, int pendingTasks)
        {
            try
            {
                // Gerekli kontroller
                if (completedTasks < 0) completedTasks = 0;
                if (pendingTasks < 0) pendingTasks = 0;
                
                // Pasta grafiği serileri güncelle
                TaskPieChart.Series[0].Values = new ChartValues<double> { completedTasks };
                TaskPieChart.Series[1].Values = new ChartValues<double> { pendingTasks };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Grafik güncellenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Yeni eklenen buton event işleyicileri
        private void GeneralReports_Click(object sender, RoutedEventArgs e)
        {
            ActivatePanel(GeneralReportsPanel);
            SetActiveButton(GeneralReportsButton);
        }
        
        private void UserReports_Click(object sender, RoutedEventArgs e)
        {
            ActivatePanel(UserReportsPanel);
            SetActiveButton(UserReportsButton);
        }
        
        private void CategoryReports_Click(object sender, RoutedEventArgs e)
        {
            ActivatePanel(CategoryReportsPanel);
            SetActiveButton(CategoryReportsButton);
        }
        
        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        // Panel aktivasyon metodu
        private void ActivatePanel(UIElement panelToActivate)
        {
            // Tüm panelleri gizle
            GeneralReportsPanel.Visibility = Visibility.Collapsed;
            UserReportsPanel.Visibility = Visibility.Collapsed;
            CategoryReportsPanel.Visibility = Visibility.Collapsed;
            
            // Seçili paneli göster
            panelToActivate.Visibility = Visibility.Visible;
        }
        
        // Aktif butonu ayarla
        private void SetActiveButton(Button activeButton)
        {
            // Tüm butonlara normal sekme stili uygula
            GeneralReportsButton.Style = (Style)FindResource("TabButtonStyle");
            UserReportsButton.Style = (Style)FindResource("TabButtonStyle");
            CategoryReportsButton.Style = (Style)FindResource("TabButtonStyle");
            
            // Aktif butona özel stil uygula
            activeButton.Style = (Style)FindResource("ActiveTabButtonStyle");
        }
        
        // Kategori analizi için verileri yükle
        private void LoadCategoryAnalysis()
        {
            try
            {
                // Kategorilere göre görev sayılarını al
                var categories = _dbContext.Todos
                    .GroupBy(t => t.Category)
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .Take(5)
                    .ToList();
                
                // Pie chart'ı temizle
                CategoryPieChart.Series.Clear();
                
                // Renk listesi
                var colors = new List<string> { "#2196F3", "#4CAF50", "#FFC107", "#9C27B0", "#FF5722" };
                
                // Her kategori için PieSeries ekle
                for (int i = 0; i < categories.Count; i++)
                {
                    var item = categories[i];
                    var series = new LiveCharts.Wpf.PieSeries
                    {
                        Title = item.Category,
                        Values = new LiveCharts.ChartValues<double> { item.Count },
                        DataLabels = true,
                        Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[i % colors.Count]))
                    };
                    
                    CategoryPieChart.Series.Add(series);
                }
                
                // Kategori tamamlanma oranları grafiği için verileri hazırla
                var completionRates = _dbContext.Todos
                    .GroupBy(t => t.Category)
                    .Select(g => new {
                        Category = g.Key,
                        CompletionRate = g.Count() > 0 ? (double)g.Count(t => t.IsCompleted) / g.Count() * 100 : 0
                    })
                    .OrderByDescending(x => x.CompletionRate)
                    .Take(5)
                    .ToList();
                
                // X ekseni etiketleri
                CategoryCompletionChart.AxisX[0].Labels = completionRates.Select(x => x.Category).ToList();
                
                // Verileri güncelle
                var columnSeries = CategoryCompletionChart.Series[0] as LiveCharts.Wpf.ColumnSeries;
                if (columnSeries != null)
                {
                    columnSeries.Values = new LiveCharts.ChartValues<double>(
                        completionRates.Select(x => Math.Round(x.CompletionRate, 1)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kategori analizi yüklenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Kullanıcı performans raporu için verileri yükle
        private void LoadUserPerformanceReport()
        {
            try
            {
                // Kullanıcı performans verileri hazırla
                var userPerformance = _dbContext.Users.Select(u => new {
                    User = u,
                    Tasks = _dbContext.Todos.Where(t => t.UserId == u.Id).ToList()
                }).ToList();
                
                // ViewModel listesi oluştur
                var performanceItems = userPerformance.Select(up => {
                    int totalTasks = up.Tasks.Count;
                    int completedTasks = up.Tasks.Count(t => t.IsCompleted);
                    int pendingTasks = totalTasks - completedTasks;
                    double completionRate = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;
                    
                    return new UserPerformanceViewModel {
                        UserId = up.User.Id,
                        Username = up.User.Username,
                        TotalTasks = totalTasks,
                        CompletedTasks = completedTasks,
                        PendingTasks = pendingTasks,
                        CompletionRate = completionRate,
                        CompletionRateText = $"{Math.Round(completionRate, 1)}%",
                        CompletionRateColor = GetCompletionRateColor(completionRate)
                    };
                }).ToList();
                
                // DataGrid'e verileri ata
                ApplyUserFilter(performanceItems);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı performans raporu yüklenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ApplyUserFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Kullanıcı performans verilerini yeniden yükle ve filtreyi uygula
                LoadUserPerformanceReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Filtre uygulanırken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void ApplyUserFilter(List<UserPerformanceViewModel> performanceItems)
        {
            try
            {
                // Seçilen filtre seçeneğine göre filtreleme yap
                var selectedFilter = UserPerformanceFilterComboBox.SelectedIndex;
                List<UserPerformanceViewModel> filteredItems;
                
                switch (selectedFilter)
                {
                    case 1: // En Yüksek Tamamlama Oranı
                        filteredItems = performanceItems
                            .Where(p => p.TotalTasks > 0) // Sadece görevi olanları göster
                            .OrderByDescending(p => p.CompletionRate)
                            .ToList();
                        break;
                    
                    case 2: // En Düşük Tamamlama Oranı
                        filteredItems = performanceItems
                            .Where(p => p.TotalTasks > 0) // Sadece görevi olanları göster
                            .OrderBy(p => p.CompletionRate)
                            .ToList();
                        break;
                    
                    case 3: // En Çok Göreve Sahip
                        filteredItems = performanceItems
                            .OrderByDescending(p => p.TotalTasks)
                            .ToList();
                        break;
                    
                    case 4: // En Az Göreve Sahip
                        filteredItems = performanceItems
                            .OrderBy(p => p.TotalTasks)
                            .ToList();
                        break;
                    
                    default: // Tüm Kullanıcılar (0)
                        filteredItems = performanceItems
                            .OrderBy(p => p.Username)
                            .ToList();
                        break;
                }
                
                // DataGrid'e filtrelenmiş verileri ata
                UserPerformanceDataGrid.ItemsSource = filteredItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı filtresi uygulanırken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Tamamlanma oranına göre renk belirle
        private SolidColorBrush GetCompletionRateColor(double rate)
        {
            if (rate >= 80)
                return new SolidColorBrush(Color.FromRgb(76, 175, 80)); // Yeşil
            else if (rate >= 50)
                return new SolidColorBrush(Color.FromRgb(255, 193, 7)); // Sarı
            else
                return new SolidColorBrush(Color.FromRgb(244, 67, 54)); // Kırmızı
        }
        
        // Görev sıralamasını değiştirme
        private void TaskSorting_Changed(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (UserTasksListView.ItemsSource == null) return;
                
                var tasks = new List<Todo>(UserTasksListView.ItemsSource.Cast<Todo>());
                int selectedIndex = TaskSortingComboBox.SelectedIndex;
                
                switch (selectedIndex)
                {
                    case 0: // Son Değişiklik
                        tasks = tasks.OrderByDescending(t => t.ModifiedDate).ToList();
                        break;
                    case 1: // Oluşturma
                        tasks = tasks.OrderByDescending(t => t.CreatedAt).ToList();
                        break;
                    case 2: // Alfabetik
                        tasks = tasks.OrderBy(t => t.Title).ToList();
                        break;
                    default:
                        break;
                }
                
                UserTasksListView.ItemsSource = tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Görev sıralama sırasında hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Görev detaylarını görüntüleme komutu
        public ICommand ViewTaskDetailsCommand
        {
            get
            {
                if (_viewTaskDetailsCommand == null)
                {
                    _viewTaskDetailsCommand = new RelayCommand<Todo>(ShowTaskDetails);
                }
                return _viewTaskDetailsCommand;
            }
        }
        
        // Görev detaylarını görüntüleme
        private void ShowTaskDetails(Todo task)
        {
            if (task == null) return;
            
            try
            {
                // Alt görev sayısını hesapla
                int subTaskCount = _dbContext.SubTasks.Count(st => st.TodoId == task.Id);
                
                // Görev detaylarını hazırla
                string category = string.IsNullOrEmpty(task.Category) ? "Kategorisiz" : task.Category;
                string status = task.IsCompleted ? "Tamamlandı" : "Bekliyor";
                string subTaskInfo = subTaskCount > 0 ? $"Alt Görev Sayısı: {subTaskCount}" : "Alt görev yok";
                
                // Detay mesajını oluştur
                string details = $"Başlık: {task.Title}\n\n" +
                              $"Açıklama: {(string.IsNullOrEmpty(task.Description) ? "-" : task.Description)}\n\n" +
                              $"Kategori: {category}\n" +
                              $"Durum: {status}\n" +
                              $"Öncelik: {task.Priority}\n\n" +
                              $"Oluşturulma: {task.CreatedAt.ToString("dd.MM.yyyy HH:mm")}\n" +
                              $"Son Güncelleme: {task.ModifiedDate.ToString("dd.MM.yyyy HH:mm")}\n\n" +
                              $"{subTaskInfo}";
                
                // Mesaj kutusunu göster
                MessageBox.Show(details, "Görev Detayları", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Görev detayları gösterilirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    
    // Kullanıcı Performans görünüm modeli
    public class UserPerformanceViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public double CompletionRate { get; set; }
        public string CompletionRateText { get; set; }
        public SolidColorBrush CompletionRateColor { get; set; }
    }
} 