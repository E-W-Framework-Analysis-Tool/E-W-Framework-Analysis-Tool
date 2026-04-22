namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles disability status data across three populations — K-12, postsecondary,
/// and adult education — from RDS.FactK12StudentEnrollments, RDS.FactPsStudentCourseTranscripts,
/// and RDS.FactAeStudentEnrollments respectively, each joined to RDS.DimDisabilityStatuses.
/// Assesses record count and completeness of DisabilityStatusCode per source.
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
        COUNT(*)                                    AS TotalRecords,
        COUNT(CASE
            WHEN DisabilityStatusCode IS NOT NULL
             AND DisabilityStatusCode <> ''
             -- NOTE: No default sentinel was confirmed in the DimDisabilityStatuses
             -- DDL. 'MISSING' is excluded defensively per the pattern used elsewhere.
             -- Review distinct DisabilityStatusCode values and extend if needed.
             AND DisabilityStatusCode <> 'MISSING'
            THEN 1
        END)                                        AS PopulatedRecords
    FROM K12Base
),
PsCounts AS (
    SELECT
        COUNT(*)                                    AS TotalRecords,
        COUNT(CASE
            WHEN DisabilityStatusCode IS NOT NULL
             AND DisabilityStatusCode <> ''
             AND DisabilityStatusCode <> 'MISSING'
            THEN 1
        END)                                        AS PopulatedRecords
    FROM PsBase
),
AeCounts AS (
    SELECT
        COUNT(*)                                    AS TotalRecords,
        COUNT(CASE
            WHEN DisabilityStatusCode IS NOT NULL
             AND DisabilityStatusCode <> ''
             AND DisabilityStatusCode <> 'MISSING'
            THEN 1
        END)                                        AS PopulatedRecords
    FROM AeBase
)
INSERT INTO #EWFProfilerResults
-- RecordCount - K12
SELECT
    '{DataElementName}'                         AS DataElementName,
    'RecordCount'                               AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    NULL                                        AS SubItemLabel,
    'Source: FactK12StudentEnrollments'         AS Remarks
FROM K12Counts
UNION ALL
-- Completeness - TotalRecords - K12
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    'TotalRecords'                              AS SubItemLabel,
    'Source: FactK12StudentEnrollments'         AS Remarks
FROM K12Counts
UNION ALL
-- Completeness - PopulatedRecords - K12
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))     AS Value,
    'PopulatedRecords'                          AS SubItemLabel,
    'Source: FactK12StudentEnrollments'         AS Remarks
FROM K12Counts
UNION ALL
-- RecordCount - PS
SELECT
    '{DataElementName}'                         AS DataElementName,
    'RecordCount'                               AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    NULL                                        AS SubItemLabel,
    'Source: FactPsStudentCourseTranscripts'    AS Remarks
FROM PsCounts
UNION ALL
-- Completeness - TotalRecords - PS
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    'TotalRecords'                              AS SubItemLabel,
    'Source: FactPsStudentCourseTranscripts'    AS Remarks
FROM PsCounts
UNION ALL
-- Completeness - PopulatedRecords - PS
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))     AS Value,
    'PopulatedRecords'                          AS SubItemLabel,
    'Source: FactPsStudentCourseTranscripts'    AS Remarks
FROM PsCounts
UNION ALL
-- RecordCount - AE
SELECT
    '{DataElementName}'                         AS DataElementName,
    'RecordCount'                               AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    NULL                                        AS SubItemLabel,
    'Source: FactAeStudentEnrollments'          AS Remarks
FROM AeCounts
UNION ALL
-- Completeness - TotalRecords - AE
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    'TotalRecords'                              AS SubItemLabel,
    'Source: FactAeStudentEnrollments'          AS Remarks
FROM AeCounts
UNION ALL
-- Completeness - PopulatedRecords - AE
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))     AS Value,
    'PopulatedRecords'                          AS SubItemLabel,
    'Source: FactAeStudentEnrollments'          AS Remarks
FROM AeCounts";

    public string AssessmentDescription =>
        "Assesses disability status flag (DisabilityStatusCode, Yes/No) across K-12 enrollment, " +
        "postsecondary course transcripts, and adult education enrollment populations. Record count " +
        "and completeness are reported per source via Remarks. Related K-12 indicators " +
        "(IdeaStatusId, PrimaryDisabilityTypeId, SecondaryDisabilityTypeId) are not profiled here.";
}
