using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles course outcome data for CTE courses from RDS.FactK12StudentCourseSections,
/// scoped to Career and Technical Education general (73044) and dual-credit (73045) courses
/// via RDS.DimK12CourseStatuses. Assesses completeness of StudentCourseSectionGradeEarned
/// and distribution of CourseSectionExitTypeCode. No grade level filter is applied.
/// </summary>
public class CteCourseCompletionCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "CTE course completion";

    public string Query => $@"
WITH Base AS (
    SELECT
        f.StudentCourseSectionGradeEarned,
        e.CourseSectionExitTypeCode
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimK12CourseStatuses cs
        ON f.K12CourseStatusId = cs.DimK12CourseStatusId
    JOIN RDS.DimK12CourseSectionEnrollmentStatuses e
        ON f.K12CourseSectionEnrollmentStatusId = e.DimK12CourseSectionEnrollmentStatusId
    WHERE cs.CourseLevelCharacteristicCode IN ('73044', '73045')
    -- 73044 = Career and technical education general course
    -- 73045 = Career and technical education dual-credit course
),
Counts AS (
    SELECT
        COUNT(*)                                        AS TotalRecords,
        COUNT(CASE
            WHEN StudentCourseSectionGradeEarned IS NOT NULL
             AND StudentCourseSectionGradeEarned <> ''
            THEN 1
        END)                                            AS PopulatedRecords
    FROM Base
),
ExitDistribution AS (
    SELECT
        CourseSectionExitTypeCode,
        COUNT(*)                                        AS ExitCount
    FROM Base
    WHERE CourseSectionExitTypeCode IS NOT NULL
      AND CourseSectionExitTypeCode <> ''
      -- NOTE: 'MISSING' is the known default sentinel for this column per
      -- DimK12CourseSectionEnrollmentStatuses DDL. DimK12CourseStatuses has
      -- no confirmed sentinel — review distinct CourseLevelCharacteristicCode
      -- values and extend exclusions if additional sentinels are present.
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
    CAST(PopulatedRecords AS NVARCHAR(MAX))         AS Value,
    'PopulatedRecords'                              AS SubItemLabel,
    'Populated = StudentCourseSectionGradeEarned is non-NULL and non-empty' AS Remarks
FROM Counts
UNION ALL
-- Distribution - one row per distinct exit type present in CTE courses
SELECT
    '{DataElementName}'                             AS DataElementName,
    'Distribution'                                  AS CharacteristicType,
    CAST(ExitCount AS NVARCHAR(MAX))                AS Value,
    CourseSectionExitTypeCode                       AS SubItemLabel,
    NULL                                            AS Remarks
FROM ExitDistribution";

    public string AssessmentDescription =>
        "Assesses course outcome data readiness for CTE courses (CourseLevelCharacteristicCode 73044 " +
        "and 73045) using StudentCourseSectionGradeEarned as the primary completeness signal, " +
        "supplemented by distribution of CourseSectionExitTypeCode. No grade level filter is applied.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
