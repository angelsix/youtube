using System;

namespace BatchProcess3.DataStorage.DataModels;

public class ActionsTabPrintDataModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    public string JobName { get; set; } = "";
    
    public string Description { get; set; } = "";
    
    public string PrintDrawingRange { get; set; } = "";
    
    public string DrawingExclusionList { get; set; } = "";
    
    public bool DrawingExclusionIsWhiteList { get; set; }
    
    public bool PrintModels { get; set; }
    
    public bool PrintDrawings { get; set; }
    
    public string PrinterSettingsId { get; set; } = "";
    
    public PrintSettingsDataModel PrinterSettings { get; set; }
}