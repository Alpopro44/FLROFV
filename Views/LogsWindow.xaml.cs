using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TodoListApp.Data;
using TodoListApp.Models;
using TodoListApp.Services;

namespace TodoListApp.Views
{
    public partial class LogsWindow : Window
    {
        private readonly DatabaseContext _dbContext;
        private readonly LogService _logService;
        private List<Log> _allLogs;
        private List<Log> _filteredLogs;
        private int _currentPage = 1;
        private int _pageSize = 25;
        private int _totalPages = 1;

        public LogsWindow()
        {
            InitializeComponent();
            
            _dbContext = new DatabaseContext();
            _logService = new LogService(_dbContext);
            
            // Varsayılan değerleri ayarla
            try 
            {
                if (LogTypeFilter != null)
                {
                    LogTypeFilter.SelectedIndex = 0; // "Tüm İşlemler"
                }
                
                if (PageSizeComboBox != null)
                {
                    PageSizeComboBox.SelectedIndex = 1; // "25"
                }
            }
            catch (Exception) { /* Hatayı sessizce geç */ }
            
            LoadLogs();
        }

        private void LoadLogs()
        {
            try
            {
                _allLogs = _logService.GetAllLogs();
                _filteredLogs = _allLogs;
                
                UpdatePagination();
                DisplayCurrentPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Log kayıtları yüklenirken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void UpdatePagination()
        {
            if (_filteredLogs == null || PageSizeComboBox == null)
                return;
                
            // PageSize değerini kontrol et
            if (PageSizeComboBox.SelectedItem is ComboBoxItem selectedPageSize)
            {
                if (selectedPageSize.Content.ToString() == "Tümü")
                {
                    _pageSize = _filteredLogs.Count > 0 ? _filteredLogs.Count : 1;
                }
                else
                {
                    int.TryParse(selectedPageSize.Content.ToString(), out _pageSize);
                    if (_pageSize <= 0) _pageSize = 25; // Varsayılan sayfa boyutu
                }
            }
            
            // Toplam sayfa sayısını hesapla
            _totalPages = (_filteredLogs.Count + _pageSize - 1) / _pageSize;
            if (_totalPages <= 0) _totalPages = 1;
            
            // Mevcut sayfayı kontrol et
            if (_currentPage > _totalPages)
                _currentPage = _totalPages;
            if (_currentPage <= 0)
                _currentPage = 1;
                
            // Sayfalama kontrollerini güncelle
            if (PageInfoTextBlock != null)
                PageInfoTextBlock.Text = $"{_currentPage} / {_totalPages}";
                
            if (PreviousPageButton != null)
                PreviousPageButton.IsEnabled = _currentPage > 1;
                
            if (NextPageButton != null)
                NextPageButton.IsEnabled = _currentPage < _totalPages;
                
            // Kayıt bilgilerini güncelle
            if (LogCountInfoTextBlock != null)
            {
                int visibleRecords = Math.Min(_pageSize, _filteredLogs.Count - ((_currentPage - 1) * _pageSize));
                LogCountInfoTextBlock.Text = $"Toplam {_filteredLogs.Count} kayıt, {visibleRecords} kayıt görüntüleniyor.";
            }
        }
        
        private void DisplayCurrentPage()
        {
            if (_filteredLogs == null || LogsDataGrid == null)
                return;
                
            int startIndex = (_currentPage - 1) * _pageSize;
            var currentPageLogs = _filteredLogs
                .Skip(startIndex)
                .Take(_pageSize)
                .ToList();
                
            LogsDataGrid.ItemsSource = currentPageLogs;
            
            UpdatePagination();
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            // Eğer kontroller henüz yüklenmemişse işlem yapma
            if (LogTypeFilter == null || StartDateFilter == null || EndDateFilter == null || LogsDataGrid == null)
                return;
                
            ApplyFilters();
        }
        
        private void PageSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentPage = 1; // Sayfa boyutu değiştiğinde ilk sayfaya dön
            UpdatePagination();
            DisplayCurrentPage();
        }
        
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                DisplayCurrentPage();
            }
        }
        
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                DisplayCurrentPage();
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }
        
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Anlık arama yapabilir veya Enter tuşuna basılınca aramayı aktif edebilirsiniz
            // Şu an için bir şey yapmıyoruz, button click ile arama yapacağız
        }
        
        private void ApplyFilters()
        {
            try
            {
                if (_allLogs == null || LogsDataGrid == null || LogTypeFilter == null)
                    return;
                    
                _filteredLogs = _allLogs;

                // İşlem tipi filtresi
                if (LogTypeFilter.SelectedItem is ComboBoxItem selectedType && selectedType.Tag.ToString() != "All")
                {
                    var logType = (LogType)Enum.Parse(typeof(LogType), selectedType.Tag.ToString());
                    _filteredLogs = _filteredLogs.Where(l => l.Type == logType).ToList();
                }

                // Tarih filtreleri - null check eklendi
                if (StartDateFilter != null && StartDateFilter.SelectedDate.HasValue)
                {
                    var startDate = StartDateFilter.SelectedDate.Value.Date;
                    _filteredLogs = _filteredLogs.Where(l => l.Timestamp.Date >= startDate).ToList();
                }

                if (EndDateFilter != null && EndDateFilter.SelectedDate.HasValue)
                {
                    var endDate = EndDateFilter.SelectedDate.Value.Date.AddDays(1).AddSeconds(-1); // Günün sonuna ayarla
                    _filteredLogs = _filteredLogs.Where(l => l.Timestamp <= endDate).ToList();
                }
                
                // Metin içeriği filtresi
                if (SearchTextBox != null && !string.IsNullOrWhiteSpace(SearchTextBox.Text))
                {
                    string searchText = SearchTextBox.Text.ToLower();
                    _filteredLogs = _filteredLogs.Where(l => 
                        l.Description.ToLower().Contains(searchText) || 
                        (l.User != null && l.User.Username.ToLower().Contains(searchText)) ||
                        l.Type.ToString().ToLower().Contains(searchText)
                    ).ToList();
                }

                // Filtreleme sonrası sayfalamayı sıfırla
                _currentPage = 1;
                UpdatePagination();
                DisplayCurrentPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Filtreler uygulanırken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            // Null kontrolleri eklendi
            try
            {
                if (LogTypeFilter != null)
                    LogTypeFilter.SelectedIndex = 0; // "Tüm İşlemler"
                    
                if (StartDateFilter != null)
                    StartDateFilter.SelectedDate = null;
                    
                if (EndDateFilter != null)
                    EndDateFilter.SelectedDate = null;
                    
                if (SearchTextBox != null)
                    SearchTextBox.Text = string.Empty;
                
                // Filtreleri sıfırla ve tüm kayıtları göster
                _filteredLogs = _allLogs;
                _currentPage = 1;
                UpdatePagination();
                DisplayCurrentPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Filtreler temizlenirken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 