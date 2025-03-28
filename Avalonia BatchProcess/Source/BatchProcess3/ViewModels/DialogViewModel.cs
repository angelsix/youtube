using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace BatchProcess3.ViewModels;

public partial class DialogViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isDialogOpen;
    
    protected TaskCompletionSource closeTask = new TaskCompletionSource();

    public async Task WaitAsnyc()
    {
        await closeTask.Task;
    }
    
    public void Show()
    {
        if (closeTask.Task.IsCompleted)
            closeTask = new TaskCompletionSource();
    
        IsDialogOpen = true;
    }

    public void Close()
    {
        IsDialogOpen = false;
        
        closeTask.TrySetResult();
    }
}