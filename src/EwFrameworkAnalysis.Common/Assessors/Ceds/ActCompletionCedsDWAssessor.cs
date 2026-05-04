using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles ACT completion for K-12 students from RDS.FactK12StudentAssessments,
/// joined to RDS.DimAssessments. Completion is defined as the presence of a
/// non-null, non-empty AssessmentResultScoreValueACTScore. Assesses the count
/// of records with an ACT score present.
///
/// NOTE: The assessor filters on AssessmentTypeCode = 'AchievementTest' as a
/// broad guard, but does not filter on assessment name. If the target state
/// records non-ACT achievement tests in the same table, consider also filtering
/// on AssessmentFamilyShortName or AssessmentTitle (e.g. LIKE '%ACT%') to narrow
/// results to ACT specifically.
/// </summary>
public class ActCompletionCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "ACT completion";

    public string Query => $@"
WITH ActBase AS (
    SELECT
        ase.AssessmentResultScoreValueACTScore
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
FROM ActBase";

    public string AssessmentDescription =>
        "Assesses ACT completion for K-12 students by counting records with a non-null, non-empty " +
        "AssessmentResultScoreValueACTScore. The record count itself is the completion signal. " +
        "The AchievementTest type filter may include non-ACT assessments — consider narrowing " +
        "by assessment name if the target state records multiple achievement test types.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
