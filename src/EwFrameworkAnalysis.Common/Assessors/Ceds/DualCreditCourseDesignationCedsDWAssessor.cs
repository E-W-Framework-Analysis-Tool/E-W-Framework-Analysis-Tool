namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles dual credit course designation from RDS.FactK12StudentCourseSections,
/// joined to RDS.DimK12CourseStatuses (CourseLevelCharacteristicCode = '73048').
/// Assesses the count of distinct dual credit courses present in the data.
///
/// NOTE: Record count uses COUNT(DISTINCT StateK12CourseId) to approximate
/// distinct courses. If StateK12CourseId is not populated by the target state,
/// substitute K12CourseSectionId as a fallback course identifier.
/// </summary>
public class DualCreditCourseDesignationCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Dual credit course designation";

    public string Query => $@"
WITH DualCreditCourses AS (
    SELECT DISTINCT f.StateK12CourseId
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimK12CourseStatuses cs ON cs.DimK12CourseStatusId = f.K12CourseStatusId
    WHERE cs.CourseLevelCharacteristicCode = '73048'
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
FROM DualCreditCourses";

    public string AssessmentDescription =>
        "Assesses dual credit course designation by counting distinct StateK12CourseId values " +
        "where CourseLevelCharacteristicCode = '73048' (Dual enrollment). " +
        "A non-zero count indicates dual credit course designation data is present.";
}
