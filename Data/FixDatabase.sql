-- StartDateTime ve EndDateTime sütunlarını Todos tablosuna ekle
ALTER TABLE Todos ADD COLUMN StartDateTime datetime NULL;
ALTER TABLE Todos ADD COLUMN EndDateTime datetime NULL; 