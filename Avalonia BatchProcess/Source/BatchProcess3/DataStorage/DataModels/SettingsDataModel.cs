using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.DataStorage.DataModels;

public class SettingsDataModel
{
    [MaxLength(100)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    public bool SkipNoActionFiles { get; set; }
    
    public bool AllowDuplicateEntries { get; set; }

    [MaxLength(100000)]
    public List<string> LocationPaths { get; set; } = [];

    [MaxLength(100)]
    public string SolidWorksHost { get; set; } = "";
    
    [MaxLength(100)]
    public string PdmeVaultName { get; set; } = "";
    
    [MaxLength(100)]
    public string PdmeUsername { get; set; } = "";
    
    [MaxLength(100)]
    public string PdmePassword { get; set; } = "";
    
    [MaxLength(100000)]
    public List<string> DrawingTemplateSearchPaths { get; set; } = [];
}