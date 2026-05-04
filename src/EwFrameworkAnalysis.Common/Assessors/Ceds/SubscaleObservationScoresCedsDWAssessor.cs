using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles subscale observation scores from RDS.FactK12StaffEvaluationParts.
/// Each row represents a subscale component score from a staff evaluation rubric
/// (e.g., Danielson's Framework for Teaching, Marzano Causal Teacher Evaluation Model).
/// Assesses record count, completeness of subscale score/rating, and distribution
/// across evaluation part scales to indicate which observation frameworks are present.
/// </summary>
public class SubscaleObservationScoresCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Subscale observation scores";

    public string Query => $@"
WITH EvalPartBase AS (
    SELECT
        StaffEvaluationPartScoreOrRating,
        StaffEvaluationPartScale
    FROM RDS.FactK12StaffEvaluationParts
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM EvalPartBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL                AS Remarks
FROM EvalPartBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN StaffEvaluationPartScoreOrRating IS NOT NULL
                     AND StaffEvaluationPartScoreOrRating <> '' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM EvalPartBase
UNION ALL
-- Distribution - by evaluation part scale (observation framework)
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    StaffEvaluationPartScale AS SubItemLabel,
    NULL                AS Remarks
FROM EvalPartBase
WHERE StaffEvaluationPartScale IS NOT NULL
  AND StaffEvaluationPartScale <> ''
GROUP BY StaffEvaluationPartScale
";

    public string AssessmentDescription =>
        "Assesses subscale observation scores from RDS.FactK12StaffEvaluationParts, where each " +
        "row represents a subscale component of a staff evaluation rubric such as Danielson's " +
        "Framework for Teaching or the Marzano Causal Teacher Evaluation Model. Reports total " +
        "record count, completeness of StaffEvaluationPartScoreOrRating, and distribution across " +
        "StaffEvaluationPartScale values to indicate which observation frameworks are present. " +
        "Overall evaluation scale and outcome fields (StaffEvaluationScale, StaffEvaluationOutcome) " +
        "are also available on this table but are not profiled here as this element focuses on " +
        "subscale scores specifically.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
