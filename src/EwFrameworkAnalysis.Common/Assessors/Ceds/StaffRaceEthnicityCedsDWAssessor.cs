namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles staff race/ethnicity from RDS.FactK12StaffEmployments, joined via
/// RDS.BridgeK12StaffEmploymentRaces to RDS.DimRaces. Assesses record count of
/// staff employment records, completeness of race/ethnicity assignment, and
/// distribution across race/ethnicity categories.
/// </summary>
public class StaffRaceEthnicityCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Staff race/ethnicity";

    public string Query => $@"
WITH StaffRaceBase AS (
    SELECT
        f.FactK12StaffEmploymentId,
        d.RaceDescription
    FROM RDS.FactK12StaffEmployments f
    LEFT JOIN RDS.BridgeK12StaffEmploymentRaces b
        ON f.FactK12StaffEmploymentId = b.FactK12StaffEmploymentId
    LEFT JOIN RDS.DimRaces d
        ON b.RaceId = d.DimRaceId
    WHERE d.RaceCode NOT IN ('MISSING', 'Unknown', 'Not Applicable') -- NOTE: Verify default/missing dimension label(s) in DimRaces
       OR d.RaceCode IS NULL
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(DISTINCT FactK12StaffEmploymentId) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactK12StaffEmployments
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(DISTINCT FactK12StaffEmploymentId) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactK12StaffEmployments
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(DISTINCT FactK12StaffEmploymentId) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM StaffRaceBase
WHERE RaceDescription IS NOT NULL
UNION ALL
-- Distribution - by race/ethnicity category
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    RaceDescription     AS SubItemLabel,
    NULL                AS Remarks
FROM StaffRaceBase
WHERE RaceDescription IS NOT NULL
GROUP BY RaceDescription
";

    public string AssessmentDescription =>
        "Assesses staff race/ethnicity from RDS.FactK12StaffEmployments joined via " +
        "RDS.BridgeK12StaffEmploymentRaces to RDS.DimRaces. Reports total staff employment " +
        "record count, completeness of race/ethnicity assignment (staff with at least one " +
        "non-missing race entry), and distribution across race/ethnicity categories.";
}
