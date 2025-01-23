using BatchProcess3.Data;

namespace BatchProcess3.ViewModels;

public partial class HomePageViewModel() : PageViewModel(ApplicationPageNames.Home)
{
    public string Test { get; set; } = "Home";
}