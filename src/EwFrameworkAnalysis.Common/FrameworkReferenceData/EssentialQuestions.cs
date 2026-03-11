using EwFrameworkAnalysis.Common.Models.Framework;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public static class EwFrameworkEssentialQuestions
{
    public static readonly List<EssentialQuestion> Questions =
    [
        new()
        {
            QuestionNumber = 1,
            Question = "Do students and families have access to adequate public supports and neighborhood conditions to enable them to succeed academically and in the workforce?",
            QuestionSummary = "Public supports and neighborhood conditions",
            ApplicableSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            RelatedIndicatorNames = [
                "Childhood experiences",
                "Health insurance coverage",
                "Food security",
                "Access to affordable housing",
                "Access to technology",
                "Access to transportation",
                "Exposure to neighborhood crime",
                "Neighborhood economic diversity",
                "Neighborhood racial diversity",
                "Neighborhood juvenile arrests"
            ]
        },
        new()
        {
            QuestionNumber = 2,
            Question = "Are eligible children enrolled in quality, full-day pre-K programs?",
            QuestionSummary = "Quality pre-K enrollment",
            ApplicableSectors = [Sector.PK],
            RelatedIndicatorNames = [
                "Enrollment in quality public pre-K",
                "Access to quality public pre-K",
                "Access to full-day pre-K",
                "Access to early intervention screening",
                "Access to child care subsidies"
            ]
        },
        new()
        {
            QuestionNumber = 3,
            Question = "Are children demonstrating kindergarten readiness across the five learning domains?",
            QuestionSummary = "Kindergarten readiness",
            ApplicableSectors = [Sector.PK, Sector.K12],
            RelatedIndicatorNames = [
                "Kindergarten readiness: language and literacy",
                "Kindergarten readiness: cognition",
                "Kindergarten readiness: social-emotional development",
                "Kindergarten readiness: approaches to learning",
                "Kindergarten readiness: perceptual, motor, and physical development",
                "Access to quality public pre-K",
                "Access to full-day pre-K",
                "School-family engagement",
                "Teacher credentials",
                "Teacher experience",
                "Educator retention",
                "Classroom observations of instructional practice",
                "Access to early intervention screening",
                "Access to child care subsidies"
            ]
        },
        new()
        {
            QuestionNumber = 4,
            Question = "Do students have access to quality, full-day kindergarten?",
            QuestionSummary = "Quality kindergarten access",
            ApplicableSectors = [Sector.K12],
            RelatedIndicatorNames = [
                "Early grades on track"
            ]
        },
        new()
        {
            QuestionNumber = 5,
            Question = "Are students demonstrating satisfactory academic progress, consistent attendance, and positive behavior to be considered on track in the early grades?",
            QuestionSummary = "Early grades on track",
            ApplicableSectors = [Sector.PK, Sector.K12],
            RelatedIndicatorNames = [
                "Kindergarten readiness: approaches to learning",
                "Kindergarten readiness: cognition",
                "Kindergarten readiness: language and literacy",
                "Kindergarten readiness: social-emotional development",
                "Kindergarten readiness: perceptual, motor, and physical development",
                "Early grades on track",
                "Educator retention",
                "Teacher credentials",
                "Teacher experience"
            ]
        },
        new()
        {
            QuestionNumber = 6,
            Question = "Do students have access to quality school environments including quality curricula and instruction, experienced teachers, effective leaders, and adequate funding?",
            QuestionSummary = "Quality school environments",
            ApplicableSectors = [Sector.PK, Sector.K12, Sector.PS],
            RelatedIndicatorNames = [
                "6th grade on track",
                "9th grade on track",
                "High school graduation",
                "Student perceptions of teaching",
                "Effective program and school leadership",
                "Institutions' contributions to student outcomes",
                "Expenditures per student",
                "Access to quality, culturally responsive curricula",
                "Classroom observations of instructional practice",
                "Early grades on track",
                "Educator retention",
                "Teacher credentials",
                "Teachers' contributions to student learning growth"
            ]
        },
        new()
        {
            QuestionNumber = 7,
            Question = "Are there populations of students that disproportionately experience exclusionary discipline practices that disrupt their educational experience?",
            QuestionSummary = "Equitable discipline practices",
            ApplicableSectors = [Sector.PK, Sector.K12],
            RelatedIndicatorNames = [
                "Positive behavior",
                "Consistent attendance",
                "Equitable discipline practices",
                "School safety",
                "Inclusive environments",
                "Equitable discipline practices"
            ]
        },
        new()
        {
            QuestionNumber = 8,
            Question = "Are students meeting reading and math benchmarks in grades 3 and 8?",
            QuestionSummary = "Reading and math benchmarks",
            ApplicableSectors = [Sector.K12],
            RelatedIndicatorNames = [
                "Math and reading proficiency in grade 3",
                "8th grade on track",
                "Math and reading proficiency in grade 8",
                "6th grade on track",
                "Classroom observations of instructional practice",
                "Early grades on track",
                "Educator retention",
                "Effective program and school leadership",
                "Institutions' contributions to student outcomes",
            ]
        },
        new()
        {
            QuestionNumber = 9,
            Question = "Are teachers and schools making sufficient contributions to academic growth for students?",
            QuestionSummary = "Teacher and school contributions",
            ApplicableSectors = [Sector.K12, Sector.PS],
            RelatedIndicatorNames = [
                "Grade point average",
                "Math and reading proficiency in high school",
                "Gateway course completion",
                "English learner progress",
                "6th grade on track",
                "8th grade on track",
                "9th grade on track",
                "Early grades on track",
                "Institutions' contributions to student outcomes",
                "Math and reading proficiency in grade 3",
                "Math and reading proficiency in grade 8",
                "Teachers' contributions to student learning growth"
            ]
        },
        new()
        {
            QuestionNumber = 10,
            Question = "Do students attend schools with safe, inclusive, and supportive environments that support their social, emotional, mental, and physical development and well-being?",
            QuestionSummary = "Safe and supportive environments",
            ApplicableSectors = [Sector.PK, Sector.K12, Sector.PS],
            RelatedIndicatorNames = [
                "Self-management",
                "Growth mindset",
                "Self-efficacy",
                "Social awareness",
                "Cultural competency",
                "Social capital",
                "Mental and emotional well-being",
                "Physical development and well-being",
                "Representational racial and ethnic diversity of educators",
                "School and workplace racial and ethnic diversity",
                "School and workplace socioeconomic diversity",
                "Access to health, mental health, and social supports",
                "School-family engagement",
                "Equitable discipline practices",
                "Inclusive environments",
                "Representational racial and ethnic diversity of educators",
                "School and workplace racial and ethnic diversity",
                "School and workplace socioeconomic diversity",
                "Access to health, mental health, and social supports",
            ]
        },
        new()
        {
            QuestionNumber = 11,
            Question = "Are students demonstrating satisfactory academic progress, consistent attendance, and positive behavior to be considered on track for high school graduation?",
            QuestionSummary = "High school graduation track",
            ApplicableSectors = [Sector.K12],
            RelatedIndicatorNames = [
                "Positive behavior",
                "8th grade on track",
                "9th grade on track",
                "Access to quality, culturally responsive curricula",
                "Classroom observations of instructional practice",
                "Consistent attendance",
                "Educator retention",
                "Equitable discipline practices",
                "Grade point average",
                "Institutions' contributions to student outcomes",
                "Math and reading proficiency in grade 8",
                "Math and reading proficiency in high school",
                "Consistent attendance",
                "Equitable discipline practices",
                "Teacher credentials",
                "Teachers' contributions to student learning growth"
            ]
        },
        new()
        {
            QuestionNumber = 12,
            Question = "Do students have access to and complete rigorous and accelerated college preparatory coursework?",
            QuestionSummary = "College preparatory coursework",
            ApplicableSectors = [Sector.K12],
            RelatedIndicatorNames = [
                "Successful completion of Algebra I by 9th grade",
                "College preparatory coursework completion",
                "Early college coursework completion",
                "SAT and ACT participation and performance",
                "Access to college preparatory coursework",
                "Access to early college coursework",
                "Equitable placement in rigorous coursework",
                "Access to college and career advising",
                "Access to quality, culturally responsive curricula"
            ]
        },
        new()
        {
            QuestionNumber = 13,
            Question = "Are students taking the necessary steps to apply to college after high school with sufficient counseling support?",
            QuestionSummary = "College application steps",
            ApplicableSectors = [Sector.K12],
            RelatedIndicatorNames = [
                "FAFSA completion",
                "College applications",
                "Access to college and career advising",
                "SAT and ACT participation and performance",
                "Social capital"
            ]
        },
        new()
        {
            QuestionNumber = 14,
            Question = "Are students graduating from high school on time and successfully transitioning into further education, training, or employment?",
            QuestionSummary = "High school graduation and transition",
            ApplicableSectors = [Sector.K12, Sector.PS, Sector.WF],
            RelatedIndicatorNames = [
                "Senior summer on track",
                "Postsecondary enrollment directly after high school graduation",
                "Successful career transition after high school",
                "CTE pathway concentration",
                "Participation in work-based learning",
                "Access to in-demand CTE pathways",
                "Expenditures on workforce development programs",
                "Access to jobs paying a living wage",
                "Access to college and career advising",
                "High school graduation"
            ]
        },
        new()
        {
            QuestionNumber = 15,
            Question = "Are there quality pathways for students who pursue career training that lead to employment in quality jobs?",
            QuestionSummary = "Career training pathways",
            ApplicableSectors = [Sector.K12, Sector.PS, Sector.WF],
            RelatedIndicatorNames = [
                "Transfer (if applicable)",
                "Access to ongoing career skills development",
                "Access to in-demand CTE pathways",
                "CTE pathway concentration",
                "Participation in work-based learning",
                "Employment in a quality job",
                "Access to in-demand CTE pathways",
                "Expenditures on workforce development programs",
                "Participation in work-based learning"
            ]
        },
        new()
        {
            QuestionNumber = 16,
            Question = "Are students matriculating to well-matched postsecondary institutions that successfully graduate their students with credentials of value?",
            QuestionSummary = "Well-matched postsecondary institutions",
            ApplicableSectors = [Sector.K12, Sector.PS],
            RelatedIndicatorNames = [
                // "College match",
                // "Institution graduation rate",
                "Selection of a well-matched postsecondary institution",
                "Postsecondary persistence",
                "Postsecondary certificate or degree completion",
                "Minimum economic return",
                "Unmet financial need",
                "Cumulative student debt",
                "Access to college and career advising",
                "Institutions' contributions to student outcomes",
                "Senior summer on track"
            ]
        },
        new()
        {
            QuestionNumber = 17,
            Question = "Do students attend postsecondary institutions that provide adequate financial aid and that are adequately funded to offer a quality educational experience?",
            QuestionSummary = "Postsecondary funding and aid",
            ApplicableSectors = [Sector.PS],
            RelatedIndicatorNames = [
                // "Financial aid",
                // "Institution funding",
                "Student loan repayment",
                "Cumulative student debt",
                "Expenditures per student",
                "Unmet financial need"
            ]
        },
        new()
        {
            QuestionNumber = 18,
            Question = "Are students experiencing sufficient early momentum in postsecondary education to be on track for on-time completion?",
            QuestionSummary = "Postsecondary early momentum",
            ApplicableSectors = [Sector.PS],
            RelatedIndicatorNames = [
                // "Credit accumulation",
                "Gateway course completion",
                "First-year credit accumulation",
                "First-year program of study concentration",
                "Access to college and career advising",
                "Postsecondary persistence",
                "Transfer (if applicable)",
                "Unmet financial need"
            ]
        },
        new()
        {
            QuestionNumber = 19,
            Question = "Are students completing credentials of value after high school that set them up for success in the workforce?",
            QuestionSummary = "Credentials of value",
            ApplicableSectors = [Sector.PS, Sector.WF],
            RelatedIndicatorNames = [
                // "Postsecondary credential completion",
                // "Credential value",
                "Graduate degree completion",
                "Enrollment in graduate education",
                "Postsecondary certificate or degree completion",
                "Civic engagement",
                "Industry-recognized credential",
                "Digital skills",
                "Communication skills",
                "Higher-order thinking skills",
                "Cultural competency",
                "Cumulative student debt",
                "Institutions' contributions to student outcomes",
                "Minimum economic return",
                "Student loan repayment",
                "Institutions' contributions to student outcomes",
                "Cumulative student debt",
            ]
        },
        new()
        {
            QuestionNumber = 20,
            Question = "Are students gaining access to quality jobs that offer economic mobility and security after high school or postsecondary training and education?",
            QuestionSummary = "Quality job access",
            ApplicableSectors = [Sector.WF],
            RelatedIndicatorNames = [
                // "Employment rate",
                // "Earnings",
                // "Job quality",
                "Employment in a quality job",
                "Economic mobility",
                "Economic security",
                "Access to jobs paying a living wage",
                "Access to ongoing career skills development"
            ]
        }
    ];
}

public class EssentialQuestion
{
    public int QuestionNumber { get; set; }
    public string Question { get; set; } = string.Empty;
    public string QuestionSummary { get; set; } = string.Empty;
    public List<Sector> ApplicableSectors { get; set; } = [];
    public List<string> RelatedIndicatorNames { get; set; } = [];
}
