-- Veritabanını oluştur
CREATE DATABASE IF NOT EXISTS TodoListDB;
USE TodoListDB;

-- Kullanıcılar tablosunu oluştur
CREATE TABLE IF NOT EXISTS Users (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    KullaniciAdi VARCHAR(50) NOT NULL UNIQUE,
    Sifre VARCHAR(255) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    OlusturulmaTarihi TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Görevler tablosunu oluştur
CREATE TABLE IF NOT EXISTS Tasks (
    ID INT AUTO_INCREMENT PRIMARY KEY,
    KullaniciID INT NOT NULL,
    Baslik VARCHAR(100) NOT NULL,
    Aciklama TEXT,
    Durum ENUM('Beklemede', 'Tamamlandı') DEFAULT 'Beklemede',
    Oncelik ENUM('Düşük', 'Orta', 'Yüksek') DEFAULT 'Orta',
    OlusturulmaTarihi TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    GuncellemeTarihi TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (KullaniciID) REFERENCES Users(ID) ON DELETE CASCADE
);

-- Örnek kullanıcı ekle (şifre: 123456)
INSERT INTO Users (KullaniciAdi, Sifre, Email) 
VALUES ('admin', '123456', 'admin@example.com');

-- Örnek görevler ekle
INSERT INTO Tasks (KullaniciID, Baslik, Aciklama, Durum, Oncelik) 
VALUES 
(1, 'Örnek Görev 1', 'Bu bir örnek görev açıklamasıdır.', 'Beklemede', 'Yüksek'),
(1, 'Örnek Görev 2', 'Bu başka bir örnek görev açıklamasıdır.', 'Tamamlandı', 'Orta'),
(1, 'Örnek Görev 3', 'Bu da bir örnek görev açıklamasıdır.', 'Beklemede', 'Düşük'); 