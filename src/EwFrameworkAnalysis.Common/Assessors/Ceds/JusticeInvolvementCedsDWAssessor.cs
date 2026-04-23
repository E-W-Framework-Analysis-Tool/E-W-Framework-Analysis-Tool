namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles justice involvement for K-12 students from RDS.FactK12StudentDisciplines
/// joined to RDS.DimDisciplineStatuses, filtered to disciplinary action codes that
/// indicate justice system contact: juvenile justice referral (03088), law enforcement
/// referral (03089), placed in juvenile detention center (75000), and school-related
/// arrest (75001). Postsecondary and Workforce sectors are represented with zero counts
/// as no supporting schema was identified. Assesses record count and distribution by
/// disciplinary action type. No Completeness characteristic is produced as the source
/// is pre-filtered to justice-contact records only.
/// Note: The original DE query filtered only on 'Juvenile justice referral' by
/// description string. This assessor broadens the filter to all four justice-contact
/// codes and filters on code rather than description for reliability.
/// </summary>
public class JusticeInvolvementCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Justice involvement";

    public string Query => $@"
WITH JusticeBase AS (
    SELECT
        d.DisciplinaryActionTakenCode,
        d.DisciplinaryActionTakenDescription
    FROM RDS.FactK12StudentDisciplines f
    JOIN RDS.DimDisciplineStatuses d
        ON f.DisciplineStatusId = d.DimDisciplineStatusId
    WHERE d.DisciplinaryActionTakenCode IN (
        '03088',    -- Juvenile justice referral
        '03089',    -- Law enforcement referral
        '75000',    -- Placed in juvenile detention center
        '75001'     -- School-related arrest
    )
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'RecordCount'                                                       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                                     AS Value,
    NULL                                                                AS SubItemLabel,
    NULL                                                                AS Remarks
FROM JusticeBase
UNION ALL
-- Distribution - by disciplinary action type
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'Distribution'                                                      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))                                     AS Value,
    COALESCE(
        NULLIF(LTRIM(RTRIM(DisciplinaryActionTakenDescription)), ''),
        NULLIF(LTRIM(RTRIM(DisciplinaryActionTakenCode)), ''),
        '(Unknown)'
    )                                                                   AS SubItemLabel,
    NULL                                                                AS Remarks
FROM JusticeBase
GROUP BY
    COALESCE(
        NULLIF(LTRIM(RTRIM(DisciplinaryActionTakenDescription)), ''),
        NULLIF(LTRIM(RTRIM(DisciplinaryActionTakenCode)), ''),
        '(Unknown)'
    )
UNION ALL
-- Distribution - Postsecondary (no supporting schema identified)
-- NOTE: No PS fact table with a justice involvement indicator was found in the
-- known schema. Replace this stub if a suitable PS fact table becomes available.
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'Distribution'                                                      AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX))                                            AS Value,
    'Postsecondary'                                                     AS SubItemLabel,
    'No source table available for Postsecondary sector'                AS Remarks
UNION ALL
-- Distribution - Workforce (no supporting schema identified)
-- NOTE: No workforce fact table with a justice involvement indicator was found in the
-- known schema. Replace this stub if FactWfProgramParticipation or similar becomes available.
SELECT
    '{DataElementName}'                                                 AS DataElementName,
    'Distribution'                                                      AS CharacteristicType,
    CAST(0 AS NVARCHAR(MAX))                                            AS Value,
    'Workforce'                                                         AS SubItemLabel,
    'No source table available for Workforce sector'                    AS Remarks";

    public string AssessmentDescription =>
        "Assesses justice involvement for K-12 students from RDS.FactK12StudentDisciplines joined " +
        "to RDS.DimDisciplineStatuses, filtered to disciplinary actions indicating justice system " +
        "contact: juvenile justice referral (03088), law enforcement referral (03089), placed in " +
        "juvenile detention center (75000), and school-related arrest (75001). Reports record count " +
        "and distribution by action type. Postsecondary and Workforce sectors emit zero pending " +
        "schema identification.";
}
