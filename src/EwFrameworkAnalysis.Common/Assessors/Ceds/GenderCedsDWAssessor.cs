using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles gender (sex) data across K-12, Postsecondary, and Workforce sectors.
/// Assesses total record count across all sectors and presence of any gender data
/// per sector. Pre-K is included as a sector in the distribution but always emits
/// zero as no source table is available.
/// </summary>
public class GenderCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Gender";

    public string Query => $@"
WITH K12Gender AS (
    SELECT COUNT(*) AS RecordCount
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimK12Demographics d ON f.K12DemographicId = d.DimK12DemographicId
    WHERE d.SexCode IS NOT NULL AND d.SexCode <> ''
),
-- NOTE: FactPsStudentEnrollments does not carry PsDemographicId.
-- FactPsStudentAcademicRecord is used as a proxy; may undercount students
-- with enrollment records but no corresponding academic record rows.
PSGender AS (
    SELECT COUNT(*) AS RecordCount
    FROM RDS.FactPsStudentAcademicRecords f
    JOIN RDS.DimPsDemographics d ON f.PsDemographicId = d.DimPsDemographicId
    WHERE d.SexCode IS NOT NULL AND d.SexCode <> ''
),
WFGender AS (
    SELECT COUNT(*) AS RecordCount
    FROM RDS.FactAeStudentEnrollments f
    JOIN RDS.DimAeDemographics d ON f.AeDemographicId = d.DimAeDemographicId
    WHERE d.SexCode IS NOT NULL AND d.SexCode <> ''
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'                                                      AS DataElementName,
    'RecordCount'                                                            AS CharacteristicType,
    CAST(
        (SELECT RecordCount FROM K12Gender) +
        (SELECT RecordCount FROM PSGender) +
        (SELECT RecordCount FROM WFGender)
    AS NVARCHAR(MAX))                                                        AS Value,
    NULL                                                                     AS SubItemLabel,
    NULL                                                                     AS Remarks
UNION ALL
-- Distribution - Pre-K
SELECT
    '{DataElementName}'  AS DataElementName,
    'Distribution'       AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX)) AS Value,
    'Pre-K'              AS SubItemLabel,
    'No source table available for Pre-K sector' AS Remarks
UNION ALL
-- Distribution - K-12
SELECT
    '{DataElementName}'  AS DataElementName,
    'Distribution'       AS CharacteristicType,
    CAST(CASE WHEN (SELECT RecordCount FROM K12Gender) > 0 THEN 1 ELSE 0 END AS NVARCHAR(MAX)) AS Value,
    'K-12'               AS SubItemLabel,
    NULL                 AS Remarks
UNION ALL
-- Distribution - Postsecondary
SELECT
    '{DataElementName}'  AS DataElementName,
    'Distribution'       AS CharacteristicType,
    CAST(CASE WHEN (SELECT RecordCount FROM PSGender) > 0 THEN 1 ELSE 0 END AS NVARCHAR(MAX)) AS Value,
    'Postsecondary'      AS SubItemLabel,
    NULL                 AS Remarks
UNION ALL
-- Distribution - Workforce
SELECT
    '{DataElementName}'  AS DataElementName,
    'Distribution'       AS CharacteristicType,
    CAST(CASE WHEN (SELECT RecordCount FROM WFGender) > 0 THEN 1 ELSE 0 END AS NVARCHAR(MAX)) AS Value,
    'Workforce'          AS SubItemLabel,
    NULL                 AS Remarks
";

    public string AssessmentDescription =>
        "Assesses gender (sex) data presence across K-12, Postsecondary, and Workforce sectors. " +
        "Reports total count of records with a populated SexCode across all sectors, and a " +
        "per-sector presence indicator (1 = data present, 0 = no data). Pre-K always returns 0 " +
        "as no source table is available. Postsecondary uses FactPsStudentAcademicRecord as a " +
        "proxy since FactPsStudentEnrollments does not carry a demographic foreign key.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
