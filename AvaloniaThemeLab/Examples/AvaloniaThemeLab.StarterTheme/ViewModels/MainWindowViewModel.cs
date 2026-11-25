namespace AvaloniaThemeLab.StarterTheme.ViewModels;

public sealed partial class MainWindowViewModel(ExampleContentViewModel content) : ViewModelBase
{
    #region Properties

    public ExampleContentViewModel Content { get; } = content;

    #endregion Properties
}
