using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles postsecondary enrollment date from RDS.FactPsStudentEnrollments.
/// Both EnrollmentEntryDateId and EntryDateIntoPostSecondaryId are non-nullable,
/// so completeness is guaranteed by schema constraint. RecordCount alone is reported.
/// </summary>
public class PostsecondaryEnrollmentDateCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Postsecondary enrollment date";
    public string Query => $@"
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM RDS.FactPsStudentEnrollments
";
    public string AssessmentDescription =>
        "Assesses postsecondary enrollment date from RDS.FactPsStudentEnrollments. " +
        "EnrollmentEntryDateId and EntryDateIntoPostSecondaryId are both non-nullable, " +
        "so completeness is schema-guaranteed. RecordCount reflects the total number of " +
        "postsecondary enrollment records present.";

    public string MinVersion => CedsDwVersions.V13;
    public string? MaxVersion => null;
}
