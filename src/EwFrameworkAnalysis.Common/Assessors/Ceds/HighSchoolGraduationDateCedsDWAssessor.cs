using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles high school graduation date from two sources:
/// (1) RDS.FactK12GraduationCohorts filtered to graduated cohort statuses
///     (EdFactsCohortGraduationStatusCode IN ('COHYES', 'COHALTDPL'))
/// (2) RDS.FactK12StudentAcademicAwards via DiplomaOrCredentialAwardDateId
/// Each source is reported as a separate RecordCount row in Remarks for
/// visibility into which source has data in the target state's warehouse.
/// </summary>
public class HighSchoolGraduationDateCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "High school graduation date";

    public string Query => $@"
WITH GraduationCohortDates AS (
    SELECT
        f.DiplomaOrCredentialAwardDateId
    FROM RDS.FactK12GraduationCohorts f
    JOIN RDS.DimCohortStatuses d ON f.CohortStatusId = d.DimCohortStatusId
    WHERE d.EdFactsCohortGraduationStatusCode IN ('COHYES', 'COHALTDPL')
      AND f.DiplomaOrCredentialAwardDateId IS NOT NULL
),
AcademicAwardDates AS (
    SELECT
        f.DiplomaOrCredentialAwardDateId
    FROM RDS.FactK12StudentAcademicAwards f
    WHERE f.DiplomaOrCredentialAwardDateId IS NOT NULL
)
INSERT INTO #EWFProfilerResults
-- RecordCount - FactK12GraduationCohorts source
SELECT
    '{DataElementName}'                         AS DataElementName,
    'RecordCount'                               AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))             AS Value,
    NULL                                        AS SubItemLabel,
    'Source: FactK12GraduationCohorts'          AS Remarks
FROM GraduationCohortDates
UNION ALL
-- RecordCount - FactK12StudentAcademicAwards source
SELECT
    '{DataElementName}'                         AS DataElementName,
    'RecordCount'                               AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX))             AS Value,
    NULL                                        AS SubItemLabel,
    'Source: FactK12StudentAcademicAwards'      AS Remarks
FROM AcademicAwardDates";

    public string AssessmentDescription =>
        "Assesses high school graduation date presence from two sources: " +
        "FactK12GraduationCohorts (filtered to COHYES and COHALTDPL cohort statuses) and " +
        "FactK12StudentAcademicAwards. Each source is reported as a separate RecordCount row " +
        "so gaps between sources are visible. A non-zero count in either row indicates " +
        "graduation date data is present in that source.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
