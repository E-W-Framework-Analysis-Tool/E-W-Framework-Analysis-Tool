using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles cohort year for K-12 students from RDS.FactK12GraduationCohorts.
/// Assesses the count of 9th grade cohort records with CohortYearId populated.
/// </summary>
public class CohortYearCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Cohort year";

    public string Query => $@"
WITH CohortBase AS (
    SELECT
        f.CohortYearId
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
    '{DataElementName}'                        AS DataElementName,
    'Completeness'                             AS CharacteristicType,
    CAST(COUNT(CohortYearId) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'                         AS SubItemLabel,
    NULL                                       AS Remarks
FROM CohortBase";

    public string AssessmentDescription =>
        "Assesses cohort year for 9th grade students from RDS.FactK12GraduationCohorts. " +
        "Reports total record count and completeness of CohortYearId.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
