using System;
using System.ComponentModel.DataAnnotations;

namespace TodoListApp.Models
{
    public enum LogType
    {
        // Temel log tipleri
        SystemError = 0,
        SystemInfo = 1,
        Login = 2,
        Logout = 3,
        Registration = 4,
        UserManagement = 5,
        
        // Kullanıcı işlemleri
        UserCreated = 6,
        UserUpdated = 7,
        UserDeleted = 8,
        
        // Todo işlemleri
        TodoCreated = 9,
        TodoAssigned = 10,
        TodoRead = 11,
        TodoUpdated = 12,
        TodoDeleted = 13,
        TodoCompleted = 14,
        
        // SubTask işlemleri
        SubTaskCreated = 15,
        SubTaskUpdated = 16,
        SubTaskDeleted = 17,
        SubTaskCompleted = 18
    }

    public class Log
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public LogType Type { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        public int? UserId { get; set; }
        
        public User User { get; set; }
        
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;
        
        public string IpAddress { get; set; }
        
        public string UserAgent { get; set; }
    }
} 