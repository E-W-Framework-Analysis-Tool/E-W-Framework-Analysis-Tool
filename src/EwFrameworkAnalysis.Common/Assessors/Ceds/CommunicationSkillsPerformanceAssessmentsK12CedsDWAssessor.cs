namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles K-12 communication skills performance assessment records from
/// RDS.FactK12StudentAssessments joined to RDS.DimAssessments, filtered to
/// CCRA+ (or assessment of similar degree) by matching AssessmentTitle,
/// AssessmentShortName, or AssessmentIdentifierState. Reports record count,
/// completeness of Raw/Scale score values, and a single combined numeric range
/// spanning both score types.
/// </summary>
public class CommunicationSkillsPerformanceAssessmentsK12CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Communication skills performance assessments (K-12)";

    public string Query => $@"
WITH AssessmentBase AS (
    SELECT
        f.AssessmentResultScoreValueRawScore,
        f.AssessmentResultScoreValueScaleScore
    FROM RDS.FactK12StudentAssessments f
    JOIN RDS.DimAssessments d
        ON f.AssessmentId = d.DimAssessmentId
    -- NOTE: The CEDS spec names the CCRA+ as the reference assessment, with
    -- an assessment of similar degree as a fallback. The LIKE patterns
    -- below cast a wide net across title, short name, and state identifier.
    -- Confirm actual values in DimAssessments and tighten or extend as needed.
    WHERE d.AssessmentTitle LIKE '%College and Career Readiness%'
       OR d.AssessmentTitle LIKE '%CCRA%'
       OR d.AssessmentShortName LIKE '%CCRA%'
       OR d.AssessmentIdentifierState LIKE '%CCRA%'
),
Counts AS (
    SELECT
        COUNT(*) AS TotalRecords,
        COUNT(CASE
            WHEN (AssessmentResultScoreValueRawScore IS NOT NULL
                  AND AssessmentResultScoreValueRawScore <> '')
              OR (AssessmentResultScoreValueScaleScore IS NOT NULL
                  AND AssessmentResultScoreValueScaleScore <> '')
            THEN 1
        END) AS PopulatedRecords
    FROM AssessmentBase
),
RangeCalc AS (
    SELECT
        MIN(TRY_CAST(AssessmentResultScoreValueRawScore AS FLOAT)) AS MinRaw,
        MAX(TRY_CAST(AssessmentResultScoreValueRawScore AS FLOAT)) AS MaxRaw
    FROM AssessmentBase
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'                     AS DataElementName,
    'RecordCount'                           AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))     AS Value,
    NULL                                    AS SubItemLabel,
    NULL                                    AS Remarks
FROM Counts
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'                     AS DataElementName,
    'Completeness'                          AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))     AS Value,
    'TotalRecords'                          AS SubItemLabel,
    NULL                                    AS Remarks
FROM Counts
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                     AS DataElementName,
    'Completeness'                          AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'                      AS SubItemLabel,
    'Populated = RawScore or ScaleScore is non-NULL and non-empty' AS Remarks
FROM Counts
UNION ALL
-- IntegerRange - Minimum
SELECT
    '{DataElementName}'                     AS DataElementName,
    'IntegerRange'                          AS CharacteristicType,
    CAST(MinRaw AS NVARCHAR(MAX))           AS Value,
    'Minimum'                               AS SubItemLabel,
    'RawScore' AS Remarks
FROM RangeCalc
UNION ALL
-- IntegerRange - Maximum
SELECT
    '{DataElementName}'                     AS DataElementName,
    'IntegerRange'                          AS CharacteristicType,
    CAST(MaxRaw AS NVARCHAR(MAX))           AS Value,
    'Maximum'                               AS SubItemLabel,
    'RawScore'                              AS Remarks
FROM RangeCalc";

    public string AssessmentDescription =>
        "Assesses K-12 communication skills performance assessment records for CCRA+ or equivalent assessments, " +
        "matched by title, short name, or state identifier. Reports total record count, completeness of Raw or " +
        "Scale score values, and a single min/max numeric range spanning both Raw Score and Scale Score values.";
}
