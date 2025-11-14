namespace EwFrameworkAnalysis.UI;

public static class AppRoutes
{
    // ===== Root =====
    public const string Home = "/";
    public const string About = "/about";
    public const string Contact = "/contact";
    public const string Dashboard = "/dashboard";
    public const string Analysis = "/analysis";
    public const string Framework = "/framework";

    public static class DataSourceDetails
    {
        public static string ForDataSource(Guid id) => $"/data-sources/{id}";
        public static bool IsMatch(string path) => path.StartsWith("/data-sources/");
        public static Guid? ExtractDataSourceId(string path)
        {
            var segments = path.Split('/');
            if (segments.Length >= 3 && Guid.TryParse(segments[2], out var id))
                return id;
            return null;
        }
    }
};
