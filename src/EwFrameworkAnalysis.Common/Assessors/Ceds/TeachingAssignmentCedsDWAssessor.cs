using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles teaching assignment (subject and grade level) from RDS.FactK12StaffCourseSections
/// joined to RDS.DimScedCodes on ScedCodeId and RDS.DimGradeLevels on
/// CourseApplicableEducationLevelId. Assesses record count, completeness of subject area
/// and grade level fields, and distributions across SCED subject area and grade level.
/// </summary>
public class TeachingAssignmentCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Teaching assignment";

    public string Query => $@"
WITH AssignmentData AS (
    SELECT
        s.ScedCourseSubjectAreaDescription,
        g.GradeLevelDescription
    FROM RDS.FactK12StaffCourseSections f
    JOIN RDS.DimScedCodes s
        ON f.ScedCodeId = s.DimScedCodeId
    JOIN RDS.DimGradeLevels g
        ON f.CourseApplicableEducationLevelId = g.DimGradeLevelId
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM AssignmentData
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM AssignmentData
UNION ALL
-- Completeness - PopulatedRecords (subject area OR grade level populated)
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(CASE WHEN
        (ScedCourseSubjectAreaDescription IS NOT NULL AND ScedCourseSubjectAreaDescription <> '')
        OR
        (GradeLevelDescription IS NOT NULL AND GradeLevelDescription <> '')
    THEN 1 END) AS NVARCHAR(MAX))   AS Value,
    'PopulatedRecords'              AS SubItemLabel,
    NULL                            AS Remarks
FROM AssignmentData
UNION ALL
-- Distribution - one row per SCED subject area
SELECT
    '{DataElementName}'                 AS DataElementName,
    'Distribution'                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))     AS Value,
    ScedCourseSubjectAreaDescription    AS SubItemLabel,
    'SubjectArea'                       AS Remarks
FROM AssignmentData
GROUP BY ScedCourseSubjectAreaDescription
UNION ALL
-- Distribution - one row per grade level
SELECT
    '{DataElementName}'             AS DataElementName,
    'Distribution'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    GradeLevelDescription           AS SubItemLabel,
    'GradeLevel'                    AS Remarks
FROM AssignmentData
GROUP BY GradeLevelDescription";

    public string AssessmentDescription =>
        "Assesses teaching assignment subject and grade level from RDS.FactK12StaffCourseSections. " +
        "Subject area is sourced from RDS.DimScedCodes (ScedCourseSubjectAreaDescription); grade level " +
        "from RDS.DimGradeLevels (GradeLevelDescription) via CourseApplicableEducationLevelId. " +
        "Completeness is measured as records where either subject area or grade level is populated. " +
        "Distributions are reported separately for subject area and grade level, distinguished via Remarks.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
