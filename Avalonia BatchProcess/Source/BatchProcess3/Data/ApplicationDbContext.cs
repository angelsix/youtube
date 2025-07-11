using BatchProcess3.DataModels;
using Microsoft.EntityFrameworkCore;

namespace BatchProcess3.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<SettingsDataModel> Settings { get; set; }
    
    public DbSet<PrintSettingsProfileDataModel>  PrintSettingsProfile { get; set; }
    
    public DbSet<PrintSettingsDataModel>  PrintSettings { get; set; }
    
    public DbSet<ActionsTabPrintDataModel>  ActionsTabPrint { get; set; }
    
    override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=settings.db");   
    }

    override protected void OnModelCreating(ModelBuilder modelBuilder)
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
        
        // Actions Tab Print
        modelBuilder.Entity<ActionsTabPrintDataModel>()
            .HasKey(f => f.Id);
    }
}