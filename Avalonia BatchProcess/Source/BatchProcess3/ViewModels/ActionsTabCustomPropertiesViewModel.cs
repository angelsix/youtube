using BatchProcess3.CustomProperties;
using BatchProcess3.DataStorage.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels;

public partial class ActionsTabCustomPropertiesViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _id = "";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _jobName = "";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _description = "";
    
    [ObservableProperty]
    private bool _isNewItem;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    [NotifyPropertyChangedFor(nameof(FieldTypeIsVisible))]
    [NotifyPropertyChangedFor(nameof(FieldNameIsVisible))]
    [NotifyPropertyChangedFor(nameof(ChangeNameToIsVisible))]
    [NotifyPropertyChangedFor(nameof(ValueRuleIsVisible))]
    [NotifyPropertyChangedFor(nameof(CopyFromConfigurationIsVisible))]
    [NotifyPropertyChangedFor(nameof(CopyToFieldIsVisible))]
    private CustomPropertiesRuleType _ruleType;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _filterLogic = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _setCustomProperty;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _setConfigSpecificProperties;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _setConfigurationPropertiesFilter = "";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeParts;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeAssemblies;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeDrawings;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private CustomPropertiesFieldTypes _fieldType;

    [JsonIgnore]
    public bool FieldTypeIsVisible => RuleType is CustomPropertiesRuleType.Add or CustomPropertiesRuleType.Update;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _fieldName = "";
    
    [JsonIgnore]
    public bool FieldNameIsVisible => RuleType is not CustomPropertiesRuleType.Clear;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _valueRule = "";
    
    [JsonIgnore]
    public bool ValueRuleIsVisible => RuleType is CustomPropertiesRuleType.Add or CustomPropertiesRuleType.Update;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _changeNameTo = "";
    
    [JsonIgnore]
    public bool ChangeNameToIsVisible => RuleType is CustomPropertiesRuleType.Update;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _copyFromConfiguration = "";
    
    [JsonIgnore]
    public bool CopyFromConfigurationIsVisible => RuleType is CustomPropertiesRuleType.Copy;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _copyToField = "";

    [JsonIgnore]
    public bool CopyToFieldIsVisible => RuleType is CustomPropertiesRuleType.Copy;
    
    [JsonIgnore]
    public new bool HasChanged => IsNewItem || (SavedState != "" && SavedState != JsonSerializer.Serialize(this, _jsonOptions));

    public ActionsTabCustomPropertiesDataModel ToDataModel() => new()
    {
        Id = Id,
        Description = Description,
        JobName = JobName,
        ChangeNameTo = ChangeNameTo,
        CopyFromConfiguration = CopyFromConfiguration,
        CopyToField =  CopyToField,
        ExcludeAssemblies = ExcludeAssemblies,
        ExcludeDrawings = ExcludeDrawings,
        ExcludeParts = ExcludeParts,
        FieldName = FieldName,
        FieldType = FieldType,
        FilterLogic = FilterLogic,
        RuleType = RuleType,
        SetAllConfigSpecificProperties = SetConfigSpecificProperties,
        SetCustomProperty = SetCustomProperty,
        SetNamedConfigurationProperties = SetConfigurationPropertiesFilter,
        ValueRule = ValueRule
    };
}