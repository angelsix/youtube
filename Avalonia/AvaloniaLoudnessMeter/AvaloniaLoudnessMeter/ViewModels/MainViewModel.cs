using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace AvaloniaLoudnessMeter.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string boldTitle = "AVALONIA";
    
    [ObservableProperty]
    private string regularTitle = "LOUDNESS METER";
}