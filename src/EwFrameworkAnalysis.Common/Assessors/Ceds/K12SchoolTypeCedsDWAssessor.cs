using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles K-12 school type from RDS.FactK12StudentEnrollments joined to
/// RDS.DimK12Schools on K12SchoolId. Assesses record count, completeness of
/// the school type code field, distribution of enrollment records by school
/// type description, and distribution by charter school indicator.
/// </summary>
public class K12SchoolTypeCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "K-12 school type";

    public string Query => $@"
WITH SchoolBase AS (
    SELECT
        d.SchoolTypeCode,
        d.SchoolTypeDescription,
        d.CharterSchoolIndicator
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimK12Schools d ON f.K12SchoolId = d.DimK12SchoolId
),
Counts AS (
    SELECT
        COUNT(*) AS TotalRecords,
        COUNT(CASE
            WHEN SchoolTypeCode IS NOT NULL
             AND SchoolTypeCode <> ''
             AND SchoolTypeCode <> 'MISSING'
            THEN 1 END) AS PopulatedRecords
    FROM SchoolBase
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM Counts
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM Counts
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'              AS SubItemLabel,
    NULL                            AS Remarks
FROM Counts
UNION ALL
-- Distribution - by school type description
SELECT
    '{DataElementName}'             AS DataElementName,
    'Distribution'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    SchoolTypeDescription           AS SubItemLabel,
    'SchoolType'                    AS Remarks
FROM SchoolBase
GROUP BY SchoolTypeDescription
UNION ALL
-- Distribution - by charter school indicator
SELECT
    '{DataElementName}'                                     AS DataElementName,
    'Distribution'                                          AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                         AS Value,
    CASE
        WHEN CharterSchoolIndicator = 1 THEN 'Charter School'
        WHEN CharterSchoolIndicator = 0 THEN 'Not Charter School'
        ELSE 'Unknown'
    END                                                     AS SubItemLabel,
    'CharterSchoolIndicator'                                AS Remarks
FROM SchoolBase
GROUP BY CharterSchoolIndicator";

    public string AssessmentDescription =>
        "Assesses K-12 school type from RDS.FactK12StudentEnrollments joined to RDS.DimK12Schools. " +
        "Reports total record count, completeness of the school type code field, distribution of " +
        "enrollment records by school type description (e.g. Regular, Charter, Magnet, Alternative), " +
        "and distribution by charter school indicator flag.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
