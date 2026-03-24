namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Assesses the "Course identifier or title" data element from RDS.FactK12StudentCourseSections
/// joined to RDS.DimK12Courses. Evaluates record count and completeness of the course
/// identifier and title fields.
/// </summary>
public class CourseIdentifierOrTitleCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Course identifier or title";

    public string Query => $@"
WITH CourseCTE AS (
    SELECT
        f.DimK12CourseId,
        -- NOTE: Verify the correct column name for course title in RDS.DimK12Courses.
        --       Replace 'CourseTitle' below with the actual column name before running.
        c.CourseTitle
    FROM RDS.FactK12StudentCourseSections f
    -- NOTE: The original query joined f.StateK12CourseId = c.DimK12CourseId, which appears
    --       to compare a state-issued identifier to a surrogate DW key. This has been
    --       corrected to f.DimK12CourseId = c.DimK12CourseId. Verify this is the intended
    --       join key before running.
    JOIN RDS.DimK12Courses c ON f.DimK12CourseId = c.DimK12CourseId
)

INSERT INTO #Results

-- RecordCount
SELECT
    '{DataElementName}'            AS DataElementName,
    'RecordCount'                  AS CharacteristicType,
    CAST(COUNT(DISTINCT DimK12CourseId) AS NVARCHAR(MAX)) AS Value,
    NULL                           AS SubItemLabel,
    NULL                           AS Remarks
FROM CourseCTE

UNION ALL

-- Completeness - TotalRecords
SELECT
    '{DataElementName}'            AS DataElementName,
    'Completeness'                 AS CharacteristicType,
    CAST(COUNT(DISTINCT DimK12CourseId) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                 AS SubItemLabel,
    NULL                           AS Remarks
FROM CourseCTE

UNION ALL

-- Completeness - PopulatedRecords
-- NOTE: This measures how many distinct course records have a non-NULL, non-empty
--       CourseTitle value. Verify the column name 'CourseTitle' against the actual
--       schema of RDS.DimK12Courses before running.
SELECT
    '{DataElementName}'            AS DataElementName,
    'Completeness'                 AS CharacteristicType,
    CAST(
        COUNT(DISTINCT CASE
            WHEN CourseTitle IS NOT NULL
             AND LTRIM(RTRIM(CourseTitle)) <> ''
            THEN DimK12CourseId
        END)
    AS NVARCHAR(MAX))              AS Value,
    'PopulatedRecords'             AS SubItemLabel,
    NULL                           AS Remarks
FROM CourseCTE
";

    public string AssessmentDescription =>
        "Assesses 'Course identifier or title' from RDS.FactK12StudentCourseSections joined " +
        "to RDS.DimK12Courses. Reports the distinct count of course records (RecordCount) and " +
        "the completeness of the course title field, comparing total distinct courses to those " +
        "with a non-null, non-empty title value (Completeness).";
}
