using BatchProcess3.CustomProperties;
using BatchProcess3.DataStorage.DataModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels;

public partial class ActionCustomPropertiesViewModel : ActionViewModel, ISelectableItemListViewModel
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _changeNameTo = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _copyFromConfiguration = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _copyToField = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeAssemblies;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeDrawings;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _excludeParts;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _fieldName = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private CustomPropertiesFieldTypes _fieldType;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _filterLogic = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    [NotifyPropertyChangedFor(nameof(FieldTypeIsVisible))]
    [NotifyPropertyChangedFor(nameof(FieldNameIsVisible))]
    [NotifyPropertyChangedFor(nameof(ChangeNameToIsVisible))]
    [NotifyPropertyChangedFor(nameof(ValueRuleIsVisible))]
    [NotifyPropertyChangedFor(nameof(CopyFromConfigurationIsVisible))]
    [NotifyPropertyChangedFor(nameof(CopyToFieldIsVisible))]
    private CustomPropertiesRuleType _ruleType;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _setConfigSpecificProperties;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _setConfigurationPropertiesFilter = "";

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _setCustomProperty;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _valueRule = "";

    [JsonIgnore]
    public bool FieldTypeIsVisible => RuleType is CustomPropertiesRuleType.Add or CustomPropertiesRuleType.Update;

    [JsonIgnore] public bool FieldNameIsVisible => RuleType is not CustomPropertiesRuleType.Clear;

    [JsonIgnore]
    public bool ValueRuleIsVisible => RuleType is CustomPropertiesRuleType.Add or CustomPropertiesRuleType.Update;

    [JsonIgnore] public bool ChangeNameToIsVisible => RuleType is CustomPropertiesRuleType.Update;

    [JsonIgnore] public bool CopyFromConfigurationIsVisible => RuleType is CustomPropertiesRuleType.Copy;

    [JsonIgnore] public bool CopyToFieldIsVisible => RuleType is CustomPropertiesRuleType.Copy;
    
    public new ActionCustomPropertiesDataModel ToDataModel() => new()
    {
        Id = Id,
        Description = Description,
        JobName = JobName,
        ChangeNameTo = ChangeNameTo,
        CopyFromConfiguration = CopyFromConfiguration,
        CopyToField = CopyToField,
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

public static class ActionCustomPropertiesViewModelExtensions
{
    public static ActionCustomPropertiesViewModel ToViewModel(this ActionCustomPropertiesDataModel dataModel) =>
        new()
        {
            Id = dataModel.Id,
            JobName = dataModel.JobName,
            Description = dataModel.Description,
            ChangeNameTo = dataModel.ChangeNameTo,
            CopyFromConfiguration = dataModel.CopyFromConfiguration,
            CopyToField = dataModel.CopyToField,
            ExcludeAssemblies = dataModel.ExcludeAssemblies,
            ExcludeParts = dataModel.ExcludeParts,
            FieldName = dataModel.FieldName,
            FilterLogic = dataModel.FilterLogic,
            SetConfigSpecificProperties = dataModel.SetAllConfigSpecificProperties,
            SetCustomProperty = dataModel.SetCustomProperty,
            SetConfigurationPropertiesFilter = dataModel.SetNamedConfigurationProperties,
            ValueRule = dataModel.ValueRule,
            ExcludeDrawings = dataModel.ExcludeDrawings,
            RuleType = dataModel.RuleType,
            FieldType = dataModel.FieldType
        };
}