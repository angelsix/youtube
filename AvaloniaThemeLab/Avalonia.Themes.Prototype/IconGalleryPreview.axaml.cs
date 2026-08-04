using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.Themes.Prototype;

/// <summary>
/// Design-time icon gallery previewer. Shows every <c>{icons:...}</c> glyph
/// inside a Button so you can visually verify chevrons, arrows, and other icons.
/// </summary>
internal partial class IconGalleryPreview : ResourceDictionary
{
    public IconGalleryPreview()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
