using System;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.DataStorage.DataModels;

public class ActionImportFileDataModel : ActionDataModel
{
    [MaxLength(1000)]
    public string FileName { get; set; } = "";

    [MaxLength(1000)]
    public string SaveLocation { get; set; } = "";

    [MaxLength(1000)]
    public string ImportArguments { get; set; } = "";

    public bool AddToProject { get; set; }
}