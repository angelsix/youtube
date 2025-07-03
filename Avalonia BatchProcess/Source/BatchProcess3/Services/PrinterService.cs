using BatchProcess3.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;

namespace BatchProcess3.Services;

public class PrinterService
{
    public ObservableCollection<PrintersViewModel> AvailablePrinters()
    {
        var printers = new ObservableCollection<PrintersViewModel>();
        
        printers.Add(new  PrintersViewModel { Id = "0", Name = "(Default)"});

        var index = 1;
        
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            var printDocument = new PrintDocument();

            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                var printerDetailsViewModel = new PrintersViewModel { Id = index.ToString(), Name = printerName };

                printDocument.PrinterSettings.PrinterName = printerName;

                // Add Default option
                printerDetailsViewModel.PaperSizes.Add(new KeyValuePair<string, string>("0", "(Default)"));
            
                var paperSizeIndex = 1;
                foreach (PaperSize paperSize in printDocument.PrinterSettings.PaperSizes)
                {
                    printerDetailsViewModel.PaperSizes.Add(new KeyValuePair<string, string>(paperSizeIndex.ToString(), paperSize.PaperName));
                    paperSizeIndex++;
                }

                // Add Default option
                printerDetailsViewModel.SourceTrays.Add(new KeyValuePair<string, string>("0", "(Default)"));
            
                var sourceTrayIndex = 1;
                foreach (PaperSource sourceTray in printDocument.PrinterSettings.PaperSources)
                {
                    printerDetailsViewModel.SourceTrays.Add(new KeyValuePair<string, string>(sourceTrayIndex.ToString(), sourceTray.SourceName));
                    sourceTrayIndex++;
                }

                printers.Add(printerDetailsViewModel);
                index++;
            }
        }
        
        return printers;
    }
}