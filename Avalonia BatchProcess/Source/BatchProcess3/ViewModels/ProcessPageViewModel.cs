
using BatchProcess3.MainApp;

namespace BatchProcess3.ViewModels;

public partial class ProcessPageViewModel() : PageViewModel(ApplicationPageNames.Process)
{
    public string Test { get; set; } = "Process";
}