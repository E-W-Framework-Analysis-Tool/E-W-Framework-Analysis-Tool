using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

// Early grades on track
public class EarlyLearningAssessmentsEdFiAssessor : IEdFiAssessor
{
    private static readonly HashSet<string> _earlyLearningCategories =
    [
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Early Learning - Approaches toward learning",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Early Learning - Cognition and general knowledge",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Early Learning - Language and literacy development",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Early Learning - Physical well-being and motor dev",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Early Learning - Social and emotional development"
    ];
    public string DataElementName => "Early Learning";

    public string AssessmentDescription =>
        "Count of students with early learning results";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Finding Early Learning assessments...");

        var assessmentIdentifiers = new HashSet<string>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                var isEarlyLearning = _earlyLearningCategories.Contains(assessment.AssessmentCategoryDescriptor ?? "");

                if (isEarlyLearning)
                {
                    assessmentIdentifiers.Add(assessment.AssessmentIdentifier);
                    context.Log($"Found Early Learning assessment: {assessment.AssessmentIdentifier}");
                }
            },
            context
        );

        context.Log($"Found {assessmentIdentifiers.Count} Early Learning assessments");

        if (assessmentIdentifiers.Count == 0)
        {
            context.Log("No matching Early Learning assessments found.");
            context.ReportProgress(100, "Complete");

            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)]
            };
        }

        context.ReportProgress(50, "Counting student assessment records...");

        var totalCount = 0;
        foreach (var assessmentId in assessmentIdentifiers)
        {
            var count = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient,
                "ed-fi/studentAssessments",
                new Dictionary<string, string>
                {
                    ["assessmentIdentifier"] = assessmentId
                }
            );
            totalCount += count;
        }

        context.Log($"Found {totalCount:N0} student Early Learning assessment records");

        // Step 3: Collect distribution by assessment category
        context.ReportProgress(75, "Collecting distribution by assessment category...");
        var categoryDistribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (assessmentIdentifiers.Contains(assessment.AssessmentIdentifier))
                {
                    var category = assessment.AssessmentCategoryDescriptor ?? "Unknown";
                    var categoryName = category.Split('#').LastOrDefault() ?? category;

                    categoryDistribution.TryAdd(categoryName, 0);
                    categoryDistribution[categoryName]++;
                }
            },
            context
        );

        context.Log("Assessment category distribution:");
        foreach (var kvp in categoryDistribution.OrderByDescending(x => x.Value))
        {
            context.Log($"  {kvp.Key}: {kvp.Value} assessment(s)");
        }

        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [
                new RecordCount(totalCount),
                new Distribution(categoryDistribution, "Assessment Category")
            ]
        };
    }
}


// Grade 3 - Mathematics
public class Grade3MathStateAssessmentsEdFiAssessor : IEdFiAssessor
{
    private static readonly HashSet<string> _stateTestCategories =
    [
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State assessment",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Benchmark test",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State summative assessment 3-8 general",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternative assessment/grade-level standards",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternative assessment/modified standards",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternate assessment/ELL"
    ];

    // Course performance (English and Math)
    public string DataElementName => "State Assessments - Grade 3 Mathematics";

    public string AssessmentDescription =>
        "Count of grade 3 students with state standardized test results in mathematics";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Finding mathematics assessments for grade 3...");

        var assessmentIdentifiers = new HashSet<string>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                var isStateTest = _stateTestCategories.Contains(assessment.AssessmentCategoryDescriptor ?? "");

                var isGrade3 = assessment.AssessedGradeLevels?.Any(gl =>
                    gl.GradeLevelDescriptor == "uri://ed-fi.org/GradeLevelDescriptor#Third grade") ?? false;

                var isMath = assessment.AcademicSubjects?.Any(subj =>
                    subj.AcademicSubjectDescriptor == "uri://ed-fi.org/AcademicSubjectDescriptor#Mathematics") ?? false;

