namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles bachelor's degree completion date from RDS.FactPsStudentAcademicAwards,
/// joined to RDS.DimPsAcademicAwardStatuses. Filters to bachelor's degree award levels
/// using PESC Award Level Type codes '2.4' (Baccalaureate Degree), '2.5' (Baccalaureate
/// Honors Degree), and 'IPEDS5' (Bachelor's Degree or equivalent). Assesses the count
/// of bachelor's degree records with an academic award date present.
/// </summary>
public class BachelorsDegreeCompletionDateCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Bachelor's degree completion date";

    public string Query => $@"
WITH BachelorsBase AS (
    SELECT
        f.PsStudentId
    FROM RDS.FactPsStudentAcademicAwards f
    JOIN RDS.DimPsAcademicAwardStatuses d ON f.PsAcademicAwardStatusId = d.DimPsAcademicAwardStatusId
    WHERE d.PescAwardLevelTypeCode IN ('2.4', '2.5', 'IPEDS5')
      AND f.AcademicAwardDateId IS NOT NULL
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName.EscapeForQuery()}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM BachelorsBase";

    public string AssessmentDescription =>
        "Assesses bachelor's degree completion date by counting records where PescAwardLevelTypeCode " +
        "is '2.4' (Baccalaureate Degree), '2.5' (Baccalaureate Honors Degree), or 'IPEDS5' " +
        "(Bachelor's Degree or equivalent), and AcademicAwardDateId is non-null. " +
        "The record count is the presence signal for this data element.";
}
