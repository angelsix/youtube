using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchProcess3.DataStorage.DataModels;

[Table("ActionFileInfo")]
public class ActionFileInfoDataModel : ActionDataModel
{
    [MaxLength(1000)]
    public string Title { get; set; } = "";
    
    [MaxLength(1000)]
    public string Subject { get; set; } = "";
    
    [MaxLength(1000)]
    public string Author { get; set; } = "";
    
    [MaxLength(1000)]
    public string Keywords { get; set; } = "";
    
    [MaxLength(5000)]
    public string Comments { get; set; } = "";
}