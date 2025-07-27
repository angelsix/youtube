using BatchProcess3.DataStorage.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BatchProcess3.ViewModels;

public partial class PrintSettingsProfileViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _id = "-1";
    
    [ObservableProperty]
    private string _type = "A Size";
    
    [ObservableProperty]
    private string _printerName = "(Default)";

    [ObservableProperty]
    private ObservableCollection<string> _printerNameOptions = [
        "(Default)"
    ];
    
    [ObservableProperty]
    private string _paperSize = "(Default)";
    
    [ObservableProperty]
    private ObservableCollection<string> _paperSizeOptions = [
        "(Default)"
    ];
    
    [ObservableProperty]
    private double _width;
    
    [ObservableProperty]
    private double _height;
    
    [ObservableProperty]
    private string  _orientation = "(Default)";
      
    [ObservableProperty]
    private ObservableCollection<string> _orientationOptions = [
        "(Default)",
        "Portrait",
        "Landscape"
    ];
    
    [ObservableProperty]
    private string _sourceTray = "(Default)";
          
    [ObservableProperty]
    private ObservableCollection<string> _sourceTrayOptions = [
        "(Default)"
    ];

    [ObservableProperty]
    private string _drawingColor = "(Default)";

    [ObservableProperty]
    private ObservableCollection<string> _drawingColorOptions = [
        "(Default)",
        "Automatic",
        "Color / Greyscale",
        "Black & White",
    ];

    [ObservableProperty]
    private bool _scaleToFit;
}

public static class PrintSettingsProfileViewModelExtensions
{
    public static PrintSettingsProfileDataModel ToDataModel(this PrintSettingsProfileViewModel viewModel)
    {
        return new PrintSettingsProfileDataModel()
        {
            Id = viewModel.Id,
            Type = viewModel.Type,
            PrinterName = viewModel.PrinterName,
            DrawingColor = viewModel.DrawingColor,
            Height = viewModel.Height,
            Width = viewModel.Width,
            Orientation = viewModel.Orientation,
            SourceTray = viewModel.SourceTray,
            PaperSize = viewModel.PaperSize,
            ScaleToFit = viewModel.ScaleToFit
        };
    }

    public static List<PrintSettingsProfileDataModel> ToDataModels(
        this ObservableCollection<PrintSettingsProfileViewModel> viewModels) =>
        viewModels.Select(ToDataModel).ToList();
    
    public static PrintSettingsProfileViewModel ToViewModel(this PrintSettingsProfileDataModel dataModel)
    {
        return new PrintSettingsProfileViewModel()
        {
            Id = dataModel.Id,
            Type = dataModel.Type,
            PrinterName = dataModel.PrinterName,
            DrawingColor = dataModel.DrawingColor,
            Height = dataModel.Height,
            Width = dataModel.Width,
            Orientation = dataModel.Orientation,
            SourceTray = dataModel.SourceTray,
            PaperSize = dataModel.PaperSize,
            ScaleToFit = dataModel.ScaleToFit
        };
    }

    public static ObservableCollection<PrintSettingsProfileViewModel> ToViewModels(
        this List<PrintSettingsProfileDataModel> dataModels) =>
        new(dataModels.Select(ToViewModel).ToList());

}