                if (isStateTest && isGrade3 && isMath)
                {
                    assessmentIdentifiers.Add(assessment.AssessmentIdentifier);
                    context.Log($"Found math assessment: {assessment.AssessmentIdentifier}");
                }
            },
            context
        );

        context.Log($"Found {assessmentIdentifiers.Count} grade 3 mathematics assessments");

        if (assessmentIdentifiers.Count == 0)
        {
            context.Log("No matching mathematics assessments found.");
            context.ReportProgress(100, "Complete");

            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)]
            };
        }

        context.ReportProgress(50, "Counting student assessment records...");

        var totalCount = 0;
        foreach (var assessmentId in assessmentIdentifiers)
        {
            var count = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient,
                "ed-fi/studentAssessments",
                new Dictionary<string, string>
                {
                    ["assessmentIdentifier"] = assessmentId
                }
            );
            totalCount += count;
        }

        context.Log($"Found {totalCount:N0} grade 3 mathematics assessment records");

        // Step 3: Collect distribution by assessment category
        context.ReportProgress(75, "Collecting distribution by assessment category...");
        var categoryDistribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (assessmentIdentifiers.Contains(assessment.AssessmentIdentifier))
                {
                    var category = assessment.AssessmentCategoryDescriptor ?? "Unknown";
                    var categoryName = category.Split('#').LastOrDefault() ?? category;

                    categoryDistribution.TryAdd(categoryName, 0);
                    categoryDistribution[categoryName]++;
                }
            },
            context
        );

        context.Log("Assessment category distribution:");
        foreach (var kvp in categoryDistribution.OrderByDescending(x => x.Value))
        {
            context.Log($"  {kvp.Key}: {kvp.Value} assessment(s)");
        }

        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [
                new RecordCount(totalCount),
                new Distribution(categoryDistribution, "Assessment Category")
            ]
        };
    }
}

// Grade 3 - ELA/Reading
public class Grade3ELAStateAssessmentsEdFiAssessor : IEdFiAssessor
{
    private static readonly HashSet<string> _stateTestCategories =
    [
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State assessment",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Benchmark test",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State summative assessment 3-8 general",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternative assessment/grade-level standards",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternative assessment/modified standards",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternate assessment/ELL"
    ];

    private static readonly HashSet<string> _elaSubjects =
    [
        "uri://ed-fi.org/AcademicSubjectDescriptor#Reading",
        "uri://ed-fi.org/AcademicSubjectDescriptor#English Language Arts"
    ];

    // Course performance (English and Math)
    public string DataElementName => "State Assessments - Grade 3 ELA/Reading";

    public string AssessmentDescription =>
        "Count of grade 3 students with state standardized test results in English Language Arts or Reading";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Finding ELA/Reading assessments for grade 3...");

        var assessmentIdentifiers = new HashSet<string>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                var isStateTest = _stateTestCategories.Contains(assessment.AssessmentCategoryDescriptor ?? "");

                var isGrade3 = assessment.AssessedGradeLevels?.Any(gl =>
                    gl.GradeLevelDescriptor == "uri://ed-fi.org/GradeLevelDescriptor#Third grade") ?? false;

                var isELA = assessment.AcademicSubjects?.Any(subj =>
                    _elaSubjects.Contains(subj.AcademicSubjectDescriptor ?? "")) ?? false;

