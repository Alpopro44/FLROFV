using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Data;
using TodoListApp.Models;

namespace TodoListApp.Services
{
    public class SubTaskService
    {
        private readonly DatabaseContext _dbContext;
        private readonly LogService _logService;

        public SubTaskService(DatabaseContext dbContext, LogService logService)
        {
            _dbContext = dbContext;
            _logService = logService;
        }

        // Alt görevleri bir todo için getirme
        public List<SubTask> GetSubTasksForTodo(int todoId)
        {
            return _dbContext.SubTasks
                .Where(st => st.TodoId == todoId)
                .OrderBy(st => st.Order)
                .ToList();
        }

        // Yeni alt görev ekleme
        public SubTask AddSubTask(int todoId, string title)
        {
            var todo = _dbContext.Todos.Find(todoId);
            if (todo == null)
                throw new Exception("Görev bulunamadı.");

            var maxOrder = _dbContext.SubTasks
                .Where(st => st.TodoId == todoId)
                .Select(st => (int?)st.Order)
                .Max() ?? 0;

            var subTask = new SubTask
            {
                Title = title,
                IsCompleted = false,
                CreatedAt = DateTime.Now,
                ModifiedDate = DateTime.Now,
                TodoId = todoId,
                Order = maxOrder + 1
            };

            _dbContext.SubTasks.Add(subTask);
            _dbContext.SaveChanges();

            _logService.AddLog(
                LogType.SubTaskCreated,
                $"Alt görev oluşturuldu: {subTask.Title} (Ana görev: {todo.Title})",
                todo.UserId
            );

            return subTask;
        }

        // Alt görev güncelleme
        public void UpdateSubTask(SubTask subTask)
        {
            var existingSubTask = _dbContext.SubTasks
                .Include(st => st.Todo)
                .FirstOrDefault(st => st.Id == subTask.Id);

            if (existingSubTask == null)
                throw new Exception("Alt görev bulunamadı.");

            existingSubTask.Title = subTask.Title;
            existingSubTask.IsCompleted = subTask.IsCompleted;
            existingSubTask.ModifiedDate = DateTime.Now;

            _dbContext.SubTasks.Update(existingSubTask);
            _dbContext.SaveChanges();

            _logService.AddLog(
                LogType.SubTaskUpdated,
                $"Alt görev güncellendi: {existingSubTask.Title}",
                existingSubTask.Todo.UserId
            );

            // Ana görev tamamlanma durumunu kontrol et
            UpdateParentTaskCompletionStatus(existingSubTask.TodoId);
        }

