using Avalonia;
using Avalonia.Svg.Skia;
using BatchProcess3.Data;
using BatchProcess3.Factories;
using BatchProcess3.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
namespace BatchProcess3.ViewModels;

public partial class MainViewModel : ViewModelBase, IDialogProvider
{
    private readonly PageFactory _pageFactory;
    
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
    private DialogViewModel _dialog;

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
// Allow nullable PageFactory for now in designer... ideally get it working
#pragma warning disable CS8618, CS9264
    public MainViewModel()
    {
        CurrentPage = new SettingsPageViewModel();
    }
#pragma warning restore CS8618, CS9264
    
    public MainViewModel(PageFactory pageFactory)
    {
        _pageFactory = pageFactory ?? throw new ArgumentNullException(nameof(pageFactory));
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