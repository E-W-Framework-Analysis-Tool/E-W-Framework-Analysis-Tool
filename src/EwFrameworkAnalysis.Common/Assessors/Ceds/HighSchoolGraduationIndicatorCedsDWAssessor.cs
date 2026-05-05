using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

/// <summary>
/// Profiles high school graduation indicator for K-12 students from
/// RDS.FactK12StudentEnrollments, joined to RDS.DimK12EnrollmentStatuses.
/// Graduation is indicated by ExitOrWithdrawalTypeCode = '01921' (Graduated
/// with regular, advanced, International Baccalaureate, or other type of diploma).
/// Assesses the count of records with a graduation exit code present.
/// </summary>
public class HighSchoolGraduationIndicatorCedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "High school graduation indicator";

    public string Query => $@"
WITH GraduationBase AS (
    SELECT
        f.K12StudentId
    FROM RDS.FactK12StudentEnrollments f
    JOIN RDS.DimK12EnrollmentStatuses e ON e.DimK12EnrollmentStatusId = f.K12EnrollmentStatusId
    WHERE e.ExitOrWithdrawalTypeCode = '01921'
)
INSERT INTO #EWFProfilerResults
-- RecordCount
SELECT
    '{DataElementName}'             AS DataElementName,
    'RecordCount'                   AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL                            AS SubItemLabel,
    NULL                            AS Remarks
FROM GraduationBase";

    public string AssessmentDescription =>
        "Assesses high school graduation indicator by counting enrollment records where " +
        "ExitOrWithdrawalTypeCode = '01921' (Graduated with regular, advanced, IB, or other " +
        "diploma). The record count is the presence signal for this data element.";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
