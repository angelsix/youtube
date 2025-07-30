using BatchProcess3.DrawingTemplates;
using System;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.DataStorage.DataModels;

public class ActionsTabDrawingTemplateDataModel
{
    [MaxLength(100)]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    [MaxLength(200)]
    public string JobName { get; set; } = "";
    
    [MaxLength(5000)]
    public string Description { get; set; } = "";
    
    public DrawingTemplateOperation Operation { get; set; }
    
    [MaxLength(1000)]
    public string CurrentTemplatePath { get; set; } = "";
    
    [MaxLength(1000)]
    public string NewTemplatePath { get; set; } = "";
}