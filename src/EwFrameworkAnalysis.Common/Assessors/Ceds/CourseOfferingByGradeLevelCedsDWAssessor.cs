using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles course-level grade span data from RDS.DimScedCodes using the
/// ScedGradeSpan field, which represents the grade span for which each SCED
/// course is appropriate. Assesses whether grade span is populated across
/// distinct courses and how courses are distributed across grade spans.
/// </summary>
public class CourseOfferingByGradeLevelCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Course offering by grade level";

    public string Query => $@"
WITH Courses AS (
    SELECT
        ScedGradeSpan
    FROM RDS.DimScedCodes
),
Counts AS (
    SELECT
        COUNT(*)                            AS TotalRecords,
        COUNT(CASE
            WHEN LTRIM(RTRIM(ScedGradeSpan)) <> ''
            THEN 1
        END)                                AS PopulatedRecords
    FROM Courses
),
Distribution AS (
    SELECT
        LTRIM(RTRIM(ScedGradeSpan))         AS GradeSpan,
        COUNT(*)                            AS SpanCount
    FROM Courses
    WHERE LTRIM(RTRIM(ScedGradeSpan)) <> ''
    GROUP BY LTRIM(RTRIM(ScedGradeSpan))
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
    'Populated = ScedGradeSpan is non-empty' AS Remarks
FROM Counts
UNION ALL
-- Distribution - one row per distinct grade span present in data
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Distribution'                              AS CharacteristicType,
    CAST(SpanCount AS NVARCHAR(MAX))            AS Value,
    GradeSpan                                   AS SubItemLabel,
    NULL                                        AS Remarks
FROM Distribution";

    public string AssessmentDescription =>
        "Assesses whether SCED courses in RDS.DimScedCodes have grade span information populated " +
        "via the ScedGradeSpan field (the grade span for which the course is appropriate). Reports " +
        "total distinct course count, completeness of grade span values, and distribution of courses " +
        "across each grade span present in the data.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
