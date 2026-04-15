namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles individual or family military status as a disaggregate across K-12,
/// postsecondary, and adult education populations from RDS.FactK12StudentEnrollments,
/// RDS.FactPsStudentCourseTranscripts, and RDS.FactAeStudentEnrollments respectively,
/// each joined to RDS.DimMilitaryStatuses. Pre-K and Workforce sectors are represented
/// with zero counts as no supporting schema was identified. Assesses record count,
/// completeness, and distribution by military connected student indicator per sector.
/// Note: FactPsStudentCourseTranscripts contains a typo in the FK column name
/// (MilitartyStatusId — extra 't'); the query uses the actual column name as-is.
/// </summary>
public class IndividualOrFamilyMilitaryStatusCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Individual or family military status";

    public string Query => $@"
WITH K12Base AS (
    SELECT
        d.MilitaryConnectedStudentIndicatorCode,
        d.MilitaryConnectedStudentIndicatorDescription
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimMilitaryStatuses d
        ON f.MilitaryStatusId = d.DimMilitaryStatusId
),
PsBase AS (
    SELECT
        d.MilitaryConnectedStudentIndicatorCode,
        d.MilitaryConnectedStudentIndicatorDescription
    FROM RDS.FactPsStudentCourseTranscripts f
    JOIN RDS.DimMilitaryStatuses d
        ON f.MilitartyStatusId = d.DimMilitaryStatusId
),
AeBase AS (
    SELECT
        d.MilitaryConnectedStudentIndicatorCode,
        d.MilitaryConnectedStudentIndicatorDescription
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
-- Distribution - K12 by military connected student indicator
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Distribution'                                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                     AS Value,
    MilitaryConnectedStudentIndicatorDescription        AS SubItemLabel,
    'Source: FactK12StudentEnrollments'                 AS Remarks
FROM K12Base
GROUP BY MilitaryConnectedStudentIndicatorDescription
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
-- Distribution - PS by military connected student indicator
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Distribution'                                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                     AS Value,
    MilitaryConnectedStudentIndicatorDescription        AS SubItemLabel,
    'Source: FactPsStudentCourseTranscripts'            AS Remarks
FROM PsBase
GROUP BY MilitaryConnectedStudentIndicatorDescription
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
-- Distribution - AE by military connected student indicator
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'Distribution'                                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                     AS Value,
    MilitaryConnectedStudentIndicatorDescription        AS SubItemLabel,
    'Source: FactAeStudentEnrollments'                  AS Remarks
FROM AeBase
GROUP BY MilitaryConnectedStudentIndicatorDescription
UNION ALL
-- RecordCount - PreK (no supporting schema identified; emitted as zero)
-- NOTE: No PreK fact table with MilitaryStatusId was found in the known schema.
-- Replace this stub if FactElChildEnrollments or similar becomes available.
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'RecordCount'                                       AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX))                            AS Value,
    NULL                                                AS SubItemLabel,
    'Source: PreK (not available in schema)'            AS Remarks
UNION ALL
-- RecordCount - Workforce (no supporting schema identified; emitted as zero)
-- NOTE: No workforce fact table with MilitaryStatusId was found in the known schema.
-- Replace this stub if FactWfProgramParticipation or similar becomes available.
SELECT
    '{DataElementName}'                                 AS DataElementName,
    'RecordCount'                                       AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX))                            AS Value,
    NULL                                                AS SubItemLabel,
    'Source: Workforce (not available in schema)'       AS Remarks";

    public string AssessmentDescription =>
        "Assesses individual or family military status as a disaggregate across K-12, postsecondary, " +
        "and adult education populations joined to RDS.DimMilitaryStatuses. Reports record count, " +
        "completeness of the military connected student indicator code, and distribution by military " +
        "connected student indicator description per sector. Pre-K and Workforce sectors are represented " +
        "with zero record counts pending schema identification. Note: the PS fact table contains a typo " +
        "in the military status FK column name (MilitartyStatusId).";
}
