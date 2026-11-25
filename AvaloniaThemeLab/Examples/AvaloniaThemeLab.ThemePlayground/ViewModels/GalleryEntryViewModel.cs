using AngelSix.Mvvm;

namespace AvaloniaThemeLab.ThemePlayground.ViewModels;

/// <summary>One row of the gallery.</summary>
public abstract class GalleryEntryViewModel(string name) : ObservableObject
{
    public string Name { get; } = name;
}
