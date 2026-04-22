<#
.SYNOPSIS
    Merges multiple AnalysisProject JSON files into a single combined project.

.DESCRIPTION
    Combines the DataSources arrays (each of which carries its own Assessments)
    from multiple AnalysisProject JSON files. Validates that there are no GUID
    conflicts before merging and outputs a new JSON file named "MergedProjects.json".

.PARAMETER InputFiles
    One or more paths to AnalysisProject JSON files to merge.

.PARAMETER OutputFile
    Path (relative or absolute) for the merged output file. Defaults to "MergedProjects.json" in the
    current directory. If the file already exists, the script will prompt to overwrite unless
    -NonInteractive is specified, in which case it errors and exits.

.PARAMETER NonInteractive
    Suppresses all prompts. If the output file already exists, the script will error and exit instead
    of prompting.

.EXAMPLE
    .\Merge-AnalysisProjects.ps1 -InputFiles "project1.json","project2.json"

.EXAMPLE
    .\Merge-AnalysisProjects.ps1 -InputFiles "project1.json","project2.json" -OutputFile "C:\Output\merged.json"

.EXAMPLE
    .\Merge-AnalysisProjects.ps1 -InputFiles (Get-ChildItem *.json) -NonInteractive
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory, ValueFromPipeline, ValueFromPipelineByPropertyName)]
    [Alias("FullName")]
    [string[]] $InputFiles,

    [Parameter()]
    [string] $OutputFile = "MergedProjects.json",

    [Parameter()]
    [switch] $NonInteractive
)

begin {
    $allFiles    = @()
    $allProjects = @()
}

process {
    $allFiles += $InputFiles
}

end {
    # -------------------------------------------------------------------------
    # 1. Validate input
    # -------------------------------------------------------------------------
    if ($allFiles.Count -lt 2) {
        Write-Error "At least two input files are required to perform a merge."
        return
    }

    # -------------------------------------------------------------------------
    # 2. Load and parse each file
    # -------------------------------------------------------------------------
    foreach ($file in $allFiles) {
        $resolved = Resolve-Path $file -ErrorAction SilentlyContinue
        if (-not $resolved) {
            Write-Error "File not found: $file"
            return
        }

        Write-Verbose "Loading $resolved"
        $raw = Get-Content $resolved -Raw -Encoding UTF8

        try {
            $project = $raw | ConvertFrom-Json -Depth 20
        } catch {
            Write-Error "Failed to parse JSON in '$file': $_"
            return
        }

        $project | Add-Member -NotePropertyName _SourceFile -NotePropertyValue ([System.IO.Path]::GetFileName($resolved))
        $allProjects += $project
    }

    # -------------------------------------------------------------------------
    # 3. Collect all DataSources, checking for GUID conflicts
    #    (Assessments are nested inside each DataSource and travel with them)
    # -------------------------------------------------------------------------
    $mergedDataSources   = [System.Collections.Generic.List[object]]::new()
    $seenDataSourceIds   = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    $seenAssessmentIds   = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)

    $conflicts = @()

    foreach ($project in $allProjects) {
        $src = $project._SourceFile

        foreach ($ds in @($project.DataSources)) {
            if (-not $ds) { continue }

            $dsId = $ds.Id?.ToString()
            if (-not $dsId) { $dsId = [Guid]::NewGuid().ToString() }

            if (-not $seenDataSourceIds.Add($dsId)) {
                $conflicts += "DataSource ID conflict: '$dsId' (in file '$src')"
            }

            # Also check assessment GUIDs within this DataSource
            foreach ($dsa in @($ds.Assessments)) {
                if (-not $dsa) { continue }

                $dsaId = $dsa.Id?.ToString()
                if (-not $dsaId) { $dsaId = [Guid]::NewGuid().ToString() }

                if (-not $seenAssessmentIds.Add($dsaId)) {
                    $conflicts += "DataSourceAssessment ID conflict: '$dsaId' (DataSource '$dsId', file '$src')"
                }
            }

            $mergedDataSources.Add($ds)
        }
    }

    # Abort if any conflicts were found
    if ($conflicts.Count -gt 0) {
        Write-Error ("GUID conflicts detected — merge aborted:`n" + ($conflicts -join "`n"))
        return
    }

    Write-Verbose "No GUID conflicts found across $($allFiles.Count) files."

    # -------------------------------------------------------------------------
    # 4. Build the merged project
    # -------------------------------------------------------------------------
    $sourceFileNames = $allProjects | ForEach-Object { $_._SourceFile }
    $description     = "Merged from: " + ($sourceFileNames -join ", ")

    $merged = [ordered]@{
        id             = [Guid]::NewGuid().ToString()
        title          = "Merged Projects"
        description    = $description
        createdAt      = (Get-Date -Format "o")
        lastModifiedAt = (Get-Date -Format "o")
        dataSources    = @($mergedDataSources)
    }

    # -------------------------------------------------------------------------
    # 5. Write output
    # -------------------------------------------------------------------------
    $outputPath = [System.IO.Path]::GetFullPath($OutputFile, (Get-Location))

    $outputDir = Split-Path $outputPath -Parent
    if (-not (Test-Path $outputDir)) {
        New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
    }
    
    if (Test-Path $outputPath) {
        if ($NonInteractive) {
            Write-Error "Output file already exists: '$outputPath'. Use a different path or remove the file first."
            return
        }

        $answer = Read-Host "Output file '$outputPath' already exists. Overwrite? [y/N]"
        if ($answer -notmatch '^[Yy]$') {
            Write-Host "Merge cancelled." -ForegroundColor Yellow
            return
        }
    }
    
    $merged | ConvertTo-Json -Depth 20 | Set-Content -Path $outputFile -Encoding UTF8

    $totalAssessments = ($mergedDataSources | ForEach-Object { @($_.Assessments).Count } | Measure-Object -Sum).Sum

    Write-Host "Merge complete." -ForegroundColor Green
    Write-Host "  Files merged  : $($allFiles.Count)"
    Write-Host "  DataSources   : $($mergedDataSources.Count)"
    Write-Host "  Assessments   : $totalAssessments"
    Write-Host "  Output        : $outputFile"
}