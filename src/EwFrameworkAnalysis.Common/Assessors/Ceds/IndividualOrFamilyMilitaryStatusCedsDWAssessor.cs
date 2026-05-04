using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles individual or family military status as a disaggregate across K-12,
/// postsecondary, and adult education populations from RDS.FactK12StudentEnrollments,
/// RDS.FactPsStudentCourseTranscripts, and RDS.FactAeStudentEnrollments respectively,
/// each joined to RDS.DimMilitaryStatuses. Pre-K and Workforce sectors are represented
/// with zero counts as no supporting schema was identified. Assesses combined record
/// count and completeness across all sectors, and per-sector populated counts as a
/// distribution.
/// Note: FactPsStudentCourseTranscripts contains a typo in the FK column name
/// (MilitartyStatusId — extra 't'); the query uses the actual column name as-is.
/// </summary>
public class IndividualOrFamilyMilitaryStatusCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Individual or family military status";

    public string Query => $@"
WITH K12Base AS (
    SELECT
        d.MilitaryConnectedStudentIndicatorCode
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimMilitaryStatuses d
        ON f.MilitaryStatusId = d.DimMilitaryStatusId
),
PsBase AS (
    SELECT
        d.MilitaryConnectedStudentIndicatorCode
    FROM RDS.FactPsStudentCourseTranscripts f
    JOIN RDS.DimMilitaryStatuses d
        ON f.MilitartyStatusId = d.DimMilitaryStatusId
),
AeBase AS (
    SELECT
        d.MilitaryConnectedStudentIndicatorCode
    FROM RDS.FactAeStudentEnrollments f
    JOIN RDS.DimMilitaryStatuses d
        ON f.MilitaryStatusId = d.DimMilitaryStatusId
),
K12Counts AS (
    SELECT
        COUNT(*) AS TotalRecords,
        COUNT(CASE
            WHEN MilitaryConnectedStudentIndicatorCode IS NOT NULL
             AND MilitaryConnectedStudentIndicatorCode <> ''
             AND MilitaryConnectedStudentIndicatorCode <> 'MISSING'
            THEN 1 END) AS PopulatedRecords
    FROM K12Base
),
PsCounts AS (
    SELECT
        COUNT(*) AS TotalRecords,
        COUNT(CASE
            WHEN MilitaryConnectedStudentIndicatorCode IS NOT NULL
             AND MilitaryConnectedStudentIndicatorCode <> ''
             AND MilitaryConnectedStudentIndicatorCode <> 'MISSING'
            THEN 1 END) AS PopulatedRecords
    FROM PsBase
),
AeCounts AS (
    SELECT
        COUNT(*) AS TotalRecords,
        COUNT(CASE
            WHEN MilitaryConnectedStudentIndicatorCode IS NOT NULL
             AND MilitaryConnectedStudentIndicatorCode <> ''
             AND MilitaryConnectedStudentIndicatorCode <> 'MISSING'
            THEN 1 END) AS PopulatedRecords
    FROM AeBase
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'RecordCount'                                                       AS CharacteristicType,
    CAST(
        (SELECT TotalRecords FROM K12Counts) +
        (SELECT TotalRecords FROM PsCounts) +
        (SELECT TotalRecords FROM AeCounts)
    AS NVARCHAR(MAX))                                                   AS Value,
    NULL                                                                AS SubItemLabel,
    NULL                                                                AS Remarks
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'Completeness'                                                      AS CharacteristicType,
    CAST(
        (SELECT TotalRecords FROM K12Counts) +
        (SELECT TotalRecords FROM PsCounts) +
        (SELECT TotalRecords FROM AeCounts)
    AS NVARCHAR(MAX))                                                   AS Value,
    'TotalRecords'                                                      AS SubItemLabel,
    NULL                                                                AS Remarks
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'Completeness'                                                      AS CharacteristicType,
    CAST(
        (SELECT PopulatedRecords FROM K12Counts) +
        (SELECT PopulatedRecords FROM PsCounts) +
        (SELECT PopulatedRecords FROM AeCounts)
    AS NVARCHAR(MAX))                                                   AS Value,
    'PopulatedRecords'                                                  AS SubItemLabel,
    NULL                                                                AS Remarks
UNION ALL
-- Distribution - Pre-K (no supporting schema identified)
-- NOTE: No PreK fact table with MilitaryStatusId was found in the known schema.
-- Replace this stub if FactElChildEnrollments or similar becomes available.
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'Distribution'                                                      AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX))                                            AS Value,
    'Pre-K'                                                             AS SubItemLabel,
    'No source table available for Pre-K sector'                        AS Remarks
UNION ALL
-- Distribution - K-12
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'Distribution'                                                      AS CharacteristicType,
    CAST((SELECT PopulatedRecords FROM K12Counts) AS NVARCHAR(MAX))     AS Value,
    'K-12'                                                              AS SubItemLabel,
    NULL                                                                AS Remarks
UNION ALL
-- Distribution - Postsecondary
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'Distribution'                                                      AS CharacteristicType,
    CAST((SELECT PopulatedRecords FROM PsCounts) AS NVARCHAR(MAX))      AS Value,
    'Postsecondary'                                                     AS SubItemLabel,
    NULL                                                                AS Remarks
UNION ALL
-- Distribution - Adult Education
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'Distribution'                                                      AS CharacteristicType,
    CAST((SELECT PopulatedRecords FROM AeCounts) AS NVARCHAR(MAX))      AS Value,
    'Adult Education'                                                   AS SubItemLabel,
    NULL                                                                AS Remarks
UNION ALL
-- Distribution - Workforce (no supporting schema identified)
-- NOTE: No workforce fact table with MilitaryStatusId was found in the known schema.
-- Replace this stub if FactWfProgramParticipation or similar becomes available.
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'Distribution'                                                      AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX))                                            AS Value,
    'Workforce'                                                         AS SubItemLabel,
    'No source table available for Workforce sector'                    AS Remarks";

    public string AssessmentDescription =>
        "Assesses individual or family military status across K-12, postsecondary, and adult " +
        "education populations joined to RDS.DimMilitaryStatuses. Reports combined record count " +
        "and completeness across all sectors, and per-sector populated counts as a distribution. " +
        "Pre-K and Workforce emit zero pending schema identification. Note: the PS fact table " +
        "contains a typo in the military status FK column name (MilitartyStatusId).";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
