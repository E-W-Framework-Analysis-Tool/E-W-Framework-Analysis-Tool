using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles data readiness for Student Growth Percentile (SGP) from
/// RDS.FactK12StudentAssessments, joined to RDS.DimAssessments and
/// RDS.DimAssessmentPerformanceLevels. Assesses the count of growth measure
/// assessment records, completeness of the percentile score field, and
/// distribution of performance level score metric types to indicate how
/// growth data is stored and whether SGP derivation is supported.
/// </summary>
public class SgpForStandardizedAssessmentsCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "SGP for standardized assessments";

    public string Query => $@"
WITH GrowthBase AS (
    SELECT
        ase.AssessmentResultScoreValuePercentile,
        pl.AssessmentPerformanceLevelScoreMetric
    FROM RDS.FactK12StudentAssessments ase
    JOIN RDS.DimAssessments da
        ON da.DimAssessmentId = ase.AssessmentId
    JOIN RDS.DimAssessmentPerformanceLevels pl
        ON pl.DimAssessmentPerformanceLevelId = ase.AssessmentPerformanceLevelId
    WHERE da.AssessmentTypeCode = 'GrowthMeasure'
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM GrowthBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL                AS Remarks
FROM GrowthBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(AssessmentResultScoreValuePercentile) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM GrowthBase
UNION ALL
-- Distribution - by AssessmentPerformanceLevelScoreMetric
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    AssessmentPerformanceLevelScoreMetric AS SubItemLabel,
    NULL                AS Remarks
FROM GrowthBase
WHERE AssessmentPerformanceLevelScoreMetric IS NOT NULL
  AND AssessmentPerformanceLevelScoreMetric <> ''
GROUP BY AssessmentPerformanceLevelScoreMetric
";

    public string AssessmentDescription =>
        "Assesses data readiness for Student Growth Percentile (SGP) from " +
        "RDS.FactK12StudentAssessments filtered to AssessmentTypeCode 'GrowthMeasure'. " +
        "Reports total record count, completeness of AssessmentResultScoreValuePercentile, " +
        "and distribution of AssessmentPerformanceLevelScoreMetric types (e.g., " +
        "Growth/value-added/indexing, Percentile) to indicate how growth data is stored " +
        "and whether SGP derivation is supported.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
