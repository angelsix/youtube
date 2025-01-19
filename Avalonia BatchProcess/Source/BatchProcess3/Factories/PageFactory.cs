using BatchProcess3.Data;
using BatchProcess3.ViewModels;
using System;

namespace BatchProcess3.Factories;

public class PageFactory(Func<ApplicationPageNames, PageViewModel> factory)
{
    public PageViewModel GetPageViewModel(ApplicationPageNames pageName) => factory.Invoke(pageName);
}