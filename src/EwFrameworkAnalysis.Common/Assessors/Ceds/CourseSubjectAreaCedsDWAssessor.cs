using EwFrameworkAnalysis.Common.Models.Project;
namespace EwFrameworkAnalysis.Common.Assessors.Ceds;
/// <summary>
/// Profiles course subject area data from RDS.FactK12StudentCourseSections
/// joined to RDS.DimScedCodes via ScedCodeId. Assesses record count,
/// completeness of SCED code population, and distribution by subject area.
/// Records where ScedCourseSubjectAreaCode = 'MISSING' are treated as unpopulated.
/// </summary>
public class CourseSubjectAreaCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Course subject area";
    public string Query => $@"
WITH ScedBase AS (
    SELECT
        s.ScedCourseSubjectAreaCode,
        s.ScedCourseSubjectAreaDescription
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimScedCodes s ON f.ScedCodeId = s.DimScedCodeId
),
Counts AS (
    SELECT
        COUNT(*)                                                        AS TotalRecords,
        COUNT(CASE
            WHEN ScedCourseSubjectAreaCode <> 'MISSING'
             AND ScedCourseSubjectAreaCode <> ''
            THEN 1
        END)                                                            AS PopulatedRecords
    FROM ScedBase
),
SubjectDistribution AS (
    SELECT
        ScedCourseSubjectAreaDescription,
        COUNT(*) AS SubjectCount
    FROM ScedBase
    WHERE ScedCourseSubjectAreaCode <> 'MISSING'
      AND ScedCourseSubjectAreaCode <> ''
    GROUP BY ScedCourseSubjectAreaDescription
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'                             AS DataElementName,
    'RecordCount'                                   AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))             AS Value,
    NULL                                            AS SubItemLabel,
    NULL                                            AS Remarks
FROM Counts
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'                             AS DataElementName,
    'Completeness'                                  AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))             AS Value,
    'TotalRecords'                                  AS SubItemLabel,
    NULL                                            AS Remarks
FROM Counts
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                             AS DataElementName,
    'Completeness'                                  AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))         AS Value,
    'PopulatedRecords'                              AS SubItemLabel,
    'Populated = ScedCourseSubjectAreaCode is non-MISSING and non-empty' AS Remarks
FROM Counts
UNION ALL
-- Distribution by subject area
SELECT
    '{DataElementName}'                             AS DataElementName,
    'Distribution'                                  AS CharacteristicType,
    CAST(SubjectCount AS NVARCHAR(MAX))             AS Value,
    ScedCourseSubjectAreaDescription                AS SubItemLabel,
    NULL                                            AS Remarks
FROM SubjectDistribution";
    public string AssessmentDescription =>
        "Assesses course subject area classification using SCED (School Courses for the Exchange of " +
        "Education Data) codes from RDS.DimScedCodes joined via ScedCodeId. Reports total record count, " +
        "completeness of SCED subject area population (excluding MISSING sentinel values), and " +
        "distribution of student course section records by subject area.";
    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
