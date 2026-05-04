using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles the presence and population of course level characteristic codes from
/// RDS.DimK12CourseStatuses across all K12 course section records, to assess whether
/// CTE courses (CourseLevelCharacteristicCode 73044 and 73045) are identifiable.
/// Distribution covers all non-missing characteristic values so the scoring layer
/// can determine CTE code presence without pre-filtering.
/// </summary>
public class CteCourseIdOrCourseTitleCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "CTE course ID or course title";

    public string Query => $@"
WITH Base AS (
    SELECT
        cs.CourseLevelCharacteristicCode,
        cs.CourseLevelCharacteristicDescription
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimK12CourseStatuses cs
        ON f.K12CourseStatusId = cs.DimK12CourseStatusId
),
Counts AS (
    SELECT
        COUNT(*)                                    AS TotalRecords,
        COUNT(CASE
            WHEN CourseLevelCharacteristicCode IS NOT NULL
             AND CourseLevelCharacteristicCode <> ''
             -- NOTE: No default sentinel was present in the DimK12CourseStatuses
             -- DDL. If ETL populates a sentinel value (e.g. 'MISSING'), add an
             -- exclusion here and in the Distribution CTE below.
             THEN 1
        END)                                        AS PopulatedRecords
    FROM Base
),
Distribution AS (
    SELECT
        CourseLevelCharacteristicDescription,
        COUNT(*)                                    AS CharacteristicCount
    FROM Base
    WHERE CourseLevelCharacteristicCode IS NOT NULL
      AND CourseLevelCharacteristicCode <> ''
    GROUP BY CourseLevelCharacteristicDescription
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
    'Populated = CourseLevelCharacteristicCode is non-NULL and non-empty' AS Remarks
FROM Counts
UNION ALL
-- Distribution - one row per distinct course level characteristic present in data
-- CTE codes will appear as: 'Career and technical education general course' (73044)
-- and 'Career and technical education dual-credit course' (73045)
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Distribution'                              AS CharacteristicType,
    CAST(CharacteristicCount AS NVARCHAR(MAX))  AS Value,
    CourseLevelCharacteristicDescription        AS SubItemLabel,
    NULL                                        AS Remarks
FROM Distribution";

    public string AssessmentDescription =>
        "Assesses whether CTE courses are identifiable in K12 course section records by profiling " +
        "completeness of CourseLevelCharacteristicCode across all course sections and distributing " +
        "records by characteristic description. CTE general (73044) and dual-credit (73045) codes " +
        "will appear in the distribution if present. No pre-filtering to CTE codes is applied.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
