using BatchProcess3.DataStorage;
using BatchProcess3.DataStorage.DataModels;
using BatchProcess3.Dialog;
using BatchProcess3.MainApp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BatchProcess3.ViewModels;

public partial class SettingsPageViewModel : PageViewModel
{
    private readonly DatabaseFactory _factory;
    private readonly DialogService _dialogService;

    [ObservableProperty] private bool _skipNoActionFiles;

    [ObservableProperty] private bool _allowDuplicateEntries;

    // TODO: Fetch from PDME
    [ObservableProperty] private ObservableCollection<string> _pdmeVaultNames = ["Vault 1", "Vault 2", "Vault 3"];

    [ObservableProperty] private string _pdmeVaultName = "";
    [ObservableProperty] private string _pdmeUsername = "";
    [ObservableProperty] private string _pdmePassword = "";
    
    [ObservableProperty] private string _solidWorksHost = "";

    // TODO: Fetch from network pings
    [ObservableProperty] private ObservableCollection<string> _solidWorksHosts = ["localhost", "127.0.0.1", "192.168.0.10"];
    
    [ObservableProperty]
    private ObservableCollection<string> _locationPaths = [];

    // Design-time constructor
    public SettingsPageViewModel() : this(new DatabaseFactory(() => new DatabaseService(new ApplicationDbContext())), new DialogService(() => null))
    {
        
    }
    
    public SettingsPageViewModel(DatabaseFactory databaseFactory, DialogService dialogService) : base(ApplicationPageNames.Settings)
    {
        _factory = databaseFactory;
        _dialogService = dialogService;

        LoadSettings();
    }

    public override void OnViewLoaded()
    {
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName is nameof(SkipNoActionFiles) or nameof(AllowDuplicateEntries) or nameof(SolidWorksHost))
                SaveSettings();
        };
    }

    [RelayCommand]
    private void PdmeLogin()
    {
        // TODO: Login to PDME
        
        // Save settings to database
        SaveSettings();
    }

    [RelayCommand]
    private void DeleteLocationPath(string path)
    {
        LocationPaths.Remove(path);

        // Commit to database
        SaveSettings();
    }
    
    [RelayCommand]
    private async Task AddLocationPath()
    {
        var result = await _dialogService.FolderPicker();

        // Do not add if duplicate or cancelled
        if (result == null || LocationPaths.Any(f => string.Equals(f, result, StringComparison.InvariantCultureIgnoreCase))) return;
        
        // Add to locations
        LocationPaths.Add(result);
        
        // Sort alphabetically
        LocationPaths = new ObservableCollection<string>(LocationPaths.Order());
        
        // Save to database
        SaveSettings();
    }

    /// <summary>
    /// Update view model from settings stored in database
    /// </summary>
    private void LoadSettings()
    {
        // Get settings from database
        using var dbContext = _factory.GetDatabaseService();
        var settings = dbContext.GetSettings();
        
        // Update view model
        LocationPaths = new ObservableCollection<string>(dbContext.GetSettings().LocationPaths ?? []);
        SolidWorksHost = settings.SolidWorksHost;
        SkipNoActionFiles = settings.SkipNoActionFiles;
        AllowDuplicateEntries = settings.AllowDuplicateEntries;
        PdmePassword = settings.PdmePassword;
        PdmeVaultName = settings.PdmeVaultName;
        PdmeUsername = settings.PdmeUsername;
    }

    private void SaveSettings()
    {
        using var dbContext = _factory.GetDatabaseService();
        dbContext.SaveSettings(ToDataModel());
    }

    private SettingsDataModel ToDataModel() => new()
    {
        LocationPaths = LocationPaths.ToList(),
        SolidWorksHost = SolidWorksHost,
        SkipNoActionFiles = SkipNoActionFiles,
        AllowDuplicateEntries = AllowDuplicateEntries,
        PdmePassword = PdmePassword,
        PdmeUsername = PdmeUsername,
        PdmeVaultName = PdmeVaultName
    };

}