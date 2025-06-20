using Microsoft.EntityFrameworkCore;

namespace BatchProcess3.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<SettingsDataModel> Settings { get; set; }
    
    override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=settings.db");
    }
}