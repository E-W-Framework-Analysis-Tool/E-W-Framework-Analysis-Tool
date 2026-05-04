using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles quarterly earnings data from RDS.FactQuarterlyEmployments joined to
/// RDS.DimDates via EmploymentRecordReferencePeriodStartDateId. Assesses completeness
/// of QuarterlyEarnings, its numeric range, and distribution of populated records
/// by reference period year.
/// NOTE: The associated metric targets earnings relative to a post-graduation window
/// (before October 31 following graduation) and a $35,000/year threshold (~$8,750/quarter).
/// Filtering to graduation cohorts and the specific timing window is analysis logic
/// and is not applied here. This assessor profiles all available quarterly earnings
/// records to assess data presence broadly.
/// </summary>
public class EarningsCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Earnings";

    public string Query => $@"
WITH Base AS (
    SELECT
        f.QuarterlyEarnings,
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
        END)                                        AS PopulatedRecords
    FROM Base
),
RangeCalc AS (
    SELECT
        MIN(QuarterlyEarnings)                      AS MinEarnings,
        MAX(QuarterlyEarnings)                      AS MaxEarnings
    FROM Base
    WHERE QuarterlyEarnings IS NOT NULL
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
-- RecordCount
SELECT
    '{DataElementName}'                         AS DataElementName,
    'RecordCount'                               AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    NULL                                        AS SubItemLabel,
    NULL                                        AS Remarks
FROM Counts
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    'TotalRecords'                              AS SubItemLabel,
    NULL                                        AS Remarks
FROM Counts
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))     AS Value,
    'PopulatedRecords'                          AS SubItemLabel,
    'Populated = QuarterlyEarnings is non-NULL' AS Remarks
FROM Counts
UNION ALL
-- NumericalRange - Minimum
SELECT
    '{DataElementName}'                         AS DataElementName,
    'NumericalRange'                            AS CharacteristicType,
    CAST(MinEarnings AS NVARCHAR(MAX))          AS Value,
    'Minimum'                                   AS SubItemLabel,
    'QuarterlyEarnings (decimal); metric threshold ~$8,750/quarter ($35,000/year)' AS Remarks
FROM RangeCalc
UNION ALL
-- NumericalRange - Maximum
SELECT
    '{DataElementName}'                         AS DataElementName,
    'NumericalRange'                            AS CharacteristicType,
    CAST(MaxEarnings AS NVARCHAR(MAX))          AS Value,
    'Maximum'                                   AS SubItemLabel,
    'QuarterlyEarnings (decimal); metric threshold ~$8,750/quarter ($35,000/year)' AS Remarks
FROM RangeCalc
UNION ALL
-- Distribution - populated earnings records by reference period year
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Distribution'                              AS CharacteristicType,
    CAST(YearCount AS NVARCHAR(MAX))            AS Value,
    CAST(ReferencePeriodYear AS NVARCHAR(MAX))  AS SubItemLabel,
    NULL                                        AS Remarks
FROM Distribution";

    public string AssessmentDescription =>
        "Assesses quarterly earnings data from RDS.FactQuarterlyEmployments. Reports completeness " +
        "of QuarterlyEarnings, min/max range (metric threshold is ~$8,750/quarter for the $35,000/year " +
        "benchmark), and distribution of populated records by reference period year. Post-graduation " +
        "cohort filtering and timing window logic are not applied at assessment time.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
