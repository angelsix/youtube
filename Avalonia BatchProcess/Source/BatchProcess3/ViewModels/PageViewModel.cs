using BatchProcess3.MainApp;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels;

public partial class PageViewModel : ViewModelBase
{
    [ObservableProperty]
    private ApplicationPageNames _pageName;

    protected PageViewModel(ApplicationPageNames pageName)
    {
        _pageName = pageName;
    }
}