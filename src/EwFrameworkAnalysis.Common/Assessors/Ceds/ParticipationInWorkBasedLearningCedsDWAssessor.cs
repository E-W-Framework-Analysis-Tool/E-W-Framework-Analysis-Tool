using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles participation in work-based learning for K-12 students from
/// RDS.FactK12StudentCourseSections, joined to RDS.DimWorkBasedLearningStatuses and
/// RDS.DimK12CourseSectionEnrollmentStatuses. Population is scoped to active and completed
/// enrollments with a valid entry date. Assesses record count, completeness of the
/// opportunity type, and distribution across work-based learning opportunity types.
/// </summary>
public class ParticipationInWorkBasedLearningCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Participation in work-based learning";
    public string Query => $@"
WITH Base AS (
    SELECT
        d.WorkBasedLearningOpportunityTypeCode,
        d.WorkBasedLearningOpportunityTypeDescription
    FROM RDS.FactK12StudentCourseSections f
    JOIN RDS.DimWorkBasedLearningStatuses d
        ON f.WorkBasedLearningStatusId = d.DimWorkBasedLearningStatusId
    JOIN RDS.DimK12CourseSectionEnrollmentStatuses d2
        ON f.K12CourseSectionEnrollmentStatusId = d2.DimK12CourseSectionEnrollmentStatusId
    WHERE d2.CourseSectionEnrollmentStatusTypeCode IN ('Registered', 'Enrolled', 'Completed')
      AND f.EnrollmentEntryDateId IS NOT NULL
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
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode IS NOT NULL
                     AND WorkBasedLearningOpportunityTypeCode <> '' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'PopulatedRecords'  AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Apprenticeship
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'Apprenticeship' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Apprenticeship'    AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Clinical work experience
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'ClinicalWork' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Clinical work experience' AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Cooperative education
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'CooperativeEducation' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Cooperative education' AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Entrepreneurship
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'Entrepreneurship' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Entrepreneurship'  AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Job shadowing
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'JobShadowing' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Job shadowing'     AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Mentorship
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'Mentorship' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Mentorship'        AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Non-Paid Internship
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'NonPaidInternship' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Non-Paid Internship' AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - On-the-Job
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'OnTheJob' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'On-the-Job'        AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Other
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'Other' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Other'             AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Paid internship
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'PaidInternship' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Paid internship'   AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - School-Based Enterprise
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'SchoolBasedEnterprise' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'School-Based Enterprise' AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Service learning
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'ServiceLearning' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Service learning'  AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Simulated Worksite
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'SimulatedWorksite' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Simulated Worksite' AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Supervised agricultural experience
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'SupervisedAgricultural' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Supervised agricultural experience' AS SubItemLabel,
    NULL                AS Remarks
FROM Base
UNION ALL
-- Distribution - Unpaid internship
SELECT
    '{DataElementName}' AS DataElementName,
    'Distribution'      AS CharacteristicType,
    CAST(COUNT(CASE WHEN WorkBasedLearningOpportunityTypeCode = 'UnpaidInternship' THEN 1 END) AS NVARCHAR(MAX)) AS Value,
    'Unpaid internship' AS SubItemLabel,
    NULL                AS Remarks
FROM Base
";
    public string AssessmentDescription =>
        "Assesses record count, completeness of work-based learning opportunity type, and " +
        "distribution across all 15 CEDS work-based learning opportunity types for K-12 students " +
        "with active or completed course section enrollments, from RDS.FactK12StudentCourseSections " +
        "joined to RDS.DimWorkBasedLearningStatuses and RDS.DimK12CourseSectionEnrollmentStatuses.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
