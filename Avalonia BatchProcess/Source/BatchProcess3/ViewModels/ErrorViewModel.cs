using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BatchProcess3.ViewModels;

public partial class ErrorViewModel : ViewModelBase
{
    [ObservableProperty] private string _title = "Unknown Error";
    [ObservableProperty] private string _description = "Unknown Error Description";
}