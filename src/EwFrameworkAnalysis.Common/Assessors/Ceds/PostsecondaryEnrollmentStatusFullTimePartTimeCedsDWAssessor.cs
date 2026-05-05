using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;
/// <summary>
/// Profiles postsecondary enrollment status (full-time/part-time) from RDS.FactPsStudentEnrollments,
/// joined to RDS.DimPsEnrollmentStatuses. Assesses record count, completeness of the status FK,
/// and distribution across enrollment status categories.
/// </summary>
public class PostsecondaryEnrollmentStatusFullTimePartTimeCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Postsecondary enrollment status (Full time/part time)";
    public string Query => $@"
WITH EnrollmentBase AS (
    SELECT
        f.PsEnrollmentStatusId,
        d2.PostsecondaryEnrollmentStatusDescription
    FROM RDS.FactPsStudentEnrollments f
    JOIN RDS.DimPsEnrollmentStatuses d2
        ON f.PsEnrollmentStatusId = d2.DimPsEnrollmentStatusId
    WHERE d2.PostsecondaryEnrollmentStatusDescription NOT IN ('MISSING', 'Unknown', 'Not Applicable')
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM EnrollmentBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactPsStudentEnrollments
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(f.PsEnrollmentStatusId) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactPsStudentEnrollments f
JOIN RDS.DimPsEnrollmentStatuses d2
    ON f.PsEnrollmentStatusId = d2.DimPsEnrollmentStatusId
WHERE d2.PostsecondaryEnrollmentStatusDescription NOT IN ('MISSING', 'Unknown', 'Not Applicable')
UNION ALL
-- Distribution - by enrollment status category
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    PostsecondaryEnrollmentStatusDescription AS SubItemLabel,
    NULL                AS Remarks
FROM EnrollmentBase
GROUP BY PostsecondaryEnrollmentStatusDescription
";
    public string AssessmentDescription =>
        "Assesses postsecondary enrollment status (full-time/part-time) from RDS.FactPsStudentEnrollments " +
        "joined to RDS.DimPsEnrollmentStatuses. Reports total record count, completeness of the status " +
        "foreign key (excluding default/unknown dimension values), and distribution across enrollment " +
        "status categories (e.g., Full-time, Less than full-time but at least half-time, etc.).";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
