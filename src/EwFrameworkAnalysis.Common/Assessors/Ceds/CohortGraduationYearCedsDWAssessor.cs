namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles cohort graduation year for K-12 students from RDS.FactK12GraduationCohorts.
/// Assesses the count of 9th grade cohort records with CohortGraduationYearId populated.
///
/// NOTE: Cohort graduation year is typically derived from cohort year (+ 4 years) and
/// may not be independently collected. A gap between cohort year and cohort graduation
/// year completeness would indicate an ETL or data population issue.
/// </summary>
public class CohortGraduationYearCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Cohort graduation year";

    public string Query => $@"
WITH CohortBase AS (
    SELECT
        f.CohortGraduationYearId
    FROM RDS.FactK12GraduationCohorts f
    JOIN RDS.DimGradeLevels g ON f.GradeLevelId = g.DimGradeLevelId
    WHERE g.GradeLevelCode = '09'
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM CohortBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM CohortBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                                   AS DataElementName,
    'Completeness'                                        AS CharacteristicType,
    CAST(COUNT(CohortGraduationYearId) AS NVARCHAR(MAX))  AS Value,
    'PopulatedRecords'                                    AS SubItemLabel,
    NULL                                                  AS Remarks
FROM CohortBase";

    public string AssessmentDescription =>
        "Assesses cohort graduation year for 9th grade students from RDS.FactK12GraduationCohorts. " +
        "Reports total record count and completeness of CohortGraduationYearId. Cohort graduation " +
        "year is typically derived from cohort year (+ 4 years) — a completeness gap between the " +
        "two would indicate an ETL or data population issue.";
}
