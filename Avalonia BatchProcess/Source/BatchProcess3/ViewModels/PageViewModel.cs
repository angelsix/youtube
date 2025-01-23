using BatchProcess3.Data;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels;

public partial class PageViewModel(ApplicationPageNames pageName) : ViewModelBase
{
    [ObservableProperty]
    private ApplicationPageNames _pageName = pageName;
}