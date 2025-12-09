using BatchProcess3.Core.SolidWorks;
using System.Collections.Generic;
using System.Linq;

namespace BatchProcess3Host.SolidWorks;

public class BatchProcessHost
{
    public string GetSolidWorksVersion() => "SolidWorks 2025 SP1.2";

    public SolidWorksFileDetails GetActiveFile() => new("SolidWorks\\Test Files\\Assem1.SLDASM");

    public List<SolidWorksFileDetails> GetActiveFileReferences() => new List<SolidWorksFileDetails>(
        System.IO.Directory.GetFiles("SolidWorks\\Test Files", 
                "*.*", System.IO.SearchOption.TopDirectoryOnly)
            .Select(f => new SolidWorksFileDetails(f)));
}