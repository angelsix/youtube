using System;

namespace AvaloniaThemeLab.ViewModels;

/// <summary>A control shown through the theme's own design preview, the <c>{TypeName}Preview</c> template.</summary>
public sealed class ControlEntryViewModel(Type controlType) : GalleryEntryViewModel(controlType.Name)
{
    public Type ControlType { get; } = controlType;
}
