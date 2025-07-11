using System;
using System.Collections.Generic;

namespace BatchProcess3.DataModels;

public class PrintSettingsDataModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    public string Name { get; set; } = "";
    
    public string Description { get; set; } = "";

    public bool CanEdit { get; set; }
    
    public bool CanDelete { get; set; }
    
    public List<PrintSettingsProfileDataModel> PrinterSettingProfiles { get; set; }

    public List<ActionsTabPrintDataModel> ActionsTabPrintDataModels { get; set; }

    public int Copies { get; set; }
}