using AngelSix.Mvvm;
using AngelSix.Themes.Default;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Avalonia.Threading;
using Xunit;

namespace AvaloniaThemeLab.Tests;

public sealed partial class IconControlTests
{
    [AvaloniaFact]
    public void Icon_resolves_string_or_geometry_and_supports_Size_plus_Width_Height_overrides()
    {
        var window = new Window { Width = 300, Height = 300 };
        var panel = new StackPanel();

        var fromNameSquare = new Icon { Content = "ChevronDown", Size = 20 };
        var fromGeometryNonSquare = new Icon { Content = StreamGeometry.Parse("M0,0 L10,10"), Width = 32, Height = 14 };
        var sizeWithWidthOverride = new Icon { Content = "ChevronDown", Size = 20, Width = 32 };
        var emptyIcon = new Icon { Content = null, Size = 20 };

        panel.Children.Add(fromNameSquare);
        panel.Children.Add(fromGeometryNonSquare);
        panel.Children.Add(sizeWithWidthOverride);
        panel.Children.Add(emptyIcon);
        window.Content = panel;

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal(new Size(20, 20), fromNameSquare.DesiredSize);
            Assert.Equal(new Size(32, 14), fromGeometryNonSquare.DesiredSize);
            Assert.Equal(new Size(32, 20), sizeWithWidthOverride.DesiredSize);
            Assert.Equal(new Size(0, 0), emptyIcon.DesiredSize);
        }
        finally
        {
            window.Close();
        }
    }

    [AvaloniaFact]
    public void Multiple_ListBoxes_bound_to_one_ActiveItem_switch_selection_cleanly()
    {
        var vm = new SharedSelectionViewModel();
        var listA = new ListBox
        {
            ItemsSource = vm.GroupA,
            [!ListBox.SelectedItemProperty] = new Binding(nameof(SharedSelectionViewModel.ActiveItem)) { Mode = BindingMode.TwoWay },
        };
        var listB = new ListBox
        {
            ItemsSource = vm.GroupB,
            [!ListBox.SelectedItemProperty] = new Binding(nameof(SharedSelectionViewModel.ActiveItem)) { Mode = BindingMode.TwoWay },
        };

        var window = new Window
        {
            Width = 300,
            Height = 300,
            DataContext = vm,
            Content = new StackPanel { Children = { listA, listB } },
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("A1", vm.ActiveItem);
            Assert.Equal("A1", listA.SelectedItem);
            Assert.Null(listB.SelectedItem);

            // Selecting an item in listB updates ActiveItem and clears listA without clobbering ActiveItem to null.
            listB.SelectedItem = "B2";
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("B2", vm.ActiveItem);
            Assert.Equal("B2", listB.SelectedItem);
            Assert.Null(listA.SelectedItem);

            // Switching back to listA clears listB cleanly too.
            listA.SelectedItem = "A2";
            Dispatcher.UIThread.RunJobs();

            Assert.Equal("A2", vm.ActiveItem);
            Assert.Equal("A2", listA.SelectedItem);
            Assert.Null(listB.SelectedItem);
        }
        finally
        {
            window.Close();
        }
    }

    private sealed class SharedSelectionViewModel : ObservableObject
    {
        private string _activeItem = "A1";

        public string[] GroupA { get; } = ["A1", "A2"];
        public string[] GroupB { get; } = ["B1", "B2"];

        public string ActiveItem
        {
            get => _activeItem;
            set
            {
                if (value is null || EqualityComparer<string>.Default.Equals(_activeItem, value))
                    return;
                _activeItem = value;
                OnPropertyChanged(nameof(ActiveItem));
            }
        }
    }
}
