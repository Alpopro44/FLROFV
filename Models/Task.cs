using System;
using System.Collections.Generic;

namespace TodoListApp.Models
{
    public class Task
    {
        public int ID { get; set; }
        public string Baslik { get; set; }
        public string Aciklama { get; set; }
        public bool Tamamlandi { get; set; }
        public DateTime OlusturulmaTarihi { get; set; }
        public int KullaniciID { get; set; }
        public string Kategori { get; set; }
        public TaskOncelik Oncelik { get; set; }
        public string Durum { get; set; }
        public List<string> Etiketler { get; set; } = new List<string>();
    }

    public enum TaskOncelik
    {
        Dusuk = 0,
        Orta = 1,
        Yuksek = 2,
        Acil = 3
    }
} 