        // Alt görevin durumunu doğrudan belirtilen değere ayarla
        public void UpdateSubTaskStatus(int subTaskId, bool isCompleted)
        {
            try
            {
                var subTask = _dbContext.SubTasks
                    .Include(st => st.Todo)
                    .FirstOrDefault(st => st.Id == subTaskId);

                if (subTask == null)
                {
                    throw new Exception($"Alt görev bulunamadı. ID: {subTaskId}");
                }

                // Değişiklik yalnızca durum farklıysa yapılır
                if (subTask.IsCompleted != isCompleted)
                {
                    subTask.IsCompleted = isCompleted;
                    subTask.ModifiedDate = DateTime.Now;

                    // Değişiklikleri loglama
                    Console.WriteLine($"UpdateSubTaskStatus: SubTask ID: {subTask.Id}, IsCompleted değişti: {subTask.IsCompleted}");
                    
                    _dbContext.SubTasks.Update(subTask);
                    int rowsAffected = _dbContext.SaveChanges();
                    
                    Console.WriteLine($"SaveChanges gerçekleşti. Etkilenen satır sayısı: {rowsAffected}");
                    
                    if (rowsAffected > 0)
                    {
                        // Log kaydı
                        _logService.AddLog(
                            LogType.SubTaskUpdated,
                            $"Alt görev durumu güncellendi: {subTask.Title} - {(isCompleted ? "Tamamlandı" : "Tamamlanmadı")}",
                            subTask.Todo.UserId
                        );

                        // Ana görev tamamlanma durumunu kontrol et
                        UpdateParentTaskCompletionStatus(subTask.TodoId);
                    }
                }
                else
                {
                    Console.WriteLine($"UpdateSubTaskStatus: Durum zaten {isCompleted}, güncelleme yapılmadı");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Alt görev durumu güncellenirken hata: {ex.Message}");
                throw;
            }
        }

        // Alt görev güncelleme
        public void ToggleSubTaskCompletion(int subTaskId)
        {
            try
            {
                var subTask = _dbContext.SubTasks
                    .Include(st => st.Todo)
                    .FirstOrDefault(st => st.Id == subTaskId);

                if (subTask == null)
                {
                    System.Windows.MessageBox.Show($"Alt görev bulunamadı. ID: {subTaskId}", "Hata", 
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    throw new Exception($"Alt görev bulunamadı. ID: {subTaskId}");
                }

                subTask.IsCompleted = !subTask.IsCompleted;
                subTask.ModifiedDate = DateTime.Now;

                try
                {
                    // Changes tracking etkinleştir
                    _dbContext.ChangeTracker.DetectChanges();
                    
                    // Değişiklikleri loglama
                    Console.WriteLine($"SubTask ID: {subTask.Id}, IsCompleted değişti: {subTask.IsCompleted}");
                    
                    _dbContext.SubTasks.Update(subTask);
                    int rowsAffected = _dbContext.SaveChanges();
                    
                    Console.WriteLine($"SaveChanges gerçekleşti. Etkilenen satır sayısı: {rowsAffected}");
                    
                    if (rowsAffected > 0)
                    {
                        // Başarılı kayıt log'u
                        _logService.AddLog(
                            LogType.SubTaskUpdated,
                            $"Alt görev durumu değiştirildi: {subTask.Title} - {(subTask.IsCompleted ? "Tamamlandı" : "Tamamlanmadı")}",
                            subTask.Todo.UserId
                        );

                        // Ana görev tamamlanma durumunu kontrol et
                        UpdateParentTaskCompletionStatus(subTask.TodoId);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Alt görev durumu veritabanında güncellenemedi.", "Uyarı", 
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Alt görev güncellenirken veritabanı hatası: {ex.Message}", "Veritabanı Hatası", 
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    throw;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Alt görev durumu değiştirilirken hata oluştu: {ex.Message}", "Hata", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                throw;
            }
        }

        // Alt görev silme
        public void DeleteSubTask(int subTaskId)
        {
            var subTask = _dbContext.SubTasks
                .Include(st => st.Todo)
                .FirstOrDefault(st => st.Id == subTaskId);

            if (subTask == null)
                throw new Exception("Alt görev bulunamadı.");

            var todoId = subTask.TodoId;
            var userId = subTask.Todo.UserId;
            var title = subTask.Title;

            _dbContext.SubTasks.Remove(subTask);
            _dbContext.SaveChanges();

            _logService.AddLog(
                LogType.SubTaskDeleted,
                $"Alt görev silindi: {title}",
                userId
            );

            // Diğer alt görevlerin sırasını güncelle
            ReorderSubTasks(todoId);
            
            // Ana görev tamamlanma durumunu kontrol et
            UpdateParentTaskCompletionStatus(todoId);
        }

        // Alt görevleri yeniden sıralama
        public void ReorderSubTasks(int todoId)
        {
            var subTasks = _dbContext.SubTasks
                .Where(st => st.TodoId == todoId)
                .OrderBy(st => st.Order)
                .ToList();

            for (int i = 0; i < subTasks.Count; i++)
            {
                subTasks[i].Order = i + 1;
                _dbContext.SubTasks.Update(subTasks[i]);
            }

            _dbContext.SaveChanges();
        }

        // Alt görevin sırasını değiştirme
        public void ChangeSubTaskOrder(int subTaskId, int newOrder)
        {
            var subTask = _dbContext.SubTasks.Find(subTaskId);
            if (subTask == null)
                throw new Exception("Alt görev bulunamadı.");

            var subTasks = _dbContext.SubTasks
                .Where(st => st.TodoId == subTask.TodoId)
                .OrderBy(st => st.Order)
                .ToList();

            // Geçerli sıra aralığını kontrol et
            if (newOrder < 1 || newOrder > subTasks.Count)
                throw new Exception("Geçersiz sıra numarası.");

            // Mevcut sırayı kaydet
            int oldOrder = subTask.Order;

            // Sıralamayı güncelle
            if (oldOrder < newOrder)
            {
                // Aşağıya taşıma
                foreach (var st in subTasks.Where(st => st.Order > oldOrder && st.Order <= newOrder))
                {
                    st.Order--;
                    _dbContext.SubTasks.Update(st);
                }
            }
            else if (oldOrder > newOrder)
            {
                // Yukarıya taşıma
                foreach (var st in subTasks.Where(st => st.Order >= newOrder && st.Order < oldOrder))
                {
                    st.Order++;
                    _dbContext.SubTasks.Update(st);
                }
            }

            // Hedef öğenin sırasını güncelle
            subTask.Order = newOrder;
            _dbContext.SubTasks.Update(subTask);

            _dbContext.SaveChanges();
        }

        // Ana görevin tamamlanma durumunu güncelleme
        private void UpdateParentTaskCompletionStatus(int todoId)
        {
            var todo = _dbContext.Todos.Find(todoId);
            if (todo == null) return;

            var subTasks = _dbContext.SubTasks
                .Where(st => st.TodoId == todoId)
                .ToList();

            // Alt görev yoksa durumu değiştirme
            if (subTasks.Count == 0) return;

            // Tüm alt görevler tamamlandıysa, ana görevi de tamamla
            bool allCompleted = subTasks.All(st => st.IsCompleted);
            
            if (todo.IsCompleted != allCompleted)
            {
                todo.IsCompleted = allCompleted;
                todo.ModifiedDate = DateTime.Now;
                
                _dbContext.Todos.Update(todo);
                _dbContext.SaveChanges();

                _logService.AddLog(
                    LogType.TodoUpdated,
                    $"Görev durumu otomatik güncellendi: {todo.Title} - {(todo.IsCompleted ? "Tamamlandı" : "Tamamlanmadı")}",
                    todo.UserId
                );
            }
        }
    }
} 