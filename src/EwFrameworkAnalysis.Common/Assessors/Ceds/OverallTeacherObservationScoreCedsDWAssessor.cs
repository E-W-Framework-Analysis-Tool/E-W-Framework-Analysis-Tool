namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles overall teacher observation scores for K-12 staff
/// from RDS.FactK12StaffAssessments, filtered to observation-type assessments
/// and overall scores (AssessmentSubtestId = -1). Assesses record count,
/// completeness of the scale score field, and the integer range of observed scores.
///
/// NOTE: The CEDS Assessment Type option set is primarily designed for student assessments.
/// 'Observation' is defined as "Developmental observation" in CEDS, which typically refers
/// to early childhood student assessments rather than educator performance observations.
/// A 'StaffEvaluation' type exists in some CEDS implementations but is inconsistently
/// populated. The filter below combines an exact code match and a description LIKE match
/// as a best-effort approach — verify against the target state's DimAssessments values
/// before relying on this assessor:
///   SELECT DISTINCT AssessmentTypeCode, AssessmentTypeDescription FROM RDS.DimAssessments ORDER BY 1
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
    WHERE (d.AssessmentTypeCode = 'Observation' OR d.AssessmentTypeCode = 'StaffEvaluation'
           OR d.AssessmentTypeDescription LIKE '%observation%')
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
        "Assesses overall teacher observation scores for K-12 staff using AssessmentTypeCode " +
        "= 'Observation' or 'StaffEvaluation', or AssessmentTypeDescription LIKE '%observation%'. " +
        "Note: CEDS assessment type codes are primarily student-oriented; educator observation " +
        "scores may be inconsistently mapped across state implementations. " +
        "The scale score field is stored as NVARCHAR(70); IntegerRange uses TRY_CAST to DECIMAL " +
        "for correct numeric min/max — unparseable values are silently excluded from the range.";
}
