namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles K-12 cultural competency assessment records from
/// RDS.FactK12StudentAssessments joined to RDS.DimAssessments, filtered to
/// known cultural competency assessments by title, short name, or state identifier.
/// Reports record count, completeness of Raw/Scale score values, and numeric range
/// for raw score values.
/// NOTE: The CEDS framework does not recommend a specific K-12 measurement tool for
/// cultural competency, citing a lack of developed instruments for youth. The
/// HEIghten Outcomes Assessment for Intercultural Competency and Diversity and the
/// Intercultural Development Inventory are adult/postsecondary tools referenced as
/// proxies. This assessor profiles whatever K-12 assessment data exists matching
/// those instruments or similar titles. Results should be interpreted with the
/// understanding that no validated K-12 instrument has been formally recommended.
/// </summary>
public class CulturalCompetencyAssessmentsK12CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Cultural competency assessments (K-12)";

    public string Query => $@"
WITH AssessmentBase AS (
    SELECT
        f.AssessmentResultScoreValueRawScore,
        f.AssessmentResultScoreValueScaleScore
    FROM RDS.FactK12StudentAssessments f
    JOIN RDS.DimAssessments d
        ON f.AssessmentId = d.DimAssessmentId
    -- NOTE: No validated K-12 cultural competency assessment has been formally
    -- recommended by the CEDS framework. The patterns below target the adult
    -- proxy instruments referenced in the spec (HEIghten and IDI) as well as
    -- broader cultural competency title matches. Confirm actual values in
    -- DimAssessments and tighten or extend as needed for your implementation.
    WHERE d.AssessmentTitle      LIKE '%HEIghten%'
       OR d.AssessmentTitle      LIKE '%Intercultural Development%'
       OR d.AssessmentTitle      LIKE '%cultural competenc%'
       OR d.AssessmentShortName  LIKE '%HEIghten%'
       OR d.AssessmentShortName  LIKE '%IDI%'
       OR d.AssessmentShortName  LIKE '%cultural competenc%'
       OR d.AssessmentIdentifierState LIKE '%HEIghten%'
       OR d.AssessmentIdentifierState LIKE '%IDI%'
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
        "Assesses K-12 cultural competency assessment records matched by title, short name, or state " +
        "identifier against known instruments (HEIghten, IDI) and similar titles. Reports total record " +
        "count, completeness of Raw or Scale score values, and min/max numeric range for raw score values. " +
        "Note: no validated K-12 cultural competency instrument has been formally recommended by CEDS; " +
        "referenced assessments are adult proxy tools.";
}
