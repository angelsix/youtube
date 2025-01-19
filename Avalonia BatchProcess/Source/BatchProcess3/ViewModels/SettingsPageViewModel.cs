using BatchProcess3.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace BatchProcess3.ViewModels;

public partial class SettingsPageViewModel : PageViewModel
{
    [ObservableProperty]
    private List<string> _locationPaths;
    
    public SettingsPageViewModel()
    {
        PageName = ApplicationPageNames.Settings;
        
        // TEMP: Remove
        LocationPaths =
        [
            @"C:\Users\Luke\Downloads\TestActions",
            @"C:\Users\Luke\Documents\BatchProcess",
            @"X:\Shared\BatchProcess\Templates"
        ];
    }
}