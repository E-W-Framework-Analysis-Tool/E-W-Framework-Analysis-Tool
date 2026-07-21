using System.ComponentModel.DataAnnotations;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public enum IndicatorCluster
{
    [Display(Name = "Kindergarten Readiness", Description = "")]
    KindergartenReadiness,

    [Display(Name = "Postsecondary Success", Description = "")]
    PostsecondarySuccess,

    [Display(Name = "Postsecondary Transitions", Description = "")]
    PostsecondaryTransitions,

    [Display(Name = "School Climate", Description = "")]
    SchoolClimate,

    [Display(Name = "Social-Emotional Learning", Description = "")]
    SocialEmotionalLearning,

    [Display(Name = "Teaching Effectiveness", Description = "")]
    TeachingEffectiveness,

    [Display(Name = "Workforce Success", Description = "")]
    WorkforceSuccess,

    [Display(Name = "Workforce Transitions", Description = "")]
    WorkforceTransitions
}
