namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles AP course designation from RDS.FactK12StudentCourseSections,
/// joined to RDS.DimK12CourseStatuses (CourseLevelCharacteristicCode = '00575').
/// Assesses the count of distinct AP courses present in the data.
///
/// NOTE: Record count uses COUNT(DISTINCT StateK12CourseId) to approximate
/// distinct courses. If StateK12CourseId is not populated by the target state,
/// substitute K12CourseSectionId as a fallback course identifier.
/// </summary>
public class APCourseDesignationCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "AP course designation";

    public string Query => $@"
WITH ApCourses AS (
    SELECT DISTINCT f.StateK12CourseId
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimK12CourseStatuses cs ON cs.DimK12CourseStatusId = f.K12CourseStatusId
    WHERE cs.CourseLevelCharacteristicCode = '00575'
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
FROM ApCourses";

    public string AssessmentDescription =>
        "Assesses AP course designation by counting distinct StateK12CourseId values " +
        "where CourseLevelCharacteristicCode = '00575' (Advanced placement course). " +
        "A non-zero count indicates AP course designation data is present.";
}
