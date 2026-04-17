namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles postsecondary student course enrollment records from
/// RDS.FactPsStudentCourseTranscripts, joined to RDS.DimCipCodes. Assesses
/// total enrollment record count, completeness of CIP code assignment, and
/// distribution across broad CIP program families (two-digit prefix) to
/// indicate whether specific instructional programs can be identified.
/// </summary>
public class StudentCourseEnrollmentRecordPostsecondaryCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Student course enrollment record (Postsecondary)";

    public string Query => $@"
WITH CourseBase AS (
    SELECT
        c.CipCode,
        LEFT(c.CipCode, 2) AS CipFamily
    FROM RDS.FactPsStudentCourseTranscripts f
    JOIN RDS.DimCipCodes c
        ON f.CipCodeId = c.DimCipCodeId
    WHERE c.CipCode IS NOT NULL
      AND c.CipCode <> ''
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactPsStudentCourseTranscripts
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactPsStudentCourseTranscripts
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
-- Distribution - by two-digit CIP program family
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    CipFamily           AS SubItemLabel,
    NULL                AS Remarks
FROM CourseBase
GROUP BY CipFamily
";

    public string AssessmentDescription =>
        "Assesses postsecondary student course enrollment records from " +
        "RDS.FactPsStudentCourseTranscripts joined to RDS.DimCipCodes. Reports total " +
        "enrollment record count, completeness of CIP code assignment, and distribution " +
        "across broad instructional program families grouped by two-digit CIP prefix " +
        "(e.g., 27 = Mathematics, 51 = Health Professions). The family distribution " +
        "indicates whether specific program types can be identified in the data.";
}
