using System;

namespace AvaloniaThemeLab.ThemePlayground.ViewModels;

/// <summary>A control shown through its gallery preview, the <c>{TypeName}Preview</c> template in the theme's control file.</summary>
public sealed class ControlEntryViewModel(Type controlType) : GalleryEntryViewModel(controlType.Name)
{
    public Type ControlType { get; } = controlType;
}
