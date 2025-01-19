using Avalonia;
using Avalonia.Svg.Skia;
using BatchProcess3.Data;
using BatchProcess3.Factories;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace BatchProcess3.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private PageFactory _pageFactory;
    
    [ObservableProperty]
    private bool _sideMenuExpanded = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HomePageIsActive))]
    [NotifyPropertyChangedFor(nameof(ProcessPageIsActive))]
    [NotifyPropertyChangedFor(nameof(ActionsPageIsActive))]
    [NotifyPropertyChangedFor(nameof(MacrosPageIsActive))]
    [NotifyPropertyChangedFor(nameof(ReporterPageIsActive))]
    [NotifyPropertyChangedFor(nameof(HistoryPageIsActive))]
    [NotifyPropertyChangedFor(nameof(SettingsPageIsActive))]
    private PageViewModel _currentPage;

    public bool HomePageIsActive => CurrentPage.PageName == ApplicationPageNames.Home;
    public bool ProcessPageIsActive => CurrentPage.PageName == ApplicationPageNames.Process;
    public bool ActionsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Actions;
    public bool MacrosPageIsActive => CurrentPage.PageName == ApplicationPageNames.Macros;
    public bool ReporterPageIsActive => CurrentPage.PageName == ApplicationPageNames.Reporter;
    public bool HistoryPageIsActive => CurrentPage.PageName == ApplicationPageNames.History;
    public bool SettingsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Settings;

    /// <summary>
    /// Design-time only constructor
    /// </summary>
    public MainViewModel()
    {
        CurrentPage = new SettingsPageViewModel();
    }
    
    public MainViewModel(PageFactory pageFactory)
    {
        _pageFactory = pageFactory;
        
        GoToHome();
    }
    
    [RelayCommand]
    private void SideMenuResize()
    {
        SideMenuExpanded = !SideMenuExpanded;
    }
    
    [RelayCommand]
    private void GoToHome() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Home);
        
    [RelayCommand]
    private void GoToProcess() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Process);
    [RelayCommand]
    private void GoToActions() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Actions);
    [RelayCommand]
    private void GoToMacros() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Macros);
    [RelayCommand]
    private void GoToReporter() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Reporter);
    [RelayCommand]
    private void GoToHistory() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.History);
    [RelayCommand]
    private void GoToSettings() => CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageNames.Settings);
  }