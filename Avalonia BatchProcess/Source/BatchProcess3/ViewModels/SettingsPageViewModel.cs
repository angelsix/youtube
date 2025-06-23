using BatchProcess3.Data;
using BatchProcess3.Factories;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace BatchProcess3.ViewModels;

public partial class SettingsPageViewModel : PageViewModel
{
    private DatabaseFactory _factory;
    
    [ObservableProperty]
    private List<string> _locationPaths;
    
    public SettingsPageViewModel(DatabaseFactory databaseFactory) : base(ApplicationPageNames.Settings)
    {
        _factory = databaseFactory;
        
        // TEMP: Remove
        using var dbContext = _factory.GetDatabaseService();
        LocationPaths = dbContext.GetSettings()?.LocationPaths ?? [];
    }
}