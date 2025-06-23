using BatchProcess3.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BatchProcess3.Services;

public class DatabaseService(ApplicationDbContext context) : IDisposable
{
    private readonly ApplicationDbContext _context = context;

    public void ApplyMigrations()
    {
        // TODO: Change to migrations once we start persisting data
        _context.Database.EnsureCreated(); 
    }

    public SettingsDataModel? GetSettings() => _context.Settings.FirstOrDefault();

    public void SaveSettings(SettingsDataModel settings)
    {
        // Remove all settings
        _context.Settings.RemoveRange(_context.Settings);

        // Add new settings
        _context.Settings.Add(settings);
        
        // Commit
        _context.SaveChanges();
    }

    public void Dispose() => _context.Dispose();
}