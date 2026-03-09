using System.ComponentModel.DataAnnotations;

namespace EwFrameworkAnalysis.Common.Models.Framework;

public enum Sector
{
    [Display(Name = "Pre-K", Description = "")]
    PK,

    [Display(Name = "K-12", Description = "")]
    K12,

    [Display(Name = "Postsecondary", Description = "")]
    PS,

    [Display(Name = "Workforce", Description = "")]
    WF
}
