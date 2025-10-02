using EwFrameworkAnalysis.Common.Models.Project;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public interface IEdFiAssessor
{
    string DataElementName { get; }
    Task<DataElementAssessment> AssessAsync(HttpClient httpClient, DataSource dataSource);
    string AssessmentDescription { get; }
}
