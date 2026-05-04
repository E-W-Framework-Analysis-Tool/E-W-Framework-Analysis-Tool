using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

// MN: This file contains two alternative approaches in separate assessors.

/// <summary>
/// Profiles teacher qualification or certification type from RDS.FactK12StaffAssignments
/// joined to RDS.DimTeachingCredentialStatuses on TeachingCredentialStatusId. Assesses
/// record count, completeness of the credential type field, and distribution across both
/// credential type and credential basis.
/// </summary>
public class TeacherQualificationOrCertificationTypeCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Teacher qualification or certification type";

    public string Query => $@"
WITH CredentialData AS (
    SELECT
        t.TeachingCredentialTypeDescription,
        t.TeachingCredentialBasisDescription
    FROM RDS.FactK12StaffAssignments f
    JOIN RDS.DimTeachingCredentialStatuses t
        ON f.TeachingCredentialStatusId = t.DimTeachingCredentialStatusId
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM CredentialData
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM CredentialData
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                                                                                                          AS DataElementName,
    'Completeness'                                                                                                               AS CharacteristicType,
    CAST(COUNT(CASE WHEN TeachingCredentialTypeDescription IS NOT NULL AND TeachingCredentialTypeDescription <> '' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'                                                                                                           AS SubItemLabel,
    NULL                                                                                                                         AS Remarks
FROM CredentialData
UNION ALL
-- Distribution - one row per credential type
SELECT
    '{DataElementName}'                 AS DataElementName,
    'Distribution'                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))     AS Value,
    TeachingCredentialTypeDescription   AS SubItemLabel,
    'CredentialType'                    AS Remarks
FROM CredentialData
GROUP BY TeachingCredentialTypeDescription
UNION ALL
-- Distribution - one row per credential basis
SELECT
    '{DataElementName}'                 AS DataElementName,
    'Distribution'                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))     AS Value,
    TeachingCredentialBasisDescription  AS SubItemLabel,
    'CredentialBasis'                   AS Remarks
FROM CredentialData
GROUP BY TeachingCredentialBasisDescription";

    public string AssessmentDescription =>
        "Assesses teacher qualification or certification type from RDS.FactK12StaffAssignments " +
        "joined to RDS.DimTeachingCredentialStatuses. Reports total record count, completeness of " +
        "the credential type field, and distributions across both teaching credential type " +
        "(e.g., Regular/Standard, Provisional, Emergency) and credential basis " +
        "(e.g., Baccalaureate degree, Master's degree).";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}




/// <summary>
/// Alternative assessor for teacher qualification or certification type using
/// RDS.FactK12StaffAssessments joined to RDS.DimCredentialDefinitions on CredentialDefinitionId.
/// Profiles the population of staff with specific named credential definitions, as opposed to
/// the broader teaching credential status population in FactK12StaffAssignments.
/// </summary>
public class TeacherQualificationOrCertificationTypeCredentialDefinitionCedsDWAssessor //: ICedsDWAssessor //commented out to avoid discovery
{
    public string DataElementName => "Teacher qualification or certification type";

    public string Query => $@"
WITH CredentialData AS (
    SELECT
        c.CredentialTypeDescription,
        c.CredentialDefinitionTitle
    FROM RDS.FactK12StaffAssessments f
    JOIN RDS.DimCredentialDefinitions c
        ON f.CredentialDefinitionId = c.DimCredentialDefinitionId
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM CredentialData
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM CredentialData
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                                                                                                    AS DataElementName,
    'Completeness'                                                                                                         AS CharacteristicType,
    CAST(COUNT(CASE WHEN CredentialTypeDescription IS NOT NULL AND CredentialTypeDescription <> '' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'                                                                                                     AS SubItemLabel,
    NULL                                                                                                                   AS Remarks
FROM CredentialData
UNION ALL
-- Distribution - one row per credential type
SELECT
    '{DataElementName}'             AS DataElementName,
    'Distribution'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    CredentialTypeDescription       AS SubItemLabel,
    'CredentialType'                AS Remarks
FROM CredentialData
GROUP BY CredentialTypeDescription
UNION ALL
-- Distribution - one row per credential definition title
SELECT
    '{DataElementName}'             AS DataElementName,
    'Distribution'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    CredentialDefinitionTitle       AS SubItemLabel,
    'CredentialDefinitionTitle'     AS Remarks
FROM CredentialData
GROUP BY CredentialDefinitionTitle";

    public string AssessmentDescription =>
        "Alternative assessor for teacher qualification or certification type using " +
        "RDS.FactK12StaffAssessments joined to RDS.DimCredentialDefinitions. Reports total " +
        "record count, completeness of the credential type field, and distributions across " +
        "both credential type category and specific named credential definition titles. " +
        "Covers only staff with credential assessment or award records; see " +
        "TeacherQualificationOrCertificationTypeCedsDWAssessor for the broader staff " +
        "assignment population.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
