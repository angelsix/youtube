using BatchProcess3.DataStorage;
using BatchProcess3.DataStorage.DataModels;
using BatchProcess3.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BatchProcess3.Actions;

public class ActionService(DatabaseService databaseService)
{
    public ObservableCollection<AvailableActionItemViewModel> GetAvailableActionsList()
    {
        List<AvailableActionItemViewModel> ToAvailableActionList<T>(string category, List<T> list)                          
            where T : ActionDataModel
        {
            var returnList = new List<AvailableActionItemViewModel> {
                // Add header
                new() { Category = category } 
            };

            // Add items
            returnList.AddRange(list.Select(f => new AvailableActionItemViewModel
            {
                ActionViewModel = f.ToProcessActionViewModel(),
                Category = category
            }));
            
            return returnList;
        }

        var prints = ToAvailableActionList("Print", databaseService.GetPrintList());
        var customProperties = ToAvailableActionList("Custom Properties", databaseService.GetCustomPropertiesList());
        var fileInfos =  ToAvailableActionList("File Info", databaseService.GetFileInfoList());
        var saveModels =  ToAvailableActionList("Save Model", databaseService.GetSaveModelList());
        var saveDrawings =  ToAvailableActionList("Save Drawing", databaseService.GetSaveDrawingList());
        var importFiles =  ToAvailableActionList("Import Files", databaseService.GetImportFileList());
        var drawingTemplates =  ToAvailableActionList("Drawing Templates", databaseService.GetDrawingTemplateList());
        var macros = ToAvailableActionList("Macros",  databaseService.GetMacrosList());

        return new ObservableCollection<AvailableActionItemViewModel>(
            prints
                .Concat(customProperties)
                .Concat(fileInfos)
                .Concat(saveModels)
                .Concat(saveDrawings)
                .Concat(importFiles)
                .Concat(drawingTemplates)
                .Concat(macros)
        );    
    }
    
}