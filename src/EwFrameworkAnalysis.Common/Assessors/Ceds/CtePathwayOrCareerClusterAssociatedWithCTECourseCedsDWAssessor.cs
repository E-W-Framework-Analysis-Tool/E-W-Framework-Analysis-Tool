using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles career cluster data for CTE courses from RDS.FactK12StudentCourseSections
/// joined to RDS.DimK12Courses via StateK12CourseId, scoped to CTE general (73044)
/// and dual-credit (73045) courses via RDS.DimK12CourseStatuses. Assesses completeness
/// of CareerClusterCode within CTE course sections and distributes by cluster.
/// No grade level filter is applied.
/// </summary>
public class CtePathwayOrCareerClusterAssociatedWithCTECourseCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "CTE pathway or career cluster associated with CTE course";

    public string Query => $@"
WITH Base AS (
    SELECT
        c.CareerClusterCode,
        c.CareerClusterDescription
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimK12CourseStatuses cs
        ON f.K12CourseStatusId = cs.DimK12CourseStatusId
    JOIN RDS.DimK12Courses c
        ON f.StateK12CourseId = c.DimK12CourseId
    WHERE cs.CourseLevelCharacteristicCode IN ('73044', '73045')
    -- 73044 = Career and technical education general course
    -- 73045 = Career and technical education dual-credit course
),
Counts AS (
    SELECT
        COUNT(*)                                    AS TotalRecords,
        COUNT(CASE
            WHEN CareerClusterCode IS NOT NULL
             AND CareerClusterCode <> ''
            THEN 1
        END)                                        AS PopulatedRecords
    FROM Base
),
Distribution AS (
    SELECT
        CareerClusterDescription,
        COUNT(*)                                    AS ClusterCount
    FROM Base
    WHERE CareerClusterCode IS NOT NULL
      AND CareerClusterCode <> ''
    GROUP BY CareerClusterDescription
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
    'Populated = CareerClusterCode is non-NULL and non-empty' AS Remarks
FROM Counts
UNION ALL
-- Distribution - one row per distinct career cluster present in CTE course sections
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Distribution'                              AS CharacteristicType,
    CAST(ClusterCount AS NVARCHAR(MAX))         AS Value,
    CareerClusterDescription                    AS SubItemLabel,
    NULL                                        AS Remarks
FROM Distribution";

    public string AssessmentDescription =>
        "Assesses whether career cluster (pathway) data is populated for CTE course sections " +
        "(CourseLevelCharacteristicCode 73044 and 73045). Reports completeness of CareerClusterCode " +
        "within the CTE course population and distributes records by career cluster. No grade level " +
        "filter is applied.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
