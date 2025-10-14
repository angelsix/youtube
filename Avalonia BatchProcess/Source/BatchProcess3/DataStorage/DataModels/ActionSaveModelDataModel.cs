using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BatchProcess3.DataStorage.DataModels;

[Table("ActionSaveModel")]
public class ActionSaveModelDataModel : ActionDataModel
{
    [MaxLength(1000)] public string FileName { get; set; } = "";

    [MaxLength(1000)] public string SaveLocation { get; set; } = "";

    public bool SaveAllConfigurations { get; set; }

    [MaxLength(1000)] public List<string> ExportFormats { get; set; } = [];
}