using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using System.Text.Json.Serialization;
using EwFrameworkAnalysis.Common.Models;
using Flurl;
using Microsoft.Extensions.Caching.Memory;

namespace EwFrameworkAnalysis.Common.Services;

public class EdFiApiDataItemAvailabilityProvider : IDataAvailabilityProvider<DataElementDataItem>
{
    public static string ProviderName => "Ed-Fi ODS API";
    public string Name => ProviderName;

    private readonly string? _clientId;
    private readonly string? _clientSecret;
    private readonly string _authUrl;
    private readonly string _baseUrl;
    private string? _accessToken;

    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly string _cacheKeyIdentifierPrefix;

#pragma warning disable CS0067 // Event is never used - required by interface
    public event Action OnCacheHit = delegate { };
    public event Action<ProviderStatusUpdate> OnStatusUpdate = delegate { };
#pragma warning restore CS0067

    public EdFiApiDataItemAvailabilityProvider(HttpClient httpClient, IMemoryCache cache, string baseUrl, string? clientId = null, string? clientSecret = null, string? tempAccessToken = null, string? authUrl = null)
    {
        if ((clientId == null || clientSecret == null) && tempAccessToken == null)
        {
            throw new ArgumentNullException($"Either {nameof(clientId)} and {nameof(clientSecret)} or {nameof(tempAccessToken)} must be provided");
        }
        _httpClient = httpClient;
        _clientId = clientId;
        _clientSecret = clientSecret;
        _accessToken = tempAccessToken;
        _baseUrl = baseUrl;
        _authUrl = authUrl ?? $"{baseUrl}/oauth/token";
        _cache = cache;

        _cacheKeyIdentifierPrefix = $"EdFiApiDataItemAvailabilityProvider_{CacheKeyUtility.CalculateHash(_clientId ?? "")}_{CacheKeyUtility.CalculateHash(_accessToken ?? "")}";
    }

    private static HashSet<string> ExplusionDescriptorValues =>
    [
        "uri://ed-fi.org/DisciplineDescriptor#Expulsion",
        "uri://ed-fi.org/DisciplineDescriptor#Expulsion with Services",
        "uri://ed-fi.org/DisciplineDescriptor#Expulsion under Guns Free School Act",
        "uri://ed-fi.org/DisciplineDescriptor#Expulsion under Guns Free School Act with Services"
    ];

    private static HashSet<string> SuspensionDescriptorValues =>
    [
        "uri://ed-fi.org/DisciplineDescriptor#Out of School Suspension",
        "uri://ed-fi.org/DisciplineDescriptor#In School Suspension"
    ];

