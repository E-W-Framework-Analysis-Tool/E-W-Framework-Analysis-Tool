namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles the number of credits earned per postsecondary course transcript record,
/// from RDS.FactPsStudentCourseTranscripts joined to RDS.DimPeople.
/// </summary>
public class NumberOfCreditsEarnedCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Number of credits earned";
    public string Query => $@"
WITH Base AS (
    SELECT f.NumberOfCreditsEarned
    FROM RDS.FactPsStudentCourseTranscripts f
    JOIN RDS.DimPeople d ON f.PsStudentId = d.DimPersonId
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(NumberOfCreditsEarned) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- NumericalRange - Minimum
SELECT
    '{DataElementName}' AS DataElementName,
    'NumericalRange'    AS CharacteristicType,
    CAST(MIN(NumberOfCreditsEarned) AS NVARCHAR(MAX)) AS Value,
    'Minimum'           AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- NumericalRange - Maximum
SELECT
    '{DataElementName}' AS DataElementName,
    'NumericalRange'    AS CharacteristicType,
    CAST(MAX(NumberOfCreditsEarned) AS NVARCHAR(MAX)) AS Value,
    'Maximum'           AS SubItemLabel,
    NULL                AS Remarks
FROM Base
";
    public string AssessmentDescription =>
        "Assesses record count, completeness, and numerical range of NumberOfCreditsEarned " +
        "from RDS.FactPsStudentCourseTranscripts, joined to RDS.DimPeople to scope to valid student records.";
}
