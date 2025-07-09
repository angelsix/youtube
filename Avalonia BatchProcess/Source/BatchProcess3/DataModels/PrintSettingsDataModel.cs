using System;
using System.Collections.Generic;

namespace BatchProcess3.DataModels;

public class PrintSettingsDataModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    public string Name = "";
    
    public string Description = "";
    
    public List<PrintSettingsProfileDataModel> PrinterSettings = [];

    public int Copies;
}