using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TodoListApp.Models
{
    public enum UserRole
    {
        Standard = 0,
        Admin = 1
    }

    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Password { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Email { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public UserRole Role { get; set; } = UserRole.Standard;
        
        public ICollection<Todo> Todos { get; set; } = new List<Todo>();
    }
} 