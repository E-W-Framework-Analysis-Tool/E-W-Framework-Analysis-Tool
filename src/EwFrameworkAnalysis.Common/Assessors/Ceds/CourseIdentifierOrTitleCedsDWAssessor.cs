namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles course identifier and title data from RDS.FactK12StudentCourseSections
/// joined to RDS.DimK12Courses. Assesses record count and combined completeness,
/// where a record is considered populated if either CourseIdentifier or CourseTitle
/// is non-empty.
/// </summary>
public class CourseIdentifierOrTitleCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Course identifier or title";
    public string Query => $@"
WITH CourseCte AS (
    SELECT
        c.CourseIdentifier,
        c.CourseTitle
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimK12Courses c
        ON f.StateK12CourseId = c.DimK12CourseId
)
-- RecordCount
INSERT INTO #EWFProfilerResults
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM CourseCte
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM CourseCte
UNION ALL
-- Completeness - PopulatedRecords (CourseIdentifier non-empty OR CourseTitle non-empty)
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(CASE WHEN (CourseIdentifier IS NOT NULL AND CourseIdentifier <> '')
                      OR (CourseTitle      IS NOT NULL AND CourseTitle      <> '')
                    THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'              AS SubItemLabel,
    NULL                            AS Remarks
FROM CourseCte";
    public string AssessmentDescription =>
        "Profiles course identifier and title data from RDS.FactK12StudentCourseSections joined to " +
        "RDS.DimK12Courses on StateK12CourseId. Assesses total record count and completeness, where a " +
        "record is considered populated if either CourseIdentifier or CourseTitle is non-empty.";
}
