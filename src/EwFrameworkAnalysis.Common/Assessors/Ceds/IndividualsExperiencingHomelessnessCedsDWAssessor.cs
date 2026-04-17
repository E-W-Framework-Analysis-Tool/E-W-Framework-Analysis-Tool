namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles homelessness status as a disaggregate across K-12, postsecondary, and
/// adult education populations from RDS.FactK12StudentEnrollments,
/// RDS.FactPsStudentCourseTranscripts, and RDS.FactAeStudentEnrollments respectively,
/// each joined to RDS.DimHomelessnessStatuses on HomelessnessStatusId. Pre-K and
/// Workforce sectors are represented with zero counts as no supporting schema was
/// identified. Assesses record count, completeness, and Yes/No distribution of
/// homelessness status per sector.
/// Note: The original DE query filtered to HomelessnessStatusCode = 'Yes', counting
/// only homeless individuals. This assessor removes that filter so RecordCount and
/// Completeness reflect the full population; the Yes/No split is surfaced via
/// Distribution instead.
/// </summary>
public class IndividualsExperiencingHomelessnessCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Individuals experiencing homelessness";

    public string Query => $@"
WITH K12Base AS (
    SELECT d.HomelessnessStatusCode
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimHomelessnessStatuses d
        ON f.HomelessnessStatusId = d.DimHomelessnessStatusId
),
PsBase AS (
    SELECT d.HomelessnessStatusCode
    FROM RDS.FactPsStudentCourseTranscripts f
    JOIN RDS.DimHomelessnessStatuses d
        ON f.HomelessnessStatusId = d.DimHomelessnessStatusId
),
AeBase AS (
    SELECT d.HomelessnessStatusCode
    FROM RDS.FactAeStudentEnrollments f
    JOIN RDS.DimHomelessnessStatuses d
        ON f.HomelessnessStatusId = d.DimHomelessnessStatusId
),
K12Counts AS (
    SELECT
        COUNT(*) AS TotalRecords,
        COUNT(CASE
            WHEN HomelessnessStatusCode IS NOT NULL
             AND HomelessnessStatusCode <> ''
             AND HomelessnessStatusCode <> 'MISSING'
            THEN 1 END) AS PopulatedRecords
    FROM K12Base
),
PsCounts AS (
    SELECT
        COUNT(*) AS TotalRecords,
        COUNT(CASE
            WHEN HomelessnessStatusCode IS NOT NULL
             AND HomelessnessStatusCode <> ''
             AND HomelessnessStatusCode <> 'MISSING'
            THEN 1 END) AS PopulatedRecords
    FROM PsBase
),
AeCounts AS (
    SELECT
        COUNT(*) AS TotalRecords,
        COUNT(CASE
            WHEN HomelessnessStatusCode IS NOT NULL
             AND HomelessnessStatusCode <> ''
             AND HomelessnessStatusCode <> 'MISSING'
            THEN 1 END) AS PopulatedRecords
    FROM AeBase
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
-- Distribution - K12 by homelessness status
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Distribution'                                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                     AS Value,
    HomelessnessStatusCode                              AS SubItemLabel,
    'Source: FactK12StudentEnrollments'                 AS Remarks
FROM K12Base
GROUP BY HomelessnessStatusCode
UNION ALL
-- RecordCount - PS
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'RecordCount'                                       AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                 AS Value,
    NULL                                                AS SubItemLabel,
    'Source: FactPsStudentCourseTranscripts'            AS Remarks
FROM PsCounts
UNION ALL
-- Completeness - TotalRecords - PS
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Completeness'                                      AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                 AS Value,
    'TotalRecords'                                      AS SubItemLabel,
    'Source: FactPsStudentCourseTranscripts'            AS Remarks
FROM PsCounts
UNION ALL
-- Completeness - PopulatedRecords - PS
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Completeness'                                      AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))             AS Value,
    'PopulatedRecords'                                  AS SubItemLabel,
    'Source: FactPsStudentCourseTranscripts'            AS Remarks
FROM PsCounts
UNION ALL
-- Distribution - PS by homelessness status
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Distribution'                                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                     AS Value,
    HomelessnessStatusCode                              AS SubItemLabel,
    'Source: FactPsStudentCourseTranscripts'            AS Remarks
FROM PsBase
GROUP BY HomelessnessStatusCode
UNION ALL
-- RecordCount - AE
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'RecordCount'                                       AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                 AS Value,
    NULL                                                AS SubItemLabel,
    'Source: FactAeStudentEnrollments'                  AS Remarks
FROM AeCounts
UNION ALL
-- Completeness - TotalRecords - AE
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Completeness'                                      AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))                 AS Value,
    'TotalRecords'                                      AS SubItemLabel,
    'Source: FactAeStudentEnrollments'                  AS Remarks
FROM AeCounts
UNION ALL
-- Completeness - PopulatedRecords - AE
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Completeness'                                      AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))             AS Value,
    'PopulatedRecords'                                  AS SubItemLabel,
    'Source: FactAeStudentEnrollments'                  AS Remarks
FROM AeCounts
UNION ALL
-- Distribution - AE by homelessness status
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Distribution'                                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                     AS Value,
    HomelessnessStatusCode                              AS SubItemLabel,
    'Source: FactAeStudentEnrollments'                  AS Remarks
FROM AeBase
GROUP BY HomelessnessStatusCode
UNION ALL
-- RecordCount - PreK (no supporting schema identified; emitted as zero)
-- NOTE: No PreK fact table with HomelessnessStatusId was found in the known schema.
-- Replace this stub if FactElChildEnrollments or similar becomes available.
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'RecordCount'                                       AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX))                            AS Value,
    NULL                                                AS SubItemLabel,
    'Source: PreK (not available in schema)'            AS Remarks
UNION ALL
-- RecordCount - Workforce (no supporting schema identified; emitted as zero)
-- NOTE: No workforce fact table with HomelessnessStatusId was found in the known schema.
-- Replace this stub if FactWfProgramParticipation or similar becomes available.
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'RecordCount'                                       AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX))                            AS Value,
    NULL                                                AS SubItemLabel,
    'Source: Workforce (not available in schema)'       AS Remarks";

    public string AssessmentDescription =>
        "Assesses homelessness status as a disaggregate across K-12, postsecondary, and adult " +
        "education populations joined to RDS.DimHomelessnessStatuses. Reports record count, " +
        "completeness of the homelessness status code, and Yes/No distribution per sector. " +
        "Pre-K and Workforce sectors are represented with zero record counts pending schema " +
        "identification.";
}
