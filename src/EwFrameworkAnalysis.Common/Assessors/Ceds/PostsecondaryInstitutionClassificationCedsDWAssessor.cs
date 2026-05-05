using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles postsecondary institution classification from RDS.FactPsStudentEnrollments,
/// joined to RDS.DimPsInstitutions. Assesses record count, completeness of the
/// MostPrevalentLevelOfInstitutionCode field, and distribution across institution
/// classification categories.
/// </summary>
public class PostsecondaryInstitutionClassificationCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Postsecondary institution classification";

    public string Query => $@"
WITH InstitutionBase AS (
    SELECT
        d.MostPrevalentLevelOfInstitutionCode
    FROM RDS.FactPsStudentEnrollments f
    JOIN RDS.DimPsInstitutions d
        ON f.PSInstitutionId = d.DimPsInstitutionID
    WHERE d.MostPrevalentLevelOfInstitutionCode IS NOT NULL
      AND d.MostPrevalentLevelOfInstitutionCode <> ''
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM InstitutionBase
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
    CAST(COUNT(d.MostPrevalentLevelOfInstitutionCode) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactPsStudentEnrollments f
JOIN RDS.DimPsInstitutions d
    ON f.PSInstitutionId = d.DimPsInstitutionID
WHERE d.MostPrevalentLevelOfInstitutionCode IS NOT NULL
  AND d.MostPrevalentLevelOfInstitutionCode <> ''
UNION ALL
-- Distribution - by institution classification code
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    MostPrevalentLevelOfInstitutionCode AS SubItemLabel,
    NULL                AS Remarks
FROM InstitutionBase
GROUP BY MostPrevalentLevelOfInstitutionCode
";

    public string AssessmentDescription =>
        "Assesses postsecondary institution classification from RDS.FactPsStudentEnrollments " +
        "joined to RDS.DimPsInstitutions. Reports total record count, completeness of the " +
        "MostPrevalentLevelOfInstitutionCode field (excluding default/unknown dimension values), " +
        "and distribution across institution classification categories.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
