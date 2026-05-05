using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles postsecondary major data from RDS.BridgePsStudentAcademicAwardCipCodes,
/// joined to RDS.FactPsStudentAcademicAwards. Assesses total academic award records
/// and completeness of major (CIP code) assignment against that population.
/// </summary>
public class PostsecondaryMajorCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Postsecondary major";

    public string Query => $@"
WITH AwardBase AS (
    SELECT
        f.FactPsStudentAcademicAwardId,
        b.FactPsStudentAcademicAwardId AS BridgeAwardId
    FROM RDS.FactPsStudentAcademicAwards f
    LEFT JOIN RDS.BridgePsStudentAcademicAwardCipCodes b
        ON f.FactPsStudentAcademicAwardId = b.FactPsStudentAcademicAwardId
        AND b.PostsecondaryProgramLevelCode = 'Major'
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactPsStudentAcademicAwards
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactPsStudentAcademicAwards
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(BridgeAwardId) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM AwardBase
";

    public string AssessmentDescription =>
        "Assesses postsecondary major data from RDS.BridgePsStudentAcademicAwardCipCodes joined to " +
        "RDS.FactPsStudentAcademicAwards. Reports total academic award records and the count of awards " +
        "that have at least one CIP code assigned with a program level of 'Major'.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
