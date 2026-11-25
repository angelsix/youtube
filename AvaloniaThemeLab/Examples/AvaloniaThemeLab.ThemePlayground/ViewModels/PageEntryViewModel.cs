using System;

namespace AvaloniaThemeLab.ThemePlayground.ViewModels;

/// <summary>A hand-written page, kept until the control's preview template replaces it.</summary>
public class PageEntryViewModel(Type controlType) : GalleryEntryViewModel(controlType.Name)
{
    public Type ControlType { get; } = controlType;
}
