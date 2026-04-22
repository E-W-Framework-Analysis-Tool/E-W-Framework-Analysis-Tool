namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles English learner status data from RDS.FactK12StudentEnrollments joined
/// to RDS.DimEnglishLearnerStatuses. Assesses completeness of both
/// EnglishLearnerStatusCode (general EL status) and PerkinsEnglishLearnerStatusCode
/// (Perkins program EL status). Both fields are Yes/No flags. No filter is applied
/// to EL-flagged records — all enrollment records are included to assess data presence.
/// </summary>
public class EnglishLearnerStatusCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "English learner status";

    public string Query => $@"
WITH Base AS (
    SELECT
        e.EnglishLearnerStatusCode,
        e.PerkinsEnglishLearnerStatusCode
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimEnglishLearnerStatuses e
        ON f.EnglishLearnerStatusId = e.DimEnglishLearnerStatusId
),
ELCounts AS (
    SELECT
        COUNT(*)                                    AS TotalRecords,
        COUNT(CASE
            WHEN EnglishLearnerStatusCode IS NOT NULL
             AND EnglishLearnerStatusCode <> ''
             AND EnglishLearnerStatusCode <> 'MISSING'
            THEN 1
        END)                                        AS PopulatedRecords
    FROM Base
),
PerkinsCounts AS (
    SELECT
        COUNT(*)                                    AS TotalRecords,
        COUNT(CASE
            WHEN PerkinsEnglishLearnerStatusCode IS NOT NULL
             AND PerkinsEnglishLearnerStatusCode <> ''
             AND PerkinsEnglishLearnerStatusCode <> 'MISSING'
            THEN 1
        END)                                        AS PopulatedRecords
    FROM Base
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'                         AS DataElementName,
   'RecordCount'                               AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    NULL                                        AS SubItemLabel,
    NULL                                        AS Remarks
FROM ELCounts
UNION ALL
-- Completeness - TotalRecords (EnglishLearnerStatusCode)
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    'TotalRecords'                              AS SubItemLabel,
    'EnglishLearnerStatusCode'                  AS Remarks
FROM ELCounts
UNION ALL
-- Completeness - PopulatedRecords (EnglishLearnerStatusCode)
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))     AS Value,
    'PopulatedRecords'                          AS SubItemLabel,
    'EnglishLearnerStatusCode'                  AS Remarks
FROM ELCounts
UNION ALL
-- Completeness - TotalRecords (PerkinsEnglishLearnerStatusCode)
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    'TotalRecords'                              AS SubItemLabel,
    'PerkinsEnglishLearnerStatusCode'           AS Remarks
FROM PerkinsCounts
UNION ALL
-- Completeness - PopulatedRecords (PerkinsEnglishLearnerStatusCode)
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))     AS Value,
    'PopulatedRecords'                          AS SubItemLabel,
    'PerkinsEnglishLearnerStatusCode'           AS Remarks
FROM PerkinsCounts";

    public string AssessmentDescription =>
        "Assesses English learner status data from RDS.FactK12StudentEnrollments. Profiles " +
        "completeness of both EnglishLearnerStatusCode (general EL status) and " +
        "PerkinsEnglishLearnerStatusCode (Perkins program EL status), each a Yes/No flag. " +
        "No filter to EL-flagged records is applied — all enrollment records are included.";
}
