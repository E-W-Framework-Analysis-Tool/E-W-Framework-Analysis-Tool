namespace EwFrameworkAnalysis.Common.Models;

public class DataElement
{
    public required string Name { get; set; }
    public required string Category { get; set; }
}

// Included for strong typing
public static class DataElementNames
{
    public const string Attendance = "Attendance";
    public const string ApprenticeshipEnrollment = "Apprenticeship enrollment";
    public const string CTEEnrollment = "Career and Technical Education (CTE) enrollment";
    public const string CareerReadinessAssessment = "Career readiness assessment";
    public const string ChildCareSubsidyEligibility = "Child care subsidy eligibility";
    public const string ChildCareSubsidyUtilization = "Child care subsidy utilization";
    public const string ClassroomObservation = "Classroom observation";
    public const string CollegeAdmittance = "College admittance";
    public const string CollegeApplicationsSubmitted = "College applications submitted";
    public const string CollegeCreditsEarned = "College credits earned";
    public const string CollegeEntranceAssessment = "College entrance assessment";
    public const string CourseAvailability = "Course availability";
    public const string CourseCollegeCreditsOffered = "Course college credits offered";
    public const string CourseCompletion = "Course completion";
    public const string CourseSubject = "Course subject";
    public const string CTECredentialEarned = "CTE credential earned";
    public const string CTEProgramRequirements = "CTE program requirements";
    public const string CulturalCompetencyAssessment = "Cultural competency assessment";
    public const string DegreeCourseRequirements = "Degree course requirements";
    public const string DevelopmentalAssessments = "Developmental assessments";
    public const string DigitalSkillsAssessment = "Digital skills assessment";
    public const string DisciplinaryEventType = "Disciplinary event type";
    public const string EarlyLearningProgramDesignation = "Early Learning program designation";
    public const string EmploymentAndEarnings = "Employment and earnings";
    public const string EnglishLearningStatusDates = "English learning status dates";
    public const string EnrollmentDatesK12 = "Enrollment dates (K-12)";
    public const string EnrollmentDatesPreK = "Enrollment dates (pre-K)";
    public const string EnrollmentDatesPostsecondary = "Enrollment dates (postsecondary)";
    public const string EnrollmentGradeLevel = "Enrollment grade level";
    public const string EnrollmentTypePostsecondary = "Enrollment type (postsecondary)";
    public const string Expulsion = "Expulsion";
    public const string FinancialAidApplications = "Financial aid applications";
    public const string GradeLevelAttempt = "Grade level attempt";
    public const string GradePointAverageK12 = "Grade point average (K-12)";
    public const string GradePointAveragePostsecondary = "Grade point average (postsecondary)";
    public const string HighSchoolGraduation = "High school graduation";
    public const string HighestEducationLevelCompletionDate = "Highest education level completion date";
    public const string JobQualityIndex = "Job quality index";
    public const string KindergartenProgramDesignation = "Kindergarten program designation";
    public const string MedianEarningsForHSGraduatesByState = "Median earnings for HS graduates by state";
    public const string MedianNetWealthByState = "Median net wealth by state";
    public const string MilitaryEnlistment = "Military enlistment";
    public const string NetWealth = "Net wealth";
    public const string NetPriceOfEducation = "Net price of education";
    public const string ParentSurveys = "Parent surveys";
    public const string PreKProgramQualityBenchmarks = "Pre-K program quality benchmarks";
    public const string PersonAge = "Person age";
    public const string PrincipalEmploymentDates = "Principal employment dates";
    public const string PopulationDemographicsByAgeAndELEligibility = "Population demographics by age and EL eligibility";
    public const string PostGraduationPlans = "Post-graduation plans";
    public const string PostsecondaryCredentialEarned = "Postsecondary credential earned";
    public const string PostsecondaryProgramLength = "Postsecondary program length";
    public const string ProgramFundingSourcePreK = "Program funding source (pre-K)";
    public const string SectionGradesELA = "Section grades: ELA";
    public const string SectionGradesMath = "Section grades: math";
    public const string SelfAssessmentSurvey = "Self-assessment survey";
    public const string StaffSurveys = "Staff surveys";
    public const string StandardizedAssessmentsELA = "Standardized assessments: ELA";
    public const string StandardizedAssessmentsMathGrades1to2 = "Standardized assessments: math (grades 1-2)";
    public const string StandardizedAssessmentsMathGrade3 = "Standardized assessments: math (grade 3)";
    public const string StandardizedAssessmentsMathGrade8 = "Standardized assessments: math (grade 8)";
    public const string StandardizedAssessmentsReadingGrades1to2 = "Standardized assessments: reading (grades 1-2)";
    public const string StandardizedAssessmentsReadingGrade3 = "Standardized assessments: reading (grade 3)";
    public const string StandardizedAssessmentsReadingGrade8 = "Standardized assessments: reading (grade 8)";
    public const string StudentLoanDetails = "Student loan details";
    public const string StudentRoster = "Student roster";
    public const string StudentTeachingSurveys = "Student teaching surveys";
    public const string Suspension = "Suspension";
    public const string TeacherCredentials = "Teacher credentials";
    public const string TeacherEmploymentDates = "Teacher employment dates";
    public const string TeacherExperience = "Teacher experience";
    public const string TransferPostsecondary = "Transfer (postsecondary)";
    public const string WorkBasedLearningOpportunityParticipation = "Work-based learning opportunity participation";
}
