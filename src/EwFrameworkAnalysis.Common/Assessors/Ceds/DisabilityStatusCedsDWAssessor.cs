using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles disability status data across three populations — K-12, postsecondary,
/// and adult education — from RDS.FactK12StudentEnrollments, RDS.FactPsStudentCourseTranscripts,
/// and RDS.FactAeStudentEnrollments respectively, each joined to RDS.DimDisabilityStatuses.
/// Reports a combined record count and completeness across all sources, and a per-sector
/// count of records with a populated DisabilityStatusCode as a distribution.
/// Note: DisabilityStatusCode is a Yes/No flag. K-12 enrollment records also carry
/// IdeaStatusId, PrimaryDisabilityTypeId, and SecondaryDisabilityTypeId as related
/// but distinct disability indicators not profiled here.
/// </summary>
public class DisabilityStatusCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Disability status";

    public string Query => $@"
WITH K12Base AS (
    SELECT
        d.DisabilityStatusCode
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimDisabilityStatuses d
        ON f.DisabilityStatusId = d.DimDisabilityStatusId
),
PsBase AS (
    SELECT
        d.DisabilityStatusCode
    FROM RDS.FactPsStudentCourseTranscripts f
    JOIN RDS.DimDisabilityStatuses d
        ON f.DisabilityStatusId = d.DimDisabilityStatusId
),
AeBase AS (
    SELECT
        d.DisabilityStatusCode
    FROM RDS.FactAeStudentEnrollments f
    JOIN RDS.DimDisabilityStatuses d
        ON f.DisabilityStatusId = d.DimDisabilityStatusId
),
K12Counts AS (
    SELECT
        COUNT(*) AS TotalRecords,
        COUNT(CASE
            WHEN DisabilityStatusCode IS NOT NULL
             AND DisabilityStatusCode <> ''
             -- NOTE: No default sentinel was confirmed in the DimDisabilityStatuses
             -- DDL. 'MISSING' is excluded defensively per the pattern used elsewhere.
             -- Review distinct DisabilityStatusCode values and extend if needed.
             AND DisabilityStatusCode <> 'MISSING'
            THEN 1
        END) AS PopulatedRecords
    FROM K12Base
),
PsCounts AS (
    SELECT
        COUNT(*) AS TotalRecords,
        COUNT(CASE
            WHEN DisabilityStatusCode IS NOT NULL
             AND DisabilityStatusCode <> ''
             AND DisabilityStatusCode <> 'MISSING'
            THEN 1
        END) AS PopulatedRecords
    FROM PsBase
),
AeCounts AS (
    SELECT
        COUNT(*) AS TotalRecords,
        COUNT(CASE
            WHEN DisabilityStatusCode IS NOT NULL
             AND DisabilityStatusCode <> ''
             AND DisabilityStatusCode <> 'MISSING'
            THEN 1
        END) AS PopulatedRecords
    FROM AeBase
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'                                             AS DataElementName,
    'RecordCount'                                                   AS CharacteristicType,
    CAST(
        (SELECT TotalRecords FROM K12Counts) +
        (SELECT TotalRecords FROM PsCounts) +
        (SELECT TotalRecords FROM AeCounts)
    AS NVARCHAR(MAX))                                               AS Value,
    NULL                                                            AS SubItemLabel,
    NULL                                                            AS Remarks
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'                                             AS DataElementName,
    'Completeness'                                                  AS CharacteristicType,
    CAST(
        (SELECT TotalRecords FROM K12Counts) +
        (SELECT TotalRecords FROM PsCounts) +
        (SELECT TotalRecords FROM AeCounts)
    AS NVARCHAR(MAX))                                               AS Value,
    'TotalRecords'                                                  AS SubItemLabel,
    NULL                                                            AS Remarks
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                                             AS DataElementName,
    'Completeness'                                                  AS CharacteristicType,
    CAST(
        (SELECT PopulatedRecords FROM K12Counts) +
        (SELECT PopulatedRecords FROM PsCounts) +
        (SELECT PopulatedRecords FROM AeCounts)
    AS NVARCHAR(MAX))                                               AS Value,
    'PopulatedRecords'                                              AS SubItemLabel,
    NULL                                                            AS Remarks
UNION ALL
-- Distribution - K-12
SELECT
    '{DataElementName}'                                             AS DataElementName,
    'Distribution'                                                  AS CharacteristicType,
    CAST((SELECT PopulatedRecords FROM K12Counts) AS NVARCHAR(MAX)) AS Value,
    'K-12'                                                          AS SubItemLabel,
    NULL                                                            AS Remarks
UNION ALL
-- Distribution - Postsecondary
SELECT
    '{DataElementName}'                                             AS DataElementName,
    'Distribution'                                                  AS CharacteristicType,
    CAST((SELECT PopulatedRecords FROM PsCounts) AS NVARCHAR(MAX))  AS Value,
    'Postsecondary'                                                 AS SubItemLabel,
    NULL                                                            AS Remarks
UNION ALL
-- Distribution - Adult Education
SELECT
    '{DataElementName}'                                             AS DataElementName,
    'Distribution'                                                  AS CharacteristicType,
    CAST((SELECT PopulatedRecords FROM AeCounts) AS NVARCHAR(MAX))  AS Value,
    'Adult Education'                                               AS SubItemLabel,
    NULL                                                            AS Remarks";

    public string AssessmentDescription =>
        "Assesses disability status flag (DisabilityStatusCode, Yes/No) across K-12 enrollment, " +
        "postsecondary course transcripts, and adult education enrollment populations. Reports a " +
        "combined record count and completeness across all three sources, and a per-sector count " +
        "of records with a populated DisabilityStatusCode as a distribution. Related K-12 indicators " +
        "(IdeaStatusId, PrimaryDisabilityTypeId, SecondaryDisabilityTypeId) are not profiled here.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
