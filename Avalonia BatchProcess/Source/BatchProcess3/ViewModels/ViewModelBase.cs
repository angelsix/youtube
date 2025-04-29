using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BatchProcess3.ViewModels;

public class ViewModelBase : ObservableObject
{
    private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
        WriteIndented = true,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
    };
    
    [property: JsonIgnore]
    public string SavedState = "";
    
    [JsonIgnore]
    public virtual bool HasChanged => SavedState != "" && SavedState != JsonSerializer.Serialize(this, _jsonOptions);

    public void SetSavedState()
    {
        SavedState = GetState();
        
        OnPropertyChanged(nameof(HasChanged));
    }
    
    public string GetState() => JsonSerializer.Serialize(this, GetType().DeclaringType ?? GetType(), _jsonOptions);

    public void RestoreState(string? stateToRestore = null)
    {
        stateToRestore ??= SavedState;
        
        var type = GetType().DeclaringType ?? GetType();
        
        var savedState = JsonSerializer.Deserialize(stateToRestore, type, _jsonOptions);

        foreach (var propertyInfo in type.GetProperties())
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