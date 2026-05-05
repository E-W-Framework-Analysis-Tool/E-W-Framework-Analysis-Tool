using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles student or family socioeconomic status from RDS.FactK12StudentEnrollments,
/// joined to RDS.DimEconomicallyDisadvantagedStatuses. The CEDS Data Warehouse does not
/// store family income as a continuous variable; this assessor uses economic disadvantage
/// status as a partial proxy, covering only the lower end of the socioeconomic spectrum.
/// Scoring should treat this element as partially available.
/// </summary>
public class StudentOrFamilySocioeconomicStatusCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Student or family socioeconomic status";

    public string Query => $@"
WITH SesBase AS (
    SELECT
        d.EconomicDisadvantageStatusCode,
        d.EconomicDisadvantageStatusDescription
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimEconomicallyDisadvantagedStatuses d
        ON f.EconomicallyDisadvantagedStatusId = d.DimEconomicallyDisadvantagedStatusId
    WHERE d.EconomicDisadvantageStatusCode IS NOT NULL
      AND d.EconomicDisadvantageStatusCode <> ''
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactK12StudentEnrollments
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactK12StudentEnrollments
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM SesBase
UNION ALL
-- Distribution - by economic disadvantage status
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    EconomicDisadvantageStatusDescription AS SubItemLabel,
    NULL                AS Remarks
FROM SesBase
GROUP BY EconomicDisadvantageStatusDescription
";

    public string AssessmentDescription =>
        "Assesses student or family socioeconomic status from RDS.FactK12StudentEnrollments " +
        "joined to RDS.DimEconomicallyDisadvantagedStatuses. Family income as a continuous " +
        "variable is not available in the CEDS Data Warehouse; this assessor uses economic " +
        "disadvantage status as a partial proxy, which reflects food service program eligibility " +
        "and direct certification indicators rather than a full socioeconomic spectrum. " +
        "Scoring should treat this element as partially available. Distribution across status " +
        "categories indicates whether any gradation beyond a simple flag is present.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
