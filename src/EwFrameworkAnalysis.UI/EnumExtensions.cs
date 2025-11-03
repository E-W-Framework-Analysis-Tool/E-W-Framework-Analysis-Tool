using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EwFrameworkAnalysis.UI;
public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        return value.GetType()
                   .GetMember(value.ToString())
                   .First()
                   .GetCustomAttribute<DisplayAttribute>()?
                   .Name ?? value.ToString();
    }

    public static string GetDisplayDescription(this Enum value)
    {
        return value.GetType()
                   .GetMember(value.ToString())
                   .First()
                   .GetCustomAttribute<DisplayAttribute>()?
                   .Description ?? value.ToString();
    }
}
