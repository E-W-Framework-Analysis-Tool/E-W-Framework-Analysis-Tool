# ECS State Data Import Tool

A .NET 9 console application that reads an ECS (Early Childhood through Workforce Systems) Excel data file and converts it into per-state JSON files consumed by the EW Framework.

---

## How It Works

1. **Reads the Excel file** — opens the first worksheet and iterates every row starting at row 2.
2. **Filters rows** — skips rows where:
   - The state column (col 2) is blank
   - The record type (col 5) is `Disaggregate`
   - The metric type (col 9) is not `Data element`
3. **Normalizes values** using two mapping sources:
   - **Indicator names** — a small hardcoded map corrects known name differences between the ECS source and the EW Framework (e.g. `"Access to full day pre-K"` → `"Access to full-day pre-K"`).
   - **Data element names** — loaded from `DataElementNormalizationMap.json` (must be present next to the executable). The tool exits with an error if this file is missing.
   - **Sector codes** — normalized to `PK`, `K12`, `PS`, or `WF`.
4. **Deduplicates** — rows sharing the same `indicator|sector|elementName` key are merged, keeping the highest-ranked `collected`/`reported` status (`Found` > `Partial` > `Not Found`).
5. **Writes output** — one JSON file per state (e.g. `Alabama.json`) to the output directory.

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

## Running the Import

Use the PowerShell wrapper script from the repo root. It handles restoring dependencies, building the tool, and invoking it with the correct arguments.

### Import all states

```powershell
.\eng\import-ecs-state-data.ps1 -ExcelPath ".\eng\dev-dependencies\ecs-state-data-import-tool\ECS-State-Data-Sample.xlsx"
```

Output is written to `src/EwFrameworkAnalysis.Common/FrameworkReferenceData/EcsStateData/` by default.

### Import specific states

```powershell
.\eng\import-ecs-state-data.ps1 `
    -ExcelPath ".\eng\dev-dependencies\ecs-state-data-import-tool\ECS-State-Data-Sample.xlsx" `
    -States "Alabama", "Texas", "California"
```

### Import to a custom output directory

```powershell
.\eng\import-ecs-state-data.ps1 `
    -ExcelPath ".\eng\dev-dependencies\ecs-state-data-import-tool\ECS-State-Data-Sample.xlsx" `
    -OutputDir "C:\Temp\EcsOutput"
```

### Combine filters and custom output

```powershell
.\eng\import-ecs-state-data.ps1 `
    -ExcelPath ".\eng\dev-dependencies\ecs-state-data-import-tool\ECS-State-Data-Sample.xlsx" `
    -States "Florida", "Georgia" `
    -OutputDir "C:\Temp\EcsOutput"
```

---

## Parameters

| Parameter    | Required | Default                                              | Description                                              |
|--------------|----------|------------------------------------------------------|----------------------------------------------------------|
| `-ExcelPath` | Yes      | —                                                    | Path to the ECS Excel (.xlsx) file to import             |
| `-States`    | No       | *(all states)*                                       | One or more state names to include; omit to export all   |
| `-OutputDir` | No       | `src/.../EcsStateData` (relative to repo root)       | Directory where JSON output files will be written        |

---

## Output Files

| File | Description |
|------|-------------|
| `<StateName>.json` | Array of data element records for that state, each with `sector`, `indicator`, `elementName`, `collected`, and `reported` fields |

---

## Project Files

| File | Description |
|------|-------------|
| `Program.cs` | Main import logic |
| `import.csproj` | Project definition (.NET 9, references ClosedXML) |
| `DataElementNormalizationMap.json` | Data element name normalization map (required at runtime) |
| `ECS-State-Data-Sample.xlsx` | Sample Excel file for testing |
