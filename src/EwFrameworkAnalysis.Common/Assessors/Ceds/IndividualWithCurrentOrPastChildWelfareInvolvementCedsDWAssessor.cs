namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles child welfare involvement for K-12 students from
/// RDS.FactK12StudentEnrollments joined to RDS.DimFosterCareStatuses on
/// FosterCareStatusId. This is the only sector with direct schema support;
/// Pre-K and Postsecondary are represented with zero counts as no supporting
/// schema was identified. Assesses record count, completeness, and distribution
/// by foster care program participation status.
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
        d.ProgramParticipationFosterCareCode,
        d.ProgramParticipationFosterCareDescription
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
-- RecordCount - K12
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'RecordCount'                                       AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                 AS Value,
    NULL                                                AS SubItemLabel,
    'Source: FactK12StudentEnrollments'                 AS Remarks
FROM K12Counts
UNION ALL
-- Completeness - TotalRecords - K12
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Completeness'                                      AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                 AS Value,
    'TotalRecords'                                      AS SubItemLabel,
    'Source: FactK12StudentEnrollments'                 AS Remarks
FROM K12Counts
UNION ALL
-- Completeness - PopulatedRecords - K12
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Completeness'                                      AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))             AS Value,
    'PopulatedRecords'                                  AS SubItemLabel,
    'Source: FactK12StudentEnrollments'                 AS Remarks
FROM K12Counts
UNION ALL
-- Distribution - K12 by foster care participation status
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Distribution'                                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                     AS Value,
    ProgramParticipationFosterCareDescription           AS SubItemLabel,
    'Source: FactK12StudentEnrollments'                 AS Remarks
FROM FosterCareBase
GROUP BY ProgramParticipationFosterCareDescription
UNION ALL
-- RecordCount - PreK (no supporting schema identified; emitted as zero)
-- NOTE: No PreK fact table with FosterCareStatusId was found in the known schema.
-- Replace this stub if FactElChildEnrollments or similar becomes available.
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'RecordCount'                                       AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX))                            AS Value,
    NULL                                                AS SubItemLabel,
    'Source: PreK (not available in schema)'            AS Remarks
UNION ALL
-- RecordCount - Postsecondary (no supporting schema identified; emitted as zero)
-- NOTE: No PS fact table with FosterCareStatusId was found in the known schema.
-- Replace this stub if a suitable PS fact table becomes available.
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'RecordCount'                                       AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX))                            AS Value,
    NULL                                                AS SubItemLabel,
    'Source: Postsecondary (not available in schema)'   AS Remarks";

    public string AssessmentDescription =>
        "Assesses child welfare involvement for K-12 students from RDS.FactK12StudentEnrollments " +
        "joined to RDS.DimFosterCareStatuses. Reports record count, completeness of the foster care " +
        "participation code, and distribution by foster care status description. Pre-K and " +
        "Postsecondary sectors are represented with zero record counts pending schema identification.";
}
