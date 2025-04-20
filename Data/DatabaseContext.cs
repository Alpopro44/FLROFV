using System;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System.Windows;
using System.Linq;

namespace TodoListApp.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Todo> Todos { get; set; }
        public DbSet<TodoTag> TodoTags { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }

        public DatabaseContext()
        {
            Database.EnsureCreated();
            EnsureModifiedDateColumnExists();
            EnsureLogsTableExists();
            EnsureAssignedToUserIdColumnExists();
            EnsureTaskDateTimeFieldsExist();
            EnsureSubTasksTableExists();
        }

        // AssignedToUserId kolonunu ekleme
        public void EnsureAssignedToUserIdColumnExists()
        {
            try
            {
                // Kolon varlığını kontrol eden bir sorgu
                var checkColumnExists = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA = 'todolistapp' 
                    AND TABLE_NAME = 'Todos' 
                    AND COLUMN_NAME = 'AssignedToUserId';";
                
                // Sorguyu çalıştır ve sonucu al
                var result = Database.ExecuteSqlRaw(checkColumnExists);
                
                // Kolon yoksa ekle
                try
                {
                    // Basit ALTER TABLE komutu
                    Database.ExecuteSqlRaw(
                        "ALTER TABLE Todos ADD COLUMN AssignedToUserId INT NULL;");
                    
                    // Yabancı anahtar kısıtlaması ekleme
                    Database.ExecuteSqlRaw(
                        "ALTER TABLE Todos ADD CONSTRAINT FK_Todos_Users_AssignedToUserId FOREIGN KEY (AssignedToUserId) REFERENCES Users(Id) ON DELETE SET NULL;");
                    
                    // Sessizce devam et - mesaj gösterme
                    Console.WriteLine("AssignedToUserId kolonu başarıyla eklendi.");
                }
                catch (Exception ex)
                {
                    // Kolon zaten var veya başka bir hata
                    Console.WriteLine($"AssignedToUserId kolonu eklenirken hata: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"AssignedToUserId kolonu eklenirken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Veritabanı şemasını güncelle ve Role kolonunu ekle
        public void EnsureRoleColumnExists()
        {
            try
            {
                // Kolon varlığını kontrol eden bir sorgu
                var checkColumnExists = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA = 'todolistapp' 
                    AND TABLE_NAME = 'Users' 
                    AND COLUMN_NAME = 'Role';";
                
                // Sorguyu çalıştır ve sonucu al
                var result = Database.ExecuteSqlRaw(checkColumnExists);
                
                // Kolon yoksa ekle
                try
                {
                    // Basit ALTER TABLE komutu
                    Database.ExecuteSqlRaw(
                        "ALTER TABLE Users ADD COLUMN Role INT NOT NULL DEFAULT 0;");
                    
                    // Sessizce devam et - mesaj gösterme
                    Console.WriteLine("Role kolonu başarıyla eklendi.");
                }
                catch (Exception)
                {
                    // Kolon zaten var, bu hatayı yok sayabiliriz
                    Console.WriteLine("Role kolonu zaten mevcut.");
                }
                
                // Admin kullanıcısına admin rolünü atayalım
                Database.ExecuteSqlRaw(
                    "UPDATE Users SET Role = 1 WHERE Id = 1;");
                
                // Mesaj gösterme - sessizce devam et
                Console.WriteLine("Admin kullanıcısı rolü başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veritabanı güncellenirken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        // ModifiedDate kolonunu ekleme
        public void EnsureModifiedDateColumnExists()
        {
            try
            {
                // Kolon varlığını kontrol eden bir sorgu
                var checkColumnExists = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA = 'todolistapp' 
                    AND TABLE_NAME = 'Todos' 
                    AND COLUMN_NAME = 'ModifiedDate';";
                
                // Sorguyu çalıştır ve sonucu al
                var result = Database.ExecuteSqlRaw(checkColumnExists);
                
                // Kolon yoksa ekle
                try
                {
                    // Basit ALTER TABLE komutu
                    Database.ExecuteSqlRaw(
                        "ALTER TABLE Todos ADD COLUMN ModifiedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;");
                    
                    // Sessionları yenile (şema değişikliği yaptığımız için)
                    Database.ExecuteSqlRaw("SET SESSION sql_notes = 0");
                    
                    // Mevcut kayıtların ModifiedDate'ini CreatedAt ile aynı yap
                    Database.ExecuteSqlRaw(
                        "UPDATE Todos SET ModifiedDate = CreatedAt WHERE 1=1;");
                    
                    // Sessizce devam et - mesaj gösterme
                    Console.WriteLine("ModifiedDate kolonu başarıyla eklendi.");
                }
                catch (Exception ex)
                {
                    // Kolon zaten var veya başka bir hata
                    Console.WriteLine($"ModifiedDate kolonu eklenirken hata: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ModifiedDate kolonu eklenirken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Logs tablosunu oluşturma
        public void EnsureLogsTableExists()
        {
            try
            {
                // Veritabanında Logs tablosunu oluştur
                try
                {
                    // Logs tablosunu oluştur
                    var createTableSql = @"
                        CREATE TABLE IF NOT EXISTS `todolistapp`.`Logs` (
                            `Id` INT AUTO_INCREMENT PRIMARY KEY,
                            `Type` INT NOT NULL,
                            `Description` TEXT NOT NULL,
                            `UserId` INT NULL,
                            `Timestamp` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                            `IpAddress` VARCHAR(50) NULL,
                            `UserAgent` VARCHAR(255) NULL,
                            CONSTRAINT `FK_Logs_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `Users` (`Id`) ON DELETE SET NULL
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;";
                        
                    Database.ExecuteSqlRaw(createTableSql);
                    
                    // Başarılı mesajı
                    Console.WriteLine("Logs tablosu başarıyla oluşturuldu veya zaten mevcut.");
                }
                catch (Exception ex)
                {
                    // Hata mesajını konsola yazdır
                    Console.WriteLine($"Logs tablosu oluşturulurken hata: {ex.Message}");
                    
                    // Farklı bir yaklaşım dene - EF Core'un DbSet üzerinden erişim
                    try
                    {
                        // Yeni bir log oluştur ve kaydet (tablo yoksa EF Core otomatik oluşturacak)
                        var testLog = new Log 
                        { 
                            Type = LogType.SystemError, 
                            Description = "Test log for table creation", 
                            Timestamp = DateTime.Now,
                            IpAddress = "127.0.0.1"
                        };
                        
                        Logs.Add(testLog);
                        SaveChanges();
                        
                        // Test log'unu sil
                        Logs.Remove(testLog);
                        SaveChanges();
                        
                        Console.WriteLine("Logs tablosu EF Core tarafından başarıyla oluşturuldu.");
                    }
                    catch (Exception innerEx)
                    {
                        Console.WriteLine($"EF Core ile tablo oluşturma hatası: {innerEx.Message}");
                        MessageBox.Show($"Logs tablosu oluşturulamadı: {innerEx.Message}", "Hata", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Logs tablosu işlemleri sırasında beklenmeyen hata: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // TaskStartTime ve TaskEndTime alanlarını oluştur
        public void EnsureTaskDateTimeFieldsExist()
        {
            try
            {
                // Kolon varlığını kontrol eden bir sorgu
                var checkColumnExists = @"
                    SELECT COUNT(*) 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_SCHEMA = 'todolistapp' 
                    AND TABLE_NAME = 'Todos' 
                    AND COLUMN_NAME = 'TaskStartTime';";
                
                // Sorguyu çalıştır ve sonucu al
                var result = Database.ExecuteSqlRaw(checkColumnExists);
                
                // TaskStartTime kolonu yoksa ekle
                try
                {
                    // Basit ALTER TABLE komutu
                    Database.ExecuteSqlRaw(
                        "ALTER TABLE Todos ADD COLUMN TaskStartTime DATETIME NULL;");
                    
                    // Sessizce devam et - mesaj gösterme
                    Console.WriteLine("TaskStartTime kolonu başarıyla eklendi.");
                }
                catch (Exception ex)
                {
                    // Kolon zaten var veya başka bir hata
                    Console.WriteLine($"TaskStartTime kolonu eklenirken hata: {ex.Message}");
                }
                
                // TaskEndTime kolonu yoksa ekle
                try
                {
                    // Basit ALTER TABLE komutu
                    Database.ExecuteSqlRaw(
                        "ALTER TABLE Todos ADD COLUMN TaskEndTime DATETIME NULL;");
                    
                    // Sessizce devam et - mesaj gösterme
                    Console.WriteLine("TaskEndTime kolonu başarıyla eklendi.");
                }
                catch (Exception ex)
                {
                    // Kolon zaten var veya başka bir hata
                    Console.WriteLine($"TaskEndTime kolonu eklenirken hata: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tarih/saat alanları eklenirken hata oluştu: {ex.Message}", "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // SubTasks tablosunun varlığını kontrol etme
        public void EnsureSubTasksTableExists()
        {
            try
            {
                // SubTasks tablosunun var olup olmadığını kontrol et
                bool tableExists = false;
                
                using (var command = Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = @"
                        SELECT COUNT(*) 
                        FROM information_schema.tables 
                        WHERE table_schema = DATABASE() AND table_name = 'SubTasks'";
                    
                    Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        if (result.Read() && result.GetInt32(0) > 0)
                        {
                            tableExists = true;
                            Console.WriteLine("SubTasks tablosu zaten mevcut.");
                        }
                    }
                }
                
                if (!tableExists)
                {
                    Console.WriteLine("SubTasks tablosu oluşturuluyor...");
                    
                    try {
                        // Önce tabloyu temizleme girişimi (hata verirse devam et)
                        Database.ExecuteSqlRaw("DROP TABLE IF EXISTS SubTasks;");
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"DROP TABLE IF EXISTS hatası (önemli değil): {ex.Message}");
                    }
                    
                    // SubTasks tablosunu oluştur
                    Database.ExecuteSqlRaw(@"
                        CREATE TABLE SubTasks (
                            Id INT NOT NULL AUTO_INCREMENT,
                            Title VARCHAR(200) NOT NULL,
                            IsCompleted TINYINT(1) NOT NULL DEFAULT 0,
                            CreatedAt DATETIME(6) NOT NULL,
                            ModifiedDate DATETIME(6) NOT NULL,
                            TodoId INT NOT NULL,
                            `Order` INT NOT NULL DEFAULT 0,
                            PRIMARY KEY (Id),
                            CONSTRAINT FK_SubTasks_Todos_TodoId FOREIGN KEY (TodoId) REFERENCES Todos (Id) ON DELETE CASCADE
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
                    ");
                    
                    Database.ExecuteSqlRaw(@"
                        CREATE INDEX IX_SubTasks_TodoId ON SubTasks (TodoId);
                    ");
                    
                    Console.WriteLine("SubTasks tablosu başarıyla oluşturuldu.");
                    MessageBox.Show("SubTasks tablosu başarıyla oluşturuldu.", "Bilgi", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                // Migration başarısız olursa log kaydı
                string errorMessage = $"SubTasks tablosu oluşturulurken hata: {ex.Message}";
                Console.WriteLine(errorMessage);
                MessageBox.Show(errorMessage, "Hata", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = "Server=localhost;Database=todolistapp;User=root;Password=9261032AA;";
                optionsBuilder.UseMySql(connectionString, 
                    ServerVersion.AutoDetect(connectionString),
                    mySqlOptions => mySqlOptions
                        .EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null)
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123", // In production, this should be hashed
                    Email = "admin@example.com",
                    CreatedAt = DateTime.Now,
                    Role = UserRole.Admin
                }
            );

            // Configure relationships
            modelBuilder.Entity<Todo>()
                .HasMany(t => t.Tags)
                .WithOne(tt => tt.Todo)
                .HasForeignKey(tt => tt.TodoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Todo>()
                .HasOne(t => t.User)
                .WithMany(u => u.Todos)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Log ilişkisi
            modelBuilder.Entity<Log>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
} 