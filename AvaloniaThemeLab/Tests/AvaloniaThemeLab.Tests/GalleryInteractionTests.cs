using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaThemeLab.ThemePlayground.Pages;
using AvaloniaThemeLab.ThemePlayground.ViewModels;
using AvaloniaThemeLab.ThemePlayground.Views;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AvaloniaThemeLab.Tests;

public sealed class GalleryInteractionTests
{
    private static IServiceProvider Services => ((App)Application.Current!).Services;

    [AvaloniaFact]
    public void Changing_content_density_preserves_settings_window_bounds()
    {
        TestTheme.With(t => t.BaseSize = 1);
        var window = new ThemeSettingsWindow { DataContext = Services.GetRequiredService<ThemeSettingsViewModel>() };
        try
        {
            window.Show();
            Pump(window);
            var before = (window.Width, window.Height);
            TestTheme.Current.BaseSize = 1.5;
            Pump(window);
            Assert.Equal(before, (window.Width, window.Height));
        }
        finally { window.Close(); TestTheme.With(null); }
    }

    [AvaloniaFact]
    public void A_press_outside_the_sidebar_does_not_defer_navigation()
    {
        var vm = Services.GetRequiredService<MainWindowViewModel>();
        var original = vm.SelectedEntry;
        var window = new MainWindow { DataContext = vm, Width = 1200, Height = 800 };
        try
        {
            vm.SelectedEntry = vm.Catalogue.Entries[0];
            window.Show();
            Pump(window);
            var search = window.GetVisualDescendants().OfType<TextBox>().First();
            var point = search.TranslatePoint(new Point(search.Bounds.Width / 2, search.Bounds.Height / 2), window)!.Value;
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            vm.SelectedEntry = vm.Catalogue.Entries[1];
            Assert.Same(vm.SelectedEntry, vm.ShownEntry);
            window.MouseUp(point, MouseButton.Left);
        }
        finally
        {
            vm.CompleteDeferredReveal();
            window.Close();
            vm.SelectedEntry = original;
        }
    }

