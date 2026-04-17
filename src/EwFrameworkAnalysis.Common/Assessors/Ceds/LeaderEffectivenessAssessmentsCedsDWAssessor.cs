namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles leader effectiveness assessment records from RDS.FactK12StaffEvaluationParts
/// joined to RDS.DimK12StaffCategories (on K12StaffCategoryId) and
/// RDS.DimStaffEvaluationPartStatuses (on StaffEvaluationPartStatusId). Filtered to
/// staff classified as Administrators, SchoolAdministrators, or LeaAdministrators.
/// Assesses record count, completeness of the overall staff evaluation score or rating,
/// and distribution by performance level and evaluation system.
/// NOTE: 'Administrators', 'SchoolAdministrators', and 'LeaAdministrators' are the
/// three K12StaffClassificationCodes used to identify leaders. If your implementation
/// uses additional or different codes for principals or other school leaders, extend
/// the WHERE clause accordingly.
/// </summary>
public class LeaderEffectivenessAssessmentsCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Leader effectiveness assessments";

    public string Query => $@"
WITH LeaderEvalBase AS (
    SELECT
        f.StaffEvaluationScoreOrRating,
        s.FacultyAndAdministrationPerformanceLevelDescription,
        s.StaffEvaluationSystem
    FROM RDS.FactK12StaffEvaluationParts f
    JOIN RDS.DimK12StaffCategories c
        ON f.K12StaffCategoryId = c.DimK12StaffCategoryId
    JOIN RDS.DimStaffEvaluationPartStatuses s
        ON f.StaffEvaluationPartStatusId = s.DimStaffEvaluationPartStatusId
    WHERE c.K12StaffClassificationCode IN (
        'Administrators',
        'SchoolAdministrators',
        'LeaAdministrators'
    )
),
Counts AS (
    SELECT
        COUNT(*) AS TotalRecords,
        COUNT(CASE
            WHEN StaffEvaluationScoreOrRating IS NOT NULL
             AND StaffEvaluationScoreOrRating <> ''
            THEN 1 END) AS PopulatedRecords
    FROM LeaderEvalBase
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'                                         AS DataElementName,
    'RecordCount'                                               AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                         AS Value,
    NULL                                                        AS SubItemLabel,
    NULL                                                        AS Remarks
FROM Counts
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'                                         AS DataElementName,
    'Completeness'                                              AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                         AS Value,
    'TotalRecords'                                              AS SubItemLabel,
    NULL                                                        AS Remarks
FROM Counts
UNION ALL
-- Completeness - PopulatedRecords (overall score or rating present)
SELECT
    '{DataElementName}'                                         AS DataElementName,
    'Completeness'                                              AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))                     AS Value,
    'PopulatedRecords'                                          AS SubItemLabel,
    NULL                                                        AS Remarks
FROM Counts
UNION ALL
-- Distribution - by performance level
SELECT
    '{DataElementName}'                                         AS DataElementName,
    'Distribution'                                              AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                             AS Value,
    FacultyAndAdministrationPerformanceLevelDescription         AS SubItemLabel,
    'PerformanceLevel'                                          AS Remarks
FROM LeaderEvalBase
GROUP BY FacultyAndAdministrationPerformanceLevelDescription
UNION ALL
-- Distribution - by evaluation system
SELECT
    '{DataElementName}'                                         AS DataElementName,
    'Distribution'                                              AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                             AS Value,
    StaffEvaluationSystem                                       AS SubItemLabel,
    'EvaluationSystem'                                          AS Remarks
FROM LeaderEvalBase
GROUP BY StaffEvaluationSystem";

    public string AssessmentDescription =>
        "Assesses leader effectiveness evaluation records from RDS.FactK12StaffEvaluationParts " +
        "joined to RDS.DimK12StaffCategories and RDS.DimStaffEvaluationPartStatuses, filtered to " +
        "staff classified as Administrators, SchoolAdministrators, or LeaAdministrators. Reports " +
        "record count, completeness of the overall staff evaluation score or rating, distribution " +
        "by faculty and administration performance level, and distribution by evaluation system name.";
}
