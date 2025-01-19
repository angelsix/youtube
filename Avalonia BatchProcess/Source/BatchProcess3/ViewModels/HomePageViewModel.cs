using BatchProcess3.Data;

namespace BatchProcess3.ViewModels;

public partial class HomePageViewModel : PageViewModel
{
    public string Test { get; set; } = "Home";

    public HomePageViewModel()
    {
        PageName = ApplicationPageNames.Home;
    }
}