using BatchProcess3.Data;
using BatchProcess3.DataModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

    public List<ActionsTabPrintDataModel> GetPrintList()
    {
        var printList = _context.ActionsTabPrint.ToList();

        if (printList.Count == 0)
        {
            // Ensure we have at least one print settings
            GetPrintSettings();
            
            // Create a default item
            _context.ActionsTabPrint.Add(new ActionsTabPrintDataModel()
            {
                JobName = "Print Only Drawings",
                Description = "Prints only drawing files",
                PrintDrawingRange = "0, 5, 7-8",
                PrintDrawings = true,
                DrawingExclusionList =
                    $"Some item 1{System.Environment.NewLine}Some item 2{System.Environment.NewLine}Some item 3",
                PrinterSettingsId = _context.PrintSettings.First().Id
            });
            
            // Save changes to database
            _context.SaveChanges();
            
            // Refresh from DB to include ID
            printList = _context.ActionsTabPrint.ToList();
        }
        
        return printList;
    }

    public List<PrintSettingsProfileDataModel> GetPrintSettingsProfiles()
    {
        return
        [
            new PrintSettingsProfileDataModel { Type = "A0Size" },
            new PrintSettingsProfileDataModel { Type = "A1Size" },
            new PrintSettingsProfileDataModel { Type = "A2Size" },
            new PrintSettingsProfileDataModel { Type = "A3Size" },
            new PrintSettingsProfileDataModel { Type = "A4Size" },
            new PrintSettingsProfileDataModel { Type = "A4VerticalSize" },
            new PrintSettingsProfileDataModel { Type = "ASize" },
            new PrintSettingsProfileDataModel { Type = "AVerticalSize" },
            new PrintSettingsProfileDataModel { Type = "BSize" },
            new PrintSettingsProfileDataModel { Type = "CSize" },
            new PrintSettingsProfileDataModel { Type = "DSize" },
            new PrintSettingsProfileDataModel { Type = "ESize" },
            new PrintSettingsProfileDataModel { Type = "UserSize1" },
            new PrintSettingsProfileDataModel { Type = "UserSize2" },
            new PrintSettingsProfileDataModel { Type = "UserSize3" },
            new PrintSettingsProfileDataModel { Type = "UserSize4" },
            new PrintSettingsProfileDataModel { Type = "UserSize5" },
            new PrintSettingsProfileDataModel { Type = "UserSize6" },
            new PrintSettingsProfileDataModel { Type = "UserSize7" },
            new PrintSettingsProfileDataModel { Type = "UserSize8" },
            new PrintSettingsProfileDataModel { Type = "UserSize9" },
            new PrintSettingsProfileDataModel { Type = "UserSize10" },
            new PrintSettingsProfileDataModel { Type = "UserSize11" },
            new PrintSettingsProfileDataModel { Type = "UserSize12" },
        ];
    }

    public List<PrintSettingsDataModel> GetPrintSettings()
    {
        var settings = _context.PrintSettings.Include(f => f.PrinterSettingProfiles).ToList();
        
        if (settings.Count == 0)
        {
            // Add default settings
            _context.PrintSettings.Add(new PrintSettingsDataModel()
            {
                Name = "(Default)", 
                Description = "Use all default settings", 
                Copies = 1,
                PrinterSettingProfiles = GetPrintSettingsProfiles()
            });

            // Save changes to database
            _context.SaveChanges();
            
            settings = _context.PrintSettings.Include(f => f.PrinterSettingProfiles).ToList();
        }
        
        return settings;
    }

    public void AddPrintSettings(ActionsTabPrintDataModel dataModel)
    {
        _context.ActionsTabPrint.Add(dataModel);
        _context.SaveChanges();
    }

    
    public SettingsDataModel GetSettings()
    {
        var settings = _context.Settings.FirstOrDefault();
        
        if (settings != null) return settings;
        
        // If we have no settings, generate default
        settings = new SettingsDataModel
        {
            LocationPaths = ["Initial Path 1", "Initial Path 2", "Initial Path 3"],
            SkipNoActionFiles = true
        };
        
        // Save to database
        SaveSettings(settings);
        
        return settings;
    }

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