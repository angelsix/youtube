using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels;

public partial class ActionsPrintViewModel : ViewModelBase
{
    [property: JsonIgnore]
    private string _savedState = "";
    
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
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _printDrawingRange = "";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _drawingExclusionList = "";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DrawingExclusionListTitle))]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _drawingExclusionIsWhiteList;
    
    public string DrawingExclusionListTitle => DrawingExclusionIsWhiteList ? "White List" : "Black List";
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _printModels;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private bool _printDrawings;

    [ObservableProperty]
    private bool _isNewItem;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasChanged))]
    private string _printerProfileId = "";
    
    [JsonIgnore]
    public bool HasChanged => IsNewItem || (_savedState != "" && _savedState != JsonSerializer.Serialize(this));

    public void SetSavedState()
    {
        _savedState = JsonSerializer.Serialize(this);
        
        OnPropertyChanged(nameof(HasChanged));
    }

    public void RestoreSavedState()
    {
        var savedState = JsonSerializer.Deserialize<ActionsPrintViewModel>(_savedState);

        foreach (var propertyInfo in GetType().GetProperties())
        {
            // Only set setters, not get only properties
            if (!propertyInfo.CanWrite)
                continue;
            
            // Ignore any properties that have a JsonIgnore attribute
            if (propertyInfo.GetCustomAttributes(typeof(JsonIgnoreAttribute), false).GetLength(0) > 0)
                continue;
            
            // Pull the saved value
            var originalValue = propertyInfo.GetValue(savedState);
            
            // Restore it to this class
            propertyInfo.SetValue(this, originalValue);
        }
    }
}