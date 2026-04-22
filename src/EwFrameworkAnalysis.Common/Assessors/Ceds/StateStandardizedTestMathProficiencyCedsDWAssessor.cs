namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles math state standardized test records for grade 3 from
/// RDS.FactK12StudentAssessments joined to RDS.DimAssessments and
/// RDS.FactK12StudentEnrollments (for grade level). Filters to
/// AssessmentAcademicSubjectCode = '01166' (Mathematics) and GradeLevelCode = '03'.
/// NOTE: The CEDS option set does not include a 'StateSummative' AssessmentTypeCode.
/// This assessor uses AssessmentTypeCode IN ('AchievementTest',
/// 'AlternateAssessmentGradeLevelStandards', 'AlternateAssessmentAlternateStandards',
/// 'AlternativeAssessmentModifiedStandards') as the best available approximation for
/// state standardized assessments. Verify that these codes capture your state's
/// summative assessment records and adjust if needed. The Distribution by assessment
/// title is intended to surface which instruments are present.
/// </summary>
public class StateStandardizedTestMathProficiencyCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "State standardized test (Math proficiency)";

    public string Query => $@"
WITH MathSummativeBase AS (
    SELECT
        da.AssessmentTitle,
        CASE
            WHEN ase.AssessmentResultScoreValueRawScore   IS NOT NULL THEN 1
            WHEN ase.AssessmentResultScoreValueScaleScore IS NOT NULL THEN 1
            WHEN ase.AssessmentResultScoreValuePercentile IS NOT NULL THEN 1
            WHEN ase.AssessmentResultScoreValueTScore     IS NOT NULL THEN 1
            WHEN ase.AssessmentResultScoreValueZScore     IS NOT NULL THEN 1
            ELSE 0
        END AS HasAnyScore
    FROM RDS.FactK12StudentAssessments ase
    JOIN RDS.DimAssessments da
        ON ase.AssessmentId = da.DimAssessmentId
    JOIN RDS.FactK12StudentEnrollments enr
        ON ase.K12StudentId = enr.K12StudentId
    JOIN RDS.DimGradeLevels g
        ON enr.EntryGradeLevelId = g.DimGradeLevelId
    -- NOTE: No 'StateSummative' code exists in the CEDS AssessmentType option set.
    -- AchievementTest is the closest match for annual state summative assessments.
    -- Alternate assessment codes are included to capture students assessed under
    -- modified or alternate standards. Verify these codes against your implementation
    -- and extend or narrow as needed.
    WHERE da.AssessmentTypeCode IN (
        'AchievementTest',
        'AlternateAssessmentGradeLevelStandards',
        'AlternateAssessmentAlternateStandards',
        'AlternativeAssessmentModifiedStandards'
    )
      AND da.AssessmentAcademicSubjectCode = '01166'
      AND g.GradeLevelCode                = '03'
),
Counts AS (
    SELECT
        COUNT(*)         AS TotalRecords,
        SUM(HasAnyScore) AS PopulatedRecords
    FROM MathSummativeBase
),
TitleCounts AS (
    SELECT
        AssessmentTitle,
        CAST(COUNT(*) AS NVARCHAR(MAX)) AS TitleCount
    FROM MathSummativeBase
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
-- Distribution - by matched assessment title
SELECT
    '{DataElementName}'                              AS DataElementName,
    'Distribution'                                   AS CharacteristicType,
    COALESCE(t.TitleCount, CAST(0 AS NVARCHAR(MAX))) AS Value,
    COALESCE(t.AssessmentTitle, 'No matching assessments found') AS SubItemLabel,
    NULL                                             AS Remarks
FROM (SELECT NULL AS Stub) AS Fallback
LEFT JOIN TitleCounts t ON 1 = 1";

    public string AssessmentDescription =>
        "Assesses math state standardized test records for grade 3 from " +
        "RDS.FactK12StudentAssessments joined to RDS.DimAssessments and " +
        "RDS.FactK12StudentEnrollments. Filters to AssessmentAcademicSubjectCode = '01166' " +
        "(Mathematics), GradeLevelCode = '03', and AssessmentTypeCode in AchievementTest and " +
        "alternate assessment variants as the closest available proxy for state summative tests. " +
        "Reports record count, score completeness, and distribution by assessment title.";
}
