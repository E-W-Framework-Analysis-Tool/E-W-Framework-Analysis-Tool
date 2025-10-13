using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public interface IEdFiAssessor
{
    string DataElementName { get; }
    Task<DataElementAssessment> AssessAsync(HttpClient httpClient, DataSource dataSource, AssessorContext context);
    string AssessmentDescription { get; }
}
