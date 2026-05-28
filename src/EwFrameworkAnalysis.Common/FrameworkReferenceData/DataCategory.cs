using System.ComponentModel.DataAnnotations;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public enum DataCategory
{
    [Display(Name = "Administrative Data", Description = "")]
    AdministrativeData,

    [Display(Name = "Assessments", Description = "")]
    Assessments,

    [Display(Name = "Classroom Observations", Description = "")]
    ClassroomObservations,

    [Display(Name = "Curriculum Materials", Description = "")]
    CurriculumMaterials,

    [Display(Name = "Educator Administrative Data", Description = "")]
    EducatorAdministrativeData,

    [Display(Name = "Rubrics", Description = "")]
    Rubrics,

    [Display(Name = "Student Transcripts", Description = "")]
    StudentTranscripts,

    [Display(Name = "Surveys", Description = "")]
    Surveys
}
