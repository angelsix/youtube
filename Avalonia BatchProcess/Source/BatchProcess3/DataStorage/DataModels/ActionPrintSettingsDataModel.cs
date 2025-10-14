using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchProcess3.DataStorage.DataModels;

[Table("ActionPrintSettings")]
public class ActionPrintSettingsDataModel : ActionDataModel
{
    public bool CanEdit { get; init; }
    
    public bool CanDelete { get; init; }

    public List<ActionPrintSettingsProfileDataModel> PrinterSettingProfiles { get; init; } = [];

    public List<ActionPrintDataModel> ActionsTabPrintDataModels { get; init; } = [];

    public int Copies { get; init; }
}