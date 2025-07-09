using System;

namespace BatchProcess3.DataModels;

public class PrintSettingsProfileDataModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    public string PrintSettingsDataModelId { get; set; } = "";

    public PrintSettingsDataModel PrintSettingsDataModel { get; set; } =  new PrintSettingsDataModel();
    
    public string Type = "";
    
    public string PrinterName = "";
    
    public string PaperSize = "";
    
    public double Width;
    
    public double Height;
    
    public string  Orientation = "";
    
    public string SourceTray = "";
          
    public string DrawingColor = "";

    public bool ScaleToFit;
}