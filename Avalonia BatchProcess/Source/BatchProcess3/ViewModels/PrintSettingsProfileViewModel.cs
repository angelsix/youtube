using BatchProcess3.DataModels;
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
    private KeyValuePair<string, string> _printerName = new("(Default)", "(Default)");

    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _printerNameOptions = [
        new("(Default)", "(Default)")
    ];
    
    [ObservableProperty]
    private KeyValuePair<string, string> _paperSize = new("(Default)", "(Default)");
    
    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _paperSizeOptions = [
        new("(Default)", "(Default)")
    ];
    
    [ObservableProperty]
    private double _width;
    
    [ObservableProperty]
    private double _height;
    
    [ObservableProperty]
    private KeyValuePair<string, string>  _orientation = new("(Default)", "(Default)");
      
    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _orientationOptions = [
        new("(Default)", "(Default)"),
        new("Portrait", "Portrait"),
        new("Landscape", "Landscape")
    ];
    
    [ObservableProperty]
    private KeyValuePair<string, string> _sourceTray = new("(Default)", "(Default)");
          
    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _sourceTrayOptions = [
        new("(Default)", "(Default)")
    ];

    [ObservableProperty]
    private KeyValuePair<string, string> _drawingColor = new("(Default)", "(Default)");

    [ObservableProperty]
    private ObservableCollection<KeyValuePair<string, string>> _drawingColorOptions = [
        new("(Default)", "(Default)"),
        new("Automatic", "Automatic"),
        new("Color / Greyscale", "Color / Greyscale"),
        new("Black & White", "Black & White"),
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
            PrinterName = viewModel.PrinterName.Value,
            DrawingColor = viewModel.DrawingColor.Value,
            Height = viewModel.Height,
            Width = viewModel.Width,
            Orientation = viewModel.Orientation.Value,
            SourceTray = viewModel.SourceTray.Value,
            PaperSize = viewModel.PaperSize.Value,
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
            PrinterName = new KeyValuePair<string, string>(dataModel.PrinterName, dataModel.PrinterName),
            DrawingColor = new KeyValuePair<string, string>(dataModel.DrawingColor, dataModel.DrawingColor),
            Height = dataModel.Height,
            Width = dataModel.Width,
            Orientation = new KeyValuePair<string, string>(dataModel.Orientation, dataModel.Orientation),
            SourceTray = new KeyValuePair<string, string>(dataModel.SourceTray, dataModel.SourceTray),
            PaperSize = new KeyValuePair<string, string>(dataModel.PaperSize, dataModel.PaperSize),
            ScaleToFit = dataModel.ScaleToFit
        };
    }

    public static ObservableCollection<PrintSettingsProfileViewModel> ToViewModels(
        this List<PrintSettingsProfileDataModel> dataModels) =>
        new(dataModels.Select(ToViewModel).ToList());

}