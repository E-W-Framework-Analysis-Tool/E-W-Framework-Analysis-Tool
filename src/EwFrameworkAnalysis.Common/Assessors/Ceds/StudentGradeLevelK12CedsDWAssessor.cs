using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles student grade level (K-12) from RDS.FactK12StudentEnrollments,
/// joined to RDS.DimGradeLevels via EntryGradeLevelId. Assesses record count,
/// completeness of the entry grade level FK, and distribution of enrollment
/// records across grade level descriptions.
/// </summary>
public class StudentGradeLevelK12CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Student grade level (K-12)";

    public string Query => $@"
WITH EnrollmentBase AS (
    SELECT
        f.EntryGradeLevelId
    FROM RDS.FactK12StudentEnrollments f
),
GradeBase AS (
    SELECT
        e.EntryGradeLevelId,
        g.GradeLevelCode,
        g.GradeLevelDescription
    FROM EnrollmentBase e
    JOIN RDS.DimGradeLevels g ON e.EntryGradeLevelId = g.DimGradeLevelId
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM EnrollmentBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM EnrollmentBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                          AS DataElementName,
    'Completeness'                               AS CharacteristicType,
    CAST(COUNT(EntryGradeLevelId) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'                           AS SubItemLabel,
    NULL                                         AS Remarks
FROM EnrollmentBase
UNION ALL
-- Distribution - by grade level
SELECT
    '{DataElementName}'                      AS DataElementName,
    'Distribution'                           AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))          AS Value,
    GradeLevelDescription                    AS SubItemLabel,
    GradeLevelCode                           AS Remarks
FROM GradeBase
GROUP BY GradeLevelDescription, GradeLevelCode";

    public string AssessmentDescription =>
        "Assesses student grade level (K-12) using EntryGradeLevelId from enrollment records. " +
        "Reports total record count, completeness of the entry grade level field, and distribution " +
        "of enrollment records across all grade levels present in the warehouse.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
