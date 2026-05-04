using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles GPA data for K-12 students from RDS.FactK12StudentAcademicRecords.
/// Assesses record count, completeness for all four GPA columns (weighted/unweighted,
/// cumulative/non-cumulative), and numerical range for the primary unweighted value.
/// </summary>
public class GradePointAverageK12CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Grade point average (K-12)";

    public string Query => $@"
WITH K12Gpa AS (
    SELECT
        HighSchoolGradePointAverageUnweighted,
        HighSchoolGradePointAverageCumulativeUnweighted,
        HighSchoolGradePointAverageWeighted,
        HighSchoolGradePointAverageCumulativeWeighted
    FROM RDS.FactK12StudentAcademicRecords
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM K12Gpa
UNION ALL
-- NumericalRange - Minimum (unweighted, non-cumulative)
SELECT
    '{DataElementName}'                                        AS DataElementName,
    'NumericalRange'                                           AS CharacteristicType,
    CAST(MIN(HighSchoolGradePointAverageUnweighted) AS NVARCHAR(MAX)) AS Value,
    'Minimum'                                                  AS SubItemLabel,
    NULL                                                       AS Remarks
FROM K12Gpa
UNION ALL
-- NumericalRange - Maximum (unweighted, non-cumulative)
SELECT
    '{DataElementName}'                                        AS DataElementName,
    'NumericalRange'                                           AS CharacteristicType,
    CAST(MAX(HighSchoolGradePointAverageUnweighted) AS NVARCHAR(MAX)) AS Value,
    'Maximum'                                                  AS SubItemLabel,
    NULL                                                       AS Remarks
FROM K12Gpa
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM K12Gpa
UNION ALL
-- Completeness - PopulatedRecords (any GPA field populated)
SELECT
    '{DataElementName}'  AS DataElementName,
    'Completeness'       AS CharacteristicType,
    CAST(COUNT(CASE
        WHEN HighSchoolGradePointAverageUnweighted            IS NOT NULL
          OR HighSchoolGradePointAverageCumulativeUnweighted   IS NOT NULL
          OR HighSchoolGradePointAverageWeighted               IS NOT NULL
          OR HighSchoolGradePointAverageCumulativeWeighted     IS NOT NULL
        THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'   AS SubItemLabel,
    NULL                 AS Remarks
FROM K12Gpa";

    public string AssessmentDescription =>
        "Profiles Grade point average (K-12) from RDS.FactK12StudentAcademicRecords. " +
        "Reports total record count, numerical range (min/max) of the unweighted non-cumulative GPA, " +
        "and completeness measured as rows where at least one of the four GPA fields " +
        "(weighted, unweighted, cumulative weighted, cumulative unweighted) is populated.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
