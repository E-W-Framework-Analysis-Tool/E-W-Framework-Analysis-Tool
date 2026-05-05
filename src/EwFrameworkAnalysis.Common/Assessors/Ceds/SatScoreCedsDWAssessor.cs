using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles SAT scores from RDS.FactK12StudentAssessments, filtered to grades 11-12
/// via DimAssessmentSubtests and achievement test type via DimAssessments.
/// Assesses record count, completeness of the SAT score field, and numerical range
/// of score values.
/// </summary>
public class SatScoreCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "SAT score";

    public string Query => $@"
WITH SatBase AS (
    SELECT
        ase.AssessmentResultScoreValueSATScore,
        TRY_CAST(ase.AssessmentResultScoreValueSATScore AS NUMERIC(10, 2)) AS SATScoreNumeric
    FROM RDS.FactK12StudentAssessments ase
    JOIN RDS.DimAssessments da
        ON da.DimAssessmentId = ase.AssessmentId
    JOIN RDS.DimAssessmentSubtests s
        ON ase.AssessmentSubtestId = s.DimAssessmentSubtestId
    WHERE da.AssessmentTypeCode = 'AchievementTest'
      AND s.AssessmentLevelForWhichDesigned IN ('11', '12')
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM SatBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL                AS Remarks
FROM SatBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(AssessmentResultScoreValueSATScore) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM SatBase
UNION ALL
-- NumericalRange - Minimum
SELECT
    '{DataElementName}' AS DataElementName,
    'NumericalRange'    AS CharacteristicType,
    CAST(MIN(SATScoreNumeric) AS NVARCHAR(MAX)) AS Value,
    'Minimum'           AS SubItemLabel,
    NULL                AS Remarks
FROM SatBase
UNION ALL
-- NumericalRange - Maximum
SELECT
    '{DataElementName}' AS DataElementName,
    'NumericalRange'    AS CharacteristicType,
    CAST(MAX(SATScoreNumeric) AS NVARCHAR(MAX)) AS Value,
    'Maximum'           AS SubItemLabel,
    NULL                AS Remarks
FROM SatBase
";

    public string AssessmentDescription =>
        "Assesses SAT scores for grades 11-12 from RDS.FactK12StudentAssessments. Reports total " +
        "record count, completeness of AssessmentResultScoreValueSATScore, and numerical range " +
        "(min/max) of score values using TRY_CAST to handle the nvarchar score column. Assessment " +
        "type is filtered to 'AchievementTest' via DimAssessments.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
