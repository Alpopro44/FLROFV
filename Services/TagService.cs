using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Data;
using TodoListApp.Models;

namespace TodoListApp.Services
{
    public class TagService
    {
        private readonly DatabaseContext _dbContext;

        public TagService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Tüm etiketleri getir
        public List<string> GetAllTags(int userId)
        {
            return _dbContext.TodoTags
                .Include(tt => tt.Todo)
                .Where(tt => tt.Todo.UserId == userId)
                .Select(tt => tt.Tag)
                .Distinct()
                .OrderBy(t => t)
                .ToList();
        }

        // Bir görevin etiketlerini getir
        public List<string> GetTagsForTodo(int todoId)
        {
            return _dbContext.TodoTags
                .Where(tt => tt.TodoId == todoId)
                .Select(tt => tt.Tag)
                .ToList();
        }

        // Bir göreve etiket ekle
        public void AddTagToTodo(int todoId, string tag)
        {
            // Etiketin zaten var olup olmadığını kontrol et
            var existingTag = _dbContext.TodoTags
                .FirstOrDefault(tt => tt.TodoId == todoId && tt.Tag == tag);

            if (existingTag == null)
            {
                var todoTag = new TodoTag
                {
                    TodoId = todoId,
                    Tag = tag
                };

                _dbContext.TodoTags.Add(todoTag);
                _dbContext.SaveChanges();
            }
        }

        // Bir görevden etiket kaldır
        public void RemoveTagFromTodo(int todoId, string tag)
        {
            var todoTag = _dbContext.TodoTags
                .FirstOrDefault(tt => tt.TodoId == todoId && tt.Tag == tag);

            if (todoTag != null)
            {
                _dbContext.TodoTags.Remove(todoTag);
                _dbContext.SaveChanges();
            }
        }

        // Bir görevin tüm etiketlerini güncelle
        public void UpdateTodoTags(int todoId, List<string> tags)
        {
            // Görevin mevcut etiketlerini getir
            var existingTags = _dbContext.TodoTags
                .Where(tt => tt.TodoId == todoId)
                .ToList();

            // Silinecek etiketleri bul ve kaldır
            foreach (var existingTag in existingTags)
            {
                if (!tags.Contains(existingTag.Tag))
                {
                    _dbContext.TodoTags.Remove(existingTag);
                }
            }

            // Yeni etiketleri ekle
            foreach (var tag in tags)
            {
                if (!existingTags.Any(et => et.Tag == tag))
                {
                    _dbContext.TodoTags.Add(new TodoTag
                    {
                        TodoId = todoId,
                        Tag = tag
                    });
                }
            }

            _dbContext.SaveChanges();
        }

        // Belirli bir etikete sahip görevleri getir
        public List<Todo> GetTodosByTag(int userId, string tag)
        {
            return _dbContext.TodoTags
                .Include(tt => tt.Todo)
                .Where(tt => tt.Tag == tag && tt.Todo.UserId == userId)
                .Select(tt => tt.Todo)
                .ToList();
        }
    }
} 