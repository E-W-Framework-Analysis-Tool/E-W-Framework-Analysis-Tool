namespace EwFrameworkAnalysis.UI;

public static class AppRoutes
{
    // ===== Root =====
    public const string Home = "/";
    public const string About = "/about";
    public const string Contact = "/contact";
    public const string Dashboard = "/dashboard";
    public const string Framework = "/framework";
    public const string DataSource = "/data-source/{id}";


    // ===== Utility helper methods =====
    public static string DataSourceById(int id) => $"/data-source/{id}";

    public static readonly Dictionary<string, string> RouteTitles = new()
    {
        { Home, "Home" },
        { Dashboard, "Dashboard" },
        { DataSource, "Data Sources" }
    };
};
