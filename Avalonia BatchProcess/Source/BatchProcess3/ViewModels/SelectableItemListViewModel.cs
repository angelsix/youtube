using BatchProcess3.Dialog;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace BatchProcess3.ViewModels;

public interface ISelectableItemListViewModel
{
    string Id { get; set; }
    
    string JobName { get; set; }
    
    bool IsNewItem { get; set; }

    void SetSavedState();

    void RestoreState(string? stateToRestore = null);
}

public partial class SelectableItemListViewModel<TViewModel>(
    string title,
    MainViewModel mainViewModel,
    DialogService dialogService,
    Func<ObservableCollection<TViewModel>> getList,
    Func<TViewModel> createItem,
    Action<string> deleteItem,
    Action<TViewModel> addItem,
    Action<TViewModel> updateItem
    ) : ViewModelBase
    where  TViewModel : class, ISelectableItemListViewModel
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasItems))]
    private ObservableCollection<TViewModel> _list = [];
    
    public bool HasItems => List.Any();

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(SelectedItem))]
    private string _selectedItemId = "";

    public TViewModel? SelectedItem =>
        List.FirstOrDefault(f => f.Id == SelectedItemId);

    [RelayCommand]
    public void FetchList()
    {
        List = getList();
        
        // Update FileInfoListHasItems when collection changes
        List.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasItems));

        if (List.Count <= 0) return;

        // Select first item
        SelectedItemId = List.First().Id;

        // Store last fetched database save states
        foreach (var listItem in List)
            listItem.SetSavedState();
    }

    [RelayCommand]
    public void AddNewItem()
    {
        // Create a new item
        var newItem = createItem();

        // Add to the print list
        List.Add(newItem);

        // Select item
        SelectedItemId = newItem.Id;
    }

    [RelayCommand]
    public async Task CancelItem()
    {
        // Ignore if nothing is selected
        if (SelectedItem == null)
            return;

        // If the selected item is new, delete it
        // Otherwise, restore from save state
        if (SelectedItem.IsNewItem)
            await DeleteItemFromUIAsync(SelectedItem.Id, false);
        else
            SelectedItem.RestoreState();
    }

    [RelayCommand]
    public async Task DeleteItemAsync(string id)
    {
        if (List.Count(x => x.Id == id) != 1)
            // TODO: Throw/Warn?
            return;

        // If user selected to remove from UI (via Confirm dialog)
        if (await DeleteItemFromUIAsync(id))
            deleteItem(id);
    }

    // ReSharper disable once InconsistentNaming
    public async Task<bool> DeleteItemFromUIAsync(string id, bool warn = true)
    {
        var index = List.IndexOf(List.First(x => x.Id == id));
        if (index == -1)
            return false;

        if (warn)
        {
            var confirmViewModel = new ConfirmDialogViewModel
            {
                Title = $"Delete {title} Item?",
                Message = $"Are you sure you want to delete ' {List[index].JobName}'?",
                DialogWidth = 500
            };

            await dialogService.ShowDialog(mainViewModel, confirmViewModel);

            // Ignore if we clicked cancel
            if (!confirmViewModel.Confirmed)
                return false;
        }

        // Remove item
        List.RemoveAt(index);

        // Select the item below the deleted one
        if (index > 0) index--;

        if (List.Count > 0)
            SelectedItemId = List[index].Id;

        return true;
    }

    [RelayCommand]
    public Task SaveItemAsync()
    {
        // Ignore if no selection
        if (SelectedItem == null)
            return Task.CompletedTask;

        // If the selected item is new...
        if (SelectedItem.IsNewItem)
            addItem(SelectedItem);
        else
            updateItem(SelectedItem);

        // Flag new item as not new
        SelectedItem.IsNewItem = false;
        SelectedItem.SetSavedState();

        return Task.CompletedTask;
    }

}