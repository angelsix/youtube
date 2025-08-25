using BatchProcess3.DataStorage.DataModels;
using BatchProcess3.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

namespace BatchProcess3.DataStorage;

public class DatabaseService(ApplicationDbContext context) : IDisposable
{
    #region Members

    private readonly ApplicationDbContext _context = context;

    #endregion

    #region Lifecycle

    public void Dispose() => _context.Dispose();

    #endregion

    #region Migrations

    public void ApplyMigrations()
    { 
        // TODO: Change to migrations once we start persisting data
        _context.Database.EnsureCreated();
    }

    #endregion

    #region Actions
    
    #region Print

    public List<ActionPrintDataModel> GetPrintList()
    {
        var printList = _context.ActionPrint.ToList();

        if (printList.Count == 0)
        {
            // Ensure we have at least one print settings
            GetPrintSettings();

            // Create a default item
            _context.ActionPrint.Add(new ActionPrintDataModel
            {
                JobName = "Print Only Drawings",
                Description = "Prints only drawing files",
                PrintDrawingRange = "0, 5, 7-8",
                PrintDrawings = true,
                DrawingExclusionList =
                    $"Some item 1{Environment.NewLine}Some item 2{Environment.NewLine}Some item 3",
                PrinterSettingsId = _context.ActionPrintSettings.First().Id
            });

            // Save changes to database
            _context.SaveChanges();

            // Refresh from DB to include ID
            printList = _context.ActionPrint.ToList();
        }

        return printList;
    }

    public void AddPrintListItem(ActionPrintDataModel dataModel)
    {
        _context.ActionPrint.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdatePrintListItem(ActionPrintDataModel dataModel)
    {
        // Remove existing
        DeletePrintListItem(dataModel.Id);

        // Add new
        AddPrintListItem(dataModel);
    }

    public void DeletePrintListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionPrint.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionPrint.Remove(existingItem);
        _context.SaveChanges();
    }

    #endregion

    #region Print Settings

    public List<ActionPrintSettingsProfileDataModel> GetPrintSettingsProfiles()
    {
        return
        [
            new ActionPrintSettingsProfileDataModel { Type = "A0Size" },
            new ActionPrintSettingsProfileDataModel { Type = "A1Size" },
            new ActionPrintSettingsProfileDataModel { Type = "A2Size" },
            new ActionPrintSettingsProfileDataModel { Type = "A3Size" },
            new ActionPrintSettingsProfileDataModel { Type = "A4Size" },
            new ActionPrintSettingsProfileDataModel { Type = "A4VerticalSize" },
            new ActionPrintSettingsProfileDataModel { Type = "ASize" },
            new ActionPrintSettingsProfileDataModel { Type = "AVerticalSize" },
            new ActionPrintSettingsProfileDataModel { Type = "BSize" },
            new ActionPrintSettingsProfileDataModel { Type = "CSize" },
            new ActionPrintSettingsProfileDataModel { Type = "DSize" },
            new ActionPrintSettingsProfileDataModel { Type = "ESize" },
            new ActionPrintSettingsProfileDataModel { Type = "UserSize1" },
            new ActionPrintSettingsProfileDataModel { Type = "UserSize2" },
            new ActionPrintSettingsProfileDataModel { Type = "UserSize3" },
            new ActionPrintSettingsProfileDataModel { Type = "UserSize4" },
            new ActionPrintSettingsProfileDataModel { Type = "UserSize5" },
            new ActionPrintSettingsProfileDataModel { Type = "UserSize6" },
            new ActionPrintSettingsProfileDataModel { Type = "UserSize7" },
            new ActionPrintSettingsProfileDataModel { Type = "UserSize8" },
            new ActionPrintSettingsProfileDataModel { Type = "UserSize9" },
            new ActionPrintSettingsProfileDataModel { Type = "UserSize10" },
            new ActionPrintSettingsProfileDataModel { Type = "UserSize11" },
            new ActionPrintSettingsProfileDataModel { Type = "UserSize12" }
        ];
    }

