# BatchProcess3 - Codebase Guide

## Overview

BatchProcess3 is a SolidWorks automation tool that helps CAD users batch-process common tasks (printing, exporting, custom properties, macros, etc.) across multiple files. It consists of a **Desktop Client** built with Avalonia UI, a **SolidWorks Host** that runs inside SolidWorks and exposes a REST API, and a **shared Core library** that defines the communication contracts between them.

**Tech Stack:** C# / .NET 10.0 / Avalonia UI 11.3 / EF Core (SQLite) / CommunityToolkit.Mvvm / ASP.NET Core (Kestrel)

---

## Solution Structure

```
BatchProcess3.sln
├── Client/
│   ├── BatchProcess3/              # Main UI library (Views, ViewModels, Services)
│   └── BatchProcess3.Desktop/      # Platform executable (Windows/macOS entry point)
├── Shared/
│   └── BatchProcess3.Core/         # Shared models and API contracts
└── SolidWorks Host/
    └── BatchProcess3Host/          # SolidWorks host with embedded Kestrel web server
```

---

## Projects

### BatchProcess3.Core (Shared Library)

Defines the communication contracts between Client and Host. Contains no dependencies beyond .NET.

| File | Purpose |
|------|---------|
| `SolidWorks/BatchProcessHostUrls.cs` | Static API endpoint URL constants (e.g. `/solidworks/active/list`) |
| `SolidWorks/SolidWorksFileDetails.cs` | Shared data model for SolidWorks file info (`FilePath`, `FileName`, `IsActiveInSolidWorks`) |

### BatchProcess3 (Main UI Library)

The core application - all Views, ViewModels, Services, and business logic live here. Uses MVVM with CommunityToolkit.Mvvm and Avalonia UI.

#### Key Directories

| Directory | Purpose |
|-----------|---------|
| `Actions/` | `ActionService` - loads available actions from DB and converts to ViewModels |
| `Bootstrap/` | `Bootstrapper.cs` - DI container configuration (all service registrations) |
| `Controls/` | Custom Avalonia controls (`HeaderBanner`, `IconButton`) and control templates |
| `Crash/` | `CrashService` - crash logging/recovery, stores to `lastcrash.json` |
| `DataStorage/` | EF Core DbContext, `DatabaseService` (repository), and all data models |
| `DataStorage/DataModels/` | Entity models - `ActionDataModel` (base), 8 action types, `ProcessDataModel`, `SettingsDataModel` |
| `Dialog/` | `DialogService` - modal dialog management, file/folder pickers, `TopLevelLocator` |
| `MainApp/` | `PageFactory`, `ApplicationPageNames` enum (Home, Process, Actions, Macros, Reporter, History, Settings) |
| `Printer/` | `PrinterService` - system printer enumeration |
| `SolidWorks/` | `BatchProcessClient` - HTTP client for communicating with Host. Has `DummyData` mode for dev without SolidWorks |
| `Styles/` | AXAML style resources |
| `ValueConverters/` | Avalonia value converters for data binding |
| `ViewModels/` | All ViewModels (see ViewModel hierarchy below) |
| `Views/` | All Avalonia AXAML views |

#### ViewModel Hierarchy

```
ViewModelBase                          # JSON state management, HasChanged dirty tracking
├── DialogViewModel                    # Base for dialog ViewModels
│   ├── ConfirmDialogViewModel
│   ├── ErrorViewModel
│   └── ActionPrintSettingsViewModel
├── PageViewModel                      # Base for pages (has PageName property)
│   ├── MainViewModel                  # Navigation hub, dialog overlay host
│   ├── HomePageViewModel              # Job runner - file list + action composition
│   ├── ProcessPageViewModel           # Saved process management (CRUD)
│   ├── ActionsPageViewModel           # Action library (8 action types, each with CRUD)
│   ├── SettingsPageViewModel          # App config (host address, PDME vault, paths)
│   ├── MacrosPageViewModel            # (placeholder)
│   ├── ReporterPageViewModel          # (placeholder)
│   └── HistoryPageViewModel           # (placeholder)
├── ActionViewModel                    # Base action ViewModel
├── ProcessActionViewModel             # Action within a process
├── ProcessViewModel                   # Process with action collection
├── JobViewModel                       # Job execution
├── AvailableActionItemViewModel       # Action in the available actions list
├── SelectableItemListViewModel<T>     # Generic reusable list with CRUD
└── KeyValueViewModel                  # Generic key-value pair
```

#### Data Model Hierarchy

```
ActionDataModel                        # Base: Id, JobName, Description, SortOrder
├── ActionPrintDataModel               # Print job config (linked to PrinterSettings)
├── ActionCustomPropertiesDataModel    # Custom property rules
├── ActionFileInfoDataModel            # File information actions
├── ActionSaveModelDataModel           # Save/export model formats
├── ActionSaveDrawingDataModel         # Save/export drawing formats
├── ActionImportFileDataModel          # Import file actions
├── ActionDrawingTemplateDataModel     # Drawing template operations
├── ActionMacrosDataModel              # Macro execution config
└── ProcessActionDataModel             # Links an Action to a Process

ActionPrintSettingsDataModel           # Print settings profile (standalone)
ActionPrintSettingsProfileDataModel    # Printer config per paper size (child of PrintSettings)
ProcessDataModel                       # Process entity with child Actions
SettingsDataModel                      # App settings (host, PDME vault, paths)
```

