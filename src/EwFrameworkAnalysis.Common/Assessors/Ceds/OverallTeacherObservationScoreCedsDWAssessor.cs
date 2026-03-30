namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles overall teacher observation scores for K-12 staff
/// from RDS.FactK12StaffAssessments, filtered to observation-type assessments
/// and overall scores (AssessmentSubtestId = -1). Assesses record count,
/// completeness of the scale score field, and the integer range of observed scores.
///
/// NOTE: The assessment type filter (AssessmentTypeAdministeredDescription LIKE '%observation%')
/// is a non-standardized placeholder. DimAssessments values vary by state implementation.
/// Before deploying, verify the correct AssessmentTypeAdministeredDescription value(s)
/// used in the target state's CEDS Data Warehouse and update the WHERE clause accordingly.
/// </summary>
public class OverallTeacherObservationScoreCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Overall teacher observation score";

    public string Query => $@"
WITH ObservationBase AS (
    SELECT
        f.AssessmentResultScoreValueScaleScore
    FROM RDS.FactK12StaffAssessments f
    JOIN RDS.DimAssessments d ON f.AssessmentId = d.DimAssessmentId
    -- !! NOTE: The filter below is a non-standardized placeholder !!
    -- AssessmentTypeAdministeredDescription values are not standardized across state
    -- CEDS Data Warehouse implementations. Before deploying, query DimAssessments in
    -- the target environment to identify the correct value(s) for educator observations:
    --   SELECT DISTINCT AssessmentTypeAdministeredDescription FROM RDS.DimAssessments ORDER BY 1
    -- Then replace the LIKE condition below with an exact match (=) for the confirmed value(s).
      AND d.AssessmentTypeAdministeredDescription LIKE '%observation%'
      AND f.AssessmentSubtestId = -1
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM ObservationBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM ObservationBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                                                                AS DataElementName,
    'Completeness'                                                                     AS CharacteristicType,
    CAST(COUNT(AssessmentResultScoreValueScaleScore) AS NVARCHAR(MAX))                 AS Value,
    'PopulatedRecords'                                                                 AS SubItemLabel,
    NULL                                                                               AS Remarks
FROM ObservationBase
UNION ALL
-- IntegerRange - Minimum
SELECT
    '{DataElementName}'                                                                    AS DataElementName,
    'IntegerRange'                                                                         AS CharacteristicType,
    CAST(MIN(TRY_CAST(AssessmentResultScoreValueScaleScore AS DECIMAL(18,2))) AS NVARCHAR(MAX)) AS Value,
    'Minimum'                                                                              AS SubItemLabel,
    NULL                                                                                   AS Remarks
FROM ObservationBase
UNION ALL
-- IntegerRange - Maximum
SELECT
    '{DataElementName}'                                                                    AS DataElementName,
    'IntegerRange'                                                                         AS CharacteristicType,
    CAST(MAX(TRY_CAST(AssessmentResultScoreValueScaleScore AS DECIMAL(18,2))) AS NVARCHAR(MAX)) AS Value,
    'Maximum'                                                                              AS SubItemLabel,
    NULL                                                                                   AS Remarks
FROM ObservationBase";

    public string AssessmentDescription =>
        "Assesses overall teacher observation scores for K-12 staff, drawn from observation-type " +
        "assessments (filtered by AssessmentTypeAdministeredDescription) at the overall score level " +
        "(AssessmentSubtestId = -1). Reports total record count, completeness of the scale score " +
        "The scale score field is stored as NVARCHAR(70); IntegerRange uses TRY_CAST to DECIMAL " +
        "for correct numeric min/max — unparseable values are silently excluded from the range.";
}
