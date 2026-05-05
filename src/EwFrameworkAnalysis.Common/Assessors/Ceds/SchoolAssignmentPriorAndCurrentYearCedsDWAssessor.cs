using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles school assignment (prior and current year) for K-12 staff
/// from RDS.FactK12StaffAssignments, joined to RDS.DimK12Schools and RDS.DimSchoolYears.
/// Assesses total assignment record count, completeness of the school FK,
/// and distribution of assignment records across school years.
/// </summary>
public class SchoolAssignmentPriorAndCurrentYearCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "School assignment (prior and current year)";

    public string Query => $@"
WITH AssignmentBase AS (
    SELECT
        f.K12SchoolId,
        sy.SchoolYear
    FROM RDS.FactK12StaffAssignments f
    JOIN RDS.DimK12Schools  d  ON f.K12SchoolId   = d.DimK12SchoolId
    JOIN RDS.DimSchoolYears sy ON f.SchoolYearId  = sy.DimSchoolYearId
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM AssignmentBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM AssignmentBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                       AS DataElementName,
    'Completeness'                            AS CharacteristicType,
    CAST(COUNT(K12SchoolId) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'                        AS SubItemLabel,
    NULL                                      AS Remarks
FROM AssignmentBase
UNION ALL
-- Distribution - by school year
SELECT
    '{DataElementName}'                      AS DataElementName,
    'Distribution'                           AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))          AS Value,
    CAST(SchoolYear AS NVARCHAR(MAX))        AS SubItemLabel,
    NULL                                     AS Remarks
FROM AssignmentBase
GROUP BY SchoolYear";

    public string AssessmentDescription =>
        "Assesses school assignment records for K-12 staff, covering prior and current year. " +
        "Reports total assignment record count, completeness of the school foreign key, " +
        "and distribution of assignment records across school years.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
