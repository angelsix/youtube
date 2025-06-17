using BatchProcess3.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;

namespace BatchProcess3.Services;

public class PrinterService
{
    public ObservableCollection<PrinterDetailsViewModel> AvailablePrinters()
    {
        var printers = new ObservableCollection<PrinterDetailsViewModel>();
        
        printers.Add(new  PrinterDetailsViewModel { Id = "0", Name = "(Default)"});

        var index = 1;
        
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            var printDocument = new PrintDocument();

            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {
                printers.Add(new  PrinterDetailsViewModel { Id = index.ToString(), Name = printerName } );
                index++;
                
                printDocument.PrinterSettings.PrinterName = printerName;
                //printDocument.PrinterSettings.PaperSizes;
            }
        }
        
        return printers;
    }
}