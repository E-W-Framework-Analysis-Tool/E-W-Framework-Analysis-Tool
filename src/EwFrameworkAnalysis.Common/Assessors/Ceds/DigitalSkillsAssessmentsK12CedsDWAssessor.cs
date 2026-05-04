using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles K-12 digital skills assessment records from RDS.FactK12StudentAssessments
/// joined to RDS.DimAssessments, filtered by known instrument names and broad title
/// matching. Reports record count, completeness of Raw/Scale score values, and numeric
/// range for raw score values.
/// NOTE: The CEDS framework explicitly states no validated K-12 digital skills
/// assessment instrument is currently available. The two instruments previously
/// referenced in literature — the Instant Digital Competence Assessment (iDCA) and
/// the Student Tool for Technology Literacy (ST2L) — are no longer available. The
/// PS/Workforce spec references research instruments not designed for K-12 use.
/// This assessor is expected to return zero or near-zero records in most
/// implementations. It is included to surface any locally-adopted digital skills
/// assessments that may exist under similar titles.
/// </summary>
public class DigitalSkillsAssessmentsK12CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Digital skills assessments (K-12)";

    public string Query => $@"
WITH AssessmentBase AS (
    SELECT
        f.AssessmentResultScoreValueRawScore,
        f.AssessmentResultScoreValueScaleScore
    FROM RDS.FactK12StudentAssessments f
    JOIN RDS.DimAssessments d
        ON f.AssessmentId = d.DimAssessmentId
    -- NOTE: No currently available validated K-12 digital skills assessment exists
    -- per the CEDS framework. Patterns below target the two now-unavailable
    -- instruments (iDCA, ST2L) and a broad digital skills title match to surface
    -- any locally-adopted instruments. Confirm actual values in DimAssessments
    -- and extend as needed. Results are expected to be sparse or empty in most
    -- implementations.
    WHERE d.AssessmentTitle           LIKE '%digital%skill%'
       OR d.AssessmentTitle           LIKE '%digital%competenc%'
       OR d.AssessmentTitle           LIKE '%iDCA%'
       OR d.AssessmentTitle           LIKE '%ST2L%'
       OR d.AssessmentTitle           LIKE '%technology literacy%'
       OR d.AssessmentShortName       LIKE '%iDCA%'
       OR d.AssessmentShortName       LIKE '%ST2L%'
       OR d.AssessmentIdentifierState LIKE '%iDCA%'
       OR d.AssessmentIdentifierState LIKE '%ST2L%'
),
Counts AS (
    SELECT
        COUNT(*)                                        AS TotalRecords,
        COUNT(CASE
            WHEN (AssessmentResultScoreValueRawScore IS NOT NULL
                  AND AssessmentResultScoreValueRawScore <> '')
              OR (AssessmentResultScoreValueScaleScore IS NOT NULL
                  AND AssessmentResultScoreValueScaleScore <> '')
            THEN 1
        END)                                            AS PopulatedRecords
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
    'Populated = RawScore or ScaleScore is non-NULL and non-empty' AS Remarks
FROM Counts
UNION ALL
-- NumericalRange - Minimum
SELECT
    '{DataElementName}'                         AS DataElementName,
    'NumericalRange'                            AS CharacteristicType,
    CAST(MinRaw AS NVARCHAR(MAX))               AS Value,
    'Minimum'                                   AS SubItemLabel,
    NULL                                        AS Remarks
FROM RangeCalc
UNION ALL
-- NumericalRange - Maximum
SELECT
    '{DataElementName}'                         AS DataElementName,
    'NumericalRange'                            AS CharacteristicType,
    CAST(MaxRaw AS NVARCHAR(MAX))               AS Value,
    'Maximum'                                   AS SubItemLabel,
    NULL                                        AS Remarks
FROM RangeCalc";

    public string AssessmentDescription =>
        "Assesses K-12 digital skills assessment records matched by known instrument names (iDCA, ST2L) " +
        "and broad digital skills title patterns. Reports total record count, completeness of Raw or Scale " +
        "score values, and min/max numeric range for raw score values. Note: no validated K-12 digital " +
        "skills instrument is currently available per CEDS; this assessor is expected to return zero or " +
        "near-zero records in most implementations but is included to surface locally-adopted instruments.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
