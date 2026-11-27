using AngelSix.Mvvm;

namespace AvaloniaThemeLab.ThemePlayground.ViewModels;

/// <summary>One row of the gallery.</summary>
public abstract partial class GalleryEntryViewModel([Property] string name) : ViewModelBase;
