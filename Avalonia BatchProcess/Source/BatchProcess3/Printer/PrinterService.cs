using BatchProcess3.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;

namespace BatchProcess3.Printer;

public class PrinterService
{
    public ObservableCollection<PrintersViewModel> AvailablePrinters()
    {
        var printers = new ObservableCollection<PrintersViewModel>();
        
        printers.Add(new  PrintersViewModel { Id = "(Default)", Name = "(Default)"});

        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            var printDocument = new PrintDocument();

            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                var printerDetailsViewModel = new PrintersViewModel { Id = printerName, Name = printerName };

                printDocument.PrinterSettings.PrinterName = printerName;

                // Add Default option
                printerDetailsViewModel.PaperSizes.Add("(Default)");
            
                foreach (PaperSize paperSize in printDocument.PrinterSettings.PaperSizes)
                    printerDetailsViewModel.PaperSizes.Add(paperSize.PaperName);

                // Add Default option
                printerDetailsViewModel.SourceTrays.Add("(Default)");
            
                foreach (PaperSource sourceTray in printDocument.PrinterSettings.PaperSources)
                    printerDetailsViewModel.SourceTrays.Add(sourceTray.SourceName);

                printers.Add(printerDetailsViewModel);
            }
        }
        
        return printers;
    }
}