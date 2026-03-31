namespace EwFrameworkAnalysis.Common;
public static class QueryExtensions
{
    public static string EscapeForQuery(this string originalValue)
    {
        return originalValue.Replace("'", "''");
    }
}
