using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles student course enrollment records from RDS.FactK12StudentCourseSections,
/// joined to RDS.DimScedCodes. Assesses total enrollment record count, completeness
/// of SCED subject area assignment, and distribution across course subject areas
/// to indicate whether specific courses (e.g., Algebra 1) can be identified.
/// </summary>
public class StudentCourseEnrollmentRecordK12CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Student course enrollment record (K-12)";

    public string Query => $@"
WITH CourseBase AS (
    SELECT
        s.ScedCourseSubjectAreaCode,
        s.ScedCourseSubjectAreaDescription
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimScedCodes s
        ON f.ScedCodeId = s.DimScedCodeId
    WHERE s.ScedCourseSubjectAreaCode NOT IN ('MISSING', 'Unknown', 'Not Applicable') -- NOTE: Verify default/missing dimension label(s) in DimScedCodes
      AND s.ScedCourseSubjectAreaCode <> ''
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactK12StudentCourseSections
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactK12StudentCourseSections
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM CourseBase
UNION ALL
-- Distribution - by course subject area
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    ScedCourseSubjectAreaDescription AS SubItemLabel,
    NULL                AS Remarks
FROM CourseBase
GROUP BY ScedCourseSubjectAreaDescription
";

    public string AssessmentDescription =>
        "Assesses student course enrollment records from RDS.FactK12StudentCourseSections " +
        "joined to RDS.DimScedCodes. Reports total enrollment record count, completeness of " +
        "SCED subject area assignment (excluding default/unknown dim values), and distribution " +
        "across course subject areas. The subject area distribution indicates whether specific " +
        "courses such as Algebra 1 (Mathematics, subject area 02) can be identified in the data.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
