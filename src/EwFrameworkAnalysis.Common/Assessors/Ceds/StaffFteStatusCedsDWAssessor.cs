namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Assesses the Staff FTE status data element from RDS.FactK12StaffEmployments,
/// joined to RDS.DimPeople_Current. Evaluates record volume, completeness of the
/// FullTimeEquivalency field, its numeric range, and distribution across FTE
/// status categories.
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

WITH BaseData AS (
    SELECT
        f.FullTimeEquivalency
        -- NOTE: If a categorical FTE status column exists on FactK12StaffEmployments
        -- or a joined dimension (e.g. DimK12StaffEmployment or similar), replace
        -- the FullTimeEquivalency references in the Distribution block below with
        -- that column. Verify the correct column name before running.
    FROM RDS.FactK12StaffEmployments f
    JOIN RDS.DimPeople_Current d ON d.DimPersonId = f.K12Staff_CurrentId
)

INSERT INTO #Results

-- RecordCount
SELECT
    '{DataElementName}'            AS DataElementName,
    'RecordCount'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                           AS SubItemLabel,
    NULL                           AS Remarks
FROM BaseData

UNION ALL

-- Completeness - TotalRecords
SELECT
    '{DataElementName}'            AS DataElementName,
    'Completeness'                 AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                 AS SubItemLabel,
    NULL                           AS Remarks
FROM BaseData

UNION ALL

-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                                          AS DataElementName,
    'Completeness'                                              AS CharacteristicType,
    CAST(COUNT(FullTimeEquivalency) AS NVARCHAR(MAX))           AS Value,
    'PopulatedRecords'                                          AS SubItemLabel,
    'Counts rows where FullTimeEquivalency is not NULL'         AS Remarks
FROM BaseData

UNION ALL

-- Distribution by FTE status category
-- NOTE: The distribution below uses derived bands from FullTimeEquivalency as a
-- proxy for FTE status categories. If a dedicated FTE status dimension column
-- exists (e.g. FteStatusCode, FteStatusDescription, or similar on a joined
-- dimension table), replace this CASE expression with that column and adjust
-- the category labels to match the CEDS-defined values for this element.
-- Verify that all expected CEDS FTE status categories are represented.
SELECT
    '{DataElementName}'                    AS DataElementName,
    'Distribution'                         AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))        AS Value,
    CASE
        WHEN FullTimeEquivalency >= 1.0              THEN 'Full Time (1.0 <= x)'
        WHEN FullTimeEquivalency >= 0.5
             AND FullTimeEquivalency < 1.0           THEN 'Part Time (0.5 <= x < 1)'
        WHEN FullTimeEquivalency > 0.0
             AND FullTimeEquivalency < 0.5           THEN 'Less Than Half Time (0 < x < 0.5)'
        WHEN FullTimeEquivalency = 0.0               THEN 'Not Employed'
        ELSE                                              'Unknown / Not Reported'
    END                                    AS SubItemLabel,
    NULL                                   AS Remarks
FROM BaseData
WHERE FullTimeEquivalency IS NOT NULL
GROUP BY
    CASE
        WHEN FullTimeEquivalency >= 1.0              THEN 'Full Time (1.0 <= x)'
        WHEN FullTimeEquivalency >= 0.5
             AND FullTimeEquivalency < 1.0           THEN 'Part Time (0.5 <= x < 1)'
        WHEN FullTimeEquivalency > 0.0
             AND FullTimeEquivalency < 0.5           THEN 'Less Than Half Time (0 < x < 0.5)'
        WHEN FullTimeEquivalency = 0.0               THEN 'Not Employed'
        ELSE                                              'Unknown / Not Reported'
    END
";

    public string AssessmentDescription =>
        "Assesses Staff FTE status from RDS.FactK12StaffEmployments joined to " +
        "RDS.DimPeople_Current. Reports total record count, completeness of the " +
        "FullTimeEquivalency field, its observed minimum and maximum values, and " +
        "distribution of records across FTE status bands (Full Time, Part Time, " +
        "Less Than Half Time, Not Employed, Unknown / Not Reported). Distribution " +
        "categories are derived from FullTimeEquivalency thresholds pending " +
        "confirmation of a dedicated FTE status dimension column.";
}
