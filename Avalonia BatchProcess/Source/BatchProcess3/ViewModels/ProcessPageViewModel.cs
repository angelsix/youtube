using BatchProcess3.Data;

namespace BatchProcess3.ViewModels;

public partial class ProcessPageViewModel : PageViewModel
{
    public string Test { get; set; } = "Process";

    public ProcessPageViewModel()
    {
        PageName = ApplicationPageNames.Process;
    }
}