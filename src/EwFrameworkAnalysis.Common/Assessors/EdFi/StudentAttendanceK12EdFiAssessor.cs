using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;

namespace EwFrameworkAnalysis.Common.Assessors.EdFi;

public class StudentAttendanceK12EdFiAssessor : IEdFiAssessor
{
    private static readonly HashSet<string> _presentDescriptors =
    [
        "uri://ed-fi.org/AttendanceEventCategoryDescriptor#In Attendance",
        "uri://ed-fi.org/AttendanceEventCategoryDescriptor#Present"
    ];

    private static readonly HashSet<string> _absentDescriptors =
    [
        "uri://ed-fi.org/AttendanceEventCategoryDescriptor#Excused Absence",
        "uri://ed-fi.org/AttendanceEventCategoryDescriptor#Unexcused Absence"
    ];

    public string DataElementName => "Student attendance rate (K-12)";

    public string AssessmentDescription =>
        "Analysis of student attendance events including category distribution and student coverage";

    public async Task<DataElementAssessment> AssessAsync(
        HttpClient httpClient,
        DataSource dataSource,
        AssessorContext context)
    {
        context.ReportProgress(0, "Counting students...");

        var totalStudents = await EdFiApiPatterns.CountFromHeaderAsync(
            httpClient,
            "ed-fi/students"
        );

        context.ReportProgress(20, "Analyzing attendance events...");

        var totalEvents = 0;
        var studentsWithAttendance = new HashSet<string>();
        var categoryDistribution = new Dictionary<string, int>();
        var presenceDistribution = new Dictionary<string, int>
        {
            ["Present"] = 0,
            ["Absent"] = 0,
            ["Other"] = 0
        };

        await EdFiApiPatterns.PageAndProcessAsync<EdFiStudentSchoolAttendanceEvent>(
            httpClient,
            "ed-fi/studentSchoolAttendanceEvents",
            attendanceEvent =>
            {
                totalEvents++;

                // Track unique students
                if (attendanceEvent.StudentReference?.StudentUniqueId != null)
                {
                    studentsWithAttendance.Add(attendanceEvent.StudentReference.StudentUniqueId);
                }

                var descriptor = attendanceEvent.AttendanceEventCategoryDescriptor ?? "Unknown";

                // Full category distribution
                var category = descriptor.Split('#').LastOrDefault() ?? descriptor;
                categoryDistribution.TryAdd(category, 0);
                categoryDistribution[category]++;

                // Simplified presence distribution
                if (_presentDescriptors.Contains(descriptor))
                    presenceDistribution["Present"]++;
                else if (_absentDescriptors.Contains(descriptor))
                    presenceDistribution["Absent"]++;
                else
                    presenceDistribution["Other"]++;
            },
            context
        );

        var studentsWithAttendanceCount = studentsWithAttendance.Count;
        var coveragePercent = totalStudents > 0
            ? (studentsWithAttendanceCount * 100.0 / totalStudents)
            : 0;
        var avgEventsPerStudent = studentsWithAttendanceCount > 0
            ? (totalEvents * 1.0 / studentsWithAttendanceCount)
            : 0;

        context.Log($"Attendance Analysis:");
        context.Log($"  Total students: {totalStudents:N0}");
        context.Log($"  Students with attendance: {studentsWithAttendanceCount:N0} ({coveragePercent:F1}%)");
        context.Log($"  Total events: {totalEvents:N0}");
        context.Log($"  Avg events per student: {avgEventsPerStudent:F1}");
        context.Log($"");
        context.Log($"Presence Distribution:");
        context.Log($"  Present: {presenceDistribution["Present"]:N0} ({(presenceDistribution["Present"] * 100.0 / Math.Max(totalEvents, 1)):F1}%)");
        context.Log($"  Absent: {presenceDistribution["Absent"]:N0} ({(presenceDistribution["Absent"] * 100.0 / Math.Max(totalEvents, 1)):F1}%)");
        context.Log($"  Other: {presenceDistribution["Other"]:N0} ({(presenceDistribution["Other"] * 100.0 / Math.Max(totalEvents, 1)):F1}%)");
        context.Log($"");
        context.Log($"Top Event Categories:");
        foreach (var kvp in categoryDistribution.OrderByDescending(x => x.Value).Take(5))
        {
            context.Log($"  {kvp.Key}: {kvp.Value:N0} ({(kvp.Value * 100.0 / totalEvents):F1}%)");
        }

        // Data quality observations
        if (coveragePercent < 80)
        {
            context.Log($"NOTE: {coveragePercent:F1}% of students have attendance records. For full attendance analysis, higher coverage is needed.");
        }

        if (presenceDistribution["Present"] == 0 && presenceDistribution["Absent"] > 0)
        {
            context.Log("NOTE: Only absence events found. This may indicate negative attendance tracking (only absences recorded).");
        }
        else if (presenceDistribution["Present"] == 0 && presenceDistribution["Absent"] == 0)
        {
            context.Log("NOTE: No standard present/absent events found. Check attendance event category descriptors.");
        }

        context.ReportProgress(100, "Complete");

        return new DataElementAssessment
        {
            DataElementName = DataElementName,
            Characteristics = [
                new RecordCount(totalEvents),
                new Distribution(presenceDistribution, "Presence Status"),
                new Distribution(categoryDistribution, "Event Category")
            ]
        };
    }
}
