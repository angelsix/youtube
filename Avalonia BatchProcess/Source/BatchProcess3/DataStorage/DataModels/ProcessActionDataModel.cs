using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchProcess3.DataStorage.DataModels;

[Table("ProcessAction")]
public class ProcessActionDataModel : ActionDataModel
{
    [MaxLength(100)] public string ActionId { get; init; } = "";

    [MaxLength(100)]
    public string ProcessId {get; init;} = "";
    
    public ProcessDataModel? Process {get; set;}
}