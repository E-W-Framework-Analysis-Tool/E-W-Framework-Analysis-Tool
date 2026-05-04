using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles SAT completion from RDS.FactK12StudentAssessments, filtered to grades 11-12
/// via DimAssessmentSubtests and achievement test type via DimAssessments.
/// Completion is proxied by a non-null AssessmentResultScoreValueSATScore.
/// </summary>
public class SatCompletionCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "SAT completion";

    public string Query => $@"
INSERT INTO #EWFProfilerResults
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactK12StudentAssessments ase
JOIN RDS.DimAssessments da
    ON da.DimAssessmentId = ase.AssessmentId
JOIN RDS.DimAssessmentSubtests s
    ON ase.AssessmentSubtestId = s.DimAssessmentSubtestId
WHERE ase.AssessmentResultScoreValueSATScore IS NOT NULL
  AND da.AssessmentTypeCode = 'AchievementTest'
  AND s.AssessmentLevelForWhichDesigned IN ('11', '12')
";

    public string AssessmentDescription =>
        "Counts K-12 assessment records with a non-null SAT score for grades 11-12 " +
        "(via DimAssessmentSubtests.AssessmentLevelForWhichDesigned) and assessment type " +
        "'AchievementTest'. A non-null AssessmentResultScoreValueSATScore is used as a " +
        "proxy for SAT completion.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
