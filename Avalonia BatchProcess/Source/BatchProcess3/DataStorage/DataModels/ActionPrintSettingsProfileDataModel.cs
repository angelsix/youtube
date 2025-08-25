using System;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.DataStorage.DataModels;

public class ActionPrintSettingsProfileDataModel
{
    [MaxLength(100)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    [MaxLength(100)]
    public string PrintSettingsDataModelId { get; set; } = "";

    public ActionPrintSettingsDataModel ActionPrintSettingsDataModel { get; set; }
    
    [MaxLength(100)]
    public string Type { get; set; } = "";
    
    [MaxLength(500)]
    public string PrinterName { get; set; } = "(Default)";
    
    [MaxLength(100)]
    public string PaperSize { get; set; } = "(Default)";
    
    public double Width { get; set; } = -1;
    
    public double Height { get; set; } = -1;
    
    [MaxLength(100)]
    public string  Orientation { get; set; } = "(Default)";
    
    [MaxLength(100)]
    public string SourceTray { get; set; } = "(Default)";
          
    [MaxLength(100)]
    public string DrawingColor { get; set; } = "(Default)";

    public bool ScaleToFit { get; set; }
}