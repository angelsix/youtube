using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.DataStorage.DataModels;

public class ActionPrintSettingsDataModel : ActionDataModel
{
    public bool CanEdit { get; set; }
    
    public bool CanDelete { get; set; }
    
    public List<ActionPrintSettingsProfileDataModel> PrinterSettingProfiles { get; set; }

    public List<ActionPrintDataModel> ActionsTabPrintDataModels { get; set; }

    public int Copies { get; set; }
}