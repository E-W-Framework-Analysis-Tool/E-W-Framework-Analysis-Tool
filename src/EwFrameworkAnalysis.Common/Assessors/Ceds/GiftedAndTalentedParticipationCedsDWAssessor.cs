using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles gifted and talented program participation from RDS.FactK12ProgramParticipations
/// filtered to ProgramTypeCode '04930' (Gifted and talented program).
/// Assesses record count and completeness of ProgramParticipationStartDateId.
/// </summary>
public class GiftedAndTalentedParticipationCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Gifted and talented participation";

    public string Query => $@"
WITH GiftedBase AS (
    SELECT
        p.ProgramParticipationStartDateId
    FROM RDS.FactK12ProgramParticipations p
    JOIN RDS.DimK12ProgramTypes pt ON p.K12ProgramTypeId = pt.DimK12ProgramTypeId
    WHERE pt.ProgramTypeCode = '04930'
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM GiftedBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM GiftedBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                                    AS DataElementName,
    'Completeness'                                         AS CharacteristicType,
    CAST(COUNT(ProgramParticipationStartDateId) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'                                     AS SubItemLabel,
    NULL                                                   AS Remarks
FROM GiftedBase
";

    public string AssessmentDescription =>
        "Assesses gifted and talented program participation from RDS.FactK12ProgramParticipations " +
        "filtered to ProgramTypeCode '04930'. Reports total participation record count and " +
        "completeness of ProgramParticipationStartDateId. Exit date is intentionally excluded " +
        "from completeness as not all participants will have exited the program.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
