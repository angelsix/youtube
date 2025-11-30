using System.Collections.Generic;
using System.Linq;

namespace BatchProcess3Host.SolidWorks;

public class BatchProcessHost
{
    public string GetSolidWorksVersion() => "SolidWorks 2025 SP1.2";

    public SolidWorksFileDetails GetActiveFile() => new("C:\\Users\\conta\\Desktop\\SW Test Files\\Assem1.SLDASM");

    public List<SolidWorksFileDetails> GetActiveFileReferences() => new List<SolidWorksFileDetails>(
        System.IO.Directory.GetFiles("C:\\Users\\conta\\Desktop\\SW Test Files", 
                "*.*", System.IO.SearchOption.TopDirectoryOnly)
            .Select(f => new SolidWorksFileDetails(f)));
}