    [AvaloniaFact]
    public void Sidebar_navigation_still_defers_reveal_when_the_list_is_wrapped()
    {
        var vm = Services.GetRequiredService<MainWindowViewModel>();
        var original = vm.SelectedEntry;
        var window = new MainWindow { DataContext = vm, Width = 1200, Height = 800 };
        try
        {
            vm.SelectedEntry = vm.Catalogue.Entries[0];
            var shown = vm.ShownEntry;
            var grid = Assert.IsType<Grid>(window.Content);
            var sidebar = window.FindControl<ListBox>("SidebarEntries")!;
            grid.Children.Remove(sidebar);
            var wrapper = new Border { Child = sidebar };
            Grid.SetRow(wrapper, 1);
            grid.Children.Add(wrapper);
            window.Show();
            Pump(window);
            var row = Assert.IsType<ListBoxItem>(sidebar.ContainerFromIndex(1));
            var point = row.TranslatePoint(new Point(row.Bounds.Width / 2, row.Bounds.Height / 2), window)!.Value;
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);

            Assert.Same(vm.Catalogue.Entries[1], vm.SelectedEntry);
            Assert.Same(shown, vm.ShownEntry);
            window.MouseUp(point, MouseButton.Left);
            Pump(window);
            Assert.Same(vm.SelectedEntry, vm.ShownEntry);
        }
        finally
        {
            vm.CompleteDeferredReveal();
            window.Close();
            vm.SelectedEntry = original;
        }
    }

    [AvaloniaFact]
    public async Task Window_commands_reach_injected_services_without_control_parameters()
    {
        var actions = new WindowActions();
        var vm = new WindowPageViewModel(actions, actions);
        vm.OpenDialogCommand.Execute(null);
        vm.CloseDialogCommand.Execute(null);
        await vm.PushModalCommand.ExecuteAsync(null);
        await vm.PopModalCommand.ExecuteAsync(null);
        Assert.Same(vm, actions.DialogContext);
        Assert.Same(vm, actions.ModalContext);
        Assert.Equal(1, actions.DialogsClosed);
        Assert.Equal(1, actions.ModalsPopped);

        var main = new MainWindowViewModel(Services.GetRequiredService<GalleryCatalogue>(), actions);
        main.OpenSettingsCommand.Execute(null);
        Assert.Equal(1, actions.SettingsOpened);
    }

    [AvaloniaFact]
    public async Task The_registered_page_opens_and_closes_a_real_modal_through_its_view_adapter()
    {
        TestTheme.With(theme => theme.Animation = 0);
        var entry = Services.GetRequiredService<GalleryCatalogue>().Entries.OfType<WindowPageViewModel>().Single();
        var locator = Services.GetRequiredService<ViewLocator>();
        var page = Assert.IsType<WindowPage>(locator.Build(entry));
        var vm = Assert.IsType<WindowPageViewModel>(page.DataContext);
        Assert.NotSame(entry, vm);
        var host = page.FindControl<NavigationPage>("ModalHost")!;
        var window = new Window { Content = page, Width = 600, Height = 700 };
        try
        {
            window.Show();
            Pump(window);
            await vm.PushModalCommand.ExecuteAsync(null);
            Pump(window);
            var modal = Assert.IsType<WindowPageModal>(Assert.Single(host.ModalStack));
            Assert.Same(vm, modal.DataContext);
            await vm.PopModalCommand.ExecuteAsync(null);
            Pump(window);
            Assert.Empty(host.ModalStack);
            window.Close();
            await vm.PushModalCommand.ExecuteAsync(null);
            Assert.Empty(host.ModalStack);
        }
        finally { window.Close(); }
    }

    [AvaloniaFact]
    public void The_locator_covers_registered_pages_and_reports_unknown_entries()
    {
        var locator = Services.GetRequiredService<ViewLocator>();
        var pages = Services.GetRequiredService<GalleryCatalogue>().Entries.OfType<PageEntryViewModel>().ToArray();
        Assert.NotEmpty(pages);
        foreach (var page in pages)
        {
            Assert.True(locator.Match(page));
            var shown = Assert.IsAssignableFrom<PageEntryViewModel>(locator.Build(page).DataContext);
            Assert.Equal(page.ControlType, shown.ControlType);
            Assert.NotSame(page, shown);
        }
        var unknown = new PageEntryViewModel(typeof(Window));
        Assert.False(locator.Match(unknown));
        Assert.Throws<ArgumentException>(() => locator.Build(unknown));
    }

    private static void Pump(Window window)
    {
        for (var i = 0; i < 5; i++)
        {
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        }
    }

    private sealed class WindowActions : IDialogWindowService, IModalNavigationService, IThemeSettingsWindowService
    {
        public object? DialogContext { get; private set; }
        public object? ModalContext { get; private set; }
        public int DialogsClosed { get; private set; }
        public int ModalsPopped { get; private set; }
        public int SettingsOpened { get; private set; }
        public void OpenDialog(object? dataContext) => DialogContext = dataContext;
        public void CloseTopDialog() => DialogsClosed++;
        public Task PushAsync(object? dataContext) { ModalContext = dataContext; return Task.CompletedTask; }
        public Task PopAsync() { ModalsPopped++; return Task.CompletedTask; }
        public void Show() => SettingsOpened++;
    }

    private static Button PushButton(WindowPage page) =>
        page.GetVisualDescendants().OfType<Button>().First(b => (b.Content as string) == "Push a modal page");

    [AvaloniaFact]
    public async Task Each_live_page_pushes_to_its_own_modal_host()
    {
        TestTheme.With(theme => theme.Animation = 0);
        var vm = Services.GetRequiredService<WindowPageViewModel>();
        var locator = Services.GetRequiredService<ViewLocator>();
        var pageA = (WindowPage)locator.Build(vm);
        var pageB = (WindowPage)locator.Build(vm);
        var winA = new Window { Content = pageA, Width = 600, Height = 700 };
        var winB = new Window { Content = pageB, Width = 600, Height = 700 };
        try
        {
            winA.Show(); Pump(winA); winB.Show(); Pump(winB);
            var button = PushButton(pageA);
            await Assert.IsAssignableFrom<AngelSix.Mvvm.IAsyncRelayCommand>(button.Command).ExecuteAsync(button.CommandParameter);
            Pump(winA); Pump(winB);

            Assert.Single(pageA.FindControl<NavigationPage>("ModalHost")!.ModalStack);
            Assert.Empty(pageB.FindControl<NavigationPage>("ModalHost")!.ModalStack);
        }
        finally { winB.Close(); winA.Close(); await Task.Yield(); }
    }

    [AvaloniaFact]
    public async Task Closing_a_sibling_page_does_not_disconnect_the_survivor()
    {
        TestTheme.With(theme => theme.Animation = 0);
        var vm = Services.GetRequiredService<WindowPageViewModel>();
        var locator = Services.GetRequiredService<ViewLocator>();
        var pageA = (WindowPage)locator.Build(vm);
        var winA = new Window { Content = pageA, Width = 600, Height = 700 };
        var winB = new Window { Content = locator.Build(vm), Width = 600, Height = 700 };
        try
        {
            winA.Show(); Pump(winA); winB.Show(); Pump(winB);
            winB.Close(); Pump(winA);
            var button = PushButton(pageA);
            await Assert.IsAssignableFrom<AngelSix.Mvvm.IAsyncRelayCommand>(button.Command).ExecuteAsync(button.CommandParameter);
            Pump(winA);

            Assert.Single(pageA.FindControl<NavigationPage>("ModalHost")!.ModalStack);
        }
        finally { winA.Close(); }
    }

    [AvaloniaFact]
    public void Changing_content_density_preserves_native_main_window_bounds()
    {
        var vm = Services.GetRequiredService<MainWindowViewModel>();
        var window = new MainWindow { DataContext = vm };
        try
        {
            TestTheme.With(t => t.BaseSize = 1);
            window.Show(); Pump(window);
            var before = (window.Width, window.Height);
            TestTheme.Current.BaseSize = 1.5;
            Pump(window);

            Assert.Equal(before, (window.Width, window.Height));
        }
        finally { window.Close(); TestTheme.With(null); }
    }
}
