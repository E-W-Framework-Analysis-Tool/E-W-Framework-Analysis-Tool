namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public static class EcsElementMapping
{
    /// <summary>
    /// Maps an ECS data element name and sector context to a framework data element name.
    /// Returns null if no mapping exists.
    /// </summary>
    public static string? MapToFrameworkElement(string ecsElementName, string? sector)
    {
        var normalizedName = ecsElementName.Trim();

        // Check sector-specific mappings first
        if (sector != null && _sectorSpecificMappings.TryGetValue(normalizedName, out var sectorMap))
        {
            var normalizedSector = NormalizeSector(sector);
            if (normalizedSector != null && sectorMap.TryGetValue(normalizedSector, out var sectorResult))
                return sectorResult;

            // Fall back to default if sector doesn't match
            if (sectorMap.TryGetValue("default", out var defaultResult))
                return defaultResult;
        }

        // Check direct mappings
        if (_directMappings.TryGetValue(normalizedName, out var directResult))
            return directResult;

        // Check if it's already a valid framework element name
        if (EwFrameworkDataElements.Elements.ContainsKey(normalizedName))
            return normalizedName;

        return null;
    }

    private static string? NormalizeSector(string sector)
    {
        var lower = sector.Trim().ToLowerInvariant();
        return lower switch
        {
            "pk" or "pre-k" or "prek" => "PK",
            "k-12" or "k12" => "K-12",
            "ps" or "postsecondary" => "PS",
            "wf" or "workforce" => "WF",
            _ => null
        };
    }

    /// <summary>
    /// Mappings where the target framework element depends on the sector context.
    /// </summary>
    private static readonly Dictionary<string, Dictionary<string, string>> _sectorSpecificMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        ["In school suspension"] = new()
        {
            ["K-12"] = "Suspensions and expulsions (K-12)",
            ["PK"] = "Suspensions and expulsions (PK)",
            ["default"] = "Suspensions and expulsions (K-12)"
        },
        ["Out of school suspension"] = new()
        {
            ["K-12"] = "Suspensions and expulsions (K-12)",
            ["PK"] = "Suspensions and expulsions (PK)",
            ["default"] = "Suspensions and expulsions (K-12)"
        },
        ["Expulsions"] = new()
        {
            ["K-12"] = "Suspensions and expulsions (K-12)",
            ["PK"] = "Suspensions and expulsions (PK)",
            ["default"] = "Suspensions and expulsions (K-12)"
        },
        ["Expulsion"] = new()
        {
            ["K-12"] = "Suspensions and expulsions (K-12)",
            ["PK"] = "Suspensions and expulsions (PK)",
            ["default"] = "Suspensions and expulsions (K-12)"
        },
        ["Suspension"] = new()
        {
            ["K-12"] = "Suspensions and expulsions (K-12)",
            ["PK"] = "Suspensions and expulsions (PK)",
            ["default"] = "Suspensions and expulsions (K-12)"
        },
        ["Disciplinary use of restraint and seclusion"] = new()
        {
            ["K-12"] = "Restraint and seclusion (K-12)",
            ["PK"] = "Restraint and seclusion (PK)",
            ["default"] = "Restraint and seclusion (K-12)"
        },
        ["Student attendance rate"] = new()
        {
            ["K-12"] = "Student attendance rate (K-12)",
            ["PK"] = "Student attendance rate (PK)",
            ["default"] = "Student attendance rate (K-12)"
        },
        ["Student grade level"] = new()
        {
            ["K-12"] = "Student grade level (K-12)",
            ["PK"] = "Student grade level (PK)",
            ["default"] = "Student grade level (K-12)"
        },
        ["GPA"] = new()
        {
            ["K-12"] = "Grade point average (K-12)",
            ["PS"] = "Grade point average (Postsecondary)",
            ["default"] = "Grade point average (K-12)"
        },
        ["Grade point average"] = new()
        {
            ["K-12"] = "Grade point average (K-12)",
            ["PS"] = "Grade point average (Postsecondary)",
            ["default"] = "Grade point average (K-12)"
        },
    };

    /// <summary>
    /// Direct one-to-one mappings from ECS element names to framework element names.
    /// </summary>
    private static readonly Dictionary<string, string> _directMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        // Math/Reading proficiency mappings
        ["Math proficiency on state standardized test"] = "State standardized test (Math proficiency)",
        ["Math proficiency (or math assessment score)"] = "State standardized test (Math proficiency)",
        ["Reading proficiency on state standardized test"] = "State standardized test (Reading proficiency)",
        ["Reading proficiency (or reading assessment score)"] = "State standardized test (Reading proficiency)",

        // Course performance consolidation
        ["Course performance in English"] = "Course performance (English and Math)",
        ["Course performance in Math"] = "Course performance (English and Math)",

        // PK assessments
        ["Results on direct child assessment (cognition)"] = "Direct child assessments (cognition)",
        ["Results on direct child assessment (language and literacy)"] = "Direct child assessments (language and literacy)",
        ["Results on kindergarten readiness assessment (cognition)"] = "Kindergarten readiness assessments (cognition)",
        ["Results on teacher-reported kindergarten readiness assessment (language and literacy)"] = "Teacher-reported kindergarten readiness assessments (language and literacy)",

        // Course and enrollment mappings
        ["Course ID or course title"] = "Course identifier or title",
        ["Course failure"] = "Course outcome",
        ["Course passage or completion"] = "Course outcome",
        ["Age (or birth date)"] = "Age",
        ["AP enrollment"] = "Student course enrollment record",
        ["IB enrollment"] = "Student course enrollment record",
        ["Dual credit enrollment"] = "Dual credit course designation",

        // PS institution rate
        ["PS institution graduation rate, by race/ethnicity and Pell grant receipt"] = "PS institution graduation rate; by race/ethnicity and Pell grant receipt",

        // Direct matches (ECS name matches framework name exactly)
        ["ACT completion"] = "ACT completion",
        ["ACT score"] = "ACT score",
        ["SAT completion"] = "SAT completion",
        ["SAT score"] = "SAT score",
        ["AP course passage"] = "AP course passage",
        ["AP credit earned (or AP test scores)"] = "AP credit earned (or AP test scores)",
        ["IB course passage"] = "IB course passage",
        ["IB credit earned (or IB test scores)"] = "IB credit earned (or IB test scores)",
        ["Dual credit course passage"] = "Dual credit course passage",
        ["Dual credit earned"] = "Dual credit earned",
        ["High school graduation date"] = "High school graduation date",
        ["High school graduation indicator"] = "High school graduation indicator",
        ["High school diploma type"] = "High school diploma type",
        ["Diploma or credential award date"] = "Diploma or credential award date",
        ["FAFSA completion date"] = "FAFSA completion date",
        ["Postsecondary applications submitted"] = "Postsecondary applications submitted",
        ["Postsecondary enrollment date"] = "Postsecondary enrollment date",
        ["Postsecondary Institution ID"] = "Postsecondary Institution ID",
        ["Reported intent to enroll in postsecondary education"] = "Reported intent to enroll in postsecondary education",
        ["Enrollment in public pre-K"] = "Enrollment in public pre-K",
        ["Pre-K eligbility status"] = "Pre-K eligbility status",
        ["Cohort graduation year"] = "Cohort graduation year",
        ["Cohort year"] = "Cohort year",
        ["Enrollment date"] = "Enrollment date",
        ["First-time 9th grade student status"] = "First-time 9th grade student status",
    };
}
