using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles IB course designation from RDS.FactK12StudentCourseSections,
/// joined to RDS.DimK12CourseStatuses (CourseLevelCharacteristicCode = '00574').
/// Assesses the count of distinct IB courses present in the data.
///
/// NOTE: Record count uses COUNT(DISTINCT StateK12CourseId) to approximate
/// distinct courses. If StateK12CourseId is not populated by the target state,
/// substitute K12CourseSectionId as a fallback course identifier.
/// </summary>
public class IbCourseDesignationCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "IB course designation";

    public string Query => $@"
WITH IbCourses AS (
    SELECT DISTINCT f.StateK12CourseId
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimK12CourseStatuses cs ON cs.DimK12CourseStatusId = f.K12CourseStatusId
    WHERE cs.CourseLevelCharacteristicCode = '00574'
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
FROM IbCourses";

    public string AssessmentDescription =>
        "Assesses IB course designation by counting distinct StateK12CourseId values " +
        "where CourseLevelCharacteristicCode = '00574' (International Baccalaureate course). " +
        "A non-zero count indicates IB course designation data is present.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
