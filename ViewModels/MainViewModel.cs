using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using TodoListApp.Models;
using TodoListApp.Data;
using TodoListApp.Commands;
using System.Linq;
using TodoListApp.Views;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Services;

namespace TodoListApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private Todo _selectedTodo;
        private bool _isEditMode;
        private string _editTodoTitle;
        private string _editTodoDescription;
        private string _editTodoCategory;
        private Priority _editTodoPriority;
        private DateTime? _editTodoStartDate;
        private DateTime? _editTodoStartTime;
        private DateTime? _editTodoEndDate;
        private DateTime? _editTodoEndTime;
        private readonly DatabaseContext _dbContext;
        private readonly LogService _logService;
        private ObservableCollection<Todo> _todos;
        private string _selectedCategory;
        private Priority _selectedPriority;
        private string _selectedIsCompleted;
        private string _searchText;
        private string _username;
        private int _userId;
        private bool _isAdmin;
        private UserRole _userRole;
        private bool _isNewMode;
        private bool _isAssignPanelVisible;
        private string _assignTodoTitle;
        private string _assignTodoDescription;
        private string _assignTodoCategory;
        private Priority _assignTodoPriority;
        private DateTime? _assignTodoStartDate;
        private DateTime? _assignTodoStartTime;
        private DateTime? _assignTodoEndDate;
        private DateTime? _assignTodoEndTime;
        private User _selectedUser;
        private ObservableCollection<User> _users;
        private bool _isAddPanelVisible;
        private string _newTodoTitle;
        private string _newTodoDescription;
        private string _newTodoCategory;
        private Priority _newTodoPriority;
        private DateTime? _newTodoStartDate;
        private DateTime? _newTodoStartTime;
        private DateTime? _newTodoEndDate;
        private DateTime? _newTodoEndTime;
        private readonly TagService _tagService;
        private ObservableCollection<string> _tags;
        private string _selectedTag;
        private ObservableCollection<string> _editTodoTags;
        private ObservableCollection<string> _newTodoTags;
        private ObservableCollection<string> _assignTodoTags;
        private bool _isFilteringByTag;
        private string _newTagInput;
        private string _searchTagText;
        
        // Alt görev alanları
        private ObservableCollection<SubTask> _subTasks;
        private SubTask _selectedSubTask;
        private string _newSubTaskTitle;
        private readonly SubTaskService _subTaskService;

        public bool IsAssignPanelVisible
        {
            get => _isAssignPanelVisible;
            set
            {
                _isAssignPanelVisible = value;
                OnPropertyChanged(nameof(IsAssignPanelVisible));
            }
        }

        public string AssignTodoTitle
        {
            get => _assignTodoTitle;
            set
            {
                _assignTodoTitle = value;
                OnPropertyChanged(nameof(AssignTodoTitle));
            }
        }

        public string AssignTodoDescription
        {
            get => _assignTodoDescription;
            set
            {
                _assignTodoDescription = value;
                OnPropertyChanged(nameof(AssignTodoDescription));
            }
        }

        public string AssignTodoCategory
        {
            get => _assignTodoCategory;
            set
            {
                _assignTodoCategory = value;
                OnPropertyChanged(nameof(AssignTodoCategory));
            }
        }

        public Priority AssignTodoPriority
        {
            get => _assignTodoPriority;
            set
            {
                _assignTodoPriority = value;
                OnPropertyChanged(nameof(AssignTodoPriority));
            }
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public UserRole UserRole
        {
            get => _userRole;
            set
            {
                _userRole = value;
                IsAdmin = value == UserRole.Admin;
                OnPropertyChanged(nameof(UserRole));
                OnPropertyChanged(nameof(IsAdmin));
            }
        }

        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged(nameof(IsAdmin));
            }
        }

        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(WelcomeMessage));
            }
        }

        public string WelcomeMessage => $"Hoş Geldin {Username}!";

        public ObservableCollection<Todo> Todos
        {
            get => _todos;
            set
            {
                _todos = value;
                OnPropertyChanged(nameof(Todos));
            }
        }

        public Todo SelectedTodo
        {
            get => _selectedTodo;
            set
            {
                _selectedTodo = value;
                OnPropertyChanged(nameof(SelectedTodo));
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
            }
        }

        public string EditTodoTitle
        {
            get => _editTodoTitle;
            set
            {
                _editTodoTitle = value;
                OnPropertyChanged(nameof(EditTodoTitle));
            }
        }

        public string EditTodoDescription
        {
            get => _editTodoDescription;
            set
            {
                _editTodoDescription = value;
                OnPropertyChanged(nameof(EditTodoDescription));
            }
        }

        public string EditTodoCategory
        {
            get => _editTodoCategory;
            set
            {
                _editTodoCategory = value;
                OnPropertyChanged(nameof(EditTodoCategory));
            }
        }

        public Priority EditTodoPriority
        {
            get => _editTodoPriority;
            set
            {
                _editTodoPriority = value;
                OnPropertyChanged(nameof(EditTodoPriority));
            }
        }

        public DateTime? EditTodoStartDate
        {
            get => _editTodoStartDate;
            set
            {
                _editTodoStartDate = value;
                OnPropertyChanged(nameof(EditTodoStartDate));
            }
        }

        public DateTime? EditTodoStartTime
        {
            get => _editTodoStartTime;
            set
            {
                _editTodoStartTime = value;
                OnPropertyChanged(nameof(EditTodoStartTime));
            }
        }

        public DateTime? EditTodoEndDate
        {
            get => _editTodoEndDate;
            set
            {
                _editTodoEndDate = value;
                OnPropertyChanged(nameof(EditTodoEndDate));
            }
        }

        public DateTime? EditTodoEndTime
        {
            get => _editTodoEndTime;
            set
            {
                _editTodoEndTime = value;
                OnPropertyChanged(nameof(EditTodoEndTime));
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                FilterTodos();
            }
        }

        public Priority SelectedPriority
        {
            get => _selectedPriority;
            set
            {
                _selectedPriority = value;
                OnPropertyChanged(nameof(SelectedPriority));
                FilterTodos();
            }
        }

        public string SelectedIsCompleted
        {
            get => _selectedIsCompleted;
            set
            {
                _selectedIsCompleted = value;
                OnPropertyChanged(nameof(SelectedIsCompleted));
                FilterTodos();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterTodos();
            }
        }

        public ObservableCollection<string> Categories { get; private set; }
        public ObservableCollection<Priority> Priorities { get; private set; }
        public ObservableCollection<Priority> EditPriorities { get; private set; }
        public ObservableCollection<string> CompletionStatuses { get; private set; }

        public ICommand AddTodoCommand { get; private set; }
        public ICommand StartEditTodoCommand { get; private set; }
        public ICommand DeleteTodoCommand { get; private set; }
        public ICommand SaveEditTodoCommand { get; private set; }
        public ICommand CancelEditTodoCommand { get; private set; }
        public TodoListApp.Commands.RelayCommand<Todo> CompleteTodoCommand { get; private set; }
        public ICommand AssignTodoCommand { get; private set; }
        public ICommand ShowAssignPanelCommand { get; private set; }
        public ICommand CloseAssignPanelCommand { get; private set; }
        public ICommand ShowAddPanelCommand { get; private set; }
        public ICommand CloseAddPanelCommand { get; private set; }
        public ICommand SaveNewTodoCommand { get; private set; }
        public ICommand AddTagCommand { get; private set; }
        public ICommand RemoveTagCommand { get; private set; }
        public ICommand ClearTagFilterCommand { get; private set; }
        public ICommand FilterTodosCommand { get; private set; }
        
        // Alt görev komutları
        public ICommand AddSubTaskCommand { get; private set; }
        public ICommand DeleteSubTaskCommand { get; private set; }
        public ICommand ToggleSubTaskCompletionCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel(DatabaseContext dbContext, int userId, string username, UserRole role = UserRole.Standard)
        {
            _dbContext = dbContext;
            _logService = new LogService(dbContext);
            _tagService = new TagService(dbContext);
            _subTaskService = new SubTaskService(dbContext, _logService);
            UserId = userId;
            Username = username;
            UserRole = role;
            
            LoadCategories();
            LoadPriorities();
            LoadCompletionStatuses();
            LoadTags();

            if (IsAdmin)
            {
                LoadUsers();
            }

            AddTodoCommand = new RelayCommand(ShowAddPanel);
            StartEditTodoCommand = new RelayCommand<Todo>(StartEditTodo);
            DeleteTodoCommand = new RelayCommand<Todo>(DeleteTodo);
            SaveEditTodoCommand = new RelayCommand(SaveEditTodo);
            CancelEditTodoCommand = new RelayCommand(CancelEditTodo);
            CompleteTodoCommand = new TodoListApp.Commands.RelayCommand<Todo>(CompleteTodo);
            ShowAssignPanelCommand = new RelayCommand(ShowAssignPanel);
            CloseAssignPanelCommand = new RelayCommand(CloseAssignPanel);
            AssignTodoCommand = new RelayCommand(AssignTodo);
            ShowAddPanelCommand = new RelayCommand(ShowAddPanel);
            CloseAddPanelCommand = new RelayCommand(CloseAddPanel);
            SaveNewTodoCommand = new RelayCommand(SaveNewTodo);
            AddTagCommand = new RelayCommand<string>(AddTag);
            RemoveTagCommand = new RelayCommand<string>(RemoveTag);
            ClearTagFilterCommand = new RelayCommand(ClearTagFilter);
            FilterTodosCommand = new RelayCommand(FilterTodos);
            
            // Alt görev komutları
            AddSubTaskCommand = new RelayCommand(AddSubTask, CanAddSubTask);
            DeleteSubTaskCommand = new RelayCommand<SubTask>(DeleteSubTask);
            ToggleSubTaskCompletionCommand = new RelayCommand<SubTask>(ToggleSubTaskCompletion);

            FilterTodos();
        }

        private void LoadCategories()
        {
            var categories = _dbContext.Todos
                .Where(t => t.UserId == UserId)
                .Select(t => t.Category)
                .Distinct()
                .ToList();
            
            var defaultCategories = new List<string> { "İş", "Kişisel", "Alışveriş", "Eğitim", "Sağlık", "Diğer" };
            
            foreach (var category in defaultCategories)
            {
                if (!categories.Contains(category))
                {
                    categories.Add(category);
                }
            }
            
            categories = categories.OrderBy(c => c).ToList();
            categories.Insert(0, "Hepsi");
            
            Categories = new ObservableCollection<string>(categories);
            SelectedCategory = "Hepsi";
        }

        private void LoadPriorities()
        {
            var filterPriorities = Enum.GetValues(typeof(Priority))
                .Cast<Priority>()
                .OrderBy(p => (int)p)
                .ToList();
            
            var editPriorities = Enum.GetValues(typeof(Priority))
                .Cast<Priority>()
                .Where(p => p != Priority.All)
                .OrderBy(p => (int)p)
                .ToList();
            
            Priorities = new ObservableCollection<Priority>(filterPriorities);
            EditPriorities = new ObservableCollection<Priority>(editPriorities);
            SelectedPriority = Priority.All;
        }

        private void LoadCompletionStatuses()
        {
            CompletionStatuses = new ObservableCollection<string> { "Hepsi", "Tamamlanmadı", "Tamamlandı" };
            SelectedIsCompleted = "Hepsi";
        }

        private void LoadUsers()
        {
            try
            {
                var users = _dbContext.Users
                    .Where(u => u.Id != UserId)
                    .OrderBy(u => u.Username)
                    .ToList();
                
                Users = new ObservableCollection<User>(users);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcılar yüklenirken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterTodos()
        {
            try
            {
                var query = _dbContext.Todos
                    .Include(t => t.Tags)
                    .Where(t => t.AssignedToUserId == UserId || (t.UserId == UserId && t.AssignedToUserId == null));

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    query = query.Where(t => t.Title.Contains(SearchText) || t.Description.Contains(SearchText));
                }

                if (SelectedCategory != "Hepsi")
                {
                    query = query.Where(t => t.Category == SelectedCategory);
                }

                if (SelectedPriority != Priority.All)
                {
                    query = query.Where(t => t.Priority == SelectedPriority);
                }

                if (SelectedIsCompleted != "Hepsi")
                {
                    var isCompleted = SelectedIsCompleted == "Tamamlandı";
                    query = query.Where(t => t.IsCompleted == isCompleted);
                }

                var todos = query.ToList();
                Todos = new ObservableCollection<Todo>(todos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Görevler filtrelenirken bir hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddNewTodo()
        {
            ShowAddPanel();
        }

        private void StartEditTodo(Todo todo)
        {
            if (todo == null) return;

            SelectedTodo = todo;
            EditTodoTitle = todo.Title;
            EditTodoDescription = todo.Description;
            EditTodoCategory = todo.Category;
            EditTodoPriority = todo.Priority;
            
            // Tarih ve saat bilgilerini yükle
            if (todo.TaskStartTime.HasValue)
            {
                EditTodoStartDate = todo.TaskStartTime.Value.Date;
                EditTodoStartTime = todo.TaskStartTime.Value;
            }
            else
            {
                EditTodoStartDate = null;
                EditTodoStartTime = null;
            }
            
            if (todo.TaskEndTime.HasValue)
            {
                EditTodoEndDate = todo.TaskEndTime.Value.Date;
                EditTodoEndTime = todo.TaskEndTime.Value;
            }
            else
            {
                EditTodoEndDate = null;
                EditTodoEndTime = null;
            }
            
            // Etiketleri yükle
            var tags = _tagService.GetTagsForTodo(todo.Id);
            EditTodoTags = new ObservableCollection<string>(tags);
            
            // Alt görevleri yükle
            LoadSubTasks();
            
            IsEditMode = true;
        }

        private void SaveEditTodo()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EditTodoTitle))
                {
                    MessageBox.Show("Lütfen başlık alanını doldurunuz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                if (IsNewMode)
                {
                    var newTodo = new Todo
                    {
                        Title = EditTodoTitle,
                        Description = EditTodoDescription,
                        Category = EditTodoCategory ?? "Genel",
                        Priority = EditTodoPriority,
                        UserId = UserId,
                        CreatedAt = DateTime.Now,
                        ModifiedDate = DateTime.Now,
                        IsCompleted = false
                    };
                    
                    _dbContext.Todos.Add(newTodo);
                    _dbContext.SaveChanges();
                    
                    _logService.AddLog(
                        LogType.TodoCreated,
                        $"Yeni görev oluşturuldu: {newTodo.Title}",
                        UserId
                    );
                    
                    IsNewMode = false;
                }
                else if (SelectedTodo != null)
                {
                    string originalTitle = SelectedTodo.Title;
                                        
                    SelectedTodo.Title = EditTodoTitle;
                    SelectedTodo.Description = EditTodoDescription;
                    SelectedTodo.Category = EditTodoCategory;
                    SelectedTodo.Priority = EditTodoPriority;
                    SelectedTodo.ModifiedDate = DateTime.Now;
                    
                    // Tarih ve saat bilgilerini güncelle
                    if (EditTodoStartDate.HasValue)
                    {
                        DateTime startDate = EditTodoStartDate.Value;
                        TimeSpan? startTime = EditTodoStartTime?.TimeOfDay;
                        
                        if (startTime.HasValue)
                        {
                            SelectedTodo.TaskStartTime = startDate.Date.Add(startTime.Value);
                        }
                        else
                        {
                            SelectedTodo.TaskStartTime = startDate.Date;
                        }
                    }
                    else
                    {
                        SelectedTodo.TaskStartTime = null;
                    }
                    
                    if (EditTodoEndDate.HasValue)
                    {
                        DateTime endDate = EditTodoEndDate.Value;
                        TimeSpan? endTime = EditTodoEndTime?.TimeOfDay;
                        
                        if (endTime.HasValue)
                        {
                            SelectedTodo.TaskEndTime = endDate.Date.Add(endTime.Value);
                        }
                        else
                        {
                            SelectedTodo.TaskEndTime = endDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                        }
                        
                        // Bitiş tarihi başlangıç tarihinden önce olmamalı
                        if (SelectedTodo.TaskStartTime.HasValue && SelectedTodo.TaskEndTime.HasValue && 
                            SelectedTodo.TaskEndTime.Value < SelectedTodo.TaskStartTime.Value)
                        {
                            MessageBox.Show("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.", "Uyarı", 
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                    else
                    {
                        SelectedTodo.TaskEndTime = null;
                    }

                    _dbContext.Todos.Update(SelectedTodo);
                    _dbContext.SaveChanges();

                    // Etiketleri güncelle
                    _tagService.UpdateTodoTags(SelectedTodo.Id, EditTodoTags.ToList());

                    _logService.AddLog(
                        LogType.TodoUpdated,
                        $"Görev güncellendi: {originalTitle} -> {SelectedTodo.Title}",
                        UserId
                    );
                }
                
                IsEditMode = false;
                FilterTodos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Görev güncellenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                
                _logService.AddLog(
                    LogType.SystemError,
                    $"Görev güncellenirken hata: {ex.Message}",
                    UserId
                );
            }
        }

        private void CancelEditTodo()
        {
            IsEditMode = false;
            IsNewMode = false;
            SelectedTodo = null;
            EditTodoTitle = string.Empty;
            EditTodoDescription = string.Empty;
            EditTodoCategory = null;
            EditTodoPriority = Priority.Medium;
            
            // Tarih ve saat alanlarını sıfırla
            EditTodoStartDate = null;
            EditTodoStartTime = null;
            EditTodoEndDate = null;
            EditTodoEndTime = null;
            
            // Etiketleri temizle
            EditTodoTags = new ObservableCollection<string>();
            NewTagInput = string.Empty;
        }

        private void DeleteTodo(Todo todo)
        {
            try
            {
                if (todo != null)
                {
                    if (todo.UserId != UserId && todo.AssignedToUserId != UserId)
                    {
                        MessageBox.Show("Bu görevi silme yetkiniz bulunmamaktadır.", "Uyarı", 
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var result = MessageBox.Show("Bu görevi silmek istediğinize emin misiniz?", "Onay", 
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        _dbContext.Todos.Remove(todo);
                        _dbContext.SaveChanges();
                        
                        _logService.AddLog(
                            LogType.TodoDeleted,
                            $"Görev silindi: {todo.Title}",
                            UserId
                        );
                        
                        FilterTodos();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Görev silinirken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                
                _logService.AddLog(
                    LogType.SystemError,
                    $"Görev silinirken hata: {ex.Message}",
                    UserId
                );
            }
        }

        private void CompleteTodo(Todo todo)
        {
            try
            {
                if (todo != null)
                {
                    // todo.IsCompleted değerini tersine çevirme, çünkü zaten checkbox'tan doğru değer geliyor
                    // todo.IsCompleted = !todo.IsCompleted;
                    todo.ModifiedDate = DateTime.Now;
                    
                    _dbContext.Todos.Update(todo);
                    _dbContext.SaveChanges();
                    
                    _logService.AddLog(
                        LogType.TodoUpdated,
                        $"Görev durumu değiştirildi: {todo.Title} - {(todo.IsCompleted ? "Tamamlandı" : "Tamamlanmadı")}",
                        UserId
                    );
                    
                    FilterTodos();
                    Console.WriteLine($"Görev başarıyla güncellendi: {todo.Id} - {todo.Title} - Durum: {todo.IsCompleted}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Görev güncellenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                
                _logService.AddLog(
                    LogType.SystemError,
                    $"Görev tamamlanırken hata: {ex.Message}",
                    UserId
                );
            }
        }

        public bool IsNewMode
        {
            get => _isNewMode;
            set
            {
                _isNewMode = value;
                OnPropertyChanged(nameof(IsNewMode));
            }
        }

        public DateTime? AssignTodoStartDate
        {
            get => _assignTodoStartDate;
            set
            {
                _assignTodoStartDate = value;
                OnPropertyChanged(nameof(AssignTodoStartDate));
            }
        }

        public DateTime? AssignTodoStartTime
        {
            get => _assignTodoStartTime;
            set
            {
                _assignTodoStartTime = value;
                OnPropertyChanged(nameof(AssignTodoStartTime));
            }
        }

        public DateTime? AssignTodoEndDate
        {
            get => _assignTodoEndDate;
            set
            {
                _assignTodoEndDate = value;
                OnPropertyChanged(nameof(AssignTodoEndDate));
            }
        }

        public DateTime? AssignTodoEndTime
        {
            get => _assignTodoEndTime;
            set
            {
                _assignTodoEndTime = value;
                OnPropertyChanged(nameof(AssignTodoEndTime));
            }
        }

        private void ShowAssignPanel()
        {
            AssignTodoTitle = string.Empty;
            AssignTodoDescription = string.Empty;
            AssignTodoCategory = Categories.FirstOrDefault(c => c != "Hepsi");
            AssignTodoPriority = Priority.Medium;
            SelectedUser = null;
            
            // Tarih ve saat alanlarını sıfırla
            AssignTodoStartDate = null;
            AssignTodoStartTime = null;
            AssignTodoEndDate = null;
            AssignTodoEndTime = null;
            
            // Etiketleri temizle
            AssignTodoTags = new ObservableCollection<string>();
            NewTagInput = string.Empty;
            
            IsAssignPanelVisible = true;
        }

        private void CloseAssignPanel()
        {
            IsAssignPanelVisible = false;
        }

        private void AssignTodo()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(AssignTodoTitle))
                {
                    MessageBox.Show("Lütfen başlık alanını doldurunuz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (SelectedUser == null)
                {
                    MessageBox.Show("Lütfen görevi atayacağınız kullanıcıyı seçiniz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var todo = new Todo
                {
                    Title = AssignTodoTitle,
                    Description = AssignTodoDescription,
                    Category = AssignTodoCategory ?? "Genel",
                    Priority = AssignTodoPriority,
                    UserId = UserId,
                    AssignedToUserId = SelectedUser.Id,
                    CreatedAt = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    IsCompleted = false
                };
                
                // Başlangıç ve bitiş tarih/saatlerini ayarla
                if (AssignTodoStartDate.HasValue)
                {
                    DateTime startDate = AssignTodoStartDate.Value;
                    TimeSpan? startTime = AssignTodoStartTime?.TimeOfDay;
                    
                    if (startTime.HasValue)
                    {
                        todo.TaskStartTime = startDate.Date.Add(startTime.Value);
                    }
                    else
                    {
                        todo.TaskStartTime = startDate.Date;
                    }
                }
                
                if (AssignTodoEndDate.HasValue)
                {
                    DateTime endDate = AssignTodoEndDate.Value;
                    TimeSpan? endTime = AssignTodoEndTime?.TimeOfDay;
                    
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

                _dbContext.Todos.Add(todo);
                _dbContext.SaveChanges();

                // Etiketleri kaydet
                foreach (var tag in AssignTodoTags)
                {
                    _tagService.AddTagToTodo(todo.Id, tag);
                }

                _logService.AddLog(
                    LogType.TodoAssigned,
                    $"Görev atandı: {todo.Title} -> {SelectedUser.Username}",
                    UserId
                );

                CloseAssignPanel();
                FilterTodos();

                MessageBox.Show($"'{todo.Title}' görevi '{SelectedUser.Username}' kullanıcısına başarıyla atandı.", 
                    "Görev Atandı", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Görev atanırken hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                _logService.AddLog(
                    LogType.SystemError,
                    $"Görev atanırken hata: {ex.Message}",
                    UserId
                );
            }
        }

        public bool IsAddPanelVisible
        {
            get => _isAddPanelVisible;
            set
            {
                _isAddPanelVisible = value;
                OnPropertyChanged(nameof(IsAddPanelVisible));
            }
        }

        public string NewTodoTitle
        {
            get => _newTodoTitle;
            set
            {
                _newTodoTitle = value;
                OnPropertyChanged(nameof(NewTodoTitle));
            }
        }

        public string NewTodoDescription
        {
            get => _newTodoDescription;
            set
            {
                _newTodoDescription = value;
                OnPropertyChanged(nameof(NewTodoDescription));
            }
        }

        public string NewTodoCategory
        {
            get => _newTodoCategory;
            set
            {
                _newTodoCategory = value;
                OnPropertyChanged(nameof(NewTodoCategory));
            }
        }

        public Priority NewTodoPriority
        {
            get => _newTodoPriority;
            set
            {
                _newTodoPriority = value;
                OnPropertyChanged(nameof(NewTodoPriority));
            }
        }

        public DateTime? NewTodoStartDate
        {
            get => _newTodoStartDate;
            set
            {
                _newTodoStartDate = value;
                OnPropertyChanged(nameof(NewTodoStartDate));
            }
        }

        public DateTime? NewTodoStartTime
        {
            get => _newTodoStartTime;
            set
            {
                _newTodoStartTime = value;
                OnPropertyChanged(nameof(NewTodoStartTime));
            }
        }

        public DateTime? NewTodoEndDate
        {
            get => _newTodoEndDate;
            set
            {
                _newTodoEndDate = value;
                OnPropertyChanged(nameof(NewTodoEndDate));
            }
        }

        public DateTime? NewTodoEndTime
        {
            get => _newTodoEndTime;
            set
            {
                _newTodoEndTime = value;
                OnPropertyChanged(nameof(NewTodoEndTime));
            }
        }

        private void ShowAddPanel()
        {
            NewTodoTitle = string.Empty;
            NewTodoDescription = string.Empty;
            NewTodoCategory = Categories.FirstOrDefault(c => c != "Hepsi");
            NewTodoPriority = Priority.Medium;
            
            // Tarih ve saat alanlarını sıfırla
            NewTodoStartDate = null;
            NewTodoStartTime = null;
            NewTodoEndDate = null;
            NewTodoEndTime = null;
            
            // Etiketleri temizle
            NewTodoTags = new ObservableCollection<string>();
            NewTagInput = string.Empty;
            
            IsAddPanelVisible = true;
        }

        private void CloseAddPanel()
        {
            IsAddPanelVisible = false;
        }

        private void SaveNewTodo()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewTodoTitle))
                {
                    MessageBox.Show("Lütfen başlık alanını doldurunuz.", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var todo = new Todo
                {
                    Title = NewTodoTitle,
                    Description = NewTodoDescription,
                    Category = NewTodoCategory ?? "Genel",
                    Priority = NewTodoPriority,
                    UserId = UserId,
                    CreatedAt = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    IsCompleted = false
                };

                // Başlangıç ve bitiş tarih/saatlerini ayarla
                if (NewTodoStartDate.HasValue)
                {
                    DateTime startDate = NewTodoStartDate.Value;
                    TimeSpan? startTime = NewTodoStartTime?.TimeOfDay;
                    
                    if (startTime.HasValue)
                    {
                        todo.TaskStartTime = startDate.Date.Add(startTime.Value);
                    }
                    else
                    {
                        todo.TaskStartTime = startDate.Date;
                    }
                }
                
                if (NewTodoEndDate.HasValue)
                {
                    DateTime endDate = NewTodoEndDate.Value;
                    TimeSpan? endTime = NewTodoEndTime?.TimeOfDay;
                    
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

                _dbContext.Todos.Add(todo);
                _dbContext.SaveChanges();

                // Etiketleri kaydet
                foreach (var tag in NewTodoTags)
                {
                    _tagService.AddTagToTodo(todo.Id, tag);
                }

                _logService.AddLog(
                    LogType.TodoCreated,
                    $"Yeni görev oluşturuldu: {todo.Title}",
                    UserId
                );

                CloseAddPanel();
                FilterTodos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Görev eklenirken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                
                _logService.AddLog(
                    LogType.SystemError,
                    $"Görev eklenirken hata: {ex.Message}",
                    UserId
                );
            }
        }

        // Düzenlenen görevi güncelle
        public void UpdateTodo(Todo todo)
        {
            try
            {
                // Veritabanında Todo nesnesini güncelle
                _dbContext.Update(todo);
                _dbContext.SaveChanges();
                
                // Filtreleri uygula
                FilterTodos();
                
                // Log kaydı ekle - Bu kısmı MainWindow.xaml.cs'den çağırıyoruz
                // _logService.AddLog(LogType.TodoUpdated, $"Görev güncellendi: {todo.Title}", UserId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Görev güncellenirken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                
                // Hata logu
                _logService.AddLog(LogType.SystemError, $"Görev güncellenirken hata: {ex.Message}", UserId);
            }
        }

        // Tüm etiketleri yükleme
        private void LoadTags()
        {
            var tags = _tagService.GetAllTags(UserId);
            Tags = new ObservableCollection<string>(tags);
            
            // Varsayılan etiketler
            var defaultTags = new List<string> { "Önemli", "Acil", "Beklemede", "İnceleme", "Toplantı" };
            
            foreach (var tag in defaultTags)
            {
                if (!Tags.Contains(tag))
                {
                    Tags.Add(tag);
                }
            }
            
            Tags = new ObservableCollection<string>(Tags.OrderBy(t => t));
            
            // Yeni görev ve düzenleme için boş etiket koleksiyonları
            NewTodoTags = new ObservableCollection<string>();
            EditTodoTags = new ObservableCollection<string>();
            AssignTodoTags = new ObservableCollection<string>();
        }

        // Etiket ekleme
        private void AddTag(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName)) return;
            
            tagName = tagName.Trim();
            
            if (IsEditMode && !EditTodoTags.Contains(tagName))
            {
                EditTodoTags.Add(tagName);
                if (!Tags.Contains(tagName))
                {
                    Tags.Add(tagName);
                    // Tags'i yeniden sıralama
                    Tags = new ObservableCollection<string>(Tags.OrderBy(t => t));
                }
            }
            else if (IsAddPanelVisible && !NewTodoTags.Contains(tagName))
            {
                NewTodoTags.Add(tagName);
                if (!Tags.Contains(tagName))
                {
                    Tags.Add(tagName);
                    // Tags'i yeniden sıralama
                    Tags = new ObservableCollection<string>(Tags.OrderBy(t => t));
                }
            }
            else if (IsAssignPanelVisible && !AssignTodoTags.Contains(tagName))
            {
                AssignTodoTags.Add(tagName);
                if (!Tags.Contains(tagName))
                {
                    Tags.Add(tagName);
                    // Tags'i yeniden sıralama
                    Tags = new ObservableCollection<string>(Tags.OrderBy(t => t));
                }
            }
            
            NewTagInput = string.Empty;
        }

        // Etiket silme
        private void RemoveTag(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName)) return;
            
            if (IsEditMode && EditTodoTags.Contains(tagName))
            {
                EditTodoTags.Remove(tagName);
            }
            else if (IsAddPanelVisible && NewTodoTags.Contains(tagName))
            {
                NewTodoTags.Remove(tagName);
            }
            else if (IsAssignPanelVisible && AssignTodoTags.Contains(tagName))
            {
                AssignTodoTags.Remove(tagName);
            }
        }

        // Etiket filtresini temizleme
        private void ClearTagFilter()
        {
            SelectedTag = null;
            SearchTagText = string.Empty;
            IsFilteringByTag = false;
            FilterTodos();
        }

        // Etikete göre filtreleme
        private void FilterByTag()
        {
            if (SelectedTag == null)
            {
                IsFilteringByTag = false;
                FilterTodos();
                return;
            }
            
            IsFilteringByTag = true;
            
            var todos = _tagService.GetTodosByTag(UserId, SelectedTag);
            Todos = new ObservableCollection<Todo>(todos);
        }

        // Etiket metni ile filtreleme
        public void FilterByTagText()
        {
            if (string.IsNullOrWhiteSpace(SearchTagText))
            {
                IsFilteringByTag = false;
                FilterTodos();
                return;
            }
            
            IsFilteringByTag = true;
            
            // Etiket arama metniyle eşleşen etiketleri bul
            var matchingTag = Tags.FirstOrDefault(t => t.Equals(SearchTagText, StringComparison.OrdinalIgnoreCase));
            
            if (matchingTag != null)
            {
                // Tam eşleşen etiket varsa o etikete sahip görevleri getir
                var todos = _tagService.GetTodosByTag(UserId, matchingTag);
                Todos = new ObservableCollection<Todo>(todos);
            }
            else
            {
                // Eşleşen etiket yoksa aramaya göre filtrele
                var allTodos = _dbContext.Todos
                    .Include(t => t.Tags)
                    .Where(t => t.UserId == UserId)
                    .ToList();
                
                var filteredTodos = allTodos.Where(t => 
                    t.Tags.Any(tag => tag.Tag.Contains(SearchTagText, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
                
                Todos = new ObservableCollection<Todo>(filteredTodos);
            }
        }

        // Etiketler için property'ler
        public ObservableCollection<string> Tags
        {
            get => _tags;
            set
            {
                _tags = value;
                OnPropertyChanged(nameof(Tags));
            }
        }

        public string SelectedTag
        {
            get => _selectedTag;
            set
            {
                _selectedTag = value;
                OnPropertyChanged(nameof(SelectedTag));
                FilterByTag();
            }
        }

        public ObservableCollection<string> EditTodoTags
        {
            get => _editTodoTags;
            set
            {
                _editTodoTags = value;
                OnPropertyChanged(nameof(EditTodoTags));
            }
        }

        public ObservableCollection<string> NewTodoTags
        {
            get => _newTodoTags;
            set
            {
                _newTodoTags = value;
                OnPropertyChanged(nameof(NewTodoTags));
            }
        }
        
        public ObservableCollection<string> AssignTodoTags
        {
            get => _assignTodoTags;
            set
            {
                _assignTodoTags = value;
                OnPropertyChanged(nameof(AssignTodoTags));
            }
        }

        public bool IsFilteringByTag
        {
            get => _isFilteringByTag;
            set
            {
                _isFilteringByTag = value;
                OnPropertyChanged(nameof(IsFilteringByTag));
            }
        }

        public string NewTagInput
        {
            get => _newTagInput;
            set
            {
                _newTagInput = value;
                OnPropertyChanged(nameof(NewTagInput));
            }
        }
        
        public string SearchTagText
        {
            get => _searchTagText;
            set
            {
                _searchTagText = value;
                OnPropertyChanged(nameof(SearchTagText));
            }
        }

        // Alt görevler için property'ler
        public ObservableCollection<SubTask> SubTasks
        {
            get => _subTasks;
            set
            {
                _subTasks = value;
                OnPropertyChanged(nameof(SubTasks));
            }
        }
        
        public SubTask SelectedSubTask
        {
            get => _selectedSubTask;
            set
            {
                _selectedSubTask = value;
                OnPropertyChanged(nameof(SelectedSubTask));
            }
        }
        
        public string NewSubTaskTitle
        {
            get => _newSubTaskTitle;
            set
            {
                _newSubTaskTitle = value;
                OnPropertyChanged(nameof(NewSubTaskTitle));
                CommandManager.InvalidateRequerySuggested(); // CanAddSubTask'ın yeniden değerlendirilmesini sağlar
            }
        }
        
        // Alt görev ekleme
        private void AddSubTask()
        {
            if (SelectedTodo == null || string.IsNullOrWhiteSpace(NewSubTaskTitle))
                return;
                
            try
            {
                var subTask = _subTaskService.AddSubTask(SelectedTodo.Id, NewSubTaskTitle);
                LoadSubTasks();
                NewSubTaskTitle = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Alt görev eklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Alt görev eklenebilir mi kontrolü
        private bool CanAddSubTask()
        {
            return SelectedTodo != null && !string.IsNullOrWhiteSpace(NewSubTaskTitle);
        }
        
        // Alt görev silme
        private void DeleteSubTask(SubTask subTask)
        {
            if (subTask == null) return;
            
            try
            {
                var result = MessageBox.Show($"'{subTask.Title}' alt görevini silmek istediğinizden emin misiniz?", 
                    "Alt Görev Silme", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    
                if (result == MessageBoxResult.Yes)
                {
                    _subTaskService.DeleteSubTask(subTask.Id);
                    LoadSubTasks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Alt görev silinirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // Alt görev durumunu değiştirme
        private void ToggleSubTaskCompletion(SubTask subTask)
        {
            if (subTask == null) return;
            
            try
            {
                Console.WriteLine($"ToggleSubTaskCompletion çağırıldı. SubTask ID: {subTask.Id}, Mevcut durum: {subTask.IsCompleted}");
                
                // Checkbox tarafından güncellenen IsCompleted değerini kontrol et
                Console.WriteLine($"Checkbox'tan gelen IsCompleted değeri: {subTask.IsCompleted}");
                
                // SubTaskService içindeki metodu çağır
                _subTaskService.ToggleSubTaskCompletion(subTask.Id);
                
                Console.WriteLine("SubTaskService.ToggleSubTaskCompletion metodu başarıyla çağırıldı.");
                
                // Alt görevleri yeniden yükle
                LoadSubTasks();
                
                // Todo listesini güncelle (tamamlanma yüzdesi değişmiş olabilir)
                FilterTodos();
                
                Console.WriteLine("İşlem tamamlandı: Alt görev durumu güncellendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Alt görev durumu değiştirilirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                // Hata durumunda UI'da görünen değeri eski haline döndür
                subTask.IsCompleted = !subTask.IsCompleted;
            }
        }
        
        // Bir todo için alt görevleri yükleme
        private void LoadSubTasks()
        {
            if (SelectedTodo == null) return;
            
            try
            {
                Console.WriteLine($"Alt görevler yükleniyor... (TodoId: {SelectedTodo.Id})");
                var subTasks = _subTaskService.GetSubTasksForTodo(SelectedTodo.Id);
                
                Console.WriteLine($"Yüklenen alt görev sayısı: {subTasks.Count}");
                foreach (var st in subTasks)
                {
                    Console.WriteLine($"Alt görev: ID={st.Id}, Title={st.Title}, IsCompleted={st.IsCompleted}");
                }
                
                SubTasks = new ObservableCollection<SubTask>(subTasks);
                
                // Yüklendikten sonra değerleri kontrol et
                foreach (var st in SubTasks)
                {
                    Console.WriteLine($"ObservableCollection'a yüklenen: ID={st.Id}, Title={st.Title}, IsCompleted={st.IsCompleted}");
                }
                
                // CompletionPercentage değerini yenile
                OnPropertyChanged(nameof(SelectedTodo.CompletionPercentage));
                Console.WriteLine($"CompletionPercentage: {SelectedTodo.CompletionPercentage}%");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Alt görevler yüklenirken hata oluştu: {ex.Message}", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                SubTasks = new ObservableCollection<SubTask>();
            }
        }

        // SubTaskService'e dışarıdan erişim sağlamak için
        public SubTaskService GetSubTaskService()
        {
            return _subTaskService;
        }
        
        // Seçili görevin alt görevlerini yeniden yükle
        public void LoadSelectedTodoSubTasks()
        {
            LoadSubTasks();
        }
        
        // Todo listesini yeniden yükle
        public void RefreshTodoList()
        {
            FilterTodos();
        }

        // Alt görevin durumunu güncelle
        public void UpdateSubTaskStatus(SubTask subTask, bool isCompleted)
        {
            if (subTask == null) return;
            
            try
            {
                Console.WriteLine($"UpdateSubTaskStatus çağrıldı: ID={subTask.Id}, IsCompleted={isCompleted}");
                
                // Alt görevin durumunu güncelle
                subTask.IsCompleted = isCompleted;
                
                // SubTaskService üzerinden veritabanına kaydet
                _subTaskService.UpdateSubTaskStatus(subTask.Id, isCompleted);
                
                // Alt görevleri yeniden yükle
                LoadSubTasks();
                
                // Görev listesini güncelle
                FilterTodos();
                
                // Bağlı özellikleri bildir
                OnPropertyChanged(nameof(SubTasks));
                
                if (SelectedTodo != null)
                {
                    OnPropertyChanged(nameof(SelectedTodo.CompletionPercentage));
                }
                
                Console.WriteLine("Alt görev durumu başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Alt görev durumu güncellenirken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
} 