using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles graduate credential attainment dates from RDS.FactPsStudentAcademicAwards,
/// joined to RDS.DimPsAcademicAwardStatuses. Filters to graduate-level award codes
/// (master's, doctoral, first professional, post-baccalaureate, and related IPEDS codes).
/// Assesses record count and completeness of AcademicAwardDateId.
/// </summary>
public class GraduateCredentialAttainmentDateCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Graduate credential attainment date";

    public string Query => $@"
WITH GraduateBase AS (
    SELECT
        f.AcademicAwardDateId
    FROM RDS.FactPsStudentAcademicAwards f
    JOIN RDS.DimPsAcademicAwardStatuses d ON f.PsAcademicAwardStatusId = d.DimPsAcademicAwardStatusId
    WHERE d.PescAwardLevelTypeCode IN (
        '3.1', '3.2', '3.3',
        '4.0', '4.1', '4.2', '4.3', '4.4', '4.5',
        'IPEDS6', 'IPEDS7', 'IPEDS8', 'IPEDS9',
        'IPEDS10', 'IPEDS11', 'IPEDS17', 'IPEDS18', 'IPEDS19'
    )
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM GraduateBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM GraduateBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(AcademicAwardDateId) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'              AS SubItemLabel,
    NULL                            AS Remarks
FROM GraduateBase";

    public string AssessmentDescription =>
        "Assesses graduate credential attainment date from RDS.FactPsStudentAcademicAwards, " +
        "filtered to graduate-level PESC award codes (master's, doctoral, first professional, " +
        "post-baccalaureate, and related IPEDS equivalents). Reports record count and completeness " +
        "of the academic award date field.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
