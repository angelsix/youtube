using Avalonia.Controls;
using Avalonia.Platform.Storage;
using BatchProcess3.Interfaces;
using BatchProcess3.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BatchProcess3.Dialog;

public class DialogService(Func<TopLevel?> topLevel)
{
    public async Task ShowDialog<THost, TDialogViewModel>(THost host, TDialogViewModel dialogViewModel)
        where TDialogViewModel : DialogViewModel
        where THost : IDialogProvider
    {
        // Set host dialog to provided one
        host.Dialog = dialogViewModel;
        dialogViewModel.Show();

        // Wait for dialog to close
        await dialogViewModel.WaitAsnyc();
    }

    public async Task<string?> FolderPicker()
    {
        var topLevelVisual = topLevel();
        if (topLevelVisual == null) return null;
        
        var folders = await topLevelVisual.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
        {
            AllowMultiple = false,
            Title = "Select a folder"
        });

        var path = folders.FirstOrDefault()?.Path;
        if (path == null) return null;
        return path.IsAbsoluteUri ? path.LocalPath : path.OriginalString;
    }
}