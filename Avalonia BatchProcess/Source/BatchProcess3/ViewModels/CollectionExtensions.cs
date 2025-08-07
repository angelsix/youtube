using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BatchProcess3.ViewModels;

public static class CollectionExtensions
{
    public static void SetAndObserveEverything<T>(this ViewModelBase parent, ObservableCollection<T> value,
        ref ObservableCollection<T> existing, string[]? propertyChangedNames = null,
        [CallerMemberName] string caller = "")
    {
        if (value == existing)
            return;

        existing = value;

        PropertyChangedEventHandler propertyChangedDelegate = (sender, e) =>
        {
            if (!string.IsNullOrWhiteSpace(caller))
                parent.RaiseOnPropertyChanged(caller);

            if (propertyChangedNames == null) return;

            foreach (var propertyChangedName in propertyChangedNames)
                parent.RaiseOnPropertyChanged(propertyChangedName);
        };

        var properties = existing.OfType<ViewModelBase>();

        void CollectionChangedDelegate(object? sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var property in properties)
            {
                property.PropertyChanged -= propertyChangedDelegate;
                property.PropertyChanged += propertyChangedDelegate;
            }

            propertyChangedDelegate(null, null!);
        }

        foreach (var property in existing.OfType<ViewModelBase>())
            property.PropertyChanged += propertyChangedDelegate;

        existing.CollectionChanged += CollectionChangedDelegate;

        propertyChangedDelegate(null, null!);
    }
}