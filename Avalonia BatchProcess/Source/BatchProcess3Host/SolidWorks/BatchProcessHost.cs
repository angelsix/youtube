using BatchProcess3.Core.SolidWorks;
using System.Collections.Generic;
using System.Linq;

namespace BatchProcess3Host.SolidWorks;

public class BatchProcessHost
{
    public string GetSolidWorksVersion() => "SolidWorks 2025 SP1.2";

    private static readonly string TestFilesPath = System.IO.Path.Combine("SolidWorks", "Test Files");

    public SolidWorksFileDetails GetActiveFile() => new(System.IO.Path.Combine(TestFilesPath, "Assem1.SLDASM"));

    public List<SolidWorksFileDetails> GetActiveFileReferences() => new List<SolidWorksFileDetails>(
        System.IO.Directory.GetFiles(TestFilesPath,
                "*.*", System.IO.SearchOption.TopDirectoryOnly)
            .Select(f => new SolidWorksFileDetails(f)));
}