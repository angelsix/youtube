using CommunityToolkit.Mvvm.ComponentModel;

namespace BatchProcess3.ViewModels;

public partial class KeyValueViewModel<TKey, TValue>(TKey key, TValue value) : ViewModelBase
{
    [ObservableProperty] private TKey _key = key;

    [ObservableProperty] private TValue _value = value;

    public override string ToString() => $"{Key}: {Value}";
}