namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles years in current position for K-12 school leaders
/// from RDS.FactK12StaffAssignments, joined to RDS.DimK12StaffCategories,
/// RDS.DimK12Schools, and RDS.DimDates (via AssignmentStartDateId).
/// Tenure is calculated as the difference in years between AssignmentStartDate
/// and the current date. Assesses record count, completeness of the assignment
/// start date, and integer range of calculated tenure years.
/// NOTE: School leader classification filter requires confirmation — see inline comment.
/// </summary>
public class YearsInCurrentPositionCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Years in current position";

    public string Query => $@"
WITH LeaderBase AS (
SELECT
          -- NOTE: Replace the LIKE filter below with the confirmed K12StaffClassificationCode
        -- value(s) that identify school leaders in RDS.DimK12StaffCategories.
        -- Example values might be 'Principal', 'SchoolLeader', 'Administrator', etc.
        -- Until confirmed, this placeholder captures likely can
    CAST(DATEDIFF(day, 
        startDate.DateValue, 
        CASE 
            WHEN f.AssignmentEndDateId = -1 THEN GETDATE() 
            ELSE endDate.DateValue 
        END
    ) / 365 AS  INT) AS TenureYears
	 FROM RDS.FactK12StaffAssignments f
    JOIN RDS.DimK12StaffCategories c ON f.K12StaffCategoryId = c.DimK12StaffCategoryId
    JOIN RDS.DimDates startDate ON f.AssignmentStartDateId = startDate.DimDateId
	JOIN RDS.DimDates endDate ON f.AssignmentEndDateId = endDate.DimDateId
    WHERE c.K12StaffClassificationCode LIKE '%Principal%'
       OR c.K12StaffClassificationCode LIKE '%Leader%'
       OR c.K12StaffClassificationCode LIKE '%Administrator%'
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM LeaderBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM LeaderBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                                                    AS DataElementName,
    'Completeness'                                                         AS CharacteristicType,
    CAST(COUNT(TenureYears) AS NVARCHAR(MAX))                              AS Value,
    'PopulatedRecords'                                                     AS SubItemLabel,
    NULL                                                                   AS Remarks
FROM LeaderBase
UNION ALL
-- NumericalRange - Minimum
SELECT
    '{DataElementName}'                      AS DataElementName,
    'NumericalRange'                         AS CharacteristicType,
    CAST(MIN(TenureYears) AS NVARCHAR(MAX))  AS Value,
    'Minimum'                                AS SubItemLabel,
    NULL                                     AS Remarks
FROM LeaderBase
UNION ALL
-- NumericalRange - Maximum
SELECT
    '{DataElementName}'                      AS DataElementName,
    'NumericalRange'                         AS CharacteristicType,
    CAST(MAX(TenureYears) AS NVARCHAR(MAX))  AS Value,
    'Maximum'                                AS SubItemLabel,
    NULL                                     AS Remarks
FROM LeaderBase";

    public string AssessmentDescription =>
        "Assesses years in current position for K-12 school leaders, calculated from " +
        "AssignmentStartDate to the current date. Reports total record count, completeness " +
        "of the assignment start date, and the minimum and maximum tenure in years. " +
        "School leader classification filter uses partial string matching on K12StaffClassificationCode " +
        "(LIKE '%Principal%', '%Leader%', '%Administrator%') as a placeholder — exact code values require " +
        "confirmation against RDS.DimK12StaffCategories before use in production.";
}
