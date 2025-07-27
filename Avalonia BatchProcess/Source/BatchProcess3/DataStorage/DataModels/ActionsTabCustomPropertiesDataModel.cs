using BatchProcess3.CustomProperties;
using System;
using System.Collections.ObjectModel;

namespace BatchProcess3.DataStorage.DataModels;

public class ActionsTabCustomPropertiesDataModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    public string JobName { get; set; } = "";
    
    public string Description { get; set; } = "";

    public CustomPropertyRuleType RuleType { get; set; }

    public string FilterLogic { get; set; }

    public bool SetCustomProperty { get; set; }
    
    public bool SetAllConfigSpecificProperties { get; set; }
    
    public string SetNamedConfigurationProperties { get; set; }
    
    public bool ExcludeParts { get; set; }
    
    public bool ExcludeAssemblies { get; set; }
    
    public bool ExcludeDrawings { get; set; }
    
    public string FieldType { get; set; }

    public string FieldName { get; set; }
    
    public string ValueRule { get; set; }
    
    public string ChangeNameTo { get; set; }
    
    public string CopyFromConfiguration { get; set; }
    
    public string CopyToField { get; set; }
}