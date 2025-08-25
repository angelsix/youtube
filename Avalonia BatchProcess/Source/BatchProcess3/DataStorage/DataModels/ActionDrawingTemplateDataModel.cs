using BatchProcess3.DrawingTemplates;
using System;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.DataStorage.DataModels;

public class ActionDrawingTemplateDataModel : ActionDataModel
{
   public DrawingTemplateOperation Operation { get; set; }
    
    [MaxLength(1000)]
    public string? CurrentTemplatePath { get; set; }
    
    [MaxLength(1000)]
    public string? NewTemplatePath { get; set; }
}