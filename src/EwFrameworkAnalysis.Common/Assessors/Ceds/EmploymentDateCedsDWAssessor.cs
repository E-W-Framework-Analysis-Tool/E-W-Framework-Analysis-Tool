using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles employment start date data from RDS.FactQuarterlyEmployments joined to
/// RDS.DimDates via EmploymentStartDateId. Assesses completeness of the employment
/// start date and distributes populated records by year. HireDateId and reference
/// period dates are also available on this fact table but EmploymentStartDateId is
/// the most direct representation of employment date.
/// </summary>
public class EmploymentDateCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Employment date";

    public string Query => $@"
WITH Base AS (
    SELECT
        d.DateValue                                 AS EmploymentStartDate,
        d.Year                                      AS EmploymentStartYear
    FROM RDS.FactQuarterlyEmployments f
    JOIN RDS.DimDates d
        ON f.EmploymentStartDateId = d.DimDateId
),
Counts AS (
    SELECT
        COUNT(*)                                    AS TotalRecords,
        COUNT(CASE
            WHEN EmploymentStartDate IS NOT NULL
            THEN 1
        END)                                        AS PopulatedRecords
    FROM Base
),
Distribution AS (
    SELECT
        EmploymentStartYear,
        COUNT(*)                                    AS YearCount
    FROM Base
    WHERE EmploymentStartDate IS NOT NULL
    GROUP BY EmploymentStartYear
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'                         AS DataElementName,
    'RecordCount'                               AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    NULL                                        AS SubItemLabel,
    NULL                                        AS Remarks
FROM Counts
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    'TotalRecords'                              AS SubItemLabel,
    NULL                                        AS Remarks
FROM Counts
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))     AS Value,
    'PopulatedRecords'                          AS SubItemLabel,
    'Populated = EmploymentStartDateId resolves to a non-NULL DateValue in DimDates' AS Remarks
FROM Counts
UNION ALL
-- Distribution - populated employment start records by year
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Distribution'                              AS CharacteristicType,
    CAST(YearCount AS NVARCHAR(MAX))            AS Value,
    CAST(EmploymentStartYear AS NVARCHAR(MAX))  AS SubItemLabel,
    NULL                                        AS Remarks
FROM Distribution";

    public string AssessmentDescription =>
        "Assesses employment start date completeness from RDS.FactQuarterlyEmployments via " +
        "EmploymentStartDateId joined to RDS.DimDates. Distributes populated records by year. " +
        "HireDateId and reference period date fields are also available on this fact table.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
