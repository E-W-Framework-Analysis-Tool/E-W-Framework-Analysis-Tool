using System.ComponentModel.DataAnnotations;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public enum IndicatorDomain
{
    [Display(Name = "Academic Progress & Completion", Description = "")]
    AcademicProgressCompletion,

    [Display(Name = "Career Readiness & Economic Success", Description = "")]
    CareerReadinessEconomicSuccess,

    [Display(Name = "Social, Emotional, & Physical Wellbeing", Description = "")]
    SocialEmotionalPhysicalWellbeing,

    [Display(Name = "Cross-Domain", Description = "")]
    CrossDomain
}
