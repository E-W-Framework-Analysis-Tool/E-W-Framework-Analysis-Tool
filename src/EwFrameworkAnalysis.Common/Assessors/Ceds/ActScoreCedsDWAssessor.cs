namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles ACT scores for K-12 students from RDS.FactK12StudentAssessments,
/// joined to RDS.DimAssessments. Scoped to records with a non-null, non-empty
/// AssessmentResultScoreValueACTScore (i.e. ACT completion records). Assesses
/// completeness of a parseable numeric score value and the integer range of
/// observed scores.
///
/// NOTE: The assessor filters on AssessmentTypeCode = 'AchievementTest' as a
/// broad guard, but does not filter on assessment name. If the target state
/// records non-ACT achievement tests in the same table, consider also filtering
/// on AssessmentFamilyShortName or AssessmentTitle (e.g. LIKE '%ACT%') to narrow
/// results to ACT specifically.
/// </summary>
public class ActScoreCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "ACT score";

    public string Query => $@"
WITH ActBase AS (
    SELECT
        ase.AssessmentResultScoreValueACTScore,
        TRY_CAST(ase.AssessmentResultScoreValueACTScore AS DECIMAL(18,2)) AS ACTScoreNumeric
    FROM RDS.FactK12StudentAssessments ase
    JOIN RDS.DimAssessments da ON da.DimAssessmentId = ase.AssessmentId
    WHERE da.AssessmentTypeCode = 'AchievementTest'
      AND ase.AssessmentResultScoreValueACTScore IS NOT NULL
      AND ase.AssessmentResultScoreValueACTScore <> ''
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM ActBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM ActBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                                          AS DataElementName,
    'Completeness'                                               AS CharacteristicType,
    CAST(COUNT(ACTScoreNumeric) AS NVARCHAR(MAX))                AS Value,
    'PopulatedRecords'                                           AS SubItemLabel,
    NULL                                                         AS Remarks
FROM ActBase
UNION ALL
-- NumericalRange - Minimum
SELECT
    '{DataElementName}'                                          AS DataElementName,
    'NumericalRange'                                             AS CharacteristicType,
    CAST(MIN(ACTScoreNumeric) AS NVARCHAR(MAX))                  AS Value,
    'Minimum'                                                    AS SubItemLabel,
    NULL                                                         AS Remarks
FROM ActBase
UNION ALL
-- NumericalRange - Maximum
SELECT
    '{DataElementName}'                                          AS DataElementName,
    'NumericalRange'                                             AS CharacteristicType,
    CAST(MAX(ACTScoreNumeric) AS NVARCHAR(MAX))                  AS Value,
    'Maximum'                                                    AS SubItemLabel,
    NULL                                                         AS Remarks
FROM ActBase";

    public string AssessmentDescription =>
        "Assesses ACT score values for K-12 students, scoped to records where " +
        "AssessmentResultScoreValueACTScore is non-null and non-empty (ACT completion records). " +
        "Reports total completion record count, completeness of a parseable numeric score, " +
        "and the minimum and maximum observed scores. Unparseable score values are excluded " +
        "from the numeric range via TRY_CAST but are counted in TotalRecords, making any " +
        "gap between TotalRecords and PopulatedRecords visible.";
}
