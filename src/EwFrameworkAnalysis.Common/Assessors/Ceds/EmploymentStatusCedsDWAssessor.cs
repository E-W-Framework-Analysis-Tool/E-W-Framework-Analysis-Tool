using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles employment status data from RDS.FactQuarterlyEmployments. The CEDS DW
/// does not contain a discrete employment status dimension — employment status is
/// implicitly encoded by the presence of a quarterly employment record with a
/// populated QuarterlyEarnings value and valid reference period. This assessor
/// reports record count and distribution by reference period year as a proxy for
/// employment status coverage.
/// NOTE: No DimEmploymentStatuses or equivalent table exists in the CEDS DW schema.
/// Full-time/part-time intensity is available via FullTimeEquivalency (decimal 0–1)
/// if a more granular status signal is needed at analysis time.
/// </summary>
public class EmploymentStatusCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Employment status";

    public string Query => $@"
WITH Base AS (
    SELECT
        f.QuarterlyEarnings,
        f.FullTimeEquivalency,
        d.Year                                      AS ReferencePeriodYear
    FROM RDS.FactQuarterlyEmployments f
    JOIN RDS.DimDates d
        ON f.EmploymentRecordReferencePeriodStartDateId = d.DimDateId
),
Counts AS (
    SELECT
        COUNT(*)                                    AS TotalRecords,
        COUNT(CASE
            WHEN QuarterlyEarnings IS NOT NULL
            THEN 1
        END)                                        AS RecordsWithEarnings,
        COUNT(CASE
            WHEN FullTimeEquivalency IS NOT NULL
            THEN 1
        END)                                        AS RecordsWithFTE
    FROM Base
),
Distribution AS (
    SELECT
        ReferencePeriodYear,
        COUNT(*)                                    AS YearCount
    FROM Base
    WHERE QuarterlyEarnings IS NOT NULL
    GROUP BY ReferencePeriodYear
)
INSERT INTO #EWFProfilerResults
-- RecordCount - all employment records
SELECT
    '{DataElementName}'                             AS DataElementName,
    'RecordCount'                                   AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))             AS Value,
    NULL                                            AS SubItemLabel,
    'Total quarterly employment records; record presence implies employed status' AS Remarks
FROM Counts
UNION ALL
-- RecordCount - records with earnings (primary employment signal)
SELECT
    '{DataElementName}'                             AS DataElementName,
    'RecordCount'                                   AS CharacteristicType,
    CAST(RecordsWithEarnings AS NVARCHAR(MAX))      AS Value,
    NULL                                            AS SubItemLabel,
    'Records with non-NULL QuarterlyEarnings (active employment signal)' AS Remarks
FROM Counts
UNION ALL
-- RecordCount - records with FTE (intensity signal)
SELECT
    '{DataElementName}'                             AS DataElementName,
    'RecordCount'                                   AS CharacteristicType,
    CAST(RecordsWithFTE AS NVARCHAR(MAX))           AS Value,
    NULL                                            AS SubItemLabel,
    'Records with non-NULL FullTimeEquivalency (full/part-time intensity signal)' AS Remarks
FROM Counts
UNION ALL
-- Distribution - employment records by reference period year
SELECT
    '{DataElementName}'                             AS DataElementName,
    'Distribution'                                  AS CharacteristicType,
    CAST(YearCount AS NVARCHAR(MAX))                AS Value,
    CAST(ReferencePeriodYear AS NVARCHAR(MAX))      AS SubItemLabel,
    NULL                                            AS Remarks
FROM Distribution";

    public string AssessmentDescription =>
        "Assesses employment status coverage from RDS.FactQuarterlyEmployments. No discrete " +
        "employment status dimension exists in the CEDS DW — status is implicitly encoded by " +
        "record presence. Reports total records, records with QuarterlyEarnings populated, and " +
        "records with FullTimeEquivalency populated as layered employment signal proxies. " +
        "Distributes populated earnings records by reference period year.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
