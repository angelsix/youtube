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
    private CustomPropertyRuleType _ruleType;

    [ObservableProperty]
    private string _filterLogic;

    [ObservableProperty]
    private bool _setCustomProperty;
    
    [ObservableProperty]
    private bool _setAllConfigSpecificProperties;
    
    [ObservableProperty]
    private string _setNamedConfigurationProperties;
    
    [ObservableProperty]
    private bool _excludeParts;
    
    [ObservableProperty]
    private bool _excludeAssemblies;
    
    [ObservableProperty]
    private bool _excludeDrawings;
    
    [ObservableProperty]
    private string _fieldType;

    [ObservableProperty]
    private ObservableCollection<string> _fieldTypeOptions = [];

    [ObservableProperty]
    private string _fieldName;
    
    [ObservableProperty]
    private string _valueRule;
    
    [ObservableProperty]
    private string _changeNameTo;
    
    [ObservableProperty]
    private string _copyFromConfiguration;
    
    [ObservableProperty]
    private string _copyToField;

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
        SetAllConfigSpecificProperties = SetAllConfigSpecificProperties,
        SetCustomProperty = SetCustomProperty,
        SetNamedConfigurationProperties = SetNamedConfigurationProperties,
        ValueRule = ValueRule
    };
}