namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Assesses the "Family engagement surveys (K-12)" data element from
/// RDS.FactK12StudentAssessments joined to RDS.DimAssessments,
/// filtered to assessments whose title contains 'family'.
/// Evaluates record count, completeness of score data, and distribution
/// of distinct assessment titles matched by the filter.
/// </summary>
public class FamilyEngagementSurveysK12CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Family engagement surveys (K-12)";
    public string Query => $@"
-- NOTE: The filter d.AssessmentTitle LIKE '%family%' is a proximity match for family
--       engagement survey instruments. Verify that this filter correctly and completely
--       identifies all relevant assessment records in your implementation, and adjust
--       the predicate (e.g. an exact title, a code column, or a category flag) if needed.

-- NOTE: Distribution rows are grouped by AssessmentTitle, so the number of Distribution
--       rows is data-driven (one per distinct matched title). This is intentional given
--       that assessment titles are not known at query-write time. A fallback row is
--       emitted when no titles match to ensure the branch always contributes at least
--       one row.
WITH FamilyEngagementBase AS (
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
    WHERE d.AssessmentTitle LIKE '%family%'
),
Counts AS (
    SELECT
        COUNT(*)         AS TotalRecords,
        SUM(HasAnyScore) AS PopulatedRecords
    FROM FamilyEngagementBase
),
TitleCounts AS (
    SELECT
        AssessmentTitle,
        CAST(COUNT(*) AS NVARCHAR(MAX)) AS TitleCount
    FROM FamilyEngagementBase
    GROUP BY AssessmentTitle
)
INSERT INTO #EWFProfilerResults

-- RecordCount
SELECT
    '{DataElementName}'                         AS DataElementName,
    'RecordCount'                               AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    NULL                                        AS SubItemLabel,
    NULL                                        AS Remarks
FROM Counts

UNION ALL

-- Completeness - TotalRecords
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    'TotalRecords'                              AS SubItemLabel,
    NULL                                        AS Remarks
FROM Counts

UNION ALL

-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))     AS Value,
    'PopulatedRecords'                          AS SubItemLabel,
    NULL                                        AS Remarks
FROM Counts

UNION ALL

-- Distribution - by matched assessment title (one row per distinct title;
-- fallback row emitted when filter matches no records)
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Distribution'                              AS CharacteristicType,
    COALESCE(t.TitleCount, CAST(0 AS NVARCHAR(MAX))) AS Value,
    COALESCE(t.AssessmentTitle, 'No matching titles found') AS SubItemLabel,
    NULL                                        AS Remarks
FROM (SELECT NULL AS Stub) AS Fallback
LEFT JOIN TitleCounts t ON 1 = 1";

    public string AssessmentDescription =>
        "Assesses family engagement survey assessment records from RDS.FactK12StudentAssessments " +
        "joined to RDS.DimAssessments (filtered to titles containing 'family'). " +
        "Reports total record count, completeness of any score value (Raw, Scale, Percentile, " +
        "T, or Z), and distribution of records by matched assessment title.";
}
