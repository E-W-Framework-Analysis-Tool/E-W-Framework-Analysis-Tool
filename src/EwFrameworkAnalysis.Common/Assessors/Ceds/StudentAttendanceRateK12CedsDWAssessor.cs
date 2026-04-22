namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles student attendance rate (K-12) from RDS.FactK12StudentAttendanceRates.
/// Assesses total record count, completeness of the StudentAttendanceRate field,
/// and numerical range of rate values to indicate scale and data quality.
/// </summary>
public class StudentAttendanceRateK12CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Student attendance rate (K-12)";

    public string Query => $@"
WITH AttendanceBase AS (
    SELECT StudentAttendanceRate
    FROM RDS.FactK12StudentAttendanceRates
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM AttendanceBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL                AS Remarks
FROM AttendanceBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(StudentAttendanceRate) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM AttendanceBase
UNION ALL
-- NumericalRange - Minimum
SELECT
    '{DataElementName}' AS DataElementName,
    'NumericalRange'    AS CharacteristicType,
    CAST(MIN(StudentAttendanceRate) AS NVARCHAR(MAX)) AS Value,
    'Minimum'           AS SubItemLabel,
    NULL                AS Remarks
FROM AttendanceBase
UNION ALL
-- NumericalRange - Maximum
SELECT
    '{DataElementName}' AS DataElementName,
    'NumericalRange'    AS CharacteristicType,
    CAST(MAX(StudentAttendanceRate) AS NVARCHAR(MAX)) AS Value,
    'Maximum'           AS SubItemLabel,
    NULL                AS Remarks
FROM AttendanceBase
";

    public string AssessmentDescription =>
        "Assesses student attendance rate (K-12) from RDS.FactK12StudentAttendanceRates. " +
        "Reports total record count, completeness of the StudentAttendanceRate field, and " +
        "numerical range (min/max) of rate values. The range is particularly useful for " +
        "scorers to determine whether rates are stored on a 0-1 or 0-100 scale and to " +
        "identify potential outliers.";
}