                if (isStateTest && isGrade3 && isELA)
                {
                    assessmentIdentifiers.Add(assessment.AssessmentIdentifier);
                    context.Log($"Found ELA/Reading assessment: {assessment.AssessmentIdentifier}");
                }
            },
            context
        );

        context.Log($"Found {assessmentIdentifiers.Count} grade 3 ELA/Reading assessments");

        if (assessmentIdentifiers.Count == 0)
        {
            context.Log("No matching ELA/Reading assessments found.");
            context.ReportProgress(100, "Complete");

            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)]
            };
        }

        context.ReportProgress(50, "Counting student assessment records...");

        var totalCount = 0;
        foreach (var assessmentId in assessmentIdentifiers)
        {
            var count = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient,
                "ed-fi/studentAssessments",
                new Dictionary<string, string>
                {
                    ["assessmentIdentifier"] = assessmentId
                }
            );
            totalCount += count;
        }

        context.Log($"Found {totalCount:N0} grade 3 ELA/Reading assessment records");

        // Step 3: Collect distribution by assessment category
        context.ReportProgress(75, "Collecting distribution by assessment category...");
        var categoryDistribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (assessmentIdentifiers.Contains(assessment.AssessmentIdentifier))
                {
                    var category = assessment.AssessmentCategoryDescriptor ?? "Unknown";
                    var categoryName = category.Split('#').LastOrDefault() ?? category;

                    categoryDistribution.TryAdd(categoryName, 0);
                    categoryDistribution[categoryName]++;
                }
            },
            context
        );

        context.Log("Assessment category distribution:");
        foreach (var kvp in categoryDistribution.OrderByDescending(x => x.Value))
        {
            context.Log($"  {kvp.Key}: {kvp.Value} assessment(s)");
        }

        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [
                new RecordCount(totalCount),
                new Distribution(categoryDistribution, "Assessment Category")
            ]
        };
    }
}

// Grade 8 - Mathematics
public class Grade8MathStateAssessmentsEdFiAssessor : IEdFiAssessor
{
    private static readonly HashSet<string> _stateTestCategories =
    [
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State assessment",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Benchmark test",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State summative assessment 3-8 general",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternative assessment/grade-level standards",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternative assessment/modified standards",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternate assessment/ELL"
    ];

    // Course performance (English and Math)
    public string DataElementName => "State Assessments - Grade 8 Mathematics";

    public string AssessmentDescription =>
        "Count of grade 8 students with state standardized test results in mathematics";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Finding mathematics assessments for grade 8...");

        var assessmentIdentifiers = new HashSet<string>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                var isStateTest = _stateTestCategories.Contains(assessment.AssessmentCategoryDescriptor ?? "");

                var isGrade8 = assessment.AssessedGradeLevels?.Any(gl =>
                    gl.GradeLevelDescriptor == "uri://ed-fi.org/GradeLevelDescriptor#Eighth grade") ?? false;

                var isMath = assessment.AcademicSubjects?.Any(subj =>
                    subj.AcademicSubjectDescriptor == "uri://ed-fi.org/AcademicSubjectDescriptor#Mathematics") ?? false;

                if (isStateTest && isGrade8 && isMath)
                {
                    assessmentIdentifiers.Add(assessment.AssessmentIdentifier);
                    context.Log($"Found math assessment: {assessment.AssessmentIdentifier}");
                }
            },
            context
        );

        context.Log($"Found {assessmentIdentifiers.Count} grade 8 mathematics assessments");

        if (assessmentIdentifiers.Count == 0)
        {
            context.Log("No matching mathematics assessments found.");
            context.ReportProgress(100, "Complete");

            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)]
            };
        }

        context.ReportProgress(50, "Counting student assessment records...");

        var totalCount = 0;
        foreach (var assessmentId in assessmentIdentifiers)
        {
            var count = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient,
                "ed-fi/studentAssessments",
                new Dictionary<string, string>
                {
                    ["assessmentIdentifier"] = assessmentId
                }
            );
            totalCount += count;
        }

        context.Log($"Found {totalCount:N0} grade 8 mathematics assessment records");

        // Step 3: Collect distribution by assessment category
        context.ReportProgress(75, "Collecting distribution by assessment category...");
        var categoryDistribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (assessmentIdentifiers.Contains(assessment.AssessmentIdentifier))
                {
                    var category = assessment.AssessmentCategoryDescriptor ?? "Unknown";
                    var categoryName = category.Split('#').LastOrDefault() ?? category;

                    categoryDistribution.TryAdd(categoryName, 0);
                    categoryDistribution[categoryName]++;
                }
            },
            context
        );

        context.Log("Assessment category distribution:");
        foreach (var kvp in categoryDistribution.OrderByDescending(x => x.Value))
        {
            context.Log($"  {kvp.Key}: {kvp.Value} assessment(s)");
        }

        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [
                new RecordCount(totalCount),
                new Distribution(categoryDistribution, "Assessment Category")
            ]
        };
    }
}

