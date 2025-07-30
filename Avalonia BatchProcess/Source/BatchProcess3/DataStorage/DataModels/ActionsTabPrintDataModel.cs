using System;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.DataStorage.DataModels;

public class ActionsTabPrintDataModel
{
    [MaxLength(100)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    [MaxLength(200)]
    public string JobName { get; set; } = "";
    
    [MaxLength(5000)]
    public string Description { get; set; } = "";
    
    [MaxLength(500)]
    public string PrintDrawingRange { get; set; } = "";
    
    [MaxLength(1000)]
    public string DrawingExclusionList { get; set; } = "";
    
    public bool DrawingExclusionIsWhiteList { get; set; }
    
    public bool PrintModels { get; set; }
    
    public bool PrintDrawings { get; set; }
    
    [MaxLength(100)]
    public string? PrinterSettingsId { get; set; } = "";
    
    public PrintSettingsDataModel? PrinterSettings { get; set; }
}