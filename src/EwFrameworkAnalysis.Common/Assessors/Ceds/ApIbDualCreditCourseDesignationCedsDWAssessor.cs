using EwFrameworkAnalysis.Common.Models.Project;
namespace EwFrameworkAnalysis.Common.Assessors.Ceds;
/// <summary>
/// Profiles AP, IB, and Dual Credit course designation from RDS.FactK12StudentCourseSections,
/// joined to RDS.DimK12CourseStatuses. Counts distinct courses per designation type using
/// CourseLevelCharacteristicCode:
///   '00575' = Advanced Placement (AP)
///   '00574' = International Baccalaureate (IB)
///   '73048' = Dual Enrollment (Dual Credit)
///
/// NOTE: Record count uses COUNT(DISTINCT StateK12CourseId) to approximate
/// distinct courses. If StateK12CourseId is not populated by the target state,
/// substitute K12CourseSectionId as a fallback course identifier.
/// </summary>
public class ApIbDualCreditCourseDesignationCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "AP, IB, or Dual Credit course designation";
    public string Query => $@"
WITH DesignatedCourses AS (
    SELECT DISTINCT
        f.StateK12CourseId,
        CASE cs.CourseLevelCharacteristicCode
            WHEN '00575' THEN 'Advanced Placement (AP)'
            WHEN '00574' THEN 'International Baccalaureate (IB)'
            WHEN '73048' THEN 'Dual Credit'
        END AS DesignationType
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimK12CourseStatuses cs ON cs.DimK12CourseStatusId = f.K12CourseStatusId
    WHERE cs.CourseLevelCharacteristicCode IN ('00575', '00574', '73048')
      AND f.StateK12CourseId IS NOT NULL
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM DesignatedCourses
UNION ALL
-- Distribution by designation type
SELECT
    '{DataElementName}'             AS DataElementName,
    'Distribution'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    DesignationType                 AS SubItemLabel,
    NULL                            AS Remarks
FROM DesignatedCourses
GROUP BY DesignationType";
    public string AssessmentDescription =>
        "Assesses AP, IB, and Dual Credit course designation by counting distinct StateK12CourseId values " +
        "where CourseLevelCharacteristicCode is '00575' (Advanced Placement), '00574' (International Baccalaureate), " +
        "or '73048' (Dual Enrollment). Returns a total count and breakdown by designation type.";
    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
