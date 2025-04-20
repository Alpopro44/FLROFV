using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Data;
using TodoListApp.Models;

namespace TodoListApp.Services
{
    public class LogService
    {
        private readonly DatabaseContext _context;
        
        public LogService(DatabaseContext context)
        {
            _context = context;
        }
        
        public void AddLog(LogType type, string description, int? userId = null, string ipAddress = null, string userAgent = null)
        {
            try
            {
                var log = new Log
                {
                    Type = type,
                    Description = description,
                    UserId = userId,
                    Timestamp = DateTime.Now,
                    IpAddress = ipAddress ?? GetLocalIPAddress(),
                    UserAgent = userAgent ?? "FavLists Desktop Client"
                };
                
                _context.Logs.Add(log);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log hatasını konsola yazdır - uygulama hata vermemeli
                Console.WriteLine($"Log eklenirken hata oluştu: {ex.Message}");
            }
        }
        
        public List<Log> GetAllLogs()
        {
            return _context.Logs
                .Include(l => l.User)
                .OrderByDescending(l => l.Timestamp)
                .ToList();
        }
        
        public List<Log> GetUserLogs(int userId)
        {
            return _context.Logs
                .Include(l => l.User)
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.Timestamp)
                .ToList();
        }
        
        public List<Log> GetLogsByType(LogType type)
        {
            return _context.Logs
                .Include(l => l.User)
                .Where(l => l.Type == type)
                .OrderByDescending(l => l.Timestamp)
                .ToList();
        }
        
        public List<Log> GetLogsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _context.Logs
                .Include(l => l.User)
                .Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate)
                .OrderByDescending(l => l.Timestamp)
                .ToList();
        }
        
        // IP adresini alma yardımcı fonksiyonu
        private string GetLocalIPAddress()
        {
            try
            {
                string hostName = Dns.GetHostName();
                IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
                IPAddress[] addresses = hostEntry.AddressList;
                
                // Bulunan ilk IPv4 adresi
                foreach (IPAddress address in addresses)
                {
                    if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return address.ToString();
                    }
                }
                
                return "127.0.0.1"; // Loopback
            }
            catch
            {
                return "0.0.0.0"; // Hata durumunda
            }
        }
    }
} 