using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles industry-recognized credential attainment from RDS.FactPsStudentAcademicAwards,
/// joined to RDS.DimPsAcademicAwardStatuses.
/// NOTE: The specific filter identifying industry-recognized credentials is uncertain.
/// ProfessionalOrTechnicalCredentialConferredCode on DimPsAcademicAwardStatuses is the most
/// likely signal — replace the placeholder WHERE clause below once the relevant code values
/// are confirmed. DimPsAcademicAwardTitles (via PsAcademicAwardTitleId) may also contain
/// relevant title-based filtering if a code-based approach is insufficient.
/// </summary>
public class IndustryRecognizedCredentialAttainmentCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Industry-recognized credential attainment";

    public string Query => $@"
WITH IndustryCredentialBase AS (
    SELECT
        f.AcademicAwardDateId
    FROM RDS.FactPsStudentAcademicAwards f
    JOIN RDS.DimPsAcademicAwardStatuses d ON f.PsAcademicAwardStatusId = d.DimPsAcademicAwardStatusId
    -- NOTE: Filter below is a placeholder. Confirm which ProfessionalOrTechnicalCredentialConferredCode
    -- values (or other column) identify industry-recognized credentials in your environment,
    -- then replace this WHERE clause accordingly.
    WHERE d.ProfessionalOrTechnicalCredentialConferredCode IS NOT NULL
      AND d.ProfessionalOrTechnicalCredentialConferredCode <> ''
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM IndustryCredentialBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM IndustryCredentialBase
UNION ALL
-- Completeness - PopulatedRecords (award date present)
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(AcademicAwardDateId) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'              AS SubItemLabel,
    NULL                            AS Remarks
FROM IndustryCredentialBase";

    public string AssessmentDescription =>
        "Assesses industry-recognized credential attainment from RDS.FactPsStudentAcademicAwards. " +
        "NOTE: Currently uses a placeholder filter on ProfessionalOrTechnicalCredentialConferredCode " +
        "being non-null/non-empty. Confirm the correct code values or alternative column before " +
        "relying on these results. Reports record count and completeness of the award date field.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
