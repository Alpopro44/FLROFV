using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TodoListApp.Commands;
using TodoListApp.Data;
using TodoListApp.Models;

namespace TodoListApp.ViewModels
{
    public class UserManagementViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseContext _dbContext;
        private ObservableCollection<User> _users;
        private User _selectedUser;
        private bool _isEditMode;
        private string _editUsername;
        private string _editEmail;
        private UserRole _editUserRole;
        private int _editUserId;

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
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

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
            }
        }

        public string EditUsername
        {
            get => _editUsername;
            set
            {
                _editUsername = value;
                OnPropertyChanged(nameof(EditUsername));
            }
        }

        public string EditEmail
        {
            get => _editEmail;
            set
            {
                _editEmail = value;
                OnPropertyChanged(nameof(EditEmail));
            }
        }

        public UserRole EditUserRole
        {
            get => _editUserRole;
            set
            {
                _editUserRole = value;
                OnPropertyChanged(nameof(EditUserRole));
            }
        }

        public string EditUserTitle => _editUserId == 0 ? "Yeni Kullanıcı" : "Kullanıcı Düzenle";

        public ObservableCollection<UserRole> UserRoles { get; private set; }

        public ICommand AddUserCommand { get; private set; }
        public ICommand EditUserCommand { get; private set; }
        public ICommand DeleteUserCommand { get; private set; }
        public ICommand SaveUserCommand { get; private set; }
        public ICommand CancelEditCommand { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public UserManagementViewModel(DatabaseContext dbContext)
        {
            _dbContext = dbContext;

            // Komutları başlat
            AddUserCommand = new RelayCommand(AddNewUser);
            EditUserCommand = new RelayCommand<User>(EditUser);
            DeleteUserCommand = new RelayCommand<User>(DeleteUser);
            SaveUserCommand = new RelayCommand<PasswordBox>(SaveUser);
            CancelEditCommand = new RelayCommand(CancelEdit);

            // Kullanıcı rollerini yükle
            LoadUserRoles();
            
            // Kullanıcıları yükle
            LoadUsers();
        }

        private void LoadUserRoles()
        {
            var roles = Enum.GetValues(typeof(UserRole))
                .Cast<UserRole>()
                .ToList();
            
            UserRoles = new ObservableCollection<UserRole>(roles);
        }

        private void LoadUsers()
        {
            try
            {
                var users = _dbContext.Users.ToList();
                Users = new ObservableCollection<User>(users);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcılar yüklenirken bir hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddNewUser()
        {
            _editUserId = 0;
            EditUsername = string.Empty;
            EditEmail = string.Empty;
            EditUserRole = UserRole.Standard;
            IsEditMode = true;
            OnPropertyChanged(nameof(EditUserTitle));
        }

        private void EditUser(User user)
        {
            if (user == null) return;

            _editUserId = user.Id;
            EditUsername = user.Username;
            EditEmail = user.Email;
            EditUserRole = user.Role;
            IsEditMode = true;
            OnPropertyChanged(nameof(EditUserTitle));
        }

        private void SaveUser(PasswordBox passwordBox)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(EditUsername))
                {
                    MessageBox.Show("Kullanıcı adı boş bırakılamaz!", "Uyarı",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(EditEmail))
                {
                    MessageBox.Show("E-posta adresi boş bırakılamaz!", "Uyarı",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Kullanıcı adının benzersiz olup olmadığını kontrol et
                if (_editUserId == 0) // Yeni kullanıcı
                {
                    var existingUser = _dbContext.Users.FirstOrDefault(u => u.Username == EditUsername);
                    if (existingUser != null)
                    {
                        MessageBox.Show("Bu kullanıcı adı zaten kullanılıyor!", "Uyarı",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }
                else // Mevcut kullanıcı
                {
                    var existingUser = _dbContext.Users.FirstOrDefault(u => u.Username == EditUsername && u.Id != _editUserId);
                    if (existingUser != null)
                    {
                        MessageBox.Show("Bu kullanıcı adı zaten kullanılıyor!", "Uyarı",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                if (_editUserId == 0) // Yeni kullanıcı
                {
                    if (string.IsNullOrWhiteSpace(passwordBox.Password))
                    {
                        MessageBox.Show("Şifre boş bırakılamaz!", "Uyarı",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var user = new User
                    {
                        Username = EditUsername,
                        Email = EditEmail,
                        Password = passwordBox.Password,
                        Role = EditUserRole,
                        CreatedAt = DateTime.Now
                    };

                    _dbContext.Users.Add(user);
                    _dbContext.SaveChanges();
                    
                    MessageBox.Show("Kullanıcı başarıyla oluşturuldu.", "Bilgi", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else // Mevcut kullanıcı
                {
                    var user = _dbContext.Users.FirstOrDefault(u => u.Id == _editUserId);
                    if (user != null)
                    {
                        user.Username = EditUsername;
                        user.Email = EditEmail;
                        user.Role = EditUserRole;

                        // Şifre değiştirilmiş mi kontrol et
                        if (!string.IsNullOrWhiteSpace(passwordBox.Password))
                        {
                            user.Password = passwordBox.Password;
                        }

                        _dbContext.SaveChanges();
                        
                        MessageBox.Show("Kullanıcı başarıyla güncellendi.", "Bilgi", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                LoadUsers();
                CancelEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı kaydedilirken bir hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteUser(User user)
        {
            try
            {
                if (user == null) return;

                var result = MessageBox.Show($"{user.Username} kullanıcısını silmek istediğinizden emin misiniz?", "Onay",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Kullanıcı ile ilişkili görevleri kontrol et
                    var tasks = _dbContext.Todos.Where(t => t.UserId == user.Id).ToList();
                    if (tasks.Any())
                    {
                        var deleteTasksResult = MessageBox.Show($"Bu kullanıcıya ait {tasks.Count} adet görev bulundu. Kullanıcıyı silmek bu görevleri de silecektir. Devam etmek istiyor musunuz?", "Uyarı",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (deleteTasksResult == MessageBoxResult.No)
                        {
                            return;
                        }

                        // Görevleri sil
                        _dbContext.Todos.RemoveRange(tasks);
                    }

                    _dbContext.Users.Remove(user);
                    _dbContext.SaveChanges();
                    LoadUsers();
                    
                    MessageBox.Show("Kullanıcı başarıyla silindi.", "Bilgi", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcı silinirken bir hata oluştu: {ex.Message}", "Hata",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEdit()
        {
            IsEditMode = false;
            _editUserId = 0;
            EditUsername = string.Empty;
            EditEmail = string.Empty;
            EditUserRole = UserRole.Standard;
            OnPropertyChanged(nameof(EditUserTitle));
        }
    }
} 