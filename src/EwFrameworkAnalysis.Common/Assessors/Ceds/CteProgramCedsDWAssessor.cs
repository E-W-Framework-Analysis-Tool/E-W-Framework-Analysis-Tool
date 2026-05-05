using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles CTE program offerings by assessing the presence and distribution of
/// distinct career clusters across CTE-coded course sections in
/// RDS.FactK12StudentCourseSections. Uses CourseLevelCharacteristicCode to scope
/// to CTE courses and CareerClusterCode on RDS.DimK12Courses as the program proxy.
///
/// NOTE: No dedicated CTE program dimension has been identified in the CEDS DW schema.
/// CareerClusterCode on DimK12Courses is used as the closest available proxy for
/// program offerings. If a FactK12CTEProgram or equivalent table exists in your
/// implementation, this assessor should be revised to use it directly.
///
/// NOTE (Postsecondary): RDS.DimPsCourseStatuses contains CourseLevelTypeCode and
/// CourseCreditLevelTypeCode option set values ('Vocational', 'TechnicalPreparatory')
/// that would be the correct CTE-scoping filter for PS course transcripts. However,
/// no foreign key from RDS.FactPsStudentCourseTranscripts (or RDS.DimPsCourses) to
/// RDS.DimPsCourseStatuses is populated in known CEDS DW implementations — the table
/// appears to be an unpopulated junk dimension stub. PS CTE assessment is therefore
/// omitted until that FK relationship is established. When it is, the PS side should
/// filter on CourseLevelTypeCode IN ('Vocational', 'TechnicalPreparatory') and/or
/// CourseCreditLevelTypeCode IN ('Vocational', 'TechnicalPreparatory') and distribute
/// by an appropriate program proxy (CipCode preferred if DimCipCode is available).
/// </summary>
public class CteProgramCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "CTE program";

    public string Query => $@"
WITH CTECourses AS (
    SELECT DISTINCT
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
    FROM CTECourses
),
Distribution AS (
    SELECT
        CareerClusterDescription,
        COUNT(*)                                    AS ClusterCount
    FROM CTECourses
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
    'Populated = distinct CTE courses with non-NULL and non-empty CareerClusterCode' AS Remarks
FROM Counts
UNION ALL
-- Distribution - one row per distinct career cluster representing a CTE program offering
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Distribution'                              AS CharacteristicType,
    CAST(ClusterCount AS NVARCHAR(MAX))         AS Value,
    CareerClusterDescription                    AS SubItemLabel,
    NULL                                        AS Remarks
FROM Distribution";

    public string AssessmentDescription =>
        "Assesses CTE program offerings by profiling distinct career clusters associated with CTE-coded " +
        "courses (CourseLevelCharacteristicCode 73044 and 73045). CareerClusterCode on DimK12Courses " +
        "is used as a proxy for program identity in the absence of a dedicated CTE program dimension. " +
        "Distribution shows which career cluster programs are represented in the data. " +
        "Postsecondary assessment is not included because RDS.DimPsCourseStatuses, which contains " +
        "the relevant CTE-scoping codes (CourseLevelTypeCode 'Vocational', 'TechnicalPreparatory'), " +
        "has no populated foreign key relationship from RDS.FactPsStudentCourseTranscripts in known " +
        "CEDS DW implementations.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
