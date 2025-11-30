using BatchProcess3.Core.SolidWorks;
using BatchProcess3Host.SolidWorks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BatchProcess3Host.ViewModels;

public partial class MainWindowViewModel(BatchProcessHost batchProcessHost) : ViewModelBase
{
    private int _count = 0;
    
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";

    [RelayCommand]
    private void IncrementValue() => Greeting = $"New value: {_count++}";
    
    [ObservableProperty]
    private ObservableCollection<SolidWorksFileDetails> _activeFilesList = 
        new ObservableCollection<SolidWorksFileDetails>(batchProcessHost.GetActiveFileReferences());

    public MainWindowViewModel() : this(new BatchProcessHost())
    {
        
    }
}
