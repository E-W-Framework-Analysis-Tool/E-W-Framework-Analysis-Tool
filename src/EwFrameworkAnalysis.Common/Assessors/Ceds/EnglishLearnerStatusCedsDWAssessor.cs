using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles English learner status data from RDS.FactK12StudentEnrollments joined
/// to RDS.DimEnglishLearnerStatuses. Assesses completeness of either EL status field
/// (EnglishLearnerStatusCode or PerkinsEnglishLearnerStatusCode) combined, with a
/// per-field populated count as a distribution. Both fields are Yes/No flags. No filter
/// is applied to EL-flagged records — all enrollment records are included to assess
/// data presence.
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
Counts AS (
    SELECT
        COUNT(*)                                    AS TotalRecords,
        COUNT(CASE
            WHEN (EnglishLearnerStatusCode IS NOT NULL
                  AND EnglishLearnerStatusCode <> ''
                  AND EnglishLearnerStatusCode <> 'MISSING')
              OR (PerkinsEnglishLearnerStatusCode IS NOT NULL
                  AND PerkinsEnglishLearnerStatusCode <> ''
                  AND PerkinsEnglishLearnerStatusCode <> 'MISSING')
            THEN 1
        END)                                        AS PopulatedRecords,
        COUNT(CASE
            WHEN EnglishLearnerStatusCode IS NOT NULL
             AND EnglishLearnerStatusCode <> ''
             AND EnglishLearnerStatusCode <> 'MISSING'
            THEN 1
        END)                                        AS ELPopulated,
        COUNT(CASE
            WHEN PerkinsEnglishLearnerStatusCode IS NOT NULL
             AND PerkinsEnglishLearnerStatusCode <> ''
             AND PerkinsEnglishLearnerStatusCode <> 'MISSING'
            THEN 1
        END)                                        AS PerkinsPopulated
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
FROM Counts
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(TotalRecords AS NVARCHAR(MAX))         AS Value,
    'TotalRecords'                              AS SubItemLabel,
    NULL                                        AS Remarks
FROM Counts
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Completeness'                              AS CharacteristicType,
    CAST(PopulatedRecords AS NVARCHAR(MAX))     AS Value,
    'PopulatedRecords'                          AS SubItemLabel,
    NULL                                        AS Remarks
FROM Counts
UNION ALL
-- Distribution - EnglishLearnerStatusCode
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Distribution'                              AS CharacteristicType,
    CAST(ELPopulated AS NVARCHAR(MAX))          AS Value,
    'EnglishLearnerStatusCode'                  AS SubItemLabel,
    NULL                                        AS Remarks
FROM Counts
UNION ALL
-- Distribution - PerkinsEnglishLearnerStatusCode
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Distribution'                              AS CharacteristicType,
    CAST(PerkinsPopulated AS NVARCHAR(MAX))     AS Value,
    'PerkinsEnglishLearnerStatusCode'           AS SubItemLabel,
    NULL                                        AS Remarks
FROM Counts";

    public string AssessmentDescription =>
        "Assesses English learner status data from RDS.FactK12StudentEnrollments. Reports a " +
        "combined completeness across both EnglishLearnerStatusCode (general EL status) and " +
        "PerkinsEnglishLearnerStatusCode (Perkins program EL status) — a record is considered " +
        "populated if either field is present. Per-field populated counts are reported as a " +
        "distribution. Both fields are Yes/No flags. No filter to EL-flagged records is applied " +
        "— all enrollment records are included.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
