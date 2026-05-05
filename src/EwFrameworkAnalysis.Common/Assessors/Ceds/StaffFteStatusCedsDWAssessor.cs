using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Assesses the Staff FTE status data element from RDS.FactK12StaffEmployments,
/// joined to RDS.DimPeople_Current. Evaluates record volume, completeness of the
/// FullTimeEquivalency field, and distribution across FTE status categories
/// derived from FullTimeEquivalency thresholds.
/// </summary>
public class StaffFteStatusCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Staff FTE status";

    public string Query => $@"
-- NOTE: The original query filtered WHERE FullTimeEquivalency > 0.75, which would
-- misrepresent RecordCount and Completeness for 'Staff FTE status' as a whole.
-- That filter has been removed from the base CTE so all staff employment records
-- are assessed. If the intent is to scope this assessor to a specific population,
-- re-add the filter to the CTE and update AssessmentDescription accordingly.

-- NOTE: The Distribution blocks below use derived bands from FullTimeEquivalency
-- as a proxy for FTE status categories. If a dedicated FTE status dimension column
-- exists (e.g. FteStatusCode, FteStatusDescription, or similar on a joined
-- dimension table), replace the CASE expressions with that column and adjust
-- the category labels to match the CEDS-defined values for this element.

-- NOTE: IntegerRange is not included because FullTimeEquivalency is a decimal/
-- float column rather than an integer. If a min/max range characteristic is
-- desired, confirm the column type and implement a custom FloatRange characteristic,
-- or add it manually once the column type is verified.

WITH BaseData AS (
    SELECT
        f.FullTimeEquivalency
    FROM RDS.FactK12StaffEmployments f
    JOIN RDS.DimPeople_Current d ON d.DimPersonId = f.K12Staff_CurrentId
)
INSERT INTO #EWFProfilerResults

-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM BaseData

UNION ALL

-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM BaseData

UNION ALL

-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                              AS DataElementName,
    'Completeness'                                   AS CharacteristicType,
    CAST(COUNT(FullTimeEquivalency) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'                               AS SubItemLabel,
    NULL                                             AS Remarks
FROM BaseData

UNION ALL

-- Distribution - Full Time (1.0 <= x)
SELECT
    '{DataElementName}'             AS DataElementName,
    'Distribution'                  AS CharacteristicType,
    CAST(COUNT(CASE WHEN FullTimeEquivalency >= 1.0 THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Full Time (1.0 <= x)'          AS SubItemLabel,
    NULL                            AS Remarks
FROM BaseData

UNION ALL

-- Distribution - Part Time (0.5 <= x < 1.0)
SELECT
    '{DataElementName}'             AS DataElementName,
    'Distribution'                  AS CharacteristicType,
    CAST(COUNT(CASE WHEN FullTimeEquivalency >= 0.5 AND FullTimeEquivalency < 1.0 THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Part Time (0.5 <= x < 1.0)'   AS SubItemLabel,
    NULL                            AS Remarks
FROM BaseData

UNION ALL

-- Distribution - Less Than Half Time (0 < x < 0.5)
SELECT
    '{DataElementName}'                  AS DataElementName,
    'Distribution'                       AS CharacteristicType,
    CAST(COUNT(CASE WHEN FullTimeEquivalency > 0.0 AND FullTimeEquivalency < 0.5 THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Less Than Half Time (0 < x < 0.5)' AS SubItemLabel,
    NULL                                 AS Remarks
FROM BaseData

UNION ALL

-- Distribution - Not Employed (0.0)
SELECT
    '{DataElementName}'             AS DataElementName,
    'Distribution'                  AS CharacteristicType,
    CAST(COUNT(CASE WHEN FullTimeEquivalency = 0.0 THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Not Employed (0.0)'            AS SubItemLabel,
    NULL                            AS Remarks
FROM BaseData

UNION ALL

-- Distribution - Unknown / Not Reported (NULL)
SELECT
    '{DataElementName}'             AS DataElementName,
    'Distribution'                  AS CharacteristicType,
    CAST(COUNT(CASE WHEN FullTimeEquivalency IS NULL THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Unknown / Not Reported'        AS SubItemLabel,
    NULL                            AS Remarks
FROM BaseData";

    public string AssessmentDescription =>
        "Assesses Staff FTE status from RDS.FactK12StaffEmployments joined to " +
        "RDS.DimPeople_Current. Reports total record count, completeness of the " +
        "FullTimeEquivalency field, and distribution of records across FTE status " +
        "bands (Full Time, Part Time, Less Than Half Time, Not Employed, Unknown / " +
        "Not Reported). Distribution categories are derived from FullTimeEquivalency " +
        "thresholds pending confirmation of a dedicated FTE status dimension column.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
