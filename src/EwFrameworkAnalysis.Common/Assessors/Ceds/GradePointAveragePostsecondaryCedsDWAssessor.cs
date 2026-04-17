namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles GPA data for postsecondary students from RDS.FactPsStudentAcademicRecords.
/// Assesses record count, completeness for both GPA columns (term and cumulative),
/// and numerical range for the primary term GPA value.
/// </summary>
public class GradePointAveragePostsecondaryCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Grade point average (Postsecondary)";

    public string Query => $@"
WITH PsGpa AS (
    SELECT
        GradePointAverage,
        GradePointAverageCumulative
    FROM RDS.FactPsStudentAcademicRecords
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM PsGpa
UNION ALL
-- NumericalRange - Minimum (term GPA)
SELECT
    '{DataElementName}'                            AS DataElementName,
    'NumericalRange'                               AS CharacteristicType,
    CAST(MIN(GradePointAverage) AS NVARCHAR(MAX))  AS Value,
    'Minimum'                                      AS SubItemLabel,
    NULL                                           AS Remarks
FROM PsGpa
UNION ALL
-- NumericalRange - Maximum (term GPA)
SELECT
    '{DataElementName}'                            AS DataElementName,
    'NumericalRange'                               AS CharacteristicType,
    CAST(MAX(GradePointAverage) AS NVARCHAR(MAX))  AS Value,
    'Maximum'                                      AS SubItemLabel,
    NULL                                           AS Remarks
FROM PsGpa
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM PsGpa
UNION ALL
-- Completeness - PopulatedRecords (either GPA field populated)
SELECT
    '{DataElementName}'  AS DataElementName,
    'Completeness'       AS CharacteristicType,
    CAST(COUNT(CASE
        WHEN GradePointAverage           IS NOT NULL
          OR GradePointAverageCumulative IS NOT NULL
        THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'   AS SubItemLabel,
    NULL                 AS Remarks
FROM PsGpa";

    public string AssessmentDescription =>
        "Profiles Grade point average (Postsecondary) from RDS.FactPsStudentAcademicRecords. " +
        "Reports total record count, numerical range (min/max) of the term GPA, " +
        "and completeness measured as rows where at least one of the two GPA fields " +
        "(term GPA or cumulative GPA) is populated.";
}
