using BatchProcess3.DataStorage.DataModels;
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

    #region Print

    public List<ActionsTabPrintDataModel> GetPrintList()
    {
        var printList = _context.ActionsTabPrint.ToList();

        if (printList.Count == 0)
        {
            // Ensure we have at least one print settings
            GetPrintSettings();

            // Create a default item
            _context.ActionsTabPrint.Add(new ActionsTabPrintDataModel
            {
                JobName = "Print Only Drawings",
                Description = "Prints only drawing files",
                PrintDrawingRange = "0, 5, 7-8",
                PrintDrawings = true,
                DrawingExclusionList =
                    $"Some item 1{Environment.NewLine}Some item 2{Environment.NewLine}Some item 3",
                PrinterSettingsId = _context.PrintSettings.First().Id
            });

            // Save changes to database
            _context.SaveChanges();

            // Refresh from DB to include ID
            printList = _context.ActionsTabPrint.ToList();
        }

        return printList;
    }

    public void AddPrintListItem(ActionsTabPrintDataModel dataModel)
    {
        _context.ActionsTabPrint.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdatePrintListItem(ActionsTabPrintDataModel dataModel)
    {
        // Remove existing
        DeletePrintListItem(dataModel.Id);

        // Add new
        AddPrintListItem(dataModel);
    }

    public void DeletePrintListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionsTabPrint.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionsTabPrint.Remove(existingItem);
        _context.SaveChanges();
    }

    #endregion

    #region Print Settings

    public List<PrintSettingsProfileDataModel> GetPrintSettingsProfiles()
    {
        return
        [
            new PrintSettingsProfileDataModel { Type = "A0Size" },
            new PrintSettingsProfileDataModel { Type = "A1Size" },
            new PrintSettingsProfileDataModel { Type = "A2Size" },
            new PrintSettingsProfileDataModel { Type = "A3Size" },
            new PrintSettingsProfileDataModel { Type = "A4Size" },
            new PrintSettingsProfileDataModel { Type = "A4VerticalSize" },
            new PrintSettingsProfileDataModel { Type = "ASize" },
            new PrintSettingsProfileDataModel { Type = "AVerticalSize" },
            new PrintSettingsProfileDataModel { Type = "BSize" },
            new PrintSettingsProfileDataModel { Type = "CSize" },
            new PrintSettingsProfileDataModel { Type = "DSize" },
            new PrintSettingsProfileDataModel { Type = "ESize" },
            new PrintSettingsProfileDataModel { Type = "UserSize1" },
            new PrintSettingsProfileDataModel { Type = "UserSize2" },
            new PrintSettingsProfileDataModel { Type = "UserSize3" },
            new PrintSettingsProfileDataModel { Type = "UserSize4" },
            new PrintSettingsProfileDataModel { Type = "UserSize5" },
            new PrintSettingsProfileDataModel { Type = "UserSize6" },
            new PrintSettingsProfileDataModel { Type = "UserSize7" },
            new PrintSettingsProfileDataModel { Type = "UserSize8" },
            new PrintSettingsProfileDataModel { Type = "UserSize9" },
            new PrintSettingsProfileDataModel { Type = "UserSize10" },
            new PrintSettingsProfileDataModel { Type = "UserSize11" },
            new PrintSettingsProfileDataModel { Type = "UserSize12" }
        ];
    }

    public List<PrintSettingsDataModel> GetPrintSettings()
    {
        var settings = _context.PrintSettings.Include(f => f.PrinterSettingProfiles).ToList();

        if (settings.Count == 0)
        {
            // Add default settings
            _context.PrintSettings.Add(new PrintSettingsDataModel
            {
                JobName = "(Default)",
                Description = "Use all default settings",
                Copies = 1,
                PrinterSettingProfiles = GetPrintSettingsProfiles()
            });

            // Save changes to database
            _context.SaveChanges();

            settings = _context.PrintSettings.Include(f => f.PrinterSettingProfiles).ToList();
        }

        return settings;
    }

    public void AddPrintSettings(PrintSettingsDataModel dataModel)
    {
        _context.PrintSettings.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdatePrintSettings(PrintSettingsDataModel dataModel)
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
        var existingItem = _context.PrintSettings.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        // If this item is not deletable...
        if (!bypass && !existingItem.CanDelete)
            throw new InvalidOperationException($"This print setting cannot be deleted. {existingItem.JobName}");

        _context.PrintSettings.Remove(existingItem);

        if (saveChanges)
            _context.SaveChanges();
    }

    #endregion

    #region Custom Properties

    public List<ActionsTabCustomPropertiesDataModel> GetCustomPropertiesList() =>
        _context.ActionsTabCustomProperties.ToList();

    public void AddCustomPropertiesItem(ActionsTabCustomPropertiesDataModel dataModel)
    {
        _context.ActionsTabCustomProperties.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateCustomPropertiesItem(ActionsTabCustomPropertiesDataModel dataModel)
    {
        // Remove existing
        DeleteCustomPropertiesListItem(dataModel.Id);

        // Add new
        AddCustomPropertiesItem(dataModel);
    }

    public void DeleteCustomPropertiesListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionsTabCustomProperties.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionsTabCustomProperties.Remove(existingItem);
        _context.SaveChanges();
    }

    #endregion

    #region File Info

    public List<ActionsTabFileInfoDataModel> GetFileInfoList() =>
        _context.ActionsTabFileInfo.ToList();

    public void AddFileInfoItem(ActionsTabFileInfoDataModel dataModel)
    {
        _context.ActionsTabFileInfo.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateFileInfoItem(ActionsTabFileInfoDataModel dataModel)
    {
        // Remove existing
        DeleteFileInfoListItem(dataModel.Id);

        // Add new
        AddFileInfoItem(dataModel);
    }

    public void DeleteFileInfoListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionsTabFileInfo.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionsTabFileInfo.Remove(existingItem);
        _context.SaveChanges();
    }

    #endregion

    #region Save Model

    public List<ActionsTabSaveModelDataModel> GetSaveModelList() =>
        _context.ActionsTabSaveModel.ToList();

    public void AddSaveModelItem(ActionsTabSaveModelDataModel dataModel)
    {
        _context.ActionsTabSaveModel.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateSaveModelItem(ActionsTabSaveModelDataModel dataModel)
    {
        // Remove existing
        DeleteSaveModelListItem(dataModel.Id);

        // Add new
        AddSaveModelItem(dataModel);
    }

    public void DeleteSaveModelListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionsTabSaveModel.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionsTabSaveModel.Remove(existingItem);
        _context.SaveChanges();
    }

    #endregion

    #region Save Drawing

    public List<ActionsTabSaveDrawingDataModel> GetSaveDrawingList() =>
        _context.ActionsTabSaveDrawing.ToList();

    public void AddSaveDrawingItem(ActionsTabSaveDrawingDataModel dataModel)
    {
        _context.ActionsTabSaveDrawing.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateSaveDrawingItem(ActionsTabSaveDrawingDataModel dataModel)
    {
        // Remove existing
        DeleteSaveDrawingListItem(dataModel.Id);

        // Add new
        AddSaveDrawingItem(dataModel);
    }

    public void DeleteSaveDrawingListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionsTabSaveDrawing.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionsTabSaveDrawing.Remove(existingItem);
        _context.SaveChanges();
    }

    #endregion

    #region Import File

    public List<ActionsTabImportFileDataModel> GetImportFileList() =>
        _context.ActionsTabImportFile.ToList();

    public void AddImportFileItem(ActionsTabImportFileDataModel dataModel)
    {
        _context.ActionsTabImportFile.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateImportFileItem(ActionsTabImportFileDataModel dataModel)
    {
        // Remove existing
        DeleteImportFileListItem(dataModel.Id);

        // Add new
        AddImportFileItem(dataModel);
    }

    public void DeleteImportFileListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionsTabImportFile.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionsTabImportFile.Remove(existingItem);
        _context.SaveChanges();
    }

    #endregion

    #region Drawing Templates

    public List<ActionsTabDrawingTemplateDataModel> GetDrawingTemplateList() =>
        _context.ActionsTabDrawingTemplate.ToList();

    public void AddDrawingTemplateItem(ActionsTabDrawingTemplateDataModel dataModel)
    {
        _context.ActionsTabDrawingTemplate.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateDrawingTemplateItem(ActionsTabDrawingTemplateDataModel dataModel)
    {
        // Remove existing
        DeleteDrawingTemplateListItem(dataModel.Id);

        // Add new
        AddDrawingTemplateItem(dataModel);
    }

    public void DeleteDrawingTemplateListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionsTabDrawingTemplate.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionsTabDrawingTemplate.Remove(existingItem);
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

    public List<ActionsTabMacrosDataModel> GetMacrosList() =>
        _context.ActionsTabMacros.ToList();

    public void AddMacrosItem(ActionsTabMacrosDataModel dataModel)
    {
        _context.ActionsTabMacros.Add(dataModel);
        _context.SaveChanges();
    }

    public void UpdateMacrosItem(ActionsTabMacrosDataModel dataModel)
    {
        // Remove existing
        DeleteMacrosListItem(dataModel.Id);

        // Add new
        AddMacrosItem(dataModel);
    }

    public void DeleteMacrosListItem(string id)
    {
        // Remove existing
        var existingItem = _context.ActionsTabMacros.FirstOrDefault(f => f.Id == id);

        if (existingItem == null)
            return;

        _context.ActionsTabMacros.Remove(existingItem);
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