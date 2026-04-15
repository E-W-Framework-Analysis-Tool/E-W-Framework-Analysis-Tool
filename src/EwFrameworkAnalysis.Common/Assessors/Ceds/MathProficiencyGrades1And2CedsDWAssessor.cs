namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles math benchmark assessment records for grades 1 and 2 from
/// RDS.FactK12StudentAssessments joined to RDS.DimAssessments and
/// RDS.FactK12StudentEnrollments (for grade level). Filters to
/// AssessmentTypeCode = 'Benchmark' and AssessmentAcademicSubjectCode = '01166'
/// (Mathematics), scoped to GradeLevelCode IN ('01', '02').
/// NOTE: Benchmark assessments vary by state and district. Some states do not
/// administer a standardized benchmark at grades 1 and 2; results may be sparse
/// or empty in those implementations. The Distribution by assessment title
/// is intended to surface which instruments are present.
/// </summary>
public class MathProficiencyGrades1And2CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Math proficiency (Grades 1 and 2)";

    public string Query => $@"
WITH MathBenchmarkBase AS (
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
    WHERE da.AssessmentTypeCode            = 'Benchmark'
      AND da.AssessmentAcademicSubjectCode = '01166'
      AND g.GradeLevelCode                IN ('01', '02')
),
Counts AS (
    SELECT
        COUNT(*)         AS TotalRecords,
        SUM(HasAnyScore) AS PopulatedRecords
    FROM MathBenchmarkBase
),
TitleCounts AS (
    SELECT
        AssessmentTitle,
        CAST(COUNT(*) AS NVARCHAR(MAX)) AS TitleCount
    FROM MathBenchmarkBase
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
        "Assesses math benchmark assessment records for grades 1 and 2 from " +
        "RDS.FactK12StudentAssessments joined to RDS.DimAssessments and " +
        "RDS.FactK12StudentEnrollments. Filters to AssessmentTypeCode = 'Benchmark', " +
        "AssessmentAcademicSubjectCode = '01166' (Mathematics), and GradeLevelCode in " +
        "('01', '02'). Reports record count, score completeness, and distribution by " +
        "assessment title. Results may be sparse in states without grade 1-2 benchmarks.";
}
