using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles credential attainment dates across all postsecondary award types from
/// RDS.FactPsStudentAcademicAwards, joined to RDS.DimPsAcademicAwardStatuses. Covers
/// the same postsecondary award codes as the individual assessors. Reports record count,
/// completeness of AcademicAwardDateId, and a distribution of records by award type
/// description — providing a combined view across graduate, bachelor's, associate,
/// certificate, and other postsecondary credential categories.
/// </summary>
public class CredentialAttainmentDatePostsecondaryCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Credential attainment date (Postsecondary)";

    public string Query => $@"
WITH PsAwardBase AS (
    SELECT
        f.AcademicAwardDateId,
        d.PescAwardLevelTypeDescription
    FROM RDS.FactPsStudentAcademicAwards f
    JOIN RDS.DimPsAcademicAwardStatuses d ON f.PsAcademicAwardStatusId = d.DimPsAcademicAwardStatusId
    WHERE d.PescAwardLevelTypeCode IN (
        '1.1', '1.2', '1.3', '1.4', '1.5',
        '2.0', '2.1', '2.2', '2.3', '2.4', '2.5', '2.6', '2.7', '2.8',
        '3.1', '3.2', '3.3',
        '4.0', '4.1', '4.2', '4.3', '4.4', '4.5',
        'IPEDS1', 'IPEDS2', 'IPEDS3', 'IPEDS4', 'IPEDS5',
        'IPEDS6', 'IPEDS7', 'IPEDS8', 'IPEDS9',
        'IPEDS10', 'IPEDS11', 'IPEDS17', 'IPEDS18', 'IPEDS19'
    )
      AND f.AcademicAwardDateId IS NOT NULL
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM PsAwardBase
UNION ALL
-- Completeness - TotalRecords (all postsecondary awards, date present)
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM PsAwardBase
UNION ALL
-- Completeness - PopulatedRecords (redundant here but kept for scoring consistency)
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(AcademicAwardDateId) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'              AS SubItemLabel,
    NULL                            AS Remarks
FROM PsAwardBase
UNION ALL
-- Distribution - Bachelor's
SELECT
    '{DataElementName}'             AS DataElementName,
    'Distribution'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'Bachelor''s degree'            AS SubItemLabel,
    NULL                            AS Remarks
FROM RDS.FactPsStudentAcademicAwards f
JOIN RDS.DimPsAcademicAwardStatuses d ON f.PsAcademicAwardStatusId = d.DimPsAcademicAwardStatusId
WHERE d.PescAwardLevelTypeCode IN ('2.4', '2.5', 'IPEDS5')
  AND f.AcademicAwardDateId IS NOT NULL
UNION ALL
-- Distribution - Graduate
SELECT
    '{DataElementName}'             AS DataElementName,
    'Distribution'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'Graduate credential'           AS SubItemLabel,
    NULL                            AS Remarks
FROM RDS.FactPsStudentAcademicAwards f
JOIN RDS.DimPsAcademicAwardStatuses d ON f.PsAcademicAwardStatusId = d.DimPsAcademicAwardStatusId
WHERE d.PescAwardLevelTypeCode IN (
    '3.1', '3.2', '3.3',
    '4.0', '4.1', '4.2', '4.3', '4.4', '4.5',
    'IPEDS6', 'IPEDS7', 'IPEDS8', 'IPEDS9',
    'IPEDS10', 'IPEDS11', 'IPEDS17', 'IPEDS18', 'IPEDS19'
)
  AND f.AcademicAwardDateId IS NOT NULL
UNION ALL
-- Distribution - Industry-recognized
-- NOTE: Placeholder filter — confirm ProfessionalOrTechnicalCredentialConferredCode values
-- (or alternative column) that identify industry-recognized credentials before using this count.
SELECT
    '{DataElementName}'             AS DataElementName,
    'Distribution'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'Industry-recognized credential' AS SubItemLabel,
    NULL                            AS Remarks
FROM RDS.FactPsStudentAcademicAwards f
JOIN RDS.DimPsAcademicAwardStatuses d ON f.PsAcademicAwardStatusId = d.DimPsAcademicAwardStatusId
WHERE d.ProfessionalOrTechnicalCredentialConferredCode IS NOT NULL
  AND d.ProfessionalOrTechnicalCredentialConferredCode <> ''
  AND f.AcademicAwardDateId IS NOT NULL
UNION ALL
-- Distribution - Other postsecondary (certificates, associate's, non-degree awards)
SELECT
    '{DataElementName}'             AS DataElementName,
    'Distribution'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'Other postsecondary credential' AS SubItemLabel,
    NULL                            AS Remarks
FROM RDS.FactPsStudentAcademicAwards f
JOIN RDS.DimPsAcademicAwardStatuses d ON f.PsAcademicAwardStatusId = d.DimPsAcademicAwardStatusId
WHERE d.PescAwardLevelTypeCode IN (
    '1.1', '1.2', '1.3', '1.4', '1.5',
    '2.0', '2.1', '2.2', '2.3', '2.6', '2.7', '2.8',
    'IPEDS1', 'IPEDS2', 'IPEDS3', 'IPEDS4'
)
  AND f.AcademicAwardDateId IS NOT NULL";

    public string AssessmentDescription =>
        "Combined postsecondary credential attainment date assessor covering all postsecondary PESC " +
        "award levels. Reports total record count, completeness of the award date field, and a " +
        "distribution of dated award records broken down by PescAwardLevelTypeDescription — providing " +
        "a cross-credential view spanning certificates, associate's, bachelor's, and graduate awards.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
