using Avalonia.Web.Blazor;

namespace AvaloniaLoudnessMeter.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        WebAppBuilder.Configure<AvaloniaLoudnessMeter.App>()
            .SetupWithSingleViewLifetime();
    }
}