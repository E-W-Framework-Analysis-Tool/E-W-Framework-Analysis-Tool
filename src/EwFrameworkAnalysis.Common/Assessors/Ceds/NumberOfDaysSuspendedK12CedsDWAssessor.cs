using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles the number of days suspended for K-12 students from RDS.FactK12StudentDisciplines,
/// scoped to records with a suspension-type DisciplinaryActionTakenCode.
/// Assesses record count, completeness of DurationOfDisciplinaryAction, numerical range, and
/// distribution across suspension consequence types.
/// </summary>
public class NumberOfDaysSuspendedK12CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Number of days suspended (K-12)";
    public string Query => $@"
WITH Base AS (
    SELECT
        fact.DurationOfDisciplinaryAction,
        discStat.DisciplinaryActionTakenDescription
    FROM RDS.FactK12StudentDisciplines fact
    JOIN RDS.DimDisciplineStatuses discStat
        ON fact.DisciplineStatusId = discStat.DimDisciplineStatusId
    WHERE discStat.DisciplinaryActionTakenCode IN (
        '03071', -- Bus suspension
        '03099', -- Suspension after school
        '03100', -- Suspension, in-school
        '03101', -- Suspension, out-of-school, with services
        '03102', -- Suspension, out-of-school, without services
        '03154', -- Suspension, out of school, greater than 10 consecutive school days
        '03155'  -- Suspension, out of school, separate days cumulating to more than 10 school days
    )
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount'       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'      AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}' AS DataElementName,
    'Completeness'      AS CharacteristicType,
    CAST(COUNT(DurationOfDisciplinaryAction) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- NumericalRange - Minimum
SELECT
    '{DataElementName}' AS DataElementName,
    'NumericalRange'    AS CharacteristicType,
    CAST(MIN(DurationOfDisciplinaryAction) AS NVARCHAR(MAX)) AS Value,
    'Minimum'           AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- NumericalRange - Maximum
SELECT
    '{DataElementName}' AS DataElementName,
    'NumericalRange'    AS CharacteristicType,
    CAST(MAX(DurationOfDisciplinaryAction) AS NVARCHAR(MAX)) AS Value,
    'Maximum'           AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Bus suspension
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN DisciplinaryActionTakenDescription = 'Bus suspension' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Bus suspension'    AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Suspension after school
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN DisciplinaryActionTakenDescription = 'Suspension after school' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Suspension after school' AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Suspension, in-school
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN DisciplinaryActionTakenDescription = 'Suspension, in-school' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Suspension, in-school' AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Suspension, out-of-school, with services
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN DisciplinaryActionTakenDescription = 'Suspension, out-of-school, with services' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Suspension, out-of-school, with services' AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Suspension, out-of-school, without services
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN DisciplinaryActionTakenDescription = 'Suspension, out-of-school, without services' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Suspension, out-of-school, without services' AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Suspension, out of school, greater than 10 consecutive school days
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN DisciplinaryActionTakenDescription = 'Suspension, out of school, greater than 10 consecutive school days' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Suspension, out of school, greater than 10 consecutive school days' AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Suspension, out of school, separate days cumulating to more than 10 school days
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN DisciplinaryActionTakenDescription = 'Suspension, out of school, separate days cumulating to more than 10 school days' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Suspension, out of school, separate days cumulating to more than 10 school days' AS SubItemLabel,
    NULL                AS Remarks
FROM Base
";
    public string AssessmentDescription =>
        "Assesses record count, completeness, numerical range, and distribution by suspension type " +
        "for K-12 student discipline records with a suspension-type consequence, " +
        "from RDS.FactK12StudentDisciplines joined to RDS.DimDisciplineStatuses.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
