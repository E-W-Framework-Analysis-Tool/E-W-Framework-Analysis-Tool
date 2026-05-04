using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles English learner classification date data from RDS.FactK12StudentEnrollments
/// joined to RDS.DimDates via StatusStartDateEnglishLearnerId. The status start date
/// represents when EL status was assigned and is the closest available proxy for
/// classification date in the CEDS DW schema. StatusEndDateEnglishLearnerId
/// (reclassification or exit date) is also available on the fact table but is not
/// the primary classification date signal.
/// </summary>
public class EnglishLearnerClassificationDateCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "English learner classification date";

    public string Query => $@"
WITH Base AS (
    SELECT
        sd.DateValue                                AS StatusStartDate,
        sd.Year                                     AS StatusStartYear
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimDates sd
        ON f.StatusStartDateEnglishLearnerId = sd.DimDateId
),
Counts AS (
    SELECT
        COUNT(*)                                    AS TotalRecords,
        COUNT(CASE
            WHEN StatusStartDate IS NOT NULL
            THEN 1
        END)                                        AS PopulatedRecords
    FROM Base
),
Distribution AS (
    SELECT
        StatusStartYear,
        COUNT(*)                                    AS YearCount
    FROM Base
    WHERE StatusStartDate IS NOT NULL
    GROUP BY StatusStartYear
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
    'Populated = StatusStartDateEnglishLearnerId resolves to non-NULL DateValue in DimDates' AS Remarks
FROM Counts
UNION ALL
-- Distribution - populated classification date records by year
SELECT
    '{DataElementName}'                         AS DataElementName,
    'Distribution'                              AS CharacteristicType,
    CAST(YearCount AS NVARCHAR(MAX))            AS Value,
    CAST(StatusStartYear AS NVARCHAR(MAX))      AS SubItemLabel,
    NULL                                        AS Remarks
FROM Distribution";

    public string AssessmentDescription =>
        "Assesses English learner classification date presence from RDS.FactK12StudentEnrollments " +
        "via StatusStartDateEnglishLearnerId joined to RDS.DimDates. The status start date is the " +
        "closest available proxy for EL classification date in the CEDS DW schema. Reports " +
        "completeness and distribution of populated dates by year. StatusEndDateEnglishLearnerId " +
        "(reclassification/exit date) is also available on the fact table.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
