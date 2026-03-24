namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles course identifier and title data from RDS.FactK12StudentCourseSections
/// joined to RDS.DimK12Courses. Assesses record count and completeness of both
/// the CourseIdentifier and CourseTitle fields.
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
-- Completeness - TotalRecords (CourseIdentifier)
SELECT
    '{DataElementName}'                 AS DataElementName,
    'Completeness'                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))     AS Value,
    'TotalRecords'                      AS SubItemLabel,
    'CourseIdentifier'                  AS Remarks
FROM CourseCte
UNION ALL
-- Completeness - PopulatedRecords (CourseIdentifier)
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(COUNT(CASE WHEN CourseIdentifier <> '' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'                                                        AS SubItemLabel,
    'CourseIdentifier'                                                        AS Remarks
FROM CourseCte
UNION ALL
-- Completeness - TotalRecords (CourseTitle)
SELECT
    '{DataElementName}'                 AS DataElementName,
    'Completeness'                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))     AS Value,
    'TotalRecords'                      AS SubItemLabel,
    'CourseTitle'                       AS Remarks
FROM CourseCte
UNION ALL
-- Completeness - PopulatedRecords (CourseTitle)
SELECT
    '{DataElementName}'                      AS DataElementName,
    'Completeness'                           AS CharacteristicType,
    CAST(COUNT(CASE WHEN CourseTitle <> '' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'                                                    AS SubItemLabel,
    'CourseTitle'                                                         AS Remarks
FROM CourseCte";

    public string AssessmentDescription =>
        "Profiles course identifier and title data from RDS.FactK12StudentCourseSections joined to " +
        "RDS.DimK12Courses on StateK12CourseId. Assesses total record count and completeness, where a " +
        "record is considered populated if either CourseIdentifier or CourseTitle is non-empty.";
}
