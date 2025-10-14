using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchProcess3.DataStorage.DataModels;

[Table("ActionMacros")]
public class ActionMacrosDataModel : ActionDataModel
{
    [MaxLength(1000)]
    public string MacroPath { get; set; } = "";
    
    [MaxLength(500)]
    public string ModuleName { get; set; } = "";
    
    public bool ExcludeParts { get; set; }
    
    public bool ExcludeDrawings { get; set; }

    public bool ExcludeAssemblies { get; set; }
}