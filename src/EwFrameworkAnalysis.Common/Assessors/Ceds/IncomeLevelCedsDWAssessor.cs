namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles income level (economic disadvantage status) as a disaggregate across
/// K-12, postsecondary, and adult education populations from
/// RDS.FactK12StudentEnrollments, RDS.FactPsStudentCourseTranscripts, and
/// RDS.FactAeStudentEnrollments respectively, each joined to
/// RDS.DimEconomicallyDisadvantagedStatuses. Pre-K and Workforce sectors are
/// represented with zero counts as no supporting schema was identified.
/// Assesses record count, completeness, and distribution by economic disadvantage
/// status description per sector.
/// </summary>
public class IncomeLevelCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Income level";

    public string Query => $@"
WITH K12Base AS (
    SELECT
        d.EconomicDisadvantageStatusCode,
        d.EconomicDisadvantageStatusDescription
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimEconomicallyDisadvantagedStatuses d
        ON f.EconomicallyDisadvantagedStatusId = d.DimEconomicallyDisadvantagedStatusId
),
PsBase AS (
    SELECT
        d.EconomicDisadvantageStatusCode,
        d.EconomicDisadvantageStatusDescription
    FROM RDS.FactPsStudentCourseTranscripts f
    JOIN RDS.DimEconomicallyDisadvantagedStatuses d
        ON f.EconomicallyDisadvantagedStatusId = d.DimEconomicallyDisadvantagedStatusId
),
AeBase AS (
    SELECT
        d.EconomicDisadvantageStatusCode,
        d.EconomicDisadvantageStatusDescription
    FROM RDS.FactAeStudentEnrollments f
    JOIN RDS.DimEconomicallyDisadvantagedStatuses d
        ON f.EconomicallyDisadvantagedStatusId = d.DimEconomicallyDisadvantagedStatusId
),
K12Counts AS (
    SELECT
        COUNT(*)                                                        AS TotalRecords,
        COUNT(CASE
            WHEN EconomicDisadvantageStatusCode IS NOT NULL
             AND EconomicDisadvantageStatusCode <> ''
             AND EconomicDisadvantageStatusCode <> 'MISSING'
            THEN 1 END)                                                 AS PopulatedRecords
    FROM K12Base
),
PsCounts AS (
    SELECT
        COUNT(*)                                                        AS TotalRecords,
        COUNT(CASE
            WHEN EconomicDisadvantageStatusCode IS NOT NULL
             AND EconomicDisadvantageStatusCode <> ''
             AND EconomicDisadvantageStatusCode <> 'MISSING'
            THEN 1 END)                                                 AS PopulatedRecords
    FROM PsBase
),
AeCounts AS (
    SELECT
        COUNT(*)                                                        AS TotalRecords,
        COUNT(CASE
            WHEN EconomicDisadvantageStatusCode IS NOT NULL
             AND EconomicDisadvantageStatusCode <> ''
             AND EconomicDisadvantageStatusCode <> 'MISSING'
            THEN 1 END)                                                 AS PopulatedRecords
    FROM AeBase
)
INSERT INTO #EWFProfilerResults
-- RecordCount - K12
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'RecordCount'                                       AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                 AS Value,
    NULL                                                AS SubItemLabel,
    'Source: FactK12StudentEnrollments'                 AS Remarks
FROM K12Counts
UNION ALL
-- Completeness - TotalRecords - K12
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Completeness'                                      AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                 AS Value,
    'TotalRecords'                                      AS SubItemLabel,
    'Source: FactK12StudentEnrollments'                 AS Remarks
FROM K12Counts
UNION ALL
-- Completeness - PopulatedRecords - K12
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Completeness'                                      AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))             AS Value,
    'PopulatedRecords'                                  AS SubItemLabel,
    'Source: FactK12StudentEnrollments'                 AS Remarks
FROM K12Counts
UNION ALL
-- Distribution - K12 by economic disadvantage status
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Distribution'                                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                     AS Value,
    EconomicDisadvantageStatusDescription               AS SubItemLabel,
    'Source: FactK12StudentEnrollments'                 AS Remarks
FROM K12Base
GROUP BY EconomicDisadvantageStatusDescription
UNION ALL
-- RecordCount - PS
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'RecordCount'                                       AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                 AS Value,
    NULL                                                AS SubItemLabel,
    'Source: FactPsStudentCourseTranscripts'            AS Remarks
FROM PsCounts
UNION ALL
-- Completeness - TotalRecords - PS
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Completeness'                                      AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                 AS Value,
    'TotalRecords'                                      AS SubItemLabel,
    'Source: FactPsStudentCourseTranscripts'            AS Remarks
FROM PsCounts
UNION ALL
-- Completeness - PopulatedRecords - PS
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Completeness'                                      AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))             AS Value,
    'PopulatedRecords'                                  AS SubItemLabel,
    'Source: FactPsStudentCourseTranscripts'            AS Remarks
FROM PsCounts
UNION ALL
-- Distribution - PS by economic disadvantage status
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Distribution'                                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                     AS Value,
    EconomicDisadvantageStatusDescription               AS SubItemLabel,
    'Source: FactPsStudentCourseTranscripts'            AS Remarks
FROM PsBase
GROUP BY EconomicDisadvantageStatusDescription
UNION ALL
-- RecordCount - AE
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'RecordCount'                                       AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                 AS Value,
    NULL                                                AS SubItemLabel,
    'Source: FactAeStudentEnrollments'                  AS Remarks
FROM AeCounts
UNION ALL
-- Completeness - TotalRecords - AE
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Completeness'                                      AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                 AS Value,
    'TotalRecords'                                      AS SubItemLabel,
    'Source: FactAeStudentEnrollments'                  AS Remarks
FROM AeCounts
UNION ALL
-- Completeness - PopulatedRecords - AE
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Completeness'                                      AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))             AS Value,
    'PopulatedRecords'                                  AS SubItemLabel,
    'Source: FactAeStudentEnrollments'                  AS Remarks
FROM AeCounts
UNION ALL
-- Distribution - AE by economic disadvantage status
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Distribution'                                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                     AS Value,
    EconomicDisadvantageStatusDescription               AS SubItemLabel,
    'Source: FactAeStudentEnrollments'                  AS Remarks
FROM AeBase
GROUP BY EconomicDisadvantageStatusDescription
UNION ALL
-- RecordCount - PreK (no supporting schema identified; emitted as zero)
-- NOTE: No PreK fact table with EconomicallyDisadvantagedStatusId was found in the
-- known schema. Replace this stub if FactElChildEnrollments or similar becomes available.
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'RecordCount'                                       AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX))                            AS Value,
    NULL                                                AS SubItemLabel,
    'Source: PreK (not available in schema)'            AS Remarks
UNION ALL
-- RecordCount - Workforce (no supporting schema identified; emitted as zero)
-- NOTE: No workforce fact table with EconomicallyDisadvantagedStatusId was found in the
-- known schema. Replace this stub if FactWfProgramParticipation or similar becomes available.
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'RecordCount'                                       AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX))                            AS Value,
    NULL                                                AS SubItemLabel,
    'Source: Workforce (not available in schema)'       AS Remarks";

    public string AssessmentDescription =>
        "Assesses income level (economic disadvantage status) as a disaggregate across K-12, " +
        "postsecondary, and adult education populations joined to RDS.DimEconomicallyDisadvantagedStatuses. " +
        "Reports record count, completeness of the economic disadvantage status code, and distribution " +
        "by status description per sector. Pre-K and Workforce sectors are represented with zero record " +
        "counts pending schema identification.";
}
