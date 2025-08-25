using BatchProcess3.CustomProperties;
using System;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.DataStorage.DataModels;

public class ActionCustomPropertiesDataModel : ActionDataModel
{
    public CustomPropertiesRuleType RuleType { get; set; }

    [MaxLength(5000)] public string FilterLogic { get; set; } = "";

    public bool SetCustomProperty { get; set; }

    public bool SetAllConfigSpecificProperties { get; set; }

    [MaxLength(1000)] public string SetNamedConfigurationProperties { get; set; } = "";

    public bool ExcludeParts { get; set; }

    public bool ExcludeAssemblies { get; set; }

    public bool ExcludeDrawings { get; set; }

    public CustomPropertiesFieldTypes FieldType { get; set; }

    [MaxLength(500)] public string FieldName { get; set; } = "";

    [MaxLength(5000)] public string ValueRule { get; set; } = "";

    [MaxLength(500)] public string ChangeNameTo { get; set; } = "";

    [MaxLength(100)] public string CopyFromConfiguration { get; set; } = "";

    [MaxLength(500)] public string CopyToField { get; set; } = "";
}