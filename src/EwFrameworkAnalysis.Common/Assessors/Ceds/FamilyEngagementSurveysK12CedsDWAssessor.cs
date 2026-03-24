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
        COUNT(*)        AS TotalRecords,
        SUM(HasAnyScore) AS PopulatedRecords
    FROM FamilyEngagementBase
)

INSERT INTO #Results

-- RecordCount
SELECT
    '{DataElementName}'     AS DataElementName,
    'RecordCount'           AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX)) AS Value,
    NULL                    AS SubItemLabel,
    NULL                    AS Remarks
FROM Counts

UNION ALL

-- Completeness - TotalRecords
SELECT
    '{DataElementName}'     AS DataElementName,
    'Completeness'          AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'          AS SubItemLabel,
    N'Total records joined to a family engagement assessment' AS Remarks
FROM Counts

UNION ALL

-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'     AS DataElementName,
    'Completeness'          AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'      AS SubItemLabel,
    N'Records with at least one score value (Raw, Scale, Percentile, T, or Z) populated' AS Remarks
FROM Counts

UNION ALL

-- Distribution - by matched assessment title
SELECT
    '{DataElementName}'     AS DataElementName,
    'Distribution'          AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    d.AssessmentTitle       AS SubItemLabel,
    NULL                    AS Remarks
FROM RDS.FactK12StudentAssessments f
JOIN RDS.DimAssessments d ON f.AssessmentId = d.DimAssessmentId
WHERE d.AssessmentTitle LIKE '%family%'
GROUP BY d.AssessmentTitle
";

    public string AssessmentDescription =>
        "Assesses family engagement survey assessment records from RDS.FactK12StudentAssessments " +
        "joined to RDS.DimAssessments (filtered to titles containing 'family'). " +
        "Reports total record count, completeness of any score value, and distribution " +
        "of records by matched assessment title.";
}
