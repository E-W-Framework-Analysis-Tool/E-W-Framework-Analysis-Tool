using System.Reflection;
using System.Text;
using EwFrameworkAnalysis.Common.Assessors.Ceds;

namespace EwFrameworkAnalysis.Common.Services;

public class CedsDWAssessmentOrchestrator
{
    /// <summary>
    /// Discovers all ICedsDWAssessor implementations via reflection
    /// </summary>
    public List<ICedsDWAssessor> DiscoverAssessors()
    {
        var assessorType = typeof(ICedsDWAssessor);
        var assessors = Assembly.GetAssembly(assessorType)!
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && assessorType.IsAssignableFrom(t))
            .Select(t => (ICedsDWAssessor)Activator.CreateInstance(t)!)
            .ToList();

        return assessors;
    }

    /// <summary>
    /// Generates a UNION ALL query combining all discovered assessors
    /// </summary>
    public string GenerateUnionedQuery()
    {
        var assessors = DiscoverAssessors();
        return GenerateUnionedQuery(assessors);
    }

    /// <summary>
    /// Generates a UNION ALL query combining the provided assessors
    /// </summary>
    /// <param name="assessors">List of assessors to include in the query</param>
    public string GenerateUnionedQuery(List<ICedsDWAssessor> assessors)
    {
        if (assessors.Count == 0)
        {
            throw new InvalidOperationException("No ICedsDWAssessor implementations provided");
        }

        var sb = new StringBuilder();

        // Add header with instructions
        sb.AppendLine("/*");
        sb.AppendLine("===================================================================================");
        sb.AppendLine("CEDS DATA WAREHOUSE ASSESSMENT QUERY");
        sb.AppendLine("===================================================================================");
        sb.AppendLine();
        sb.AppendLine("INSTRUCTIONS:");
        sb.AppendLine("1. Copy this entire query into SQL Server Management Studio (SSMS)");
        sb.AppendLine("2. Connect to your CEDS Data Warehouse database");
        sb.AppendLine("3. Execute the query");
        sb.AppendLine("4. Export results to CSV:");
        sb.AppendLine("   - Right-click on results grid → Save Results As...");
        sb.AppendLine("   - Choose 'CSV (Comma delimited)' as file type");
        sb.AppendLine("   - Save with a descriptive name (e.g., 'CEDS_Assessment_2025-10-02.csv')");
        sb.AppendLine("5. Import the CSV file back into the application");
        sb.AppendLine();
        sb.AppendLine("EXPECTED CSV FORMAT:");
        sb.AppendLine("The query produces 4 columns:");
        sb.AppendLine("  - DataElementName: Name of the data element being assessed");
        sb.AppendLine("  - CharacteristicType: Type of measurement (e.g., 'RecordCount')");
        sb.AppendLine("  - Value: The measured value as a string");
        sb.AppendLine("  - Remarks: Optional notes or comments");
        sb.AppendLine();
        sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Assessor Count: {assessors.Count}");
        sb.AppendLine("===================================================================================");
        sb.AppendLine("*/");
        sb.AppendLine();

        // Build UNION ALL query
        for (var i = 0; i < assessors.Count; i++)
        {
            var assessor = assessors[i];

            if (i > 0)
            {
                sb.AppendLine();
                sb.AppendLine("UNION ALL");
                sb.AppendLine();
            }

            // Add comment for this assessor
            sb.AppendLine($"-- {assessor.DataElementName}");
            sb.AppendLine($"-- {assessor.AssessmentDescription}");

            // Add the query (trim to remove extra whitespace)
            sb.AppendLine(assessor.Query.Trim());
        }

        // Add ORDER BY at the end for better readability
        sb.AppendLine();
        sb.AppendLine("ORDER BY DataElementName, CharacteristicType;");

        return sb.ToString();
    }
}
