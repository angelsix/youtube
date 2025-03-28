using BatchProcess3.ViewModels;

namespace BatchProcess3.Interfaces;

public interface IDialogProvider
{
    DialogViewModel Dialog { get; set; }
}