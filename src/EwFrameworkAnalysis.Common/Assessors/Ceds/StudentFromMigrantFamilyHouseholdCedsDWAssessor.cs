using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles migrant student data across K-12, Postsecondary, and Workforce sectors
/// from their respective fact tables, joined to RDS.DimMigrantStatuses. Identifies
/// records where MigrantStatusCode = 'Yes' as a proxy for students from migrant
/// family households. Pre-K is included as a sector but always emits zero as no
/// source table is available.
/// </summary>
public class StudentFromMigrantFamilyHouseholdCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Student from migrant family household";

    public string Query => $@"
WITH K12Migrant AS (
    SELECT COUNT(*) AS RecordCount
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimMigrantStatuses d
        ON f.MigrantStatusId = d.DimMigrantStatusId
    WHERE d.MigrantStatusCode = 'Yes'
),
PSMigrant AS (
    SELECT COUNT(*) AS RecordCount
    FROM RDS.FactPsStudentCourseTranscripts f
    JOIN RDS.DimMigrantStatuses d
        ON f.MigrantStatusId = d.DimMigrantStatusId
    WHERE d.MigrantStatusCode = 'Yes'
),
WFMigrant AS (
    SELECT COUNT(*) AS RecordCount
    FROM RDS.FactAeStudentEnrollments f
    JOIN RDS.DimMigrantStatuses d
        ON f.MigrantStatusId = d.DimMigrantStatusId
    WHERE d.MigrantStatusCode = 'Yes'
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'  AS DataElementName,
    'RecordCount'        AS CharacteristicType,
    CAST(
        (SELECT RecordCount FROM K12Migrant) +
        (SELECT RecordCount FROM PSMigrant) +
        (SELECT RecordCount FROM WFMigrant)
    AS NVARCHAR(MAX))    AS Value,
    NULL                 AS SubItemLabel,
    NULL                 AS Remarks
UNION ALL
-- Distribution - Pre-K
SELECT
    '{DataElementName}'      AS DataElementName,
    'Distribution'           AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX)) AS Value,
    'Pre-K'                  AS SubItemLabel,
    'No source table available for Pre-K sector' AS Remarks
UNION ALL
-- Distribution - K-12
SELECT
    '{DataElementName}'  AS DataElementName,
    'Distribution'       AS CharacteristicType,
    CAST(CASE WHEN (SELECT RecordCount FROM K12Migrant) > 0 THEN 1 ELSE 0 END AS NVARCHAR(MAX)) AS Value,
    'K-12'               AS SubItemLabel,
    NULL                 AS Remarks
UNION ALL
-- Distribution - Postsecondary
SELECT
    '{DataElementName}'  AS DataElementName,
    'Distribution'       AS CharacteristicType,
    CAST(CASE WHEN (SELECT RecordCount FROM PSMigrant) > 0 THEN 1 ELSE 0 END AS NVARCHAR(MAX)) AS Value,
    'Postsecondary'      AS SubItemLabel,
    NULL                 AS Remarks
UNION ALL
-- Distribution - Workforce
SELECT
    '{DataElementName}'  AS DataElementName,
    'Distribution'       AS CharacteristicType,
    CAST(CASE WHEN (SELECT RecordCount FROM WFMigrant) > 0 THEN 1 ELSE 0 END AS NVARCHAR(MAX)) AS Value,
    'Workforce'          AS SubItemLabel,
    NULL                 AS Remarks
";

    public string AssessmentDescription =>
        "Assesses migrant student data presence across K-12, Postsecondary, and Workforce sectors. " +
        "Records are identified via MigrantStatusCode = 'Yes' on RDS.DimMigrantStatuses, which " +
        "reflects migratory child status under Title I Part C — used here as a proxy for students " +
        "from migrant family households. Reports total count of migrant-flagged records across all " +
        "sectors and a per-sector presence indicator (1 = data present, 0 = no data). Pre-K always " +
        "returns 0 as no source table is available. Postsecondary uses FactPsStudentCourseTranscripts " +
        "as the source since MigrantStatusId is available on that table.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
