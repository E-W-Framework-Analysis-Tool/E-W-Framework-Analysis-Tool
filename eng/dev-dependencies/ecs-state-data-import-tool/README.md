# ECS State Data Import Tool

A .NET 9 console application that reads an ECS (Early Childhood through Workforce Systems) Excel data file and converts
it into per-state JSON files consumed by the EW Framework.

---

## How It Works

1. **Reads the Excel file** — opens the first worksheet and iterates every row starting at row 2.
2. **Inherits reported status** — ECS metadata for Data Reported (col U) is incomplete at the Data Element level. A
   first pass collects the reported status of each Metric row (col 9 = `Metric`), keyed by state + Unique Order Key (col
   AK). During Data Element processing, if reported is blank, the tool looks up the Parent Metric Key (col AM) and
   inherits the parent Metric's reported status.
3. **Filters rows** — skips rows where:
   - The state column (col 2) is blank
   - The record type (col 5) is `Disaggregate`
   - The metric type (col 9) is not `Data element`
4. **Normalizes values** using two mapping sources:
   - **Indicator names** — a small hardcoded map corrects known name differences between the ECS source and the EW
     Framework (e.g. `"Access to full day pre-K"` → `"Access to full-day pre-K"`).
   - **Data element names** — loaded from `DataElementNormalizationMap.json` (must be present next to the executable).
     The tool exits with an error if this file is missing.
   - **Sector codes** — normalized to `PK`, `K12`, `PS`, or `WF`.
5. **Deduplicates** — rows sharing the same `indicator|sector|elementName` key are merged, keeping the highest-ranked
   `collected`/`reported` status (`Found` > `Partial` > `Not Found`).
6. **Writes output** — one JSON file per state (e.g. `Alabama.json`) to the output directory.

### DataElementNormalizationMap.json

A flat JSON object mapping raw ECS data element names to their normalized EW Framework equivalents:

```json
{
  "Raw ECS element name": "Normalized EW Framework name",
  ...
}
```

This file is copied to the build output directory automatically by the project and must not be removed.

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- An ECS Excel data file (a sample file `ECS-State-Data-Sample.xlsx` is included in this directory)

---

## Running the Import

All commands below assume you are at the **repository root** (`EW-Framework/`).

### Option 1 — PowerShell wrapper script (recommended)

The wrapper script handles dependency restore, build, and invocation in a single command.

#### Import all states

```powershell
.\eng\import-ecs-state-data.ps1 -ExcelPath ".\eng\dev-dependencies\ecs-state-data-import-tool\ECS-State-Data-Sample.xlsx"
```

Output is written to `src/EwFrameworkAnalysis.Common/FrameworkReferenceData/EcsStateData/` by default.

#### Import specific states

```powershell
.\eng\import-ecs-state-data.ps1 `
    -ExcelPath ".\eng\dev-dependencies\ecs-state-data-import-tool\ECS-State-Data-Sample.xlsx" `
    -States "Alabama", "Texas", "California"
```

#### Import to a custom output directory

```powershell
.\eng\import-ecs-state-data.ps1 `
    -ExcelPath ".\eng\dev-dependencies\ecs-state-data-import-tool\ECS-State-Data-Sample.xlsx" `
    -OutputDir "C:\Temp\EcsOutput"
```

#### Include collected status in the output

By default, the `collected` field is omitted from the JSON output. Use `-IncludeCollected` to include it.

```powershell
.\eng\import-ecs-state-data.ps1 `
    -ExcelPath ".\eng\dev-dependencies\ecs-state-data-import-tool\ECS-State-Data-Sample.xlsx" `
    -IncludeCollected
```

#### Combine filters and custom output

```powershell
.\eng\import-ecs-state-data.ps1 `
    -ExcelPath ".\eng\dev-dependencies\ecs-state-data-import-tool\ECS-State-Data-Sample.xlsx" `
    -States "Florida", "Georgia" `
    -OutputDir "C:\Temp\EcsOutput"
```

### Option 2 — Direct `dotnet run`

If you prefer to skip the wrapper script, restore and run the project directly. The tool expects positional arguments:
`<ExcelPath> <OutputDir> [--include-collected] [State1 State2 ...]`.

```powershell
# Restore dependencies (one-time)
dotnet restore eng\dev-dependencies\ecs-state-data-import-tool\import.csproj

# Import all states to the default output directory
dotnet run --project eng\dev-dependencies\ecs-state-data-import-tool\import.csproj -- `
    "eng\dev-dependencies\ecs-state-data-import-tool\ECS-State-Data-Sample.xlsx" `
    "src\EwFrameworkAnalysis.Common\FrameworkReferenceData\EcsStateData"

# Import specific states
dotnet run --project eng\dev-dependencies\ecs-state-data-import-tool\import.csproj -- `
    "eng\dev-dependencies\ecs-state-data-import-tool\ECS-State-Data-Sample.xlsx" `
    "src\EwFrameworkAnalysis.Common\FrameworkReferenceData\EcsStateData" `
    "Alabama" "Texas"
```

---

## Script Parameters

| Parameter           | Required | Default                | Description                                      |
| ------------------- | -------- | ---------------------- | ------------------------------------------------ |
| `-ExcelPath`        | Yes      | —                      | Path to the ECS Excel file                       |
| `-States`           | No       | _(all states)_         | State names to include; omit to export all       |
| `-OutputDir`        | No       | `src/.../EcsStateData` | Directory for JSON output files                  |
| `-IncludeCollected` | No       | `false`                | Include the `collected` field in the JSON output |

---

## Output Files

| File               | Description                                                                 |
| ------------------ | --------------------------------------------------------------------------- |
| `<StateName>.json` | Array of data element records for that state, each with `sector`,           |
|                    | `indicator`, `elementName`, `srcElementName`, and `reported` fields.        |
|                    | The `collected` field is included only when `--include-collected` is passed |

---

## Project Files

| File                               | Description                                               |
| ---------------------------------- | --------------------------------------------------------- |
| `Program.cs`                       | Main import logic                                         |
| `import.csproj`                    | Project definition (.NET 9, references ClosedXML)         |
| `DataElementNormalizationMap.json` | Data element name normalization map (required at runtime) |
| `ECS-State-Data-Sample.xlsx`       | Sample Excel file for testing                             |
