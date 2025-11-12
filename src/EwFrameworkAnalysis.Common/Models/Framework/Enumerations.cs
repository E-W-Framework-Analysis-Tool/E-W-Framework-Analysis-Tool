using System.ComponentModel.DataAnnotations;

namespace EwFrameworkAnalysis.Common.Models.Framework;

public enum Sector
{
    [Display(Name = "Pre-K", Description = "")]
    PK,

    [Display(Name = "k-12", Description = "")]
    K12,

    [Display(Name = "Postsecondary", Description = "")]
    PS,

    [Display(Name = "Workforce", Description = "")]
    WF
}
