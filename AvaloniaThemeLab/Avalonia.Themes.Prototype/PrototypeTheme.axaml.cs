using System;
using AngelSix.ThemeEngine;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Avalonia.Themes.Prototype
{
    /// <summary>
    /// Includes the prototype theme in an application.
    /// </summary>
    public class PrototypeTheme : Styles
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrototypeTheme"/> class.
        /// </summary>
        /// <param name="sp">The parent's service provider.</param>
        public PrototypeTheme(IServiceProvider? sp = null)
        {
            // Designer-preview fallback. ThemeContext has no parameterless constructor any more
            // (it requires a [Theme]-attributed instance), so when no DI bootstrap has set
            // Resolver we initialise with the prototype DefaultTheme so {theme:...} extensions
            // still resolve in designer previews and ad-hoc XAML loads.
            if (ThemeContext.Resolver is null)
                _ = new ThemeContext(new DefaultTheme());

            AvaloniaXamlLoader.Load(sp, this);
        }
    }
}
