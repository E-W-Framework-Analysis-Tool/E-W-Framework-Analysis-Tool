namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles course performance (grade earned) for English Language and Literature
/// (SCED subject area 01) and Mathematics (SCED subject area 02) from
/// RDS.FactK12StudentCourseSections joined to RDS.DimScedCodes via ScedCodeId.
/// No grade level filter is applied. Completeness is assessed over
/// StudentCourseSectionGradeEarned; distribution shows record counts by subject area.
/// </summary>
public class CoursePerformanceEnglishAndMathCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Course performance (English and Math)";

    public string Query => $@"
WITH Base AS (
    SELECT
        f.StudentCourseSectionGradeEarned,
        s.ScedCourseSubjectAreaCode,
        s.ScedCourseSubjectAreaDescription
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimScedCodes s
        ON f.ScedCodeId = s.DimScedCodeId
    WHERE s.ScedCourseSubjectAreaCode IN ('01', '02')
),
Counts AS (
    SELECT
        COUNT(*)                                    AS TotalRecords,
        COUNT(CASE
            WHEN StudentCourseSectionGradeEarned IS NOT NULL
             AND StudentCourseSectionGradeEarned <> ''
            THEN 1
        END)                                        AS PopulatedRecords
    FROM Base
),
Distribution AS (
    SELECT
        ScedCourseSubjectAreaDescription,
        COUNT(*) AS SubjectCount
    FROM Base
    WHERE StudentCourseSectionGradeEarned IS NOT NULL
      AND StudentCourseSectionGradeEarned <> ''
    GROUP BY ScedCourseSubjectAreaDescription
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
    'Populated = StudentCourseSectionGradeEarned is non-NULL and non-empty' AS Remarks
FROM Counts
UNION ALL
-- Distribution - record counts by subject area (English and Math)
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Distribution'                              AS CharacteristicType,
    CAST(SubjectCount AS NVARCHAR(MAX))         AS Value,
    ScedCourseSubjectAreaDescription            AS SubItemLabel,
    NULL                                        AS Remarks
FROM Distribution";

    public string AssessmentDescription =>
        "Assesses course performance data readiness for English Language and Literature (SCED 01) and " +
        "Mathematics (SCED 02) course sections. Completeness is measured over StudentCourseSectionGradeEarned. " +
        "Distribution shows populated grade counts by subject area. No grade level filter is applied.";
}
