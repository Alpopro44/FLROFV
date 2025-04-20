using System;
using System.Collections.Generic;
using System.Windows;
using TodoListApp.Models;

namespace TodoListApp.Views
{
    public partial class EditTaskDialog : Window
    {
        public string Baslik { get; set; }
        public string Aciklama { get; set; }
        public string Kategori { get; set; }
        public TaskOncelik Oncelik { get; set; }
        public string Etiketler { get; set; }
        public DateTime? TaskStartTime { get; set; }
        public DateTime? TaskEndTime { get; set; }

        public List<string> Kategoriler { get; } = new List<string> { "İş", "Kişisel", "Alışveriş", "Sağlık", "Diğer" };
        public List<TaskOncelik> Oncelikler { get; } = new List<TaskOncelik> { TaskOncelik.Dusuk, TaskOncelik.Orta, TaskOncelik.Yuksek, TaskOncelik.Acil };

        public EditTaskDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void SetValues(string baslik, string aciklama, string kategori, TaskOncelik oncelik, List<string> etiketler, DateTime? startDateTime = null, DateTime? endDateTime = null)
        {
            BaslikTextBox.Text = baslik;
            AciklamaTextBox.Text = aciklama;
            KategoriComboBox.SelectedItem = kategori;
            OncelikComboBox.SelectedItem = oncelik;
            EtiketlerTextBox.Text = string.Join(", ", etiketler);
            
            // Başlangıç ve bitiş tarih/saatlerini ayarla
            if (startDateTime.HasValue)
            {
                StartDatePicker.SelectedDate = startDateTime.Value.Date;
                StartTimePicker.SelectedTime = startDateTime.Value;
            }
            
            if (endDateTime.HasValue)
            {
                EndDatePicker.SelectedDate = endDateTime.Value.Date;
                EndTimePicker.SelectedTime = endDateTime.Value;
            }
        }

        private void Kaydet_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BaslikTextBox.Text))
            {
                MessageBox.Show("Lütfen bir başlık girin.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Baslik = BaslikTextBox.Text;
            Aciklama = AciklamaTextBox.Text;
            Kategori = KategoriComboBox.SelectedItem as string;
            Oncelik = (TaskOncelik)OncelikComboBox.SelectedItem;
            Etiketler = EtiketlerTextBox.Text;
            
            // Başlangıç ve bitiş tarih/saatlerini al
            if (StartDatePicker.SelectedDate.HasValue)
            {
                DateTime startDate = StartDatePicker.SelectedDate.Value;
                TimeSpan? startTime = StartTimePicker.SelectedTime?.TimeOfDay;
                
                if (startTime.HasValue)
                {
                    TaskStartTime = startDate.Date.Add(startTime.Value);
                }
                else
                {
                    TaskStartTime = startDate.Date;
                }
            }
            
            if (EndDatePicker.SelectedDate.HasValue)
            {
                DateTime endDate = EndDatePicker.SelectedDate.Value;
                TimeSpan? endTime = EndTimePicker.SelectedTime?.TimeOfDay;
                
                if (endTime.HasValue)
                {
                    TaskEndTime = endDate.Date.Add(endTime.Value);
                }
                else
                {
                    TaskEndTime = endDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
                
                // Bitiş tarihi başlangıç tarihinden önce olmamalı
                if (TaskStartTime.HasValue && TaskEndTime.HasValue && 
                    TaskEndTime.Value < TaskStartTime.Value)
                {
                    MessageBox.Show("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.", "Uyarı", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            DialogResult = true;
            Close();
        }

        private void Iptal_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 