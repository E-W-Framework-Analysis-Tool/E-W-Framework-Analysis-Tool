using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles occupation category for workforce records from RDS.FactQuarterlyEmployments,
/// joined to RDS.DimOnetSocOccupationTypes. Assesses record count, completeness of the
/// O*NET-SOC occupation type, and distribution across O*NET-SOC major groups (first two
/// digits of the ##.####.## code).
/// </summary>
public class OccupationCategoryCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Occupation category";
    public string Query => $@"
WITH Base AS (
    SELECT
        d.OnetSocOccupationTypeCode,
        d.OnetSocOccupationTypeDescription,
        -- Extract major group prefix (e.g., '11' from '11-1011.00')
        LEFT(d.OnetSocOccupationTypeCode, 2) AS MajorGroupCode
    FROM RDS.FactQuarterlyEmployments f
    JOIN RDS.DimOnetSocOccupationTypes d
        ON f.OnetSocOccupationTypeId = d.DimOnetSocOccupationTypeID
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
    CAST(COUNT(OnetSocOccupationTypeCode) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - by O*NET-SOC major group
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    MajorGroupCode      AS SubItemLabel,
    NULL                AS Remarks
FROM Base
WHERE MajorGroupCode IS NOT NULL
GROUP BY MajorGroupCode
";
    public string AssessmentDescription =>
        "Assesses record count, completeness, and distribution by O*NET-SOC major group " +
        "(first two digits of the occupation code) for workforce records from " +
        "RDS.FactQuarterlyEmployments, joined to RDS.DimOnetSocOccupationTypes.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
