using System;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.DataStorage.DataModels;

public class ActionsTabMacrosDataModel
{
    [MaxLength(100)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    [MaxLength(200)]
    public string JobName { get; set; } = "";
    
    [MaxLength(5000)]
    public string Description { get; set; } = "";
    
    [MaxLength(1000)]
    public string MacroPath { get; set; } = "";
    
    [MaxLength(500)]
    public string ModuleName { get; set; } = "";
    
    public bool ExcludeParts { get; set; }
    
    public bool ExcludeDrawings { get; set; }

    public bool ExcludeAssemblies { get; set; }
}