// Grade 8 - ELA/Reading
public class Grade8ELAStateAssessmentsEdFiAssessor : IEdFiAssessor
{
    private static readonly HashSet<string> _stateTestCategories =
    [
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State assessment",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#Benchmark test",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State summative assessment 3-8 general",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternative assessment/grade-level standards",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternative assessment/modified standards",
        "uri://ed-fi.org/AssessmentCategoryDescriptor#State alternate assessment/ELL"
    ];

    private static readonly HashSet<string> _elaSubjects =
    [
        "uri://ed-fi.org/AcademicSubjectDescriptor#Reading",
        "uri://ed-fi.org/AcademicSubjectDescriptor#English Language Arts"
    ];

    // Course performance (English and Math)
    public string DataElementName => "State Assessments - Grade 8 ELA/Reading";

    public string AssessmentDescription =>
        "Count of grade 8 students with state standardized test results in English Language Arts or Reading";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Finding ELA/Reading assessments for grade 8...");

        var assessmentIdentifiers = new HashSet<string>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                var isStateTest = _stateTestCategories.Contains(assessment.AssessmentCategoryDescriptor ?? "");

                var isGrade8 = assessment.AssessedGradeLevels?.Any(gl =>
                    gl.GradeLevelDescriptor == "uri://ed-fi.org/GradeLevelDescriptor#Eighth grade") ?? false;

                var isELA = assessment.AcademicSubjects?.Any(subj =>
                    _elaSubjects.Contains(subj.AcademicSubjectDescriptor ?? "")) ?? false;

                if (isStateTest && isGrade8 && isELA)
                {
                    assessmentIdentifiers.Add(assessment.AssessmentIdentifier);
                    context.Log($"Found ELA/Reading assessment: {assessment.AssessmentIdentifier}");
                }
            },
            context
        );

        context.Log($"Found {assessmentIdentifiers.Count} grade 8 ELA/Reading assessments");

        if (assessmentIdentifiers.Count == 0)
        {
            context.Log("No matching ELA/Reading assessments found.");
            context.ReportProgress(100, "Complete");

            return new DataElementAssessment
            {
                DataElementName = DataElementName,
                Characteristics = [new RecordCount(0)]
            };
        }

        context.ReportProgress(50, "Counting student assessment records...");

        var totalCount = 0;
        foreach (var assessmentId in assessmentIdentifiers)
        {
            var count = await EdFiApiPatterns.CountFromHeaderAsync(
                httpClient,
                "ed-fi/studentAssessments",
                new Dictionary<string, string>
                {
                    ["assessmentIdentifier"] = assessmentId
                }
            );
            totalCount += count;
        }

        context.Log($"Found {totalCount:N0} grade 8 ELA/Reading assessment records");

        // Step 3: Collect distribution by assessment category
        context.ReportProgress(75, "Collecting distribution by assessment category...");
        var categoryDistribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiAssessment>(
            httpClient,
            "ed-fi/assessments",
            assessment =>
            {
                if (assessmentIdentifiers.Contains(assessment.AssessmentIdentifier))
                {
                    var category = assessment.AssessmentCategoryDescriptor ?? "Unknown";
                    var categoryName = category.Split('#').LastOrDefault() ?? category;

                    categoryDistribution.TryAdd(categoryName, 0);
                    categoryDistribution[categoryName]++;
                }
            },
            context
        );

        context.Log("Assessment category distribution:");
        foreach (var kvp in categoryDistribution.OrderByDescending(x => x.Value))
        {
            context.Log($"  {kvp.Key}: {kvp.Value} assessment(s)");
        }

        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [
                new RecordCount(totalCount),
                new Distribution(categoryDistribution, "Assessment Category")
            ]
        };
    }
}
