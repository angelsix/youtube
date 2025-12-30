using System.Text.Json.Serialization;

namespace BatchProcess3.Core.SolidWorks;

public class SolidWorksFileDetails(string filePath)
{
    public string FilePath { get; set; } = filePath;

    public string FileName => Path.GetFileName(FilePath);

    public bool IsActiveInSolidWorks { get; set; }

    public override string ToString() => $"{(IsActiveInSolidWorks ? "*" : "")}{FileName}";
}