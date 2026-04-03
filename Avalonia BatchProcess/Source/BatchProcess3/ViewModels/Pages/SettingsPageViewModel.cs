using BatchProcess3.DataStorage;
using BatchProcess3.DataStorage.DataModels;
using BatchProcess3.Dialog;
using BatchProcess3.MainApp;
using BatchProcess3.SolidWorks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BatchProcess3.ViewModels.Pages;

public partial class SettingsPageViewModel : PageViewModel
{
    private readonly DatabaseFactory _factory;
    private readonly DialogService _dialogService;
    private readonly HostDiscoveryService _hostDiscoveryService;
    private CancellationTokenSource? _discoveryCts;

    [ObservableProperty] private bool _skipNoActionFiles;

    [ObservableProperty] private bool _allowDuplicateEntries;

    [ObservableProperty] private ObservableCollection<string> _drawingTemplateSearchPaths = [];

    // TODO: Fetch from PDME
    [ObservableProperty] private ObservableCollection<string> _pdmeVaultNames = ["Vault 1", "Vault 2", "Vault 3"];

    [ObservableProperty] private string _pdmeVaultName = "";
    [ObservableProperty] private string _pdmeUsername = "";
    [ObservableProperty] private string _pdmePassword = "";
    
    [ObservableProperty] private string _solidWorksHost = "";

    [NotifyPropertyChangedFor(nameof(SolidWorksHost))]
    [ObservableProperty] private ObservableCollection<string> _solidWorksHosts = [];

    [ObservableProperty] private bool _isDiscoveringHosts;
    
    [ObservableProperty]
    private ObservableCollection<string> _locationPaths = [];

    public SettingsPageViewModel(DatabaseFactory databaseFactory, DialogService dialogService, HostDiscoveryService hostDiscoveryService) : base(ApplicationPageNames.Settings)
    {
        _factory = databaseFactory;
        _dialogService = dialogService;
        _hostDiscoveryService = hostDiscoveryService;

        LoadSettings();
    }

    public override void OnViewLoaded()
    {
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName is nameof(SkipNoActionFiles) or nameof(AllowDuplicateEntries) or nameof(SolidWorksHost))
                SaveSettings();
        };

        _ = DiscoverHostsAsync();
    }

    [RelayCommand]
    private async Task DiscoverHostsAsync()
    {
        // Cancel any previous discoveries
        _discoveryCts?.CancelAsync();
        _discoveryCts = new CancellationTokenSource();

        IsDiscoveringHosts = true;

        try
        {
            // Discover hosts
            var hosts = await _hostDiscoveryService.DiscoverHostsAsync(_discoveryCts.Token);
            SolidWorksHosts = new ObservableCollection<string>(hosts);

            var originalHost = SolidWorksHost;
            SolidWorksHost = "";
            
            // Preserve selection if already in list
            if (!string.IsNullOrWhiteSpace(originalHost) && SolidWorksHosts.Contains(originalHost))
                SolidWorksHost = originalHost;

            // Select first item
            SolidWorksHost = SolidWorksHosts.Count > 0 ? SolidWorksHosts[0] : "";
        }
        catch (Exception e)
        {
            // ignored
        }
        finally
        {
            IsDiscoveringHosts = false;
        }
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
        PdmeVaultName = PdmeVaultName,
        DrawingTemplatePaths = DrawingTemplateSearchPaths.ToList()
    };

}