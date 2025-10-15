using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchProcess3.DataStorage.DataModels;

// [Table("Action")]
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