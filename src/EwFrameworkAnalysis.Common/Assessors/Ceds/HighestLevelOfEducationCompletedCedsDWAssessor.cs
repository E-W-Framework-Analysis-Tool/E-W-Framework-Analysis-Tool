namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles highest level of education completed from RDS.DimPeople, using
/// HighestLevelOfEducationCompletedCode and HighestLevelOfEducationCompletedDescription.
/// Scoped to active K-12 staff (IsActiveK12Staff = 1). Assesses record count,
/// completeness of the education level field, and distribution across education
/// level categories.
/// </summary>
public class HighestLevelOfEducationCompletedCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Highest level of education completed";

    public string Query => $@"
WITH EducationLevelBase AS (
    SELECT
        HighestLevelOfEducationCompletedCode,
        HighestLevelOfEducationCompletedDescription
    FROM RDS.DimPeople
    WHERE IsActiveK12Staff = 1
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM EducationLevelBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM EducationLevelBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(CASE
        WHEN HighestLevelOfEducationCompletedCode IS NOT NULL
         AND HighestLevelOfEducationCompletedCode <> ''
         AND HighestLevelOfEducationCompletedCode <> 'MISSING'
        THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'              AS SubItemLabel,
    NULL                            AS Remarks
FROM EducationLevelBase
UNION ALL
-- Distribution - one row per education level description
SELECT
    '{DataElementName}'                          AS DataElementName,
    'Distribution'                               AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))              AS Value,
    HighestLevelOfEducationCompletedDescription  AS SubItemLabel,
    NULL                                         AS Remarks
FROM EducationLevelBase
GROUP BY HighestLevelOfEducationCompletedDescription";

    public string AssessmentDescription =>
        "Assesses highest level of education completed from RDS.DimPeople, scoped to active " +
        "K-12 staff (IsActiveK12Staff = 1). Reports total record count, completeness of the " +
        "education level field (excluding NULL, empty, and MISSING sentinel values), and " +
        "distribution of staff across education level categories per the CEDS option set.";
}
