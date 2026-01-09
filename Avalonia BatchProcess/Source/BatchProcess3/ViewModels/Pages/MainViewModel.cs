using BatchProcess3.DataStorage;
using BatchProcess3.Dialog;
using BatchProcess3.Interfaces;
using BatchProcess3.MainApp;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;

namespace BatchProcess3.ViewModels.Pages;

public partial class MainViewModel : ViewModelBase, IDialogProvider
{
    private readonly PageFactory _pageFactory;
    private readonly DatabaseFactory _databaseFactory;
    
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
    
    [ObservableProperty]
    private DialogViewModel? _dialog;

    public bool HomePageIsActive => CurrentPage.PageName == ApplicationPageNames.Home;
    public bool ProcessPageIsActive => CurrentPage.PageName == ApplicationPageNames.Process;
    public bool ActionsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Actions;
    public bool MacrosPageIsActive => CurrentPage.PageName == ApplicationPageNames.Macros;
    public bool ReporterPageIsActive => CurrentPage.PageName == ApplicationPageNames.Reporter;
    public bool HistoryPageIsActive => CurrentPage.PageName == ApplicationPageNames.History;
    public bool SettingsPageIsActive => CurrentPage.PageName == ApplicationPageNames.Settings;
    
    public MainViewModel(PageFactory pageFactory, DatabaseFactory databaseFactory)
    {
        _pageFactory = pageFactory ?? throw new ArgumentNullException(nameof(pageFactory));
        _databaseFactory = databaseFactory ?? throw new ArgumentNullException(nameof(databaseFactory));

        using var dbContext = _databaseFactory.GetDatabaseService();
        dbContext.ApplyMigrations();

        CurrentPage = _pageFactory.GetPageViewModel<SettingsPageViewModel>();
    }
    
    [RelayCommand]
    private void SideMenuResize() => SideMenuExpanded = !SideMenuExpanded;

    [RelayCommand]
    private void GoToHome() => CurrentPage = _pageFactory.GetPageViewModel<HomePageViewModel>();
        
    [RelayCommand]
    private void GoToProcess() => CurrentPage = _pageFactory.GetPageViewModel<ProcessPageViewModel>();

    [RelayCommand]
    private void GoToActions() => CurrentPage = _pageFactory.GetPageViewModel<ActionsPageViewModel>();
    
    [RelayCommand]
    private void GoToMacros() => CurrentPage = _pageFactory.GetPageViewModel<MacrosPageViewModel>();
    [RelayCommand]
    private void GoToReporter() => CurrentPage = _pageFactory.GetPageViewModel<ReporterPageViewModel>();
    [RelayCommand]
    private void GoToHistory() => CurrentPage = _pageFactory.GetPageViewModel<HistoryPageViewModel>();
    [RelayCommand]
    private void GoToSettings() => CurrentPage = _pageFactory.GetPageViewModel<SettingsPageViewModel>();
  }