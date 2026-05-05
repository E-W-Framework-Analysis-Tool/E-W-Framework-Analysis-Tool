using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles high school diploma type for K-12 students from
/// RDS.FactK12StudentAcademicAwards, joined to RDS.DimK12AcademicAwardStatuses.
/// Assesses record count, completeness of the diploma type field, and distribution
/// of diploma types using the CEDS High School Diploma Type option set
/// (CEDS element 000138).
/// </summary>
public class HighSchoolDiplomaTypeCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "High school diploma type";

    public string Query => $@"
WITH DiplomaBase AS (
    SELECT
        a.HighSchoolDiplomaTypeCode,
        a.HighSchoolDiplomaTypeDescription
    FROM RDS.FactK12StudentAcademicAwards f
    JOIN RDS.DimK12AcademicAwardStatuses a ON f.K12AcademicAwardStatusId = a.DimK12AcademicAwardStatusId
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM DiplomaBase
UNION ALL
-- Completeness - TotalRecords
SELECT
    '{DataElementName}'             AS DataElementName,
    'Completeness'                  AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    'TotalRecords'                  AS SubItemLabel,
    NULL                            AS Remarks
FROM DiplomaBase
UNION ALL
-- Completeness - PopulatedRecords
SELECT
    '{DataElementName}'                                                              AS DataElementName,
    'Completeness'                                                                   AS CharacteristicType,
    CAST(COUNT(CASE WHEN HighSchoolDiplomaTypeCode IS NOT NULL
                     AND HighSchoolDiplomaTypeCode <> ''
                    THEN 1 END) AS NVARCHAR(MAX))                                    AS Value,
    'PopulatedRecords'                                                               AS SubItemLabel,
    NULL                                                                             AS Remarks
FROM DiplomaBase
UNION ALL
-- Distribution - by diploma type
SELECT
    '{DataElementName}'                          AS DataElementName,
    'Distribution'                               AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))              AS Value,
    HighSchoolDiplomaTypeDescription             AS SubItemLabel,
    HighSchoolDiplomaTypeCode                    AS Remarks
FROM DiplomaBase
GROUP BY HighSchoolDiplomaTypeDescription, HighSchoolDiplomaTypeCode";

    public string AssessmentDescription =>
        "Assesses high school diploma type from RDS.FactK12StudentAcademicAwards joined to " +
        "RDS.DimK12AcademicAwardStatuses. Reports total record count, completeness of the " +
        "HighSchoolDiplomaTypeCode field, and distribution across diploma type values per " +
        "CEDS element 000138 (High School Diploma Type). Diploma type code is populated via " +
        "state-specific ETL mapping through Staging.SourceSystemReferenceData.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
