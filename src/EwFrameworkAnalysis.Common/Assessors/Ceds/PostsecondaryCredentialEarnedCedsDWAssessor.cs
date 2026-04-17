namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles postsecondary credentials earned from RDS.FactPsStudentAcademicAwards,
/// joined to RDS.DimPsAcademicAwardStatuses. Includes all postsecondary award levels,
/// excluding secondary/high school codes (B-series), CEGEP, French Baccalaureate, and
/// the catch-all '0.0' code. Assesses record count and distribution across PESC award levels.
/// </summary>
public class PostsecondaryCredentialEarnedCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Postsecondary credential earned";
    public string Query => $@"
WITH PsCredentialBase AS (
    SELECT
        d.PescAwardLevelTypeCode,
        d.PescAwardLevelTypeDescription
    FROM RDS.FactPsStudentAcademicAwards f
    JOIN RDS.DimPsAcademicAwardStatuses d ON f.PsAcademicAwardStatusId = d.DimPsAcademicAwardStatusId
    WHERE d.PescAwardLevelTypeCode IN (
        '1.1', '1.2', '1.3', '1.4', '1.5',
        '2.0', '2.1', '2.2', '2.3', '2.4', '2.5', '2.6', '2.7', '2.8',
        '3.1', '3.2', '3.3',
        '4.0', '4.1', '4.2', '4.3', '4.4', '4.5',
        'IPEDS1', 'IPEDS2', 'IPEDS3', 'IPEDS4', 'IPEDS5',
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
FROM PsCredentialBase
UNION ALL
-- Distribution - by PESC award level
SELECT
    '{DataElementName}'                     AS DataElementName,
    'Distribution'                          AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))         AS Value,
    PescAwardLevelTypeDescription           AS SubItemLabel,
    NULL                                    AS Remarks
FROM PsCredentialBase
GROUP BY PescAwardLevelTypeDescription
";
    public string AssessmentDescription =>
        "Assesses postsecondary credentials earned from RDS.FactPsStudentAcademicAwards, " +
        "covering all postsecondary PESC award levels (certificates through doctoral). Excludes " +
        "secondary/high school (B-series), CEGEP, French Baccalaureate, and the '0.0' catch-all code. " +
        "Reports record count and distribution across PESC award level descriptions.";
}
