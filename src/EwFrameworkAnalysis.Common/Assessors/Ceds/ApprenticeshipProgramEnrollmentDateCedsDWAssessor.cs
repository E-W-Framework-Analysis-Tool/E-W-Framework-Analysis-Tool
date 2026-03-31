namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles apprenticeship program enrollment date from RDS.FactK12StudentCourseSections,
/// joined to RDS.DimWorkBasedLearningStatuses (WorkBasedLearningOpportunityTypeCode = 'Apprenticeship').
/// Assesses the count of apprenticeship enrollment records with an enrollment entry date present.
/// </summary>
public class ApprenticeshipProgramEnrollmentDateCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Apprenticeship program enrollment date";

    public string Query => $@"
WITH ApprenticeshipBase AS (
    SELECT
        f.FactK12StudentCourseSectionId
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimWorkBasedLearningStatuses d ON f.WorkBasedLearningStatusId = d.DimWorkBasedLearningStatusId
    WHERE d.WorkBasedLearningOpportunityTypeCode = 'Apprenticeship'
      AND f.EnrollmentEntryDateId IS NOT NULL
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM ApprenticeshipBase";

    public string AssessmentDescription =>
        "Assesses apprenticeship program enrollment date by counting records where " +
        "WorkBasedLearningOpportunityTypeCode = 'Apprenticeship' and EnrollmentEntryDateId " +
        "is non-null. The record count itself is the presence signal for this data element.";
}
