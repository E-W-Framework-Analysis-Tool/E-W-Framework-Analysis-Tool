namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles home language data for K-12 students from RDS.FactK12StudentEnrollments
/// joined to RDS.DimLanguages on LanguageHomeId. Excludes the default sentinel row
/// (DimLanguageId = -1). Assesses record count, completeness of the home language
/// field, and distribution across ISO 639-2 language descriptions.
/// </summary>
public class HomeLanguageCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Home language";

    public string Query => $@"
WITH HomeLanguageBase AS (
    SELECT
        d.Iso6392LanguageCodeCode,
        d.Iso6392LanguageCodeDescription
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimLanguages d ON f.LanguageHomeId = d.DimLanguageId
    WHERE d.DimLanguageId <> -1
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM HomeLanguageBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM HomeLanguageBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(CASE
        WHEN Iso6392LanguageCodeCode IS NOT NULL
         AND Iso6392LanguageCodeCode <> ''
         AND Iso6392LanguageCodeCode <> 'MISSING'
        THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'              AS SubItemLabel,
    NULL                            AS Remarks
FROM HomeLanguageBase
UNION ALL
-- Distribution - one row per ISO 639-2 language description
SELECT
    '{DataElementName}'                  AS DataElementName,
    'Distribution'                       AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))      AS Value,
    Iso6392LanguageCodeDescription       AS SubItemLabel,
    NULL                                 AS Remarks
FROM HomeLanguageBase
GROUP BY Iso6392LanguageCodeDescription";

    public string AssessmentDescription =>
        "Assesses home language data for K-12 students from RDS.FactK12StudentEnrollments " +
        "joined to RDS.DimLanguages (on LanguageHomeId), excluding the default sentinel row " +
        "(DimLanguageId = -1). Reports total record count, completeness of the ISO 639-2 " +
        "language code field, and distribution of enrollment records across language descriptions.";
}
