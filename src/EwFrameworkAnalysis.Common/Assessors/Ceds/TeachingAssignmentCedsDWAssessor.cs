namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Assesses the "Teaching assignment" data element from RDS.FactK12StaffAssignments,
/// joined to RDS.BridgeK12StaffAssignmentCourseSections. Evaluates record count,
/// completeness of the teaching assignment type field, and distribution across
/// assignment type categories.
/// </summary>
public class TeachingAssignmentCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Teaching assignment";
    public string Query => $@"
-- NOTE: The teaching assignment type column name is not present in the submitted
--       query. Replace TeachingAssignmentType below with the correct column name
--       from RDS.FactK12StaffAssignments or its associated dimension table (e.g.
--       a DimK12StaffAssignment or similar). Verify the column and adjust the
--       Distribution category labels to match the CEDS-defined values for this
--       element before running.
WITH BaseData AS (
    SELECT
        f.TeachingAssignmentType -- NOTE: replace with verified column name
    FROM RDS.FactK12StaffAssignments f
    JOIN RDS.BridgeK12StaffAssignmentCourseSections b
        ON b.FactK12StaffAssignmentId = f.FactK12StaffAssignmentId
),
Counts AS (
    SELECT
        COUNT(*)                                                    AS TotalRecords,
        COUNT(CASE WHEN TeachingAssignmentType IS NOT NULL
                    AND TeachingAssignmentType <> '' THEN 1 END)   AS PopulatedRecords
    FROM BaseData
)
INSERT INTO #EWFProfilerResults

-- RecordCount
SELECT
    '{DataElementName}'                         AS DataElementName,
    'RecordCount'                               AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    NULL                                        AS SubItemLabel,
    NULL                                        AS Remarks
FROM Counts

UNION ALL

-- Completeness - TotalRecords
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    'TotalRecords'                              AS SubItemLabel,
    NULL                                        AS Remarks
FROM Counts

UNION ALL

-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))     AS Value,
    'PopulatedRecords'                          AS SubItemLabel,
    NULL                                        AS Remarks
FROM Counts

UNION ALL

-- NOTE: Distribution categories below are placeholders. Replace the CASE WHEN
--       values and SubItemLabels with the actual CEDS-defined teaching assignment
--       type values present in your data once the column name is confirmed.

-- Distribution - [Category 1]
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Distribution'                              AS CharacteristicType,
    CAST(COUNT(CASE WHEN TeachingAssignmentType = 'CategoryValue1' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Category 1'                                AS SubItemLabel,
    NULL                                        AS Remarks
FROM BaseData

UNION ALL

-- Distribution - Unknown / Not Reported
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Distribution'                              AS CharacteristicType,
    CAST(COUNT(CASE WHEN TeachingAssignmentType IS NULL
                      OR TeachingAssignmentType = '' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Unknown / Not Reported'                    AS SubItemLabel,
    NULL                                        AS Remarks
FROM BaseData";

    public string AssessmentDescription =>
        "Assesses teaching assignment type from RDS.FactK12StaffAssignments joined to " +
        "RDS.BridgeK12StaffAssignmentCourseSections. Reports total record count, " +
        "completeness of the teaching assignment type field, and distribution across " +
        "assignment type categories. Distribution category labels are placeholders " +
        "pending confirmation of the assignment type column name and CEDS-defined values.";
}
