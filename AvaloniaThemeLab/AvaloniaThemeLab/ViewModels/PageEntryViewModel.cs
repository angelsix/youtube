using System;

namespace AvaloniaThemeLab.ViewModels;

/// <summary>A hand-written page, kept until the control's preview template replaces it.</summary>
/// <remarks>A page that needs state of its own derives from this and becomes the page's DataContext.</remarks>
public class PageEntryViewModel(Type controlType, Type pageType) : GalleryEntryViewModel(controlType.Name)
{
    public Type PageType { get; } = pageType;
}
