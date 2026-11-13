namespace AvaloniaThemeLab.ThemePlayground.ViewModels;

/// <summary>One row of the gallery.</summary>
public abstract class GalleryEntryViewModel(string name)
{
    public string Name { get; } = name;
}
