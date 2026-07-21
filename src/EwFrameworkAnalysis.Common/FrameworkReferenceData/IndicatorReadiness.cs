using System.ComponentModel.DataAnnotations;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public enum IndicatorReadiness
{
    [Display(Name = "Well-established", Description = "High readiness for adoption: feasible to measure, comparable across states and localities, and has strong field consensus on whether and how it should be measured.")]
    WellEstablished,

    [Display(Name = "Evolving", Description = "Medium readiness for adoption.")]
    Evolving,

    [Display(Name = "Emerging", Description = "Low readiness for adoption.")]
    Emerging
}
