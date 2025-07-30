using System;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.DataStorage.DataModels;

public class ActionsTabFileInfoDataModel
{
    [MaxLength(100)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    [MaxLength(200)]
    public string JobName { get; set; } = "";

    [MaxLength(5000)]
    public string Description { get; set; } = "";
    
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