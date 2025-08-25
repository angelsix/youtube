using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BatchProcess3.DataStorage.DataModels;

public class ActionSaveDrawingDataModel : ActionDataModel
{
    [MaxLength(1000)] public string FileName { get; set; } = "";

    [MaxLength(1000)] public string SaveLocation { get; set; } = "";

    [MaxLength(10000)] public string SheetsFilter { get; set; } = "";

    [MaxLength(1000)] public List<string> ExportFormats { get; set; } = [];

    public bool SinglePdf { get; set; }

    public bool SingleEDrawing { get; set; }

    public bool SingleDwgDxf { get; set; }
}