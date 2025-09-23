using System;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.DataStorage.DataModels;

public class ActionDataModel
{
    [MaxLength(100)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    [MaxLength(200)]
    public string JobName { get; set; } = "";
    
    [MaxLength(5000)]
    public string Description { get; set; } = "";
    
    public int SortOrder {get; set;}
}