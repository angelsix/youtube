using System;

namespace BatchProcess3.DataModels;

public class ActionsTabPrintDataModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    public string JobName = "";
    
    public string Description = "";
    
    public string PrintDrawingRange = "";
    
    public string DrawingExclusionList = "";
    
    public bool DrawingExclusionIsWhiteList;
    
    public bool PrintModels;
    
    public bool PrintDrawings;
    
    public string PrinterSettingsId = "";
    
    //public PrintSettingsDataModel PrinterSettings = "";
}