    public List<ActionPrintSettingsDataModel> GetPrintSettings()
    {
        var settings = _context.ActionPrintSettings.Include(f => f.PrinterSettingProfiles).ToList();

        if (settings.Count == 0)
        {
            // Add default settings
            _context.ActionPrintSettings.Add(new ActionPrintSettingsDataModel
            {
                JobName = "(Default)",
                Description = "Use all default settings",
                Copies = 1,
                PrinterSettingProfiles = GetPrintSettingsProfiles()
            });

            // Save changes to database
            _context.SaveChanges();

            settings = _context.ActionPrintSettings.Include(f => f.PrinterSettingProfiles).ToList();
        }

        return settings;
    }

    public void AddPrintSettings(ActionPrintSettingsDataModel dataModel)
    {
        _context.ActionPrintSettings.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdatePrintSettings(ActionPrintSettingsDataModel dataModel)
    {
        // If it is not editable...
        if (!dataModel.CanEdit)
            throw new InvalidOperationException($"This print setting cannot be edited. {dataModel.JobName}");

        // Remove existing
        DeletePrintSettings(dataModel.Id, true, false);

        // Add new
        AddPrintSettings(dataModel);
    }

    public void DeletePrintSettings(string id, bool bypass = false, bool saveChanges = true)
    {
        // Remove existing
        var existingItem = _context.ActionPrintSettings.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        // If this item is not deletable...
        if (!bypass && !existingItem.CanDelete)
            throw new InvalidOperationException($"This print setting cannot be deleted. {existingItem.JobName}");

        _context.ActionPrintSettings.Remove(existingItem);

        if (saveChanges)
            _context.SaveChanges();
    }

    #endregion

    #region Custom Properties

    public List<ActionCustomPropertiesDataModel> GetCustomPropertiesList() =>
        _context.ActionCustomProperties.ToList();

    public void AddCustomPropertiesItem(ActionCustomPropertiesDataModel dataModel)
    {
        _context.ActionCustomProperties.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateCustomPropertiesItem(ActionCustomPropertiesDataModel dataModel)
    {
        // Remove existing
        DeleteCustomPropertiesListItem(dataModel.Id);

        // Add new
        AddCustomPropertiesItem(dataModel);
    }

    public void DeleteCustomPropertiesListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionCustomProperties.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionCustomProperties.Remove(existingItem);
        _context.SaveChanges();
    }

    #endregion

    #region File Info

    public List<ActionFileInfoDataModel> GetFileInfoList() =>
        _context.ActionFileInfo.ToList();

    public void AddFileInfoItem(ActionFileInfoDataModel dataModel)
    {
        _context.ActionFileInfo.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateFileInfoItem(ActionFileInfoDataModel dataModel)
    {
        // Remove existing
        DeleteFileInfoListItem(dataModel.Id);

        // Add new
        AddFileInfoItem(dataModel);
    }

    public void DeleteFileInfoListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionFileInfo.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionFileInfo.Remove(existingItem);
        _context.SaveChanges();
    }

    #endregion

    #region Save Model

    public List<ActionSaveModelDataModel> GetSaveModelList() =>
        _context.ActionSaveModel.ToList();

    public void AddSaveModelItem(ActionSaveModelDataModel dataModel)
    {
        _context.ActionSaveModel.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateSaveModelItem(ActionSaveModelDataModel dataModel)
    {
        // Remove existing
        DeleteSaveModelListItem(dataModel.Id);

        // Add new
        AddSaveModelItem(dataModel);
    }

    public void DeleteSaveModelListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionSaveModel.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionSaveModel.Remove(existingItem);
        _context.SaveChanges();
    }

    #endregion

    #region Save Drawing

    public List<ActionSaveDrawingDataModel> GetSaveDrawingList() =>
        _context.ActionSaveDrawing.ToList();

    public void AddSaveDrawingItem(ActionSaveDrawingDataModel dataModel)
    {
        _context.ActionSaveDrawing.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateSaveDrawingItem(ActionSaveDrawingDataModel dataModel)
    {
        // Remove existing
        DeleteSaveDrawingListItem(dataModel.Id);

        // Add new
        AddSaveDrawingItem(dataModel);
    }

    public void DeleteSaveDrawingListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionSaveDrawing.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionSaveDrawing.Remove(existingItem);
        _context.SaveChanges();
    }

    #endregion

    #region Import File

    public List<ActionImportFileDataModel> GetImportFileList() =>
        _context.ActionImportFile.ToList();

    public void AddImportFileItem(ActionImportFileDataModel dataModel)
    {
        _context.ActionImportFile.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateImportFileItem(ActionImportFileDataModel dataModel)
    {
        // Remove existing
        DeleteImportFileListItem(dataModel.Id);

        // Add new
        AddImportFileItem(dataModel);
    }

    public void DeleteImportFileListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionImportFile.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionImportFile.Remove(existingItem);
        _context.SaveChanges();
    }

    #endregion

    #region Drawing Templates

    public List<ActionDrawingTemplateDataModel> GetDrawingTemplateList() =>
        _context.ActionDrawingTemplate.ToList();

    public void AddDrawingTemplateItem(ActionDrawingTemplateDataModel dataModel)
    {
        _context.ActionDrawingTemplate.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateDrawingTemplateItem(ActionDrawingTemplateDataModel dataModel)
    {
        // Remove existing
        DeleteDrawingTemplateListItem(dataModel.Id);

        // Add new
        AddDrawingTemplateItem(dataModel);
    }

    public void DeleteDrawingTemplateListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionDrawingTemplate.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionDrawingTemplate.Remove(existingItem);
        _context.SaveChanges();
    }
    
    public void AddDrawingTemplatePaths(string[] paths)
    {
        // Ignore empty
        if (paths.Length == 0)
            return;
        
        // Get existing paths
        var settings = GetSettings();
        var existingPaths = settings.DrawingTemplatePaths;
        
        // Add if not already in the list
        foreach (var path in paths)
        {
            if (!existingPaths.Any(f => string.Equals(f, path, StringComparison.InvariantCultureIgnoreCase)))
                existingPaths.Add(path);
        }
        
        // Sort alphabetically
        settings.DrawingTemplatePaths = existingPaths.Order().ToList();
        
        // Save settings
        SaveSettings(settings);
    }
    
    public void DeleteDrawingTemplatePaths(string[] paths)
    {
        // Get settings
        var settings = GetSettings();
        
        // Get paths to keep
        var filteredPathsToKeep = settings.DrawingTemplatePaths.Where(p => paths.All(f => !string.Equals(f, p, StringComparison.InvariantCultureIgnoreCase)));
        
        // Update paths
        settings.DrawingTemplatePaths = filteredPathsToKeep.ToList();
        
        // Save
        SaveSettings(settings);
    }

    #endregion

    #region Macros

    public List<ActionMacrosDataModel> GetMacrosList() =>
        _context.ActionMacros.ToList();

    public void AddMacrosItem(ActionMacrosDataModel dataModel)
    {
        _context.ActionMacros.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateMacrosItem(ActionMacrosDataModel dataModel)
    {
        // Remove existing
        DeleteMacrosListItem(dataModel.Id);

        // Add new
        AddMacrosItem(dataModel);
    }

    public void DeleteMacrosListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionMacros.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionMacros.Remove(existingItem);
        _context.SaveChanges();
    }

    #endregion
    
    #endregion
    
    #region Processes
    
    public List<ProcessDataModel> GetProcessList() => _context.Processes.ToList();

    public void AddProcessItem(ProcessDataModel dataModel)
    {
        _context.Processes.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateProcessItem(ProcessDataModel dataModel)
    {
        // Remove existing
        DeleteProcessItem(dataModel.Id);

        // Add new
        AddProcessItem(dataModel);
    }

    public void DeleteProcessItem(string id)
    {
        // Remove existing
        var existingItem = _context.Processes.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.Processes.Remove(existingItem);
        _context.SaveChanges();
    }
    
    #endregion

    #region Settings

    public SettingsDataModel GetSettings()
    {
        var settings = _context.Settings.FirstOrDefault();

        if (settings != null) return settings;

        // If we have no settings, generate default
        settings = new SettingsDataModel
        {
            LocationPaths = ["Initial Path 1", "Initial Path 2", "Initial Path 3"], SkipNoActionFiles = true
        };

        // Save to database
        SaveSettings(settings);

        return settings;
    }

    public void SaveSettings(SettingsDataModel settings)
    {
        // If this already exists in the database
        if (_context.Settings.Any(f => f.Id == settings.Id))
            // Update it
            _context.Settings.Update(settings);
        else
        {
            // Remove all settings
            _context.Settings.RemoveRange(_context.Settings);

            // Add new settings
            _context.Settings.Add(settings);
        }

        // Commit
        _context.SaveChanges();
    }

    #endregion
}