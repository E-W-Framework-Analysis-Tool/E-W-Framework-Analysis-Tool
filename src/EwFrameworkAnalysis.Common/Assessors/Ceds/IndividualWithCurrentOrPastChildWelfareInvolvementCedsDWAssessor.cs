using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles child welfare involvement for K-12 students from
/// RDS.FactK12StudentEnrollments joined to RDS.DimFosterCareStatuses on
/// FosterCareStatusId. This is the only sector with direct schema support;
/// Pre-K and Postsecondary are represented with zero counts as no supporting
/// schema was identified. Assesses record count, completeness, and per-sector
/// distribution of populated foster care participation records.
/// Note: The original DE query used RDS.FactSpecialEducation joined to
/// RDS.DimIndividualizedProgramStatuses filtered to 'Children''s protective
/// services' — that approach is narrower (special ed population only) and
/// was superseded by the direct FosterCareStatusId path on
/// FactK12StudentEnrollments.
/// </summary>
public class IndividualWithCurrentOrPastChildWelfareInvolvementCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Individual with current or past child welfare involvement";

    public string Query => $@"
WITH FosterCareBase AS (
    SELECT
        d.ProgramParticipationFosterCareCode
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimFosterCareStatuses d
        ON f.FosterCareStatusId = d.DimFosterCareStatusId
),
K12Counts AS (
    SELECT
        COUNT(*) AS TotalRecords,
        COUNT(CASE
            WHEN ProgramParticipationFosterCareCode IS NOT NULL
             AND ProgramParticipationFosterCareCode <> ''
             AND ProgramParticipationFosterCareCode <> 'MISSING'
            THEN 1 END) AS PopulatedRecords
    FROM FosterCareBase
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'RecordCount'                                                       AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                                 AS Value,
    NULL                                                                AS SubItemLabel,
    NULL                                                                AS Remarks
FROM K12Counts
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'Completeness'                                                      AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                                 AS Value,
    'TotalRecords'                                                      AS SubItemLabel,
    NULL                                                                AS Remarks
FROM K12Counts
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'Completeness'                                                      AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))                             AS Value,
    'PopulatedRecords'                                                  AS SubItemLabel,
    NULL                                                                AS Remarks
FROM K12Counts
UNION ALL
-- Distribution - Pre-K (no supporting schema identified)
-- NOTE: No PreK fact table with FosterCareStatusId was found in the known schema.
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
    CAST(PopulatedRecords AS NVARCHAR(MAX))                             AS Value,
    'K-12'                                                              AS SubItemLabel,
    NULL                                                                AS Remarks
FROM K12Counts
UNION ALL
-- Distribution - Postsecondary (no supporting schema identified)
-- NOTE: No PS fact table with FosterCareStatusId was found in the known schema.
-- Replace this stub if a suitable PS fact table becomes available.
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'Distribution'                                                      AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX))                                            AS Value,
    'Postsecondary'                                                     AS SubItemLabel,
    'No source table available for Postsecondary sector'                AS Remarks";

    public string AssessmentDescription =>
        "Assesses child welfare involvement for K-12 students from RDS.FactK12StudentEnrollments " +
        "joined to RDS.DimFosterCareStatuses. Reports record count, completeness of the foster care " +
        "participation code, and per-sector distribution of populated records. Pre-K and Postsecondary " +
        "sectors emit zero pending schema identification.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
