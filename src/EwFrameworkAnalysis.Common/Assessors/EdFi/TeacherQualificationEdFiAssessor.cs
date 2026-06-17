using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class TeacherQualificationEdFiAssessor : IEdFiAssessor
{
    private static readonly HashSet<string> _targetCredentialTypes =
    [
        "uri://ed-fi.org/CredentialTypeDescriptor#Certification",
        "uri://ed-fi.org/CredentialTypeDescriptor#Endorsement",
        "uri://ed-fi.org/CredentialTypeDescriptor#Licensure",
        "uri://ed-fi.org/CredentialTypeDescriptor#Registration",
        "uri://ed-fi.org/CredentialTypeDescriptor#Other"
    ];

    public string DataElementName => "Teacher qualification or certification type";

    public string AssessmentDescription =>
        "Iterates over all staff and identifies those holding a recognized teacher qualification " +
        "credential (Certification, Endorsement, Licensure, Registration, or Other). Each staff's " +
        "embedded credential references are resolved against the ed-fi/credentials resource to " +
        "determine the CredentialTypeDescriptor; student credential types such as High School " +
        "Diploma are excluded. Reports total staff scanned and the number of staff holding each " +
        "qualification type.";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient, DataSource dataSource, AssessorContext context)
    {
        context.ReportProgress(0, "Building credential type lookup...");

        // The credential type lives on the credential resource, not on the staff's embedded
        // credential reference, so first map each credential's natural key to its type descriptor.
        var credentialTypeByKey = new Dictionary<string, string>();
        await EdFiApiPatterns.PageAndProcessAsync<EdFiCredential>(
            httpClient,
            "ed-fi/credentials",
            credential =>
            {
                if (string.IsNullOrWhiteSpace(credential.CredentialTypeDescriptor))
                    return;

                var key = CredentialKey(
                    credential.CredentialIdentifier,
                    credential.StateOfIssueStateAbbreviationDescriptor);
                credentialTypeByKey[key] = credential.CredentialTypeDescriptor;
            },
            context);

        context.ReportProgress(50, "Scanning staff for qualifying credentials...");

        var totalStaff = 0;
        var staffWithQualification = 0;
        var distribution = new Dictionary<string, int>();

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStaff>(
            httpClient,
            "ed-fi/staffs",
            staff =>
            {
                totalStaff++;

                if (staff.Credentials is not { Count: > 0 })
                    return;

                // A staff member may hold several credentials; count each qualification type once.
                var matchedTypes = new HashSet<string>();
                foreach (var staffCredential in staff.Credentials)
                {
                    var reference = staffCredential.CredentialReference;
                    if (reference is null)
                        continue;

                    var key = CredentialKey(
                        reference.CredentialIdentifier,
                        reference.StateOfIssueStateAbbreviationDescriptor);

                    if (credentialTypeByKey.TryGetValue(key, out var type) &&
                        _targetCredentialTypes.Contains(type))
                    {
                        matchedTypes.Add(type);
                    }
                }

                if (matchedTypes.Count == 0)
                    return;

                staffWithQualification++;
                foreach (var type in matchedTypes)
                {
                    var label = EdFiDescriptorHelper.ParseDescriptorValue(type);
                    distribution[label] = distribution.GetValueOrDefault(label) + 1;
                }
            },
            context);

        var completeness = new Completeness(totalStaff, staffWithQualification, "Qualified Staff");
        context.Log($"Found {staffWithQualification:N0} of {totalStaff:N0} staff ({completeness.Percentage:F1}%) holding a recognized teacher qualification credential");
        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics =
            [
                new RecordCount(totalStaff),
                new Distribution(distribution, "Credential Type")
            ]
        };
    }

    private static string CredentialKey(string? credentialIdentifier, string? stateOfIssueDescriptor) =>
        $"{credentialIdentifier}|{stateOfIssueDescriptor}";
}
