using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles instructional credit hours completed from RDS.FactPsStudentAcademicRecord
/// joined to RDS.DimSchoolYears. Assesses completeness of InstructionalActivityHoursCompletedCredit
/// and distributes populated records by school year to support first-year cohort analysis.
/// NOTE: A dedicated first-time/first-year enrollment indicator is not present in the
/// CEDS DW schema; first-year filtering must be applied at analysis time using school year
/// relative to each student's enrollment entry date.
/// </summary>
public class CreditsEarnedInFirstYearCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Credits earned in first year";

    public string Query => $@"
WITH Base AS (
    SELECT
        f.InstructionalActivityHoursCompletedCredit,
        sy.SchoolYear
    FROM RDS.FactPsStudentAcademicRecords f
    JOIN RDS.DimSchoolYears sy
        ON f.SchoolYearId = sy.DimSchoolYearId
),
Counts AS (
    SELECT
        COUNT(*)                                    AS TotalRecords,
        COUNT(CASE
            WHEN InstructionalActivityHoursCompletedCredit IS NOT NULL
             AND InstructionalActivityHoursCompletedCredit <> 0
            THEN 1
        END)                                        AS PopulatedRecords
    FROM Base
),
Distribution AS (
    SELECT
        SchoolYear,
        COUNT(*) AS YearCount
    FROM Base
    WHERE InstructionalActivityHoursCompletedCredit IS NOT NULL
      AND InstructionalActivityHoursCompletedCredit <> 0
    GROUP BY SchoolYear
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
    'Populated = InstructionalActivityHoursCompletedCredit is non-NULL and non-zero' AS Remarks
FROM Counts
UNION ALL
-- Distribution - populated credit records by school year
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Distribution'                              AS CharacteristicType,
    CAST(YearCount AS NVARCHAR(MAX))            AS Value,
    CAST(SchoolYear AS NVARCHAR(MAX))           AS SubItemLabel,
    -- NOTE: First-year cohort filtering is not applied here as no dedicated
    -- first-time enrollment indicator exists in the CEDS DW schema. Analysis
    -- layer should correlate SchoolYear against each student''s enrollment
    -- entry date to identify first-year records.
    NULL                                        AS Remarks
FROM Distribution";

    public string AssessmentDescription =>
        "Assesses postsecondary credit hours completed from RDS.FactPsStudentAcademicRecord, " +
        "profiling completeness of InstructionalActivityHoursCompletedCredit and distributing " +
        "populated records by school year to support first-year cohort analysis. No first-year " +
        "filter is applied at assessment time; first-year identification requires correlating " +
        "school year against student enrollment entry date at analysis time.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
