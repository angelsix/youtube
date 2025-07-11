using System;

namespace BatchProcess3.DataModels;

public class PrintSettingsProfileDataModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    public string PrintSettingsDataModelId { get; set; } = "";

    public PrintSettingsDataModel PrintSettingsDataModel { get; set; }
    
    public string Type { get; set; } = "";
    
    public string PrinterName { get; set; } = "";
    
    public string PaperSize { get; set; } = "(Default)";
    
    public double Width { get; set; } = -1;
    
    public double Height { get; set; } = -1;
    
    public string  Orientation { get; set; } = "(Default)";
    
    public string SourceTray { get; set; } = "(Default)";
          
    public string DrawingColor { get; set; } = "(Default)";

    public bool ScaleToFit { get; set; }
}