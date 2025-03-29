using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace BatchProcess3.ViewModels;

public partial class ConfirmDialogViewModel : DialogViewModel
{
    [ObservableProperty] private string _title = "Confirm";
    [ObservableProperty] private string _message = "Are you sure?";
    [ObservableProperty] private string _confirmText = "Yes";
    [ObservableProperty] private string _cancelText = "No";
    [ObservableProperty] private string _iconText = "\xe4e0";
    [ObservableProperty] private string _statusText = "";
    [ObservableProperty] private string _progressText = "";

    [ObservableProperty] private double _dialogWidth = double.NaN;
    [ObservableProperty] private double _dialogHeight = double.NaN;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    private bool _busy = false;
    
    public bool NotBusy() => !Busy;
    
    [ObservableProperty]
    private bool _confirmed;
    
    public Func<ConfirmDialogViewModel, Task<bool>> OnConfirm { get; set; } = (_) => Task.FromResult(true);
    
    [RelayCommand]
    public async Task ConfirmAsync()
    {
        if (Busy)
            return;
        
        Busy = true;

        // Clear status text
        StatusText = "";
        
        // Set initial progress text
        ProgressText = "Processing...";
        
        var result = await OnConfirm(this);
        
        Busy = false;

        if (!result)
            return;

        Confirmed = true;
        Close();
    }

    [RelayCommand(CanExecute = nameof(NotBusy))]
    public async void Cancel()
    {
        Confirmed = false;
        Close();
    }
}