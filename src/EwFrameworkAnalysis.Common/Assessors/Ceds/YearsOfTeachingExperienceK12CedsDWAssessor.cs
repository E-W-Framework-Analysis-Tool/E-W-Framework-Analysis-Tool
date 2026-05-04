using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles years of prior teaching experience for K-12 staff from RDS.FactK12StaffEmployments.
/// Assesses record count, completeness (non-zero values), integer range (min/max),
/// and distribution across experience bands.
/// </summary>
public class YearsOfTeachingExperienceK12CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Years of teaching experience (K-12)";

    public string Query => $@"
WITH TeachingExp AS (
    SELECT YearsOfPriorTeachingExperience
    FROM RDS.FactK12StaffEmployments
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM TeachingExp
UNION ALL
-- Completeness - TotalRecords
-- NOTE: YearsOfPriorTeachingExperience is non-nullable (decimal(3,2)). TotalRecords = all rows.
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL                AS Remarks
FROM TeachingExp
UNION ALL
-- Completeness - PopulatedRecords
-- NOTE: Because the column is non-nullable, 0.00 is treated as ""not reported.""
--       PopulatedRecords counts rows with a value > 0.
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN YearsOfPriorTeachingExperience > 0 THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM TeachingExp
UNION ALL
-- NumericalRange - Minimum
SELECT
    '{DataElementName}' AS DataElementName,
    'NumericalRange'    AS CharacteristicType,
    CAST(MIN(YearsOfPriorTeachingExperience) AS NVARCHAR(MAX)) AS Value,
    'Minimum'           AS SubItemLabel,
    NULL                AS Remarks
FROM TeachingExp
UNION ALL
-- NumericalRange - Maximum
SELECT
    '{DataElementName}' AS DataElementName,
    'NumericalRange'    AS CharacteristicType,
    CAST(MAX(YearsOfPriorTeachingExperience) AS NVARCHAR(MAX)) AS Value,
    'Maximum'           AS SubItemLabel,
    NULL                AS Remarks
FROM TeachingExp
UNION ALL
-- Distribution - 0 (Not Reported)
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN YearsOfPriorTeachingExperience = 0 THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    '0 (Not Reported)'  AS SubItemLabel,
    NULL                AS Remarks
FROM TeachingExp
UNION ALL
-- Distribution - Less Than 1 Year
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN YearsOfPriorTeachingExperience > 0
                     AND YearsOfPriorTeachingExperience < 1 THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Less Than 1 Year'  AS SubItemLabel,
    NULL                AS Remarks
FROM TeachingExp
UNION ALL
-- Distribution - 1-3 Years
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN YearsOfPriorTeachingExperience >= 1
                     AND YearsOfPriorTeachingExperience <= 3 THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    '1-3 Years'         AS SubItemLabel,
    NULL                AS Remarks
FROM TeachingExp
UNION ALL
-- Distribution - 4-9 Years
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN YearsOfPriorTeachingExperience >= 4
                     AND YearsOfPriorTeachingExperience <= 9 THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    '4-9 Years'         AS SubItemLabel,
    NULL                AS Remarks
FROM TeachingExp
UNION ALL
-- Distribution - 10-19 Years
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN YearsOfPriorTeachingExperience >= 10
                     AND YearsOfPriorTeachingExperience <= 19 THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    '10-19 Years'       AS SubItemLabel,
    NULL                AS Remarks
FROM TeachingExp
UNION ALL
-- Distribution - 20+ Years
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN YearsOfPriorTeachingExperience >= 20 THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    '20+ Years'         AS SubItemLabel,
    NULL                AS Remarks
FROM TeachingExp";

    public string AssessmentDescription =>
        "Profiles years of prior teaching experience (K-12) from RDS.FactK12StaffEmployments. " +
        "Assesses total record count, completeness treating 0.00 as not-reported, " +
        "min/max range, and distribution across experience bands " +
        "(not reported, <1, 1–3, 4–9, 10–19, 20+).";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
