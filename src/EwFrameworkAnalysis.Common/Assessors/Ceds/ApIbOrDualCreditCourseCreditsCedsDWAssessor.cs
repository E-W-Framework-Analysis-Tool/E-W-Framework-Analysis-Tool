namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles AP, IB, or dual credit course credits from RDS.FactK12StudentCourseSections,
/// joined to RDS.DimK12CourseStatuses. Scoped to AP ('00575'), IB ('00574'), and
/// dual credit ('73048') course level characteristic codes. Assesses record count,
/// completeness of credits earned, and the range of credits earned values.
/// </summary>
public class ApIbOrDualCreditCourseCreditsCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "AP, IB, or Dual Credit course credits";

    public string Query => $@"
WITH CreditsBase AS (
    SELECT
        f.NumberOfCreditsEarned
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimK12CourseStatuses cs ON cs.DimK12CourseStatusId = f.K12CourseStatusId
    WHERE cs.CourseLevelCharacteristicCode IN ('00575', '00574', '73048')
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM CreditsBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM CreditsBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                                  AS DataElementName,
    'Completeness'                                       AS CharacteristicType,
    CAST(COUNT(NumberOfCreditsEarned) AS NVARCHAR(MAX))  AS Value,
    'PopulatedRecords'                                   AS SubItemLabel,
    NULL                                                 AS Remarks
FROM CreditsBase
UNION ALL
-- IntegerRange - Minimum
SELECT
    '{DataElementName}'                                  AS DataElementName,
    'IntegerRange'                                       AS CharacteristicType,
    CAST(MIN(NumberOfCreditsEarned) AS NVARCHAR(MAX))    AS Value,
    'Minimum'                                            AS SubItemLabel,
    NULL                                                 AS Remarks
FROM CreditsBase
UNION ALL
-- IntegerRange - Maximum
SELECT
    '{DataElementName}'                                  AS DataElementName,
    'IntegerRange'                                       AS CharacteristicType,
    CAST(MAX(NumberOfCreditsEarned) AS NVARCHAR(MAX))    AS Value,
    'Maximum'                                            AS SubItemLabel,
    NULL                                                 AS Remarks
FROM CreditsBase";

    public string AssessmentDescription =>
        "Assesses course credits earned for AP ('00575'), IB ('00574'), and dual credit ('73048') " +
        "courses. Reports total student-course record count, completeness of the NumberOfCreditsEarned " +
        "field, and the minimum and maximum credits earned. NumberOfCreditsEarned is a native " +
        "decimal field so no casting is required for numeric aggregation.";
}
