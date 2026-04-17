namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles restraint and seclusion for discipline (K-12) from RDS.FactK12StudentDisciplines,
/// filtering to Mechanical Restraint (13357), Physical Restraint (13358), and Seclusion (13359).
/// The RDS schema does not distinguish between discipline and safety contexts for these actions;
/// all restraint and seclusion records in this table are disciplinary in nature.
/// </summary>
public class RestraintAndSeclusionForDisciplineK12CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Restraint and seclusion for discipline (K-12)";

    public string Query => $@"
INSERT INTO #EWFProfilerResults
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM RDS.FactK12StudentDisciplines fact
JOIN RDS.DimDisciplineStatuses discStat
    ON fact.DisciplineStatusId = discStat.DimDisciplineStatusId
WHERE discStat.DisciplinaryActionTakenCode IN
    ('13357' -- Mechanical Restraint
    ,'13358' -- Physical Restraint
    ,'13359' -- Seclusion
    )
";

    public string AssessmentDescription =>
        "Counts K-12 discipline records where the disciplinary action taken is Mechanical Restraint, " +
        "Physical Restraint, or Seclusion, from RDS.FactK12StudentDisciplines. The RDS schema does not " +
        "distinguish between discipline and safety contexts; all restraint and seclusion records in this " +
        "table are disciplinary in nature, so this assessor and the safety assessor return the same result.";
}