    public async Task<DataAvailabilityCheckResult> CheckDataAvailability(DataElementDataItem dataItem)
    {
        string? remarks;
        int count;

        switch (dataItem.DataElement.Name)
        {
            case DataElementNames.Attendance:
                remarks = "Determined by the presence of any `studentSchoolAttendanceEvents`";
                count = await GetResourceCountAsync("studentSchoolAttendanceEvents");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.CTEEnrollment:
                remarks = "Determined by the presence of any `studentCTEProgramAssociations`";
                count = await GetResourceCountAsync("studentCTEProgramAssociations");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.CollegeAdmittance:
                remarks = "Determined by the presence of any `postSecondaryEvents` with a standard category descriptor of `College Acceptance`";
                count = await GetResourceCountAsync("postSecondaryEvents", new { postSecondaryEventCategoryDescriptor = "uri://ed-fi.org/PostSecondaryEventCategoryDescriptor#College Acceptance" });
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.CollegeApplicationsSubmitted:
                remarks = "Determined by the presence of any `postSecondaryEvents` with a standard category descriptor of `College Application`";
                count = await GetResourceCountAsync("postSecondaryEvents", new { postSecondaryEventCategoryDescriptor = "uri://ed-fi.org/PostSecondaryEventCategoryDescriptor#College Application" });
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.CollegeCreditsEarned:
                remarks = "Determined by the presence of `courseTranscripts` where `earnedAdditionalCredits` is present; up to 5 pages of results are fetched and examined";
                count = await GetResourceCountAsync("courseTranscripts", customFilter: element =>
                {
                    if (element.TryGetProperty("earnedAdditionalCredits", out var creditsArray))
                    {
                        return creditsArray.GetArrayLength() > 0;
                    }
                    return false;
                });
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            //TODO: Look up assessments of this category and then look up existance of any results for any of them.
            case DataElementNames.CollegeEntranceAssessment:
                // uri://ed-fi.org/AssessmentCategoryDescriptor#College entrance exam
                remarks = "Not currently supported; requires implementation to look up `studentAssessments` associated with assessments categorized as `College entrance exam`";
                return DataAvailabilityCheckResult.NotSupported;

            case DataElementNames.CourseCompletion:
                remarks = "Determined by the presence of any `courseTranscripts`; specific course completion (e.g., Algebra I by 9th grade) is not assessed";
                count = await GetResourceCountAsync("courseTranscripts");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.CourseSubject:
                remarks = "Determined by the precense of any `courses` where `academicSubjects` array is present and not empty; up to 5 pages of results are fetched and examined";
                count = await GetResourceCountAsync("courses", customFilter: element =>
                {
                    if (element.TryGetProperty("academicSubjects", out var subjArray))
                    {
                        return subjArray.GetArrayLength() > 0;
                    }
                    return false;
                });
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.DisciplinaryEventType:
                remarks = "Determined by the presence of any `studentDisciplineIncidentBehaviorAssociations`";
                count = await GetResourceCountAsync("studentDisciplineIncidentBehaviorAssociations");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.EnglishLearningStatusDates:
                remarks = "Determined by the precense of any `studentLanguageInstructionProgramAssociations` where `englishLearnerParticipation` is present; up to 5 pages of results are fetched and examined";
                count = await GetResourceCountAsync("studentLanguageInstructionProgramAssociations", customFilter: element =>
                {
                    return element.TryGetProperty("englishLearnerParticipation", out _);
                });
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.EnrollmentDatesK12:
                remarks = "Determined by the presence of any `studentSchoolAssociations`";
                count = await GetResourceCountAsync("studentSchoolAssociations");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.EnrollmentGradeLevel:
                remarks = "Determined by the presence of any `studentSchoolAssociations`";
                count = await GetResourceCountAsync("studentSchoolAssociations");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.Expulsion:
                remarks = "Determined by the presence of any `disciplineActions` where `disciplineDescriptor` indicates an expulsion; up to 5 pages of results are fetched and examined";
                count = await GetResourceCountAsync("disciplineActions", customFilter: element =>
                {
                    if (element.TryGetProperty("disciplines", out var disciplinesArray) && disciplinesArray.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var discipline in disciplinesArray.EnumerateArray())
                        {
                            if (discipline.TryGetProperty("disciplineDescriptor", out var disciplineDescriptor) &&
                                ExplusionDescriptorValues.Contains(disciplineDescriptor.GetString()!))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                });
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.FinancialAidApplications:
                throw new NotImplementedException();

            case DataElementNames.GradeLevelAttempt:
                remarks = "Determined by the presence of any `studentSchoolAssociations`";
                count = await GetResourceCountAsync("studentSchoolAssociations");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.GradePointAverageK12:
                remarks = "Determined by the presence of any `studentAcademicRecords` where `gradePointAverages` array is present and not empty; up to 5 pages of results are fetched and examined";
                count = await GetResourceCountAsync("studentAcademicRecords", customFilter: element =>
                {
                    return element.TryGetProperty("gradePointAverages", out var gpaArray)
                        && gpaArray.ValueKind == JsonValueKind.Array
                        && gpaArray.GetArrayLength() > 0;
                });

                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.KindergartenProgramDesignation:
                remarks = "Determined by the presence of any `studentSchoolAssociations`";
                count = await GetResourceCountAsync("studentSchoolAssociations");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.ParentSurveys:
                remarks = "Determined by the presence of any `studentSchoolAssociations`";
                count = await GetResourceCountAsync("surveyResponses");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.PersonAge:
                remarks = "Determined by the presence of any `students`";
                count = await GetResourceCountAsync("students");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.SectionGradesELA:
            case DataElementNames.SectionGradesMath:
                remarks = "Determined by the presence of any `courseTranscripts`; academic subject is not assessed";
                count = await GetResourceCountAsync("courseTranscripts");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            // TODO: Implement assessment lookups and student participation checks by assessment
            case DataElementNames.StandardizedAssessmentsELA:
                // assessments
                // "academicSubjects": [{"academicSubjectDescriptor": "uri://ed-fi.org/AcademicSubjectDescriptor#English Language Arts"}]
                // "assessmentCategoryDescriptor": "uri://ed-fi.org/AssessmentCategoryDescriptor#State assessment"
                remarks = "Determined by the presence of any `studentAssessments`; specific assessment category or academic subject is not assessed";
                count = await GetResourceCountAsync("studentAssessments");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.StandardizedAssessmentsMathGrades1to2:
                // assessments
                // "academicSubjects": [{"academicSubjectDescriptor": "uri://ed-fi.org/AcademicSubjectDescriptor#Mathematics"}]
                // "assessmentCategoryDescriptor": "uri://ed-fi.org/AssessmentCategoryDescriptor#State assessment"
                remarks = "Determined by the presence of any `studentAssessments` where `whenAssessedGradeLevelDescriptor` indicates First Grade or Second Grade; specific assessment category or academic subject is not assessed";
                count = await GetResourceCountAsync("studentAssessments", new { whenAssessedGradeLevelDescriptor = "uri://ed-fi.org/GradeLevelDescriptor#First grade" });
                count += await GetResourceCountAsync("studentAssessments", new { whenAssessedGradeLevelDescriptor = "uri://ed-fi.org/GradeLevelDescriptor#Second grade" });
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.StandardizedAssessmentsMathGrade3:
                remarks = "Determined by the presence of any `studentAssessments` where `whenAssessedGradeLevelDescriptor` indicates Third Grade; specific assessment category or academic subject is not assessed";
                count = await GetResourceCountAsync("studentAssessments", new { whenAssessedGradeLevelDescriptor = "uri://ed-fi.org/GradeLevelDescriptor#Third grade" });
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.StandardizedAssessmentsMathGrade8:
                remarks = "Determined by the presence of any `studentAssessments` where `whenAssessedGradeLevelDescriptor` indicates Eighth Grade; specific assessment category or academic subject is not assessed";
                count = await GetResourceCountAsync("studentAssessments", new { whenAssessedGradeLevelDescriptor = "uri://ed-fi.org/GradeLevelDescriptor#Eighth grade" });
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.StandardizedAssessmentsReadingGrades1to2:
                // academicSubjectDescriptor: Reading
                // "uri://ed-fi.org/AssessmentCategoryDescriptor#State assessment"
                remarks = "Determined by the presence of any `studentAssessments` where `whenAssessedGradeLevelDescriptor` indicates First Grade or Second Grade; specific assessment category or academic subject is not assessed";
                count = await GetResourceCountAsync("studentAssessments", new { whenAssessedGradeLevelDescriptor = "uri://ed-fi.org/GradeLevelDescriptor#First grade" });
                count += await GetResourceCountAsync("studentAssessments", new { whenAssessedGradeLevelDescriptor = "uri://ed-fi.org/GradeLevelDescriptor#Second grade" });
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.StandardizedAssessmentsReadingGrade3:
                remarks = "Determined by the presence of any `studentAssessments` where `whenAssessedGradeLevelDescriptor` indicates Third Grade; specific assessment category or academic subject is not assessed";
                count = await GetResourceCountAsync("studentAssessments", new { whenAssessedGradeLevelDescriptor = "uri://ed-fi.org/GradeLevelDescriptor#Third grade" });
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.StandardizedAssessmentsReadingGrade8:
                remarks = "Determined by the presence of any `studentAssessments` where `whenAssessedGradeLevelDescriptor` indicates Eighth Grade; specific assessment category or academic subject is not assessed";
                count = await GetResourceCountAsync("studentAssessments", new { whenAssessedGradeLevelDescriptor = "uri://ed-fi.org/GradeLevelDescriptor#Eighth grade" });
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.StudentRoster:
                remarks = "Determined by the presence of any `studentSectionAssociations`";
                count = await GetResourceCountAsync("studentSectionAssociations");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.StudentTeachingSurveys:
                remarks = "Determined by the presence of any `surveyResponseStaffTargetAssociations`";
                count = await GetResourceCountAsync("surveyResponseStaffTargetAssociations");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.Suspension:
                remarks = "Determined by the presence of any `disciplineActions` where `disciplineDescriptor` indicates suspension; up to 5 pages of results are fetched and examined";
                count = await GetResourceCountAsync("disciplineActions", customFilter: element =>
                {
                    if (element.TryGetProperty("disciplines", out var disciplinesArray) && disciplinesArray.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var discipline in disciplinesArray.EnumerateArray())
                        {
                            if (discipline.TryGetProperty("disciplineDescriptor", out var disciplineDescriptor) &&
                                SuspensionDescriptorValues.Contains(disciplineDescriptor.GetString()!))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                });
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.TeacherCredentials:
                remarks = "Determined by the presence of any `staffs`";
                count = await GetResourceCountAsync("staffs");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.TeacherEmploymentDates:
                remarks = "Determined by the presence of any `staffEducationOrganizationEmploymentAssociations`";
                count = await GetResourceCountAsync("staffEducationOrganizationEmploymentAssociations");
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.TeacherExperience:
                remarks = "Determined by the presence of any `staffs`";
                count = await GetResourceCountAsync("staffs", customFilter: element =>
                {
                    return element.TryGetProperty("yearsOfPriorTeachingExperience", out _);
                });
                return count > 0
                    ? DataAvailabilityCheckResult.DataAvailable(count, remarks)
                    : DataAvailabilityCheckResult.DataUnavailable(remarks);

            case DataElementNames.ApprenticeshipEnrollment:
            case DataElementNames.CareerReadinessAssessment:
            case DataElementNames.ChildCareSubsidyEligibility:
            case DataElementNames.ChildCareSubsidyUtilization:
            case DataElementNames.ClassroomObservation:
            case DataElementNames.CourseAvailability:
            case DataElementNames.CourseCollegeCreditsOffered:
            case DataElementNames.CTECredentialEarned:
            case DataElementNames.CTEProgramRequirements:
            case DataElementNames.CulturalCompetencyAssessment:
            case DataElementNames.DegreeCourseRequirements:
            case DataElementNames.DevelopmentalAssessments:
            case DataElementNames.DigitalSkillsAssessment:
            case DataElementNames.EarlyLearningProgramDesignation:
            case DataElementNames.EmploymentAndEarnings:
            case DataElementNames.EnrollmentDatesPreK:
            case DataElementNames.EnrollmentDatesPostsecondary:
            case DataElementNames.EnrollmentTypePostsecondary:
            case DataElementNames.GradePointAveragePostsecondary:
            case DataElementNames.HighSchoolGraduation:
            case DataElementNames.HighestEducationLevelCompletionDate:
            case DataElementNames.JobQualityIndex:
            case DataElementNames.MedianEarningsForHSGraduatesByState:
            case DataElementNames.MedianNetWealthByState:
            case DataElementNames.MilitaryEnlistment:
            case DataElementNames.NetWealth:
            case DataElementNames.NetPriceOfEducation:
            case DataElementNames.PrincipalEmploymentDates:
            case DataElementNames.PopulationDemographicsByAgeAndELEligibility:
            case DataElementNames.PostGraduationPlans:
            case DataElementNames.PostsecondaryCredentialEarned:
            case DataElementNames.PostsecondaryProgramLength:
            case DataElementNames.PreKProgramQualityBenchmarks:
            case DataElementNames.ProgramFundingSourcePreK:
            case DataElementNames.SelfAssessmentSurvey:
            case DataElementNames.StaffSurveys:
            case DataElementNames.StudentLoanDetails:
            case DataElementNames.TransferPostsecondary:
            case DataElementNames.WorkBasedLearningOpportunityParticipation:
                return DataAvailabilityCheckResult.NotSupported;

            default:
                // Separate from something being reported as intentionally not
                // supported by the underlying provider -- this would mean an
                // unexpected data element name came through.
                throw new NotImplementedException("Unhandled data element");
        }
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var requestBody = new
        {
            grant_type = "client_credentials",
            client_id = _clientId,
            client_secret = _clientSecret
        };

        var response = await _httpClient.PostAsJsonAsync(_authUrl, requestBody);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Failed to obtain access token. Status code: {response.StatusCode}");
        }

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return authResponse?.AccessToken ?? throw new Exception("Access token not found in response.");
    }

    private async Task<int> GetResourceCountAsync(
        string resourceName,
        object? queryParameters = null,
        Func<JsonElement, bool>? customFilter = null,
        int pagingLimitForCustomFilter = 5)
    {
        var queryParamsHash = queryParameters != null
            ? string.Join("&", queryParameters.GetType().GetProperties().Select(p => $"{p.Name}={p.GetValue(queryParameters)}"))
            : "noQueryParams";
        var customFilterHash = customFilter != null
            ? customFilter.Method.GetHashCode().ToString()
            : "noCustomFilter";

        // Create a cache key using the hashed client ID and resource name
        var cacheKey = $"{_cacheKeyIdentifierPrefix}_{resourceName}_{queryParamsHash}_{customFilterHash}";

        // Attempt to get the result from cache
        if (_cache.TryGetValue(cacheKey, out int cachedCount))
        {
            OnCacheHit();
            return cachedCount;
        }

        _accessToken ??= await GetAccessTokenAsync();

        // If no custom filter is provided, use the total-count header
        if (customFilter == null)
        {
            var requestUri = $"{_baseUrl}/data/v3/ed-fi/{resourceName}"
                .SetQueryParams(new
                {
                    offset = 0,
                    limit = 0,
                    totalCount = true
                });

            if (queryParameters != null)
            {
                requestUri = requestUri.SetQueryParams(queryParameters);
            }

            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            requestMessage.Headers.Add("Use-Snapshot", "false");

            using var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();

            // Extract the 'total-count' header
            if (response.Headers.TryGetValues("total-count", out var totalCountHeaders) &&
                int.TryParse(totalCountHeaders.FirstOrDefault(), out var totalCount))
            {
                _cache.Set(cacheKey, totalCount, TimeSpan.FromMinutes(30));
                return totalCount;
            }

            throw new Exception("Total count header not found in response.");
        }
        else
        {
            // Handle pagination if a custom filter is provided
            var totalCount = 0;
            var offset = 0;
            const int defaultLimit = 100; // Set page size for API requests
            var pagesFetched = 0;
            var morePages = true;

            while (morePages && pagesFetched < pagingLimitForCustomFilter)
            {
                // Build request URI with pagination
                var requestUri = $"{_baseUrl}/data/v3/ed-fi/{resourceName}"
                    .SetQueryParams(new
                    {
                        offset,
                        limit = defaultLimit,
                        totalCount = false // No need for totalCount here
                    });

                if (queryParameters != null)
                {
                    requestUri = requestUri.SetQueryParams(queryParameters);
                }

                using var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                requestMessage.Headers.Add("Use-Snapshot", "false");

                // Send the request and get the response
                using var response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();

                // Parse the response as JSON
                using var jsonDoc = JsonDocument.Parse(responseBody);
                var jsonArray = jsonDoc.RootElement;

                // Loop through each item in the JSON array
                foreach (var element in jsonArray.EnumerateArray())
                {
                    // Apply the custom filter if provided
                    if (customFilter(element))
                    {
                        totalCount++;
                    }
                }

                // Check if there are more pages to fetch
                if (jsonArray.GetArrayLength() < defaultLimit)
                {
                    morePages = false;
                }
                else
                {
                    offset += defaultLimit;
                    pagesFetched++;
                }
            }

            _cache.Set(cacheKey, totalCount, TimeSpan.FromMinutes(30));
            return totalCount;
        }
    }


    private class AuthResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;
    }
}
