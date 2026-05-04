using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles kindergarten enrollment dates from RDS.FactK12StudentEnrollments
/// joined to RDS.DimGradeLevels on EntryGradeLevelId, filtered to
/// GradeLevelCode = 'KG' (Kindergarten). Assesses record count and completeness
/// of the enrollment entry date field.
/// </summary>
public class KindergartenEnrollmentDateCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Kindergarten enrollment date";

    public string Query => $@"
WITH KindergartenBase AS (
    SELECT
        f.EnrollmentEntryDateId
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimGradeLevels d ON f.EntryGradeLevelId = d.DimGradeLevelId
    WHERE d.GradeLevelCode = 'KG'
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'                 AS DataElementName,
    'RecordCount'                       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))     AS Value,
    NULL                                AS SubItemLabel,
    NULL                                AS Remarks
FROM KindergartenBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'                 AS DataElementName,
    'Completeness'                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))     AS Value,
    'TotalRecords'                      AS SubItemLabel,
    NULL                                AS Remarks
FROM KindergartenBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                 AS DataElementName,
    'Completeness'                      AS CharacteristicType,
    CAST(COUNT(CASE
        WHEN EnrollmentEntryDateId IS NOT NULL
         AND EnrollmentEntryDateId <> -1
        THEN 1 END) AS NVARCHAR(MAX))   AS Value,
    'PopulatedRecords'                  AS SubItemLabel,
    NULL                                AS Remarks
FROM KindergartenBase";

    public string AssessmentDescription =>
        "Assesses kindergarten enrollment dates from RDS.FactK12StudentEnrollments joined to " +
        "RDS.DimGradeLevels, filtered to GradeLevelCode = 'KG'. Reports total record count and " +
        "completeness of the enrollment entry date field (EnrollmentEntryDateId), excluding the " +
        "default sentinel value (-1).";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
