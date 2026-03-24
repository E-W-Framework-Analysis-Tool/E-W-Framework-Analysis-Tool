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
    /// Generates a temp-table-based script combining all discovered assessors
    /// </summary>
    public string GenerateAssessmentScript()
    {
        var assessors = DiscoverAssessors();
        return GenerateAssessmentScript(assessors);
    }

    /// <summary>
    /// Generates a temp-table-based assessment script from the provided assessors.
    /// Creates #EWFProfilerResults, runs each assessor's Query verbatim, then selects all rows.
    /// Each assessor is responsible for inserting its own rows via INSERT INTO #EWFProfilerResults.
    /// </summary>
    public string GenerateAssessmentScript(List<ICedsDWAssessor> assessors)
    {
        if (assessors.Count == 0)
            throw new InvalidOperationException("No ICedsDWAssessor implementations provided");

        var sb = new StringBuilder();

        AppendHeader(sb, assessors.Count);
        AppendTempTableCreation(sb);

        foreach (var assessor in assessors)
        {
            sb.AppendLine($"-- {assessor.DataElementName}");
            sb.AppendLine($"-- {assessor.AssessmentDescription}");
            sb.AppendLine(assessor.Query.Trim());
            sb.AppendLine(";");
            sb.AppendLine();
        }

        AppendFinalSelect(sb);

        return sb.ToString();
    }

    private static void AppendTempTableCreation(StringBuilder sb)
    {
        sb.AppendLine("DROP TABLE IF EXISTS #EWFProfilerResults;");
        sb.AppendLine();
        sb.AppendLine("CREATE TABLE #EWFProfilerResults");
        sb.AppendLine("(");
        sb.AppendLine("    DataElementName    NVARCHAR(MAX),");
        sb.AppendLine("    CharacteristicType NVARCHAR(MAX),");
        sb.AppendLine("    Value              NVARCHAR(MAX),");
        sb.AppendLine("    SubItemLabel       NVARCHAR(MAX),");
        sb.AppendLine("    Remarks            NVARCHAR(MAX)");
        sb.AppendLine(");");
        sb.AppendLine();
    }

    private static void AppendFinalSelect(StringBuilder sb)
    {
        sb.AppendLine("SELECT");
        sb.AppendLine("    DataElementName,");
        sb.AppendLine("    CharacteristicType,");
        sb.AppendLine("    Value,");
        sb.AppendLine("    SubItemLabel,");
        sb.AppendLine("    Remarks");
        sb.AppendLine("FROM #EWFProfilerResults");
        sb.AppendLine("ORDER BY DataElementName, CharacteristicType, SubItemLabel;");
    }

    private static void AppendHeader(StringBuilder sb, int assessorCount)
    {
        sb.AppendLine("/*");
        sb.AppendLine("===================================================================================");
        sb.AppendLine("CEDS DATA WAREHOUSE ASSESSMENT QUERY");
        sb.AppendLine("===================================================================================");
        sb.AppendLine();
        sb.AppendLine("INSTRUCTIONS:");
        sb.AppendLine("1. Copy this entire script into SQL Server Management Studio (SSMS)");
        sb.AppendLine("2. Connect to your CEDS Data Warehouse database");
        sb.AppendLine("3. Execute the script (F5)");
        sb.AppendLine("4. Export results to CSV:");
        sb.AppendLine("   - Right-click on results grid → Save Results As...");
        sb.AppendLine("   - Choose 'CSV (Comma delimited)' as file type");
        sb.AppendLine("   - Save with a descriptive name (e.g., 'CEDS_Assessment_2025-10-02.csv')");
        sb.AppendLine("5. Import the CSV file back into the application");
        sb.AppendLine();
        sb.AppendLine("NOTES:");
        sb.AppendLine("  - This script creates a session-scoped temporary table (#EWFProfilerResults) that is");
        sb.AppendLine("    automatically dropped when the SSMS connection is closed.");
        sb.AppendLine("  - No permanent objects are created and no source data is modified.");
        sb.AppendLine("  - If you need to re-run the script in the same session, disconnect and");
        sb.AppendLine("    reconnect first, or add DROP TABLE IF EXISTS #EWFProfilerResults; at the top.");
        sb.AppendLine();
        sb.AppendLine("EXPECTED CSV FORMAT:");
        sb.AppendLine("The script produces 5 columns:");
        sb.AppendLine("  - DataElementName:    Name of the data element being assessed");
        sb.AppendLine("  - CharacteristicType: Type of measurement (e.g., 'RecordCount')");
        sb.AppendLine("  - Value:              The measured value as a string");
        sb.AppendLine("  - SubItemLabel:       Row qualifier for multi-row characteristics (e.g., 'Minimum', 'TotalRecords')");
        sb.AppendLine("  - Remarks:            Optional notes or comments");
        sb.AppendLine();
        sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Assessor Count: {assessorCount}");
        sb.AppendLine("===================================================================================");
        sb.AppendLine("*/");
        sb.AppendLine();
    }
}
