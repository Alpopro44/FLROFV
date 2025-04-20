using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Linq;

namespace TodoListApp.Models
{
    public class Todo : INotifyPropertyChanged
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        private bool _isCompleted;
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted != value)
                {
                    _isCompleted = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime ModifiedDate { get; set; } = DateTime.Now;
        
        // Görevin başlangıç ve bitiş tarihi/saati (farklı isimler kullanıldı)
        public DateTime? TaskStartTime { get; set; }
        
        public DateTime? TaskEndTime { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [MaxLength(50)]
        public string Category { get; set; }

        public Priority Priority { get; set; }
        
        // Görevin atandığı kullanıcı (Admin tarafından atanabilir)
        public int? AssignedToUserId { get; set; }
        
        [ForeignKey("AssignedToUserId")]
        public User AssignedToUser { get; set; }

        public ICollection<TodoTag> Tags { get; set; } = new List<TodoTag>();
        
        // Alt görevler
        public ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();
        
        // Alt görevlerin tamamlanma oranı
        [NotMapped]
        public int CompletionPercentage
        {
            get
            {
                if (SubTasks == null || SubTasks.Count == 0)
                    return IsCompleted ? 100 : 0;
                    
                int completedCount = SubTasks.Count(st => st.IsCompleted);
                return (int)Math.Round((double)completedCount / SubTasks.Count * 100);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TodoTag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TodoId { get; set; }

        [ForeignKey("TodoId")]
        public Todo Todo { get; set; }

        [Required]
        [MaxLength(50)]
        public string Tag { get; set; }
    }
} 