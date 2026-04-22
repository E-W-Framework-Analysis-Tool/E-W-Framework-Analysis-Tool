using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class MentalEmotionalWellBeingAssessmentsEdFiAssessor : IEdFiAssessor
{
    private static readonly string[] _wellBeingCategoryDescriptors =
    [
        "Psychological test",
        "Attitudinal test"
    ];

    private static readonly string[] _assessmentKeywords =
    [
        // Generic terms
        "mental well-being",
        "mental wellbeing",
        "emotional well-being",
        "emotional wellbeing",
        "psychological well-being",
        "psychological wellbeing",
        "subjective well-being",
        "subjective wellbeing",
        "life satisfaction",
        "flourishing",

        // Instrument named in the E-W Framework
        "Psychological Wellbeing Scale",
        "Psychological Well-Being Scale",
        "Ryff",                              // Ryff Scales of Psychological Well-Being

        // Other widely used adult / postsecondary mental & emotional well-being instruments
        "WEMWBS",                            // Warwick-Edinburgh Mental Well-Being Scale
        "Warwick-Edinburgh",
        "SWLS",                              // Satisfaction with Life Scale
        "Satisfaction with Life Scale",
        "PERMA",                             // PERMA-Profiler
        "Flourishing Scale",
        "Mental Health Continuum",
        "MHC-SF",                            // Mental Health Continuum - Short Form
        "PHQ-9",                             // Patient Health Questionnaire
        "GAD-7",                             // Generalized Anxiety Disorder
        "DASS",                              // Depression Anxiety Stress Scales
        "K6",                                // Kessler Psychological Distress Scale
        "K10",
        "Kessler",
        "PROMIS"                             // PROMIS mental health / well-being short forms
    ];

    public string DataElementName => "Mental and emotional well-being assessments";

    public string AssessmentDescription =>
        "Identifies student / individual assessments linked to postsecondary / adult mental and emotional " +
        "well-being instruments (e.g., Psychological Wellbeing Scale / Ryff Scales named in the E-W Framework, " +
        "plus Warwick-Edinburgh Mental Well-Being Scale, Satisfaction with Life Scale, PERMA-Profiler, " +
        "Flourishing Scale, Mental Health Continuum, PHQ-9, GAD-7, Kessler K6/K10, DASS, PROMIS) by matching " +
        "assessmentCategoryDescriptor values (Psychological test, Attitudinal test) and well-known instrument " +
        "names and well-being keywords in the Ed-Fi assessments catalog.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Searching assessment catalog for mental and emotional well-being instruments...");

        var matchingAssessments = new List<(string Identifier, string Namespace)>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (IsWellBeingAssessment(assessment))
                    matchingAssessments.Add((assessment.AssessmentIdentifier, assessment.Namespace));
            },
            context);

        context.Log($"Found {matchingAssessments.Count} mental/emotional well-being assessment(s) in catalog");

        if (matchingAssessments.Count == 0)
        {
            context.ReportProgress(100, "Complete — no mental/emotional well-being assessments found");
            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)],
                Remarks = AssessmentDescription +
                    " No assessments matching recognized mental / emotional well-being instruments were found " +
                    "in the assessment catalog."
            };
        }

        context.ReportProgress(50, "Loading student assessment results for well-being instruments...");

        var studentAssessments = new List<EdFiStudentAssessment>();
        var instrumentDistribution = new Dictionary<string, int>();

        foreach (var (identifier, ns) in matchingAssessments)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["assessmentIdentifier"] = identifier,
                ["namespace"] = ns
            };

            var countBefore = studentAssessments.Count;

            await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentAssessment>(
                httpClient,
                "ed-fi/studentAssessments",
                item => studentAssessments.Add(item),
                context,
                queryParams);

            var added = studentAssessments.Count - countBefore;
            if (added > 0)
                instrumentDistribution[identifier] = added;
        }

        var result = StudentAssessmentAnalyzer.Analyze(studentAssessments);

        context.Log(
            $"Found {result.RecordsWithScores:N0} of {result.TotalRecords:N0} well-being assessment results with score results");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(result.TotalRecords),
                new Distribution(instrumentDistribution, "Assessment Instrument"),
                new Distribution(result.GradeLevelDistribution, "Grade Level Assessed"),
                new Distribution(result.PerformanceLevelDistribution, "Performance Level / Well-Being Level"),
                new Completeness(result.TotalRecords, result.RecordsWithScores, "ScoreResults")
            ],
            Remarks = AssessmentDescription
        };
    }

    private static bool IsWellBeingAssessment(EdFiAssessment assessment)
    {
        if (!string.IsNullOrWhiteSpace(assessment.AssessmentCategoryDescriptor))
        {
            var category = EdFiDescriptorHelper.ParseDescriptorValue(assessment.AssessmentCategoryDescriptor);
            foreach (var descriptor in _wellBeingCategoryDescriptors)
            {
                if (category.Equals(descriptor, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }

        var title = assessment.AssessmentTitle ?? string.Empty;
        var identifier = assessment.AssessmentIdentifier ?? string.Empty;

        foreach (var keyword in _assessmentKeywords)
        {
            if (title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                identifier.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
