using System.Text.Json.Serialization;

namespace BatchProcess3.Core.SolidWorks;

public class SolidWorksFileDetails(string filePath)
{
    public string FilePath { get; set; } = filePath;

    public string FileName => Path.GetFileName(FilePath);
}