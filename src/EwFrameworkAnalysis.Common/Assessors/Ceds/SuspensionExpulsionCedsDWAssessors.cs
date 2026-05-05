using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

public class SuspensionExpulsionGrades1and2CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Suspensions and Expulsions (Grades 1 and 2)";
    public string Query => $@"
INSERT INTO #EWFProfilerResults
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount' AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL AS SubItemLabel,
    NULL AS Remarks
FROM RDS.FactK12StudentDisciplines fact
JOIN RDS.DimDisciplineStatuses discStat ON fact.DisciplineStatusId = discStat.DimDisciplineStatusId
JOIN RDS.DimGradeLevels gradeLevel ON fact.GradeLevelId = gradeLevel.DimGradeLevelId
WHERE discStat.DisciplinaryActionTakenCode IN 
	('03086' -- Expulsion with services
	,'03087' -- Expulsion without services
	,'03099' -- Suspension after school
	,'03100' -- Suspension, in-school
	,'03154' -- Suspension, out-of-school, greater than 10 consecutive days
	,'03155' -- Suspension, out-of-school, separate days culminating to more than 10 school days
	,'03101' -- Suspension, out-of-school, with services
	,'03102' -- Suspension, out-of-school, without services
	)
AND gradeLevel.GradeLevelCode IN ('01', '02')
";

    public string AssessmentDescription => "Query RDS.FactK12StudentDisciplines where GradeLevel is 01 or 02, and where DisciplinaryActionTakenCode is Expulsion or Suspension option";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}

public class SuspensionExpulsionK12CedsDWAssessor : ICedsDWAssessor
{
    public string DataElementName => "Suspensions and Expulsions (K-12)";
    public string Query => $@"
INSERT INTO #EWFProfilerResults
SELECT
    '{DataElementName}' AS DataElementName,
    'RecordCount' AS CharacteristicType,
    CAST(COUNT(*) AS NVARCHAR(MAX)) AS Value,
    NULL AS SubItemLabel,
    NULL AS Remarks
FROM RDS.FactK12StudentDisciplines fact
JOIN RDS.DimDisciplineStatuses discStat ON fact.DisciplineStatusId = discStat.DimDisciplineStatusId
WHERE discStat.DisciplinaryActionTakenCode IN 
	('03086' -- Expulsion with services
	,'03087' -- Expulsion without services
	,'03099' -- Suspension after school
	,'03100' -- Suspension, in-school
	,'03154' -- Suspension, out-of-school, greater than 10 consecutive days
	,'03155' -- Suspension, out-of-school, separate days culminating to more than 10 school days
	,'03101' -- Suspension, out-of-school, with services
	,'03102' -- Suspension, out-of-school, without services
	)
";

    public string AssessmentDescription => "Query RDS.FactK12StudentDisciplines where DisciplinaryActionTakenCode is Expulsion or Suspension option";

    public string MinVersion => CedsDWVersions.V13;
    public string? MaxVersion => null;
}
