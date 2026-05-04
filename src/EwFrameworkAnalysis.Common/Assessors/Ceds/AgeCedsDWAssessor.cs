using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Comprehensive version that also includes completeness of BirthDate
/// </summary>
public class AgeCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Age";

    public string Query => $@"
WITH PeopleStats AS (
    SELECT 
        COUNT(*) as TotalPeople,
        SUM(CASE WHEN BirthDate IS NOT NULL THEN 1 ELSE 0 END) as BirthDatePopulated,
        MIN(DATEDIFF(YEAR, BirthDate, GETDATE()) - 
            CASE 
                WHEN MONTH(BirthDate) > MONTH(GETDATE()) 
                    OR (MONTH(BirthDate) = MONTH(GETDATE()) AND DAY(BirthDate) > DAY(GETDATE()))
                THEN 1 
                ELSE 0 
            END) as MinAge,
        MAX(DATEDIFF(YEAR, BirthDate, GETDATE()) - 
            CASE 
                WHEN MONTH(BirthDate) > MONTH(GETDATE()) 
                    OR (MONTH(BirthDate) = MONTH(GETDATE()) AND DAY(BirthDate) > DAY(GETDATE()))
                THEN 1 
                ELSE 0 
            END) as MaxAge
    FROM RDS.DimPeople
)

INSERT INTO #EWFProfilerResults

-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness' AS CharacteristicType,
    CAST(BirthDatePopulated AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords' AS SubItemLabel,
    'BirthDate' AS Remarks
FROM PeopleStats

UNION ALL

-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness' AS CharacteristicType,
    CAST(TotalPeople AS NVARCHAR(MAX)) AS Value,
    'TotalRecords' AS SubItemLabel,
    NULL AS Remarks
FROM PeopleStats

UNION ALL

-- NumericalRange - Minimum Age
SELECT
    '{DataElementName}' AS DataElementName,
    'NumericalRange' AS CharacteristicType,
    CAST(MinAge AS NVARCHAR(MAX)) AS Value,
    'Minimum' AS SubItemLabel,
    'Age Range (from BirthDate)' AS Remarks
FROM PeopleStats

UNION ALL

-- NumericalRange - Maximum Age
SELECT
    '{DataElementName}' AS DataElementName,
    'NumericalRange' AS CharacteristicType,
    CAST(MaxAge AS NVARCHAR(MAX)) AS Value,
    'Maximum' AS SubItemLabel,
    NULL AS Remarks
FROM PeopleStats
";

    public string AssessmentDescription =>
        "Comprehensive assessment including BirthDate completeness and calculated age range from RDS.DimPeople";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
