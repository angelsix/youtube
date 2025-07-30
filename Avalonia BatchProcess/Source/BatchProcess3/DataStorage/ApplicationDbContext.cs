using BatchProcess3.DataStorage.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BatchProcess3.DataStorage;

public class ApplicationDbContext : DbContext
{
    public DbSet<SettingsDataModel> Settings { get; set; }

    public DbSet<PrintSettingsProfileDataModel> PrintSettingsProfile { get; set; }

    public DbSet<PrintSettingsDataModel> PrintSettings { get; set; }

    public DbSet<ActionsTabPrintDataModel> ActionsTabPrint { get; set; }

    public DbSet<ActionsTabCustomPropertiesDataModel> ActionsTabCustomProperties { get; set; }

    public DbSet<ActionsTabDrawingTemplateDataModel> ActionsTabDrawingTemplate { get; set; }

    public DbSet<ActionsTabFileInfoDataModel> ActionsTabFileInfo { get; set; }

    public DbSet<ActionsTabImportFileDataModel> ActionsTabImportFile { get; set; }

    public DbSet<ActionsTabMacrosDataModel> ActionsTabMacros { get; set; }

    public DbSet<ActionsTabSaveDrawingDataModel> ActionsTabSaveDrawing { get; set; }

    public DbSet<ActionsTabSaveModelDataModel> ActionsTabSaveModel { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseSqlite("Data Source=settings.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Settings
        modelBuilder.Entity<SettingsDataModel>()
            .HasKey(f => f.Id);

        // Print Settings Profile
        modelBuilder.Entity<PrintSettingsProfileDataModel>()
            .HasKey(f => f.Id);
        modelBuilder.Entity<PrintSettingsProfileDataModel>()
            .HasOne(f => f.PrintSettingsDataModel)
            .WithMany(f => f.PrinterSettingProfiles)
            .OnDelete(DeleteBehavior.ClientCascade);

        // Print Settings
        modelBuilder.Entity<PrintSettingsDataModel>()
            .HasKey(f => f.Id);
        modelBuilder.Entity<PrintSettingsDataModel>()
            .HasMany(f => f.ActionsTabPrintDataModels)
            .WithOne(f => f.PrinterSettings)
            .HasForeignKey(f => f.PrinterSettingsId)
            .OnDelete(DeleteBehavior.ClientCascade);

        // Actions Tab: Custom Properties
        modelBuilder.Entity<ActionsTabCustomPropertiesDataModel>()
            .HasKey(f => f.Id);

        // Actions Tab: Drawing Templates
        modelBuilder.Entity<ActionsTabDrawingTemplateDataModel>()
            .HasKey(f => f.Id);

        // Actions Tab: File Info
        modelBuilder.Entity<ActionsTabFileInfoDataModel>()
            .HasKey(f => f.Id);

        // Actions Tab: Import File
        modelBuilder.Entity<ActionsTabImportFileDataModel>()
            .HasKey(f => f.Id);

        // Actions Tab: Macros
        modelBuilder.Entity<ActionsTabMacrosDataModel>()
            .HasKey(f => f.Id);

        // Actions Tab: Print
        modelBuilder.Entity<ActionsTabCustomPropertiesDataModel>()
            .HasKey(f => f.Id);

        // Actions Tab: Save Drawings
        modelBuilder.Entity<ActionsTabSaveDrawingDataModel>()
            .HasKey(f => f.Id);

        // Actions Tab: Save Models
        modelBuilder.Entity<ActionsTabSaveModelDataModel>()
            .HasKey(f => f.Id);
    }
}