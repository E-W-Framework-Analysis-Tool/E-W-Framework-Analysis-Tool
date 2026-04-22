namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles student race/ethnicity data across K-12 and Postsecondary sectors,
/// joined via their respective bridge tables to RDS.DimRaces. Pre-K and Workforce
/// are included as sectors in the distribution but always emit zero as no source
/// tables are available.
/// </summary>
public class StudentRaceEthnicityCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Student race/ethnicity";

    public string Query => $@"
WITH K12Race AS (
    SELECT COUNT(DISTINCT f.FactK12StudentEnrollmentId) AS RecordCount
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.BridgeK12StudentEnrollmentRaces b
        ON f.FactK12StudentEnrollmentId = b.FactK12StudentEnrollmentId
    JOIN RDS.DimRaces d
        ON b.RaceId = d.DimRaceId
    WHERE d.RaceCode IS NOT NULL
      AND d.RaceCode <> ''
),
PSRace AS (
    SELECT COUNT(DISTINCT f.FactPsStudentEnrollmentId) AS RecordCount
    FROM RDS.FactPsStudentEnrollments f
    JOIN RDS.BridgePsStudentEnrollmentRaces b
        ON f.FactPsStudentEnrollmentId = b.FactPsStudentEnrollmentId
    JOIN RDS.DimRaces d
        ON b.RaceId = d.DimRaceId
    WHERE d.RaceCode IS NOT NULL
      AND d.RaceCode <> ''
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(
        (SELECT RecordCount FROM K12Race) +
        (SELECT RecordCount FROM PSRace)
    AS NVARCHAR(MAX))   AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
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
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(CASE WHEN (SELECT RecordCount FROM K12Race) > 0 THEN 1 ELSE 0 END AS NVARCHAR(MAX)) AS Value,
    'K-12'              AS SubItemLabel,
    NULL                AS Remarks
UNION ALL
-- Distribution - Postsecondary
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(CASE WHEN (SELECT RecordCount FROM PSRace) > 0 THEN 1 ELSE 0 END AS NVARCHAR(MAX)) AS Value,
    'Postsecondary'     AS SubItemLabel,
    NULL                AS Remarks
UNION ALL
-- Distribution - Workforce
SELECT
    '{DataElementName}'      AS DataElementName,
    'Distribution'           AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX)) AS Value,
    'Workforce'              AS SubItemLabel,
    'No source table available for Workforce sector' AS Remarks
";

    public string AssessmentDescription =>
        "Assesses student race/ethnicity data presence across K-12 and Postsecondary sectors " +
        "via RDS.BridgeK12StudentEnrollmentRaces and RDS.BridgePsStudentEnrollmentRaces, both " +
        "joined to RDS.DimRaces. Reports total count of distinct enrollment records with a race " +
        "entry across both sectors and a per-sector presence indicator (1 = data present, " +
        "0 = no data). Pre-K and Workforce always return 0 as no source tables are available.";
}
