using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchProcess3.DataStorage.DataModels;

[Table("ActionPrint")]
public class ActionPrintDataModel : ActionDataModel
{
    [MaxLength(500)]
    public string PrintDrawingRange { get; set; } = "";
    
    [MaxLength(1000)]
    public string DrawingExclusionList { get; set; } = "";
    
    public bool DrawingExclusionIsWhiteList { get; set; }
    
    public bool PrintModels { get; set; }
    
    public bool PrintDrawings { get; set; }
    
    [MaxLength(100)]
    public string? PrinterSettingsId { get; set; } = "";
    
    public ActionPrintSettingsDataModel? PrinterSettings { get; set; }
}