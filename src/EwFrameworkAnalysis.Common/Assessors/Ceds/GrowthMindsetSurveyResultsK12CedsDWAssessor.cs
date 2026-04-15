namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Assesses the "Growth mindset survey results (K-12)" data element from
/// RDS.FactK12StudentAssessments joined to RDS.DimAssessments, filtered to
/// assessments whose title or short name matches known growth mindset instrument
/// patterns. The primary named instrument per CEDS analysis recommendations is
/// the "Growth Mindset Scale". Evaluates record count, completeness of score
/// data, and distribution of distinct matched assessment titles.
/// </summary>
public class GrowthMindsetSurveyResultsK12CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Growth mindset survey results (K-12)";

    public string Query => $@"
-- NOTE: Filters below target the named instrument ('Growth Mindset Scale') and
--       broader growth mindset title patterns. The '%mindset%' fallback pattern
--       may match unrelated instruments; verify matched titles via the Distribution
--       rows and tighten the predicate if needed for your implementation.
WITH GrowthMindsetBase AS (
    SELECT
        d.AssessmentTitle,
        CASE
            WHEN f.AssessmentResultScoreValueRawScore   IS NOT NULL THEN 1
            WHEN f.AssessmentResultScoreValueScaleScore IS NOT NULL THEN 1
            WHEN f.AssessmentResultScoreValuePercentile IS NOT NULL THEN 1
            WHEN f.AssessmentResultScoreValueTScore     IS NOT NULL THEN 1
            WHEN f.AssessmentResultScoreValueZScore     IS NOT NULL THEN 1
            ELSE 0
        END AS HasAnyScore
    FROM RDS.FactK12StudentAssessments f
    JOIN RDS.DimAssessments d ON f.AssessmentId = d.DimAssessmentId
    WHERE d.AssessmentTitle      LIKE '%growth mindset scale%'
       OR d.AssessmentTitle      LIKE '%growth mindset%'
       OR d.AssessmentShortName  LIKE '%growth mindset%'
       OR d.AssessmentTitle      LIKE '%mindset%'
),
Counts AS (
    SELECT
        COUNT(*)         AS TotalRecords,
        SUM(HasAnyScore) AS PopulatedRecords
    FROM GrowthMindsetBase
),
TitleCounts AS (
    SELECT
        AssessmentTitle,
        CAST(COUNT(*) AS NVARCHAR(MAX)) AS TitleCount
    FROM GrowthMindsetBase
    GROUP BY AssessmentTitle
)
INSERT INTO #EWFProfilerResults

-- RecordCount
SELECT
    '{DataElementName}'                             AS DataElementName,
    'RecordCount'                                   AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))             AS Value,
    NULL                                            AS SubItemLabel,
    NULL                                            AS Remarks
FROM Counts

UNION ALL

-- Completeness - TotalRecords
SELECT
    '{DataElementName}'                             AS DataElementName,
    'Completeness'                                  AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))             AS Value,
    'TotalRecords'                                  AS SubItemLabel,
    NULL                                            AS Remarks
FROM Counts

UNION ALL

-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                             AS DataElementName,
    'Completeness'                                  AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))         AS Value,
    'PopulatedRecords'                              AS SubItemLabel,
    NULL                                            AS Remarks
FROM Counts

UNION ALL

-- Distribution - by matched assessment title (one row per distinct title;
-- fallback row emitted when filter matches no records)
SELECT
    '{DataElementName}'                             AS DataElementName,
    'Distribution'                                  AS CharacteristicType,
    COALESCE(t.TitleCount, CAST(0 AS NVARCHAR(MAX))) AS Value,
    COALESCE(t.AssessmentTitle, 'No matching titles found') AS SubItemLabel,
    NULL                                            AS Remarks
FROM (SELECT NULL AS Stub) AS Fallback
LEFT JOIN TitleCounts t ON 1 = 1";

    public string AssessmentDescription =>
        "Assesses growth mindset survey results from RDS.FactK12StudentAssessments joined to " +
        "RDS.DimAssessments, filtered to titles and short names matching growth mindset patterns " +
        "including the named instrument 'Growth Mindset Scale' per CEDS analysis recommendations. " +
        "Reports total record count, completeness of any score value (Raw, Scale, Percentile, T, or Z), " +
        "and distribution of records by matched assessment title.";
}
