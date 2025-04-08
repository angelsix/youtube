using BatchProcess3.Data;
using BatchProcess3.ViewModels;
using System;

namespace BatchProcess3.Factories;

public class PageFactory(Func<Type, PageViewModel> factory)
{
    public PageViewModel GetPageViewModel<T>(Action<T> afterCreation = null)
        where T : PageViewModel
    {
        var viewModel = factory(typeof(T));
        
        afterCreation?.Invoke((T)viewModel);
        
        return viewModel;
    }
}