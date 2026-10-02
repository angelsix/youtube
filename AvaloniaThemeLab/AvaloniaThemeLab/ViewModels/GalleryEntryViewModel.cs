namespace AvaloniaThemeLab.ViewModels;

/// <summary>One row of the gallery. The page area picks how to show it by its concrete type.</summary>
public abstract class GalleryEntryViewModel(string name)
{
    public string Name { get; } = name;
}
