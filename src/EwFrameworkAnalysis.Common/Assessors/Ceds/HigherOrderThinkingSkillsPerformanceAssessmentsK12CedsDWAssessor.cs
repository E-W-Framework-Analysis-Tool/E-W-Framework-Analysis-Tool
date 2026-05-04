using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Assesses the "Higher-order thinking skills performance assessments (K-12)" data element
/// from RDS.FactK12StudentAssessments joined to RDS.DimAssessments. The primary named
/// instrument per CEDS analysis recommendations is the College and Career Readiness
/// Assessment (CCRA+). Also matches broader higher-order thinking title patterns to
/// surface locally-adopted instruments. Evaluates record count, completeness of score
/// data, and distribution of distinct matched assessment titles.
/// </summary>
public class HigherOrderThinkingSkillsPerformanceAssessmentsK12CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Higher-order thinking skills performance assessments (K-12)";

    public string Query => $@"
-- NOTE: Filters target the CEDS-named instrument (CCRA+) and broader higher-order
--       thinking title patterns. '%higher%order%' may match unrelated instruments;
--       verify matched titles via the Distribution rows and tighten the predicate
--       if needed for your implementation.
WITH HotsBase AS (
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
    WHERE d.AssessmentTitle     LIKE '%CCRA%'
       OR d.AssessmentShortName LIKE '%CCRA%'
       OR d.AssessmentTitle     LIKE '%higher%order%thinking%'
       OR d.AssessmentTitle     LIKE '%higher%order%'
       OR d.AssessmentTitle     LIKE '%critical thinking%'
),
Counts AS (
    SELECT
        COUNT(*)         AS TotalRecords,
        SUM(HasAnyScore) AS PopulatedRecords
    FROM HotsBase
),
TitleCounts AS (
    SELECT
        AssessmentTitle,
        CAST(COUNT(*) AS NVARCHAR(MAX)) AS TitleCount
    FROM HotsBase
    GROUP BY AssessmentTitle
)
INSERT INTO #EWFProfilerResults

-- RecordCount
SELECT
    '{DataElementName}'                              AS DataElementName,
    'RecordCount'                                    AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))              AS Value,
    NULL                                             AS SubItemLabel,
    NULL                                             AS Remarks
FROM Counts

UNION ALL

-- Completeness - TotalRecords
SELECT
    '{DataElementName}'                              AS DataElementName,
    'Completeness'                                   AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))              AS Value,
    'TotalRecords'                                   AS SubItemLabel,
    NULL                                             AS Remarks
FROM Counts

UNION ALL

-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                              AS DataElementName,
    'Completeness'                                   AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))          AS Value,
    'PopulatedRecords'                               AS SubItemLabel,
    NULL                                             AS Remarks
FROM Counts

UNION ALL

-- Distribution - by matched assessment title (one row per distinct title;
-- fallback row emitted when filter matches no records)
SELECT
    '{DataElementName}'                              AS DataElementName,
    'Distribution'                                   AS CharacteristicType,
    COALESCE(t.TitleCount, CAST(0 AS NVARCHAR(MAX))) AS Value,
    COALESCE(t.AssessmentTitle, 'No matching titles found') AS SubItemLabel,
    NULL                                             AS Remarks
FROM (SELECT NULL AS Stub) AS Fallback
LEFT JOIN TitleCounts t ON 1 = 1";

    public string AssessmentDescription =>
        "Assesses higher-order thinking skills performance assessment records from " +
        "RDS.FactK12StudentAssessments joined to RDS.DimAssessments, filtered to the " +
        "CEDS-named instrument (CCRA+) and broader higher-order thinking and critical " +
        "thinking title patterns. Reports total record count, completeness of any score " +
        "value (Raw, Scale, Percentile, T, or Z), and distribution of records by matched " +
        "assessment title.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
