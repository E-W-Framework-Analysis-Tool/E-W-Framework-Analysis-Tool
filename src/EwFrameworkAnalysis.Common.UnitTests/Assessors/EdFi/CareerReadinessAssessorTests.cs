using System.Net;
using System.Text;
using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.FrameworkReferenceData;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;
using FluentAssertions;
using Newtonsoft.Json;

namespace EwFrameworkAnalysis.Common.UnitTests.Assessors.EdFi;

public class CareerReadinessAssessorTests
{
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://api.test.com/") };
    private readonly DataSource _dataSource = new() { Name = "Test", Type = DataSourceType.EdFiApi };
    private readonly AssessorContext _context = new((_, _) => { }, _ => { });

    // --- Provider helpers ---

    private static EdFiCTEProgramProvider CreateCTEProviderWithData(
        List<EdFiStudentCTEProgramAssociation> data)
    {
        var provider = new EdFiCTEProgramProvider();
        var field = typeof(EdFiCTEProgramProvider)
            .GetField("_cachedData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        field.SetValue(provider, data);
        return provider;
    }

    private static EdFiCourseProvider CreateCourseProviderWithData(
        List<EdFiCourse> data)
    {
        var provider = new EdFiCourseProvider();
        var field = typeof(EdFiCourseProvider)
            .GetField("_cachedData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        field.SetValue(provider, data);
        return provider;
    }

    private static EdFiStudentAssessmentProvider CreateAssessmentProviderWithData(
        List<EdFiStudentAssessment> data)
    {
        var provider = new EdFiStudentAssessmentProvider();
        var field = typeof(EdFiStudentAssessmentProvider)
            .GetField("_cachedData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        field.SetValue(provider, data);
        return provider;
    }

    // --- CTE Assessor Tests ---

    [Fact]
    public async Task Should_ProduceReasonExitedDistribution_When_CTECompletionAssessed()
    {
        var data = new List<EdFiStudentCTEProgramAssociation>
        {
            new(beginDate: new DateOnly(2024, 1, 1),
                educationOrganizationReference: new EdFiEducationOrganizationReference(1),
                programReference: new EdFiProgramReference(1, "CTE Program A", "uri://ed-fi.org/ProgramTypeDescriptor#CTE"),
                studentReference: new EdFiStudentReference("student1"),
                endDate: new DateOnly(2024, 6, 1),
                reasonExitedDescriptor: "uri://ed-fi.org/ReasonExitedDescriptor#Completed"),
            new(beginDate: new DateOnly(2024, 1, 1),
                educationOrganizationReference: new EdFiEducationOrganizationReference(1),
                programReference: new EdFiProgramReference(1, "CTE Program B", "uri://ed-fi.org/ProgramTypeDescriptor#CTE"),
                studentReference: new EdFiStudentReference("student2"),
                reasonExitedDescriptor: "uri://ed-fi.org/ReasonExitedDescriptor#Transferred")
        };

        var provider = CreateCTEProviderWithData(data);
        var assessor = new CTECourseCompletionEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("CTE course completion");
        result.Characteristics.Should().Contain(c => c is RecordCount);
        result.Characteristics.OfType<RecordCount>().First().Value.Should().Be(2);

        var distribution = result.Characteristics.OfType<Distribution>().First(d => d.Label == "Reason Exited");
        distribution.Counts["Completed"].Should().Be(1);
        distribution.Counts["Transferred"].Should().Be(1);

        var completeness = result.Characteristics.OfType<Completeness>().First();
        completeness.TotalRecords.Should().Be(2);
        completeness.PopulatedRecords.Should().Be(1);
        completeness.AttributeName.Should().Be("EndDate");
    }

    [Fact]
    public async Task Should_ProduceProgramDistribution_When_CTEPathwayAssessed()
    {
        var data = new List<EdFiStudentCTEProgramAssociation>
        {
            new(beginDate: new DateOnly(2024, 1, 1),
                educationOrganizationReference: new EdFiEducationOrganizationReference(1),
                programReference: new EdFiProgramReference(1, "Health Sciences", "uri://ed-fi.org/ProgramTypeDescriptor#CTE"),
                studentReference: new EdFiStudentReference("student1"),
                cteProgramServices:
                [
                    new EdFiStudentCTEProgramAssociationCTEProgramService("uri://ed-fi.org/CTEProgramServiceDescriptor#Nursing")
                ]),
            new(beginDate: new DateOnly(2024, 1, 1),
                educationOrganizationReference: new EdFiEducationOrganizationReference(1),
                programReference: new EdFiProgramReference(1, "Information Technology", "uri://ed-fi.org/ProgramTypeDescriptor#CTE"),
                studentReference: new EdFiStudentReference("student2"))
        };

        var provider = CreateCTEProviderWithData(data);
        var assessor = new CTEPathwayEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("CTE pathway or career cluster associated with CTE course");

        var programDist = result.Characteristics.OfType<Distribution>().First(d => d.Label == "CTE Program");
        programDist.Counts["Health Sciences"].Should().Be(1);
        programDist.Counts["Information Technology"].Should().Be(1);

        var serviceDist = result.Characteristics.OfType<Distribution>().First(d => d.Label == "CTE Program Service");
        serviceDist.Counts["Nursing"].Should().Be(1);

        var completeness = result.Characteristics.OfType<Completeness>().First();
        completeness.PopulatedRecords.Should().Be(1);
        completeness.TotalRecords.Should().Be(2);
    }

    [Fact]
    public async Task Should_ProduceServiceDistribution_When_WorkBasedLearningAssessed()
    {
        var data = new List<EdFiStudentCTEProgramAssociation>
        {
            new(beginDate: new DateOnly(2024, 1, 1),
                educationOrganizationReference: new EdFiEducationOrganizationReference(1),
                programReference: new EdFiProgramReference(1, "CTE", "uri://ed-fi.org/ProgramTypeDescriptor#CTE"),
                studentReference: new EdFiStudentReference("student1"),
                cteProgramServices:
                [
                    new EdFiStudentCTEProgramAssociationCTEProgramService("uri://ed-fi.org/CTEProgramServiceDescriptor#Internship"),
                    new EdFiStudentCTEProgramAssociationCTEProgramService("uri://ed-fi.org/CTEProgramServiceDescriptor#Apprenticeship")
                ]),
            new(beginDate: new DateOnly(2024, 1, 1),
                educationOrganizationReference: new EdFiEducationOrganizationReference(1),
                programReference: new EdFiProgramReference(1, "CTE", "uri://ed-fi.org/ProgramTypeDescriptor#CTE"),
                studentReference: new EdFiStudentReference("student2"))
        };

        var provider = CreateCTEProviderWithData(data);
        var assessor = new WorkBasedLearningEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Participation in work-based learning");

        var serviceDist = result.Characteristics.OfType<Distribution>().First();
        serviceDist.Counts["Internship"].Should().Be(1);
        serviceDist.Counts["Apprenticeship"].Should().Be(1);

        var completeness = result.Characteristics.OfType<Completeness>().First();
        completeness.PopulatedRecords.Should().Be(1);
        completeness.TotalRecords.Should().Be(2);
    }

    // --- Course Assessor Tests ---

    [Fact]
    public async Task Should_ProduceSubjectDistribution_When_CourseIdentifierAssessed()
    {
        var data = new List<EdFiCourse>
        {
            CreateCourse("MATH101", "Algebra I",
                academicSubjects: [new EdFiCourseAcademicSubject("uri://ed-fi.org/AcademicSubjectDescriptor#Mathematics")]),
            CreateCourse("ENG101", "English I",
                academicSubjects: [new EdFiCourseAcademicSubject("uri://ed-fi.org/AcademicSubjectDescriptor#English Language Arts")]),
            CreateCourse("OTHER", "")
        };

        var provider = CreateCourseProviderWithData(data);
        var assessor = new CourseIdentifierEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Course identifier or title");
        result.Characteristics.OfType<RecordCount>().First().Value.Should().Be(3);

        var subjectDist = result.Characteristics.OfType<Distribution>().First();
        subjectDist.Counts["Mathematics"].Should().Be(1);
        subjectDist.Counts["English Language Arts"].Should().Be(1);

        var completeness = result.Characteristics.OfType<Completeness>().First();
        completeness.PopulatedRecords.Should().Be(2);
        completeness.TotalRecords.Should().Be(3);
    }

    [Fact]
    public async Task Should_ProducePathwayDistribution_When_CTECourseIdAssessed()
    {
        var data = new List<EdFiCourse>
        {
            CreateCourse("CTE001", "Welding",
                careerPathwayDescriptor: "uri://ed-fi.org/CareerPathwayDescriptor#Manufacturing"),
            CreateCourse("CTE002", "Nursing",
                careerPathwayDescriptor: "uri://ed-fi.org/CareerPathwayDescriptor#Health Science"),
            CreateCourse("MATH101", "Algebra")
        };

        var provider = CreateCourseProviderWithData(data);
        var assessor = new CTECourseIdEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("CTE course ID or course title");
        result.Characteristics.OfType<RecordCount>().First().Value.Should().Be(3);

        var pathwayDist = result.Characteristics.OfType<Distribution>().First();
        pathwayDist.Counts["Manufacturing"].Should().Be(1);
        pathwayDist.Counts["Health Science"].Should().Be(1);

        var completeness = result.Characteristics.OfType<Completeness>().First();
        completeness.PopulatedRecords.Should().Be(2);
        completeness.TotalRecords.Should().Be(3);
    }

    // --- Standalone Assessor Tests ---

    [Fact]
    public async Task Should_ExcludeHomerooms_When_StudentCourseEnrollmentAssessed()
    {
        var testData = new List<EdFiStudentSectionAssociation>
        {
            new(beginDate: new DateOnly(2024, 1, 1),
                sectionReference: new EdFiSectionReference("CS101", 1, 2024, "S1", "Fall"),
                studentReference: new EdFiStudentReference("student1"),
                homeroomIndicator: false),
            new(beginDate: new DateOnly(2024, 1, 1),
                sectionReference: new EdFiSectionReference("HR", 1, 2024, "S2", "Fall"),
                studentReference: new EdFiStudentReference("student1"),
                homeroomIndicator: true),
            new(beginDate: new DateOnly(2024, 1, 1),
                sectionReference: new EdFiSectionReference("ENG101", 1, 2024, "S3", "Fall"),
                studentReference: new EdFiStudentReference("student2"))
        };

        using var httpClient = CreateHttpClientWithJsonResponse(testData);
        var assessor = new StudentCourseEnrollmentEdFiAssessor();

        var result = await assessor.AssessAsync(httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Student course enrollment record");
        result.Characteristics.OfType<RecordCount>().First().Value.Should().Be(3);

        var completeness = result.Characteristics.OfType<Completeness>().First();
        completeness.TotalRecords.Should().Be(3);
        completeness.PopulatedRecords.Should().Be(2);
        completeness.AttributeName.Should().Be("Non-Homeroom Enrollments");
    }

    [Fact]
    public async Task Should_ProduceCredentialTypeDistribution_When_IndustryCredentialAssessed()
    {
        var testData = new List<EdFiCredential>
        {
            new(credentialIdentifier: "CRED1",
                stateOfIssueStateAbbreviationDescriptor: "uri://ed-fi.org/StateAbbreviationDescriptor#TX",
                credentialTypeDescriptor: "uri://ed-fi.org/CredentialTypeDescriptor#Certification",
                issuanceDate: new DateOnly(2024, 1, 1),
                varNamespace: "uri://ed-fi.org",
                credentialFieldDescriptor: "uri://ed-fi.org/CredentialFieldDescriptor#Welding"),
            new(credentialIdentifier: "CRED2",
                stateOfIssueStateAbbreviationDescriptor: "uri://ed-fi.org/StateAbbreviationDescriptor#TX",
                credentialTypeDescriptor: "uri://ed-fi.org/CredentialTypeDescriptor#Endorsement",
                issuanceDate: new DateOnly(2024, 1, 1),
                varNamespace: "uri://ed-fi.org")
        };

        using var httpClient = CreateHttpClientWithJsonResponse(testData);
        var assessor = new IndustryCredentialEdFiAssessor();

        var result = await assessor.AssessAsync(httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Industry-recognized credential attainment");
        result.Characteristics.OfType<RecordCount>().First().Value.Should().Be(2);

        var typeDist = result.Characteristics.OfType<Distribution>().First();
        typeDist.Counts["Certification"].Should().Be(1);
        typeDist.Counts["Endorsement"].Should().Be(1);

        var completeness = result.Characteristics.OfType<Completeness>().First();
        completeness.PopulatedRecords.Should().Be(1);
        completeness.TotalRecords.Should().Be(2);
    }

    [Fact]
    public async Task Should_ProduceDiplomaTypeDistribution_When_GraduationDateAssessed()
    {
        var testData = new List<EdFiStudentAcademicRecord>
        {
            new(educationOrganizationReference: new EdFiEducationOrganizationReference(1),
                schoolYearTypeReference: new EdFiSchoolYearTypeReference(2024),
                studentReference: new EdFiStudentReference("student1"),
                termDescriptor: "uri://ed-fi.org/TermDescriptor#Spring Semester",
                diplomas:
                [
                    new EdFiStudentAcademicRecordDiploma(
                        diplomaAwardDate: new DateOnly(2024, 5, 25),
                        diplomaTypeDescriptor: "uri://ed-fi.org/DiplomaTypeDescriptor#Regular diploma")
                ]),
            new(educationOrganizationReference: new EdFiEducationOrganizationReference(1),
                schoolYearTypeReference: new EdFiSchoolYearTypeReference(2024),
                studentReference: new EdFiStudentReference("student2"),
                termDescriptor: "uri://ed-fi.org/TermDescriptor#Spring Semester")
        };

        using var httpClient = CreateHttpClientWithJsonResponse(testData);
        var assessor = new HighSchoolGraduationDateEdFiAssessor();

        var result = await assessor.AssessAsync(httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("High school graduation date");
        result.Characteristics.OfType<RecordCount>().First().Value.Should().Be(2);

        var diplomaDist = result.Characteristics.OfType<Distribution>().First();
        diplomaDist.Counts["Regular diploma"].Should().Be(1);

        var completeness = result.Characteristics.OfType<Completeness>().First();
        completeness.PopulatedRecords.Should().Be(1);
        completeness.TotalRecords.Should().Be(2);
    }

    // --- Student Assessment Assessor Tests ---

    [Fact]
    public async Task Should_ProduceGradeLevelDistribution_When_StudentAssessmentAssessed()
    {
        var assessmentRef = new EdFiAssessmentReference("ASSESS1", "uri://ed-fi.org");
        var data = new List<EdFiStudentAssessment>
        {
            new(assessmentReference: assessmentRef,
                studentAssessmentIdentifier: "SA1",
                studentReference: new EdFiStudentReference("student1"),
                whenAssessedGradeLevelDescriptor: "uri://ed-fi.org/GradeLevelDescriptor#Ninth grade",
                performanceLevels:
                [
                    new EdFiStudentAssessmentPerformanceLevel("uri://ed-fi.org/AssessmentReportingMethodDescriptor#Scale score",
                        performanceLevelDescriptor: "uri://ed-fi.org/PerformanceLevelDescriptor#Proficient")
                ],
                scoreResults:
                [
                    new EdFiStudentAssessmentScoreResult("uri://ed-fi.org/AssessmentReportingMethodDescriptor#Scale score",
                        result: "85", resultDatatypeTypeDescriptor: "uri://ed-fi.org/ResultDatatypeTypeDescriptor#Integer")
                ]),
            new(assessmentReference: assessmentRef,
                studentAssessmentIdentifier: "SA2",
                studentReference: new EdFiStudentReference("student2"),
                whenAssessedGradeLevelDescriptor: "uri://ed-fi.org/GradeLevelDescriptor#Tenth grade")
        };

        var provider = CreateAssessmentProviderWithData(data);

        // Test all three assessment assessors produce the same structure
        var digitalAssessor = new DigitalSkillsAssessmentsEdFiAssessor(provider);
        var result = await digitalAssessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Digital skills assessments (K-12)");
        result.Characteristics.OfType<RecordCount>().First().Value.Should().Be(2);

        var gradeDist = result.Characteristics.OfType<Distribution>().First(d => d.Label == "Grade Level Assessed");
        gradeDist.Counts["Ninth grade"].Should().Be(1);
        gradeDist.Counts["Tenth grade"].Should().Be(1);

        var perfDist = result.Characteristics.OfType<Distribution>().First(d => d.Label == "Performance Level");
        perfDist.Counts["Proficient"].Should().Be(1);

        var completeness = result.Characteristics.OfType<Completeness>().First();
        completeness.PopulatedRecords.Should().Be(1);
        completeness.TotalRecords.Should().Be(2);
    }

    [Fact]
    public async Task Should_ReturnCorrectDataElementName_When_HigherOrderThinkingAssessed()
    {
        var provider = CreateAssessmentProviderWithData([]);
        var assessor = new HigherOrderThinkingAssessmentsEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Higher-order thinking skills performance assessments (K-12)");
    }

    // --- Communication Skills (CCRA+) Assessor Tests ---

    [Fact]
    public async Task Should_FilterByCcraTitle_When_CommunicationSkillsAssessed()
    {
        var ccraAssessment = new EdFiAssessment(
            assessmentIdentifier: "CCRA-PLUS-2024",
            assessmentTitle: "The College and Career Readiness Assessment (CCRA+)",
            varNamespace: "uri://ed-fi.org",
            academicSubjects: []);
        var otherAssessment = new EdFiAssessment(
            assessmentIdentifier: "STATE-MATH-2024",
            assessmentTitle: "State Mathematics Assessment",
            varNamespace: "uri://ed-fi.org",
            academicSubjects: []);

        var ccraRef = new EdFiAssessmentReference("CCRA-PLUS-2024", "uri://ed-fi.org");
        var studentAssessments = new List<EdFiStudentAssessment>
        {
            new(assessmentReference: ccraRef,
                studentAssessmentIdentifier: "SA1",
                studentReference: new EdFiStudentReference("student1"),
                whenAssessedGradeLevelDescriptor: "uri://ed-fi.org/GradeLevelDescriptor#Eleventh grade",
                scoreResults:
                [
                    new EdFiStudentAssessmentScoreResult("uri://ed-fi.org/AssessmentReportingMethodDescriptor#Scale score",
                        result: "92", resultDatatypeTypeDescriptor: "uri://ed-fi.org/ResultDatatypeTypeDescriptor#Integer")
                ]),
            new(assessmentReference: ccraRef,
                studentAssessmentIdentifier: "SA2",
                studentReference: new EdFiStudentReference("student2"),
                whenAssessedGradeLevelDescriptor: "uri://ed-fi.org/GradeLevelDescriptor#Twelfth grade")
        };

        using var httpClient = CreateCcraHttpClient(
            [ccraAssessment, otherAssessment], studentAssessments);
        var assessor = new CommunicationSkillsAssessmentsEdFiAssessor();

        var result = await assessor.AssessAsync(httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Communication skills performance assessments (K-12)");
        result.Characteristics.OfType<RecordCount>().First().Value.Should().Be(2);

        var gradeDist = result.Characteristics.OfType<Distribution>().First(d => d.Label == "Grade Level Assessed");
        gradeDist.Counts["Eleventh grade"].Should().Be(1);
        gradeDist.Counts["Twelfth grade"].Should().Be(1);

        var completeness = result.Characteristics.OfType<Completeness>().First();
        completeness.PopulatedRecords.Should().Be(1);
        completeness.TotalRecords.Should().Be(2);
    }

    [Fact]
    public async Task Should_ReturnZeroRecords_When_NoCcraAssessmentsExist()
    {
        var otherAssessment = new EdFiAssessment(
            assessmentIdentifier: "STATE-MATH-2024",
            assessmentTitle: "State Mathematics Assessment",
            varNamespace: "uri://ed-fi.org",
            academicSubjects: []);

        using var httpClient = CreateCcraHttpClient([otherAssessment], []);
        var assessor = new CommunicationSkillsAssessmentsEdFiAssessor();

        var result = await assessor.AssessAsync(httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Communication skills performance assessments (K-12)");
        result.Characteristics.OfType<RecordCount>().First().Value.Should().Be(0);
        result.Remarks.Should().Contain("No assessments matching");
    }

    // --- Framework Validation ---

    [Fact]
    public void Should_MatchDataElementNames_When_ComparedToFrameworkData()
    {
        var emptyCteProvider = CreateCTEProviderWithData([]);
        var emptyCourseProvider = CreateCourseProviderWithData([]);
        var emptyAssessmentProvider = CreateAssessmentProviderWithData([]);

        var assessors = new IEdFiAssessor[]
        {
            new StudentCourseEnrollmentEdFiAssessor(),
            new CourseIdentifierEdFiAssessor(emptyCourseProvider),
            new CTECourseCompletionEdFiAssessor(emptyCteProvider),
            new CTECourseIdEdFiAssessor(emptyCourseProvider),
            new CTEPathwayEdFiAssessor(emptyCteProvider),
            new IndustryCredentialEdFiAssessor(),
            new WorkBasedLearningEdFiAssessor(emptyCteProvider),
            new HighSchoolGraduationDateEdFiAssessor(),
            new DigitalSkillsAssessmentsEdFiAssessor(emptyAssessmentProvider),
            new CommunicationSkillsAssessmentsEdFiAssessor(),
            new HigherOrderThinkingAssessmentsEdFiAssessor(emptyAssessmentProvider)
        };

        var frameworkElements = EwFrameworkDataElements.Elements;

        foreach (var assessor in assessors)
        {
            frameworkElements.Should().ContainKey(assessor.DataElementName,
                $"Assessor '{assessor.GetType().Name}' references data element '{assessor.DataElementName}' which must exist in the framework");
        }
    }

    [Fact]
    public void Should_PopulateRemarks_When_Assessed()
    {
        var emptyCteProvider = CreateCTEProviderWithData([]);
        var emptyCourseProvider = CreateCourseProviderWithData([]);
        var emptyAssessmentProvider = CreateAssessmentProviderWithData([]);

        var assessors = new IEdFiAssessor[]
        {
            new StudentCourseEnrollmentEdFiAssessor(),
            new CourseIdentifierEdFiAssessor(emptyCourseProvider),
            new CTECourseCompletionEdFiAssessor(emptyCteProvider),
            new CTECourseIdEdFiAssessor(emptyCourseProvider),
            new CTEPathwayEdFiAssessor(emptyCteProvider),
            new IndustryCredentialEdFiAssessor(),
            new WorkBasedLearningEdFiAssessor(emptyCteProvider),
            new HighSchoolGraduationDateEdFiAssessor(),
            new DigitalSkillsAssessmentsEdFiAssessor(emptyAssessmentProvider),
            new CommunicationSkillsAssessmentsEdFiAssessor(),
            new HigherOrderThinkingAssessmentsEdFiAssessor(emptyAssessmentProvider)
        };

        foreach (var assessor in assessors)
        {
            assessor.AssessmentDescription.Should().NotBeNullOrWhiteSpace(
                $"{assessor.GetType().Name} should have a non-empty AssessmentDescription");
        }
    }

    // --- Helpers ---

    private static EdFiCourse CreateCourse(
        string courseCode,
        string courseTitle = "",
        List<EdFiCourseAcademicSubject>? academicSubjects = null,
        string? careerPathwayDescriptor = null)
    {
        return new EdFiCourse(
            courseCode: courseCode,
            identificationCodes: [],
            educationOrganizationReference: new EdFiEducationOrganizationReference(1),
            courseTitle: courseTitle,
            numberOfParts: 1,
            academicSubjects: academicSubjects!,
            careerPathwayDescriptor: careerPathwayDescriptor!);
    }

    private static HttpClient CreateCcraHttpClient(
        List<EdFiAssessment> assessments, List<EdFiStudentAssessment> studentAssessments)
    {
        var handler = new FakeHttpMessageHandler(request =>
        {
            var uri = request.RequestUri!;
            var isStudentAssessmentsEndpoint = uri.AbsolutePath.Contains("studentAssessments");
            var isCountOnly = uri.Query.Contains("totalCount=true") && !uri.Query.Contains("limit=");

            if (isCountOnly)
            {
                var count = isStudentAssessmentsEndpoint ? studentAssessments.Count : assessments.Count;
                var countResponse = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]", Encoding.UTF8, "application/json")
                };
                countResponse.Headers.Add("total-count", count.ToString());
                return countResponse;
            }

            if (uri.Query.Contains("offset=0") || !uri.Query.Contains("offset"))
            {
                var data = isStudentAssessmentsEndpoint
                    ? (object)studentAssessments
                    : assessments;
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
                };
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            };
        });

        return new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
    }

    private static HttpClient CreateHttpClientWithJsonResponse<T>(List<T> data)
    {
        var handler = new FakeHttpMessageHandler(request =>
        {
            if (request.RequestUri!.Query.Contains("totalCount=true"))
            {
                var countResponse = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]", Encoding.UTF8, "application/json")
                };
                countResponse.Headers.Add("total-count", data.Count.ToString());
                return countResponse;
            }
            if (request.RequestUri.Query.Contains("offset=0") || !request.RequestUri.Query.Contains("offset"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
                };
            }
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            };
        });

        return new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
    }

    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }
}
