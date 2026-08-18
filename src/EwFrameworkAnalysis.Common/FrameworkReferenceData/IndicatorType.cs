using System.ComponentModel.DataAnnotations;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public enum IndicatorType
{
    [Display(Name = "Outcomes & Milestones", Description = "")]
    OutcomesMilestones,

    [Display(Name = "E-W System Conditions", Description = "")]
    EWSystemConditions,

    [Display(Name = "Adjacent System Conditions", Description = "")]
    AdjacentSystemConditions
}
