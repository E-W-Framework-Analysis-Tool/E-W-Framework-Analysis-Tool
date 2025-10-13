namespace EwFrameworkAnalysis.Common.Assessors.Ceds;

public interface ICedsDWAssessor
{
    string DataElementName { get; }
    string Query { get; }
    string AssessmentDescription { get; }
}
