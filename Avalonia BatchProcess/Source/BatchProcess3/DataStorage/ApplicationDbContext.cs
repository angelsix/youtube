using BatchProcess3.DataStorage.DataModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace BatchProcess3.DataStorage;

public class ApplicationDbContext : DbContext
{
    public DbSet<SettingsDataModel> Settings { get; set; }
    
    #region Actions
    
    public DbSet<ActionPrintSettingsProfileDataModel> ActionPrintSettingsProfile { get; set; }

    public DbSet<ActionPrintSettingsDataModel> ActionPrintSettings { get; set; }

    public DbSet<ActionPrintDataModel> ActionPrint { get; set; }

    public DbSet<ActionCustomPropertiesDataModel> ActionCustomProperties { get; set; }

    public DbSet<ActionDrawingTemplateDataModel> ActionDrawingTemplate { get; set; }

    public DbSet<ActionFileInfoDataModel> ActionFileInfo { get; set; }

    public DbSet<ActionImportFileDataModel> ActionImportFile { get; set; }

    public DbSet<ActionMacrosDataModel> ActionMacros { get; set; }

    public DbSet<ActionSaveDrawingDataModel> ActionSaveDrawing { get; set; }

    public DbSet<ActionSaveModelDataModel> ActionSaveModel { get; set; }

    #endregion
  
    public DbSet<ProcessDataModel> Processes { get; set; }

    #region Database Configuration

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Ensure folder exists
        var storagePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BatchProcess");
        Directory.CreateDirectory(storagePath);
        
        optionsBuilder.UseSqlite(
            $"Data Source={Path.Combine(storagePath, "settings.db")}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Settings
        modelBuilder.Entity<SettingsDataModel>()
            .HasKey(f => f.Id);

        // Print Settings Profile
        modelBuilder.Entity<ActionPrintSettingsProfileDataModel>()
            .HasKey(f => f.Id);
        modelBuilder.Entity<ActionPrintSettingsProfileDataModel>()
            .HasOne(f => f.ActionPrintSettingsDataModel)
            .WithMany(f => f.PrinterSettingProfiles)
            .OnDelete(DeleteBehavior.ClientCascade);

        // Print Settings
        modelBuilder.Entity<ActionPrintSettingsDataModel>()
            .HasMany(f => f.ActionsTabPrintDataModels)
            .WithOne(f => f.PrinterSettings)
            .HasForeignKey(f => f.PrinterSettingsId)
            .OnDelete(DeleteBehavior.ClientCascade);

        // Actions
        modelBuilder.Entity<ActionDataModel>()
            .HasKey(f => f.Id);

        // Processes
        modelBuilder.Entity<ProcessDataModel>()
            .HasKey(f => f.Id);
    }
    
    #endregion
}