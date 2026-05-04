using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles course outcome data from RDS.FactK12StudentCourseSections using two
/// complementary pass/fail signals: StudentCourseSectionGradeEarned (direct grade)
/// and CourseSectionExitTypeCode via RDS.DimK12CourseSectionEnrollmentStatuses
/// (completion circumstance). No grade level or subject area filter is applied.
/// </summary>
public class CourseOutcomeCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Course outcome";

    public string Query => $@"
WITH Base AS (
    SELECT
        f.StudentCourseSectionGradeEarned,
        e.CourseSectionExitTypeCode
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimK12CourseSectionEnrollmentStatuses e
        ON f.K12CourseSectionEnrollmentStatusId = e.DimK12CourseSectionEnrollmentStatusId
),
Counts AS (
    SELECT
        COUNT(*)                                        AS TotalRecords,
        COUNT(CASE
            WHEN StudentCourseSectionGradeEarned IS NOT NULL
             AND StudentCourseSectionGradeEarned <> ''
            THEN 1
        END)                                            AS PopulatedGrades
    FROM Base
),
ExitDistribution AS (
    SELECT
        CourseSectionExitTypeCode,
        COUNT(*) AS ExitCount
    FROM Base
    WHERE CourseSectionExitTypeCode IS NOT NULL
      AND CourseSectionExitTypeCode <> ''
      AND CourseSectionExitTypeCode <> 'MISSING'
    GROUP BY CourseSectionExitTypeCode
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
    CAST(PopulatedGrades AS NVARCHAR(MAX))          AS Value,
    'PopulatedRecords'                              AS SubItemLabel,
    'Populated = StudentCourseSectionGradeEarned is non-NULL and non-empty' AS Remarks
FROM Counts
UNION ALL
-- Distribution - one row per distinct exit type present in data
SELECT
    '{DataElementName}'                             AS DataElementName,
    'Distribution'                                  AS CharacteristicType,
    CAST(ExitCount AS NVARCHAR(MAX))                AS Value,
    CourseSectionExitTypeCode                       AS SubItemLabel,
    NULL                                            AS Remarks
FROM ExitDistribution";

    public string AssessmentDescription =>
        "Assesses course outcome data readiness using StudentCourseSectionGradeEarned as the primary " +
        "completeness signal, supplemented by distribution of CourseSectionExitTypeCode (course " +
        "completion circumstance) as a secondary outcome indicator. No grade level or subject area " +
        "filter is applied — all course section records are included.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