**Database:** SQLite at `~/Documents/BatchProcess/settings.db`, uses TPC (Table Per Concrete Type) mapping for the `ActionDataModel` hierarchy.

### BatchProcess3.Desktop (Platform Executable)

Thin wrapper that starts the Avalonia app. Includes crash recovery with auto-restart (10-second cooldown). Has a post-build script (`BuildScripts/create_mac_app.sh`) that creates a macOS `.app` bundle.

### BatchProcess3Host (SolidWorks Host)

Runs **both** an Avalonia UI window and an ASP.NET Core Kestrel web server (`http://localhost:5000`) in a single process. Kestrel runs on a background thread; Avalonia runs on the main thread. When the UI closes, Kestrel is gracefully shut down via `CancellationToken`.

**API Endpoints:**

| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/` | Test endpoint - returns SolidWorks version |
| GET | `/solidworks/active/list` | Returns `List<SolidWorksFileDetails>` of files in the active assembly |

**Key Service:** `BatchProcessHost` - currently uses test files from `SolidWorks/Test Files/` directory. Will eventually call the SolidWorks COM API.

---

## Communication Architecture

```
┌──────────────────────────────────┐
│  BatchProcess3.Desktop (Client)  │
│                                  │
│  HomePageViewModel               │
│    └── BatchProcessClient        │
│         (HttpClient)             │
└───────────┬──────────────────────┘
            │  HTTP GET (JSON)
            │  http://{host}:{port}/solidworks/active/list
            ▼
┌──────────────────────────────────┐
│  BatchProcess3Host               │
│                                  │
│  Kestrel (ASP.NET Core)          │
│    └── BatchProcessHost service  │
│         └── SolidWorks COM API   │
│                                  │
│  Avalonia UI (status window)     │
└──────────────────────────────────┘

 Shared via BatchProcess3.Core:
   - BatchProcessHostUrls (endpoint constants)
   - SolidWorksFileDetails (data contract)
```

The Client connects to the Host via the address configured in Settings (`SettingsDataModel.SolidWorksHost`). During development, `BatchProcessClient.DummyData = true` bypasses the HTTP call and loads sample files from disk.

---

## Dependency Injection

Configured in `Bootstrap/Bootstrapper.cs`:

**Singletons:** `MainViewModel`, `HomePageViewModel`, `DialogService`, `PageFactory`, `DatabaseFactory`

**Transient:** All other page ViewModels, `ActionService`, `PrinterService`, `BatchProcessClient`, dialog ViewModels, `ApplicationDbContext`, `DatabaseService`

Page navigation uses a factory pattern: `PageFactory` resolves ViewModels via a `Func<Type, PageViewModel>` delegate registered in DI.

---

## Key Patterns

| Pattern | Implementation |
|---------|---------------|
| **MVVM** | CommunityToolkit.Mvvm + Avalonia data binding. Views auto-resolved from ViewModel names via `ViewLocator` |
| **State Management** | `ViewModelBase` serializes state to JSON for dirty-tracking (`HasChanged`) and undo (`RestoreState()`) |
| **Repository** | `DatabaseService` wraps all EF Core operations with business-level methods |
| **Factory** | `PageFactory` for page ViewModels, `DatabaseFactory` for scoped DB access |
| **Dialog Provider** | `IDialogProvider` interface + `DialogService` for modal overlays with async results |
| **Reusable CRUD Lists** | `SelectableItemListViewModel<T>` provides generic select/add/edit/delete UI pattern |
| **Change Tracking** | `CollectionExtensions.SetAndObserveEverything()` propagates child collection changes to parent |

---

## Action System

The core concept of the application. Users create **Actions** (individual tasks like "Print to PDF" or "Set custom property") and compose them into **Processes** (ordered sequences of actions). On the Home page, users select files from SolidWorks, drag in actions/processes, and run the batch job.

**8 Action Types:**
1. **Print** - Print files with configurable printer/paper settings
2. **Custom Properties** - Read/write SolidWorks custom properties
3. **File Info** - File information operations
4. **Save Model** - Export models (STEP, IGES, etc.)
5. **Save Drawing** - Export drawings (PDF, DXF, etc.)
6. **Import File** - Import external files
7. **Drawing Templates** - Apply/manage drawing templates
8. **Macros** - Run SolidWorks VBA/VSTA macros

---

## Development Notes

- **No test projects** exist in the solution currently
- **Dummy data mode** (`BatchProcessClient.DummyData = true`) is enabled by default for development without SolidWorks
- Database uses `EnsureCreated()` - no EF migrations yet
- Central NuGet package management via `Directory.Packages.props`
- All projects target .NET 10.0
- Compiled bindings are enforced (`AvaloniaUseCompiledBindingsByDefault`)
- Host API has no authentication (localhost-only communication)
