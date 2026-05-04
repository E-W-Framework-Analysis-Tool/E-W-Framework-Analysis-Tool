using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles military enlistment status for K-12 students by examining
/// RDS.FactK12StudentEnrollments joined to RDS.DimMilitaryStatuses.
/// Assesses record count of actively enlisted students, completeness of
/// the military status join, and distribution across all four CEDS
/// military status codes.
/// </summary>
public class EnlistmentInTheMilitaryCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Enlistment in the military";

    public string Query => $@"
WITH MilitaryBase AS (
    SELECT
        f.FactK12StudentEnrollmentId,
        d.ActiveMilitaryStatusIndicatorCode
    FROM RDS.FactK12StudentEnrollments f
    LEFT JOIN RDS.DimMilitaryStatuses d
        ON f.MilitaryStatusId = d.DimMilitaryStatusId
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'                                      AS DataElementName,
    'RecordCount'                                            AS CharacteristicType,
    CAST(COUNT(FactK12StudentEnrollmentId) AS NVARCHAR(MAX)) AS Value,
    NULL                                                     AS SubItemLabel,
    NULL                                                     AS Remarks
FROM MilitaryBase
WHERE ActiveMilitaryStatusIndicatorCode = 'Active'
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM MilitaryBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'  AS DataElementName,
    'Completeness'       AS CharacteristicType,
    CAST(
        COUNT(CASE WHEN ActiveMilitaryStatusIndicatorCode IS NOT NULL
                    AND ActiveMilitaryStatusIndicatorCode <> '' THEN 1 END)
    AS NVARCHAR(MAX))    AS Value,
    'PopulatedRecords'   AS SubItemLabel,
    NULL                 AS Remarks
FROM MilitaryBase
UNION ALL
-- Distribution - Active
SELECT
    '{DataElementName}'  AS DataElementName,
    'Distribution'       AS CharacteristicType,
    CAST(COUNT(CASE WHEN ActiveMilitaryStatusIndicatorCode = 'Active' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Active'             AS SubItemLabel,
    NULL                 AS Remarks
FROM MilitaryBase
UNION ALL
-- Distribution - National Guard Or Reserve
SELECT
    '{DataElementName}'  AS DataElementName,
    'Distribution'       AS CharacteristicType,
    CAST(COUNT(CASE WHEN ActiveMilitaryStatusIndicatorCode = 'NationalGuardOrReserve' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'National Guard Or Reserve' AS SubItemLabel,
    NULL                 AS Remarks
FROM MilitaryBase
UNION ALL
-- Distribution - Not Active
SELECT
    '{DataElementName}'  AS DataElementName,
    'Distribution'       AS CharacteristicType,
    CAST(COUNT(CASE WHEN ActiveMilitaryStatusIndicatorCode = 'NotActive' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Not Active'         AS SubItemLabel,
    NULL                 AS Remarks
FROM MilitaryBase
UNION ALL
-- Distribution - Unknown
SELECT
    '{DataElementName}'  AS DataElementName,
    'Distribution'       AS CharacteristicType,
    CAST(COUNT(CASE WHEN ActiveMilitaryStatusIndicatorCode = 'Unknown' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Unknown'            AS SubItemLabel,
    NULL                 AS Remarks
FROM MilitaryBase
";

    public string AssessmentDescription =>
        "Assesses K-12 student military enlistment status from RDS.FactK12StudentEnrollments " +
        "joined to RDS.DimMilitaryStatuses. Reports the count of actively enlisted students, " +
        "completeness of the military status field across all enrollment records, and distribution " +
        "across all four CEDS status codes: Active, National Guard Or Reserve, Not Active, and Unknown.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
