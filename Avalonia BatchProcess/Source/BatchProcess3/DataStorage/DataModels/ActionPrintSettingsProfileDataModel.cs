using System;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.DataStorage.DataModels;

public class ActionPrintSettingsProfileDataModel
{
    [MaxLength(100)]
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    
    [MaxLength(100)]
    public string PrintSettingsDataModelId { get; init; } = "";

    public ActionPrintSettingsDataModel? ActionPrintSettingsDataModel { get; init; }
    
    [MaxLength(100)]
    public string Type { get; init; } = "";
    
    [MaxLength(500)]
    public string PrinterName { get; init; } = "(Default)";
    
    [MaxLength(100)]
    public string PaperSize { get; init; } = "(Default)";
    
    public double Width { get; init; } = -1;
    
    public double Height { get; init; } = -1;
    
    [MaxLength(100)]
    public string  Orientation { get; init; } = "(Default)";
    
    [MaxLength(100)]
    public string SourceTray { get; init; } = "(Default)";
          
    [MaxLength(100)]
    public string DrawingColor { get; init; } = "(Default)";

    public bool ScaleToFit { get; init; }
}