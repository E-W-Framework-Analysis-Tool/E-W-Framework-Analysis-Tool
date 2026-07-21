using EwFrameworkAnalysis.Common.Models.Framework;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public static class PreviousDataForComparison
{
    public static readonly Dictionary<string, Indicator> Indicators = new()
    {
        ["Enrollment in quality public pre-K"] =
            new()
            {
                Name = "Enrollment in quality public pre-K",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Eligible children are enrolled in a publicly funded pre-K program, which can be administered through mixed delivery systems that include Head Start, pre-K classrooms in public schools, and licensed family-based child care programs and community-based organizations.",
                RecommendedMetrics = "Percentage of eligible 3- and 4-year-olds enrolled in public pre-K",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    "Date of services provided",
                    "Early intervention screening results",
                    "Early intervention screening services referral status",
                    "Family eligibility for child care subsidies",
                    "Indicator of whether services were provided",
                    "Kindergarten enrollment date",
                    "Pre-K program days per week",
                    "Pre-K program hours per day",
                    "Pre-K program QRIS rating",
                    "Receipt of child care subsidies",
                    /* ECS data elements */
                    "Age",
                    "Enrollment in public pre-K",
                    "Pre-K eligibility status"
                ]
            },
        ["Kindergarten readiness: language and literacy"] =
            new()
            {
                Name = "Kindergarten readiness: language and literacy",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Children develop and demonstrate foundational language and literacy skills.",
                RecommendedMetrics =
                    "Percentage of children meeting benchmarks on a teacher-reported kindergarten readiness assessment, such as: - Desired Results Developmental Profile (DRDP) Language and Literacy Development domain - Ready 4 Kindergarten Early Learning Assessment (R4K ELA) Language and Literacy domain - Teaching Strategies GOLD (TS GOLD) Language and Literacy subscales Or, percentage of children meeting benchmarks on direct child assessments administered by trained assessors, such as: - Woodcock-Johnson IV Tests of Early Cognition and Academic Development (ECAD) Letter-Word and Writing subtests - Individual Growth and Development Indicators (IGDIs) Early Literacy assessment",
                DataNeeded = [DataCategory.Assessments],
                DataElementNames =
                [
                    // "Developmental assessments"
                    /* ECS data elements */
                    "Direct child assessments (language and literacy)",
                    "Reported kindergarten readiness (language and literacy)"
                ]
            },
        ["Kindergarten readiness: cognition"] =
            new()
            {
                Name = "Kindergarten readiness: cognition",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Children develop and demonstrate foundational math and scientific reasoning skills.",
                RecommendedMetrics =
                    "Percentage of children meeting benchmarks on teacher-reported kindergarten readiness assessment, such as: - DRDP Cognition domain - R4K ELA Mathematics and Science domains - TS GOLD Cognitive and Mathematics subscales Or, percentage of children meeting benchmarks on direct child assessments, such as: - Woodcock-Johnson IV Tests of ECAD Number Sense subtest - IGDIs Early Numeracy assessment - Research Based Early Mathematics Assessment (REMA)",
                DataNeeded = [DataCategory.Assessments],
                DataElementNames =
                [
                    // "Developmental assessments"
                    /* ECS data elements */
                    "Direct child assessments (cognition)",
                    "Kindergarten readiness assessments (cognition)"
                ]
            },
        ["Early grades on track"] =
            new()
            {
                Name = "Early grades on track",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students in grades 1 and 2 are on track to achieve academic proficiency in grade 3.",
                RecommendedMetrics =
                    "Percentage of students in grades 1 and 2 meeting grade-level math and reading benchmarks, with an attendance rate of 90 percent or higher, and no in- or out-of-school suspensions or expulsions",
                DataNeeded = [DataCategory.Assessments, DataCategory.AdministrativeData],
                DataElementNames =
                [
                    "Kindergarten program days per week",
                    "Kindergarten program hours per day",
                    /* ECS data elements */
                    "State standardized test (Math proficiency)",
                    "State standardized test (Reading proficiency)",
                    "Student attendance rate (PK)",
                    "Student grade level (PK)",
                    "Suspensions and expulsions (K-12)"
                ]
            },
        ["Consistent attendance"] =
            new()
            {
                Name = "Consistent attendance",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students are present for more than 90 percent of enrolled days.",
                RecommendedMetrics =
                    "Percentage of students who are present for more than 90 percent of their enrolled days, excluding students enrolled for fewer than 90 days",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    "Student attendance rate (K-12)",
                    /* ECS data elements */
                    "Student attendance rate (PK)"
                ]
            },
        ["Positive behavior"] =
            new()
            {
                Name = "Positive behavior",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Students are not suspended or expelled from school and do not experience other types of exclusionary discipline, such as restraint and seclusion.",
                RecommendedMetrics =
                    "Pre-K, K-12: Percentage of children who do not experience any of the following: in-school suspensions, out-of-school suspensions, disciplinary use of restraint and seclusion, or expulsions",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Restraint and seclusion for discipline (K-12)",
                    "Suspensions and expulsions (K-12)"
                ]
            },
        ["Math and reading proficiency in grade 3"] =
            new()
            {
                Name = "Math and reading proficiency in grade 3",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students demonstrate proficiency in math and ELA according to high-quality state standards.",
                RecommendedMetrics =
                    "Percentage of students in grade 3 who meet grade-level standards in reading/English language arts and math as measured by state standardized tests.",
                DataNeeded = [DataCategory.Assessments],
                DataElementNames =
                [
                    /* ECS data elements */
                    "State standardized test (Math proficiency)",
                    "State standardized test (Reading proficiency)",
                    "Student grade level (PK)"
                ]
            },
        ["6th grade on track"] =
            new()
            {
                Name = "6th grade on track",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Grade 6 students are on track to graduate high school on time.",
                RecommendedMetrics =
                    "Percentage of students in grade 6 with passing grades in English language arts and math, attendance of 90 percent or higher, and no in- or out-of-school suspensions or expulsions",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Course performance (English and Math)",
                    "Student attendance rate (PK)",
                    "Student grade level (PK)",
                    "Suspensions and expulsions (K-12)"
                ]
            },
        ["8th grade on track"] =
            new()
            {
                Name = "8th grade on track",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Grade 8 students are prepared to transition to high school and are on track to graduate on time.",
                RecommendedMetrics =
                    "Percentage of students in grade 8 with a GPA of 2.5 or higher, no Ds or Fs in English language arts or math, attendance of 96 percent or higher, and no in- or out-of-school suspensions or expulsions.",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Course performance (English and Math)",
                    "Grade point average (K-12)",
                    "Student attendance rate (PK)",
                    "Student grade level (PK)",
                    "Suspensions and expulsions (K-12)"
                ]
            },
        ["Math and reading proficiency in grade 8"] =
            new()
            {
                Name = "Math and reading proficiency in grade 8",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Students demonstrate proficiency in math and reading/English language arts according to high-quality state standards.",
                RecommendedMetrics =
                    "Percentage of students in grade 8 who meet grade-level standards in reading/English language arts and math as measured by state standardized tests",
                DataNeeded = [DataCategory.Assessments],
                DataElementNames =
                [
                    /* ECS data elements */
                    "State standardized test (Math proficiency)",
                    "State standardized test (Reading proficiency)",
                    "Student grade level (PK)"
                ]
            },
        ["Successful completion of Algebra I by 9th grade"] =
            new()
            {
                Name = "Successful completion of Algebra I by 9th grade",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students successfully complete Algebra I or an equivalent course before or during grade 9.",
                RecommendedMetrics =
                    "Percentage of first-time grade 9 students who complete Algebra I or an equivalent course by the end of their 9th-grade year",
                DataNeeded = [DataCategory.StudentTranscripts],
                DataElementNames =
                [
                    "Student grade level",
                    /* ECS data elements */
                    "Course identifier or title",
                    "Course outcome",
                    "First-time 9th grade student status",
                    "Student grade level (PK)"
                ]
            },
        ["9th grade on track"] =
            new()
            {
                Name = "9th grade on track",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Grade 9 students are on track to graduate high school in four years, enroll in postsecondary education, and succeed in their first year of postsecondary education.",
                RecommendedMetrics =
                    "Percentage of students in grade 9 with a GPA of 3.0 or higher, no Ds or Fs in English language arts or math, attendance of 96 percent or higher, and no in- or out-of-school suspensions or expulsions.",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Course outcome",
                    "Student attendance rate (PK)",
                    "Student grade level (PK)"
                ]
            },
        ["Grade point average"] =
            new()
            {
                Name = "Grade point average",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Middle school students earn course grades that demonstrate high school readiness; high school students earn course grades necessary to gain admission to college; and college students earn grades high enough to graduate and obtain jobs.",
                RecommendedMetrics =
                    "Percentage of students in grades 6-8 with a GPA of 3.0 or higher Percentage of students in grades 9-12 with a GPA of 3.0 or higher Percent of college students with a GPA of 3.0 or higher",
                DataNeeded = [DataCategory.StudentTranscripts],
                DataElementNames =
                [
                    "Grade point average (K-12)",
                    /* ECS data elements */
                    "Grade point average (Postsecondary)",
                    "Student grade level (PK)"
                ]
            },
        ["Math and reading proficiency in high school"] =
            new()
            {
                Name = "Math and reading proficiency in high school",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Students demonstrate proficiency in math and reading/English language arts according to high-quality state standards.",
                RecommendedMetrics =
                    "Percentage of tested students who meet grade-level standards in reading/English language arts and math as measured by state standardized tests",
                DataNeeded = [DataCategory.Assessments],
                DataElementNames =
                [
                    /* ECS data elements */
                    "State standardized test (Math proficiency)",
                    "State standardized test (Reading proficiency)",
                    "Student grade level (PK)"
                ]
            },
        ["College preparatory coursework completion"] =
            new()
            {
                Name = "College preparatory coursework completion",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "High school students meet typical coursework requirements for admission in a four-year college.",
                RecommendedMetrics =
                    "Percentage of high school graduates who successfully complete the coursework required for admission at a four-year college or university, which includes: - Four years of English classes - Four years of math classes (including at least four of the following: pre-algebra, algebra, geometry, Algebra II or trigonometry, precalculus, calculus, statistics, quantitative reasoning, and data science) - Three years of laboratory science (including biology, chemistry, and physics) - Two years of social sciences - Two years of foreign language - One year of visual or performing arts",
                DataNeeded = [DataCategory.StudentTranscripts],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Course identifier or title",
                    "Course outcome",
                    "High school graduation indicator"
                ]
            },
        ["Early college coursework completion"] =
            new()
            {
                Name = "Early college coursework completion",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "High school students successfully complete early college coursework (Advanced Placement [AP], International Baccalaureate [IB], or dual credit).",
                RecommendedMetrics =
                    "Percentage of high school students who enroll in and pass at least one early college course (AP, IB, or dual credit) Percentage of students enrolled in early college coursework who earn credit-bearing scores on end-of-course tests (for example, a score of 3 or higher on AP tests or 5 or higher on IB tests) or earn postsecondary credit within their dual enrollment courses",
                DataNeeded = [DataCategory.StudentTranscripts, DataCategory.Assessments],
                DataElementNames =
                [
                    // "College credits earned"
                    /* ECS data elements */
                    "AP, IB, or Dual Credit course credits",
                    "Student course enrollment record"
                ]
            },
        ["SAT and ACT participation and performance"] =
            new()
            {
                Name = "SAT and ACT participation and performance",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "High school students take and earn a \"college-ready\" score on the ACT or SAT before graduating high school.",
                RecommendedMetrics =
                    "Percentage of grade 11-12 students who take the SAT/ACT Percentage of grade 11-12 students who earn a \"college-ready\" score, based on the benchmarks set by the SAT and ACT",
                DataNeeded = [DataCategory.Assessments],
                DataElementNames =
                [
                    /* ECS data elements */
                    "ACT completion",
                    "ACT score",
                    "SAT completion",
                    "SAT score",
                    "Student grade level (PK)"
                ]
            },
        ["FAFSA completion"] =
            new()
            {
                Name = "FAFSA completion",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Grade 12 students eligible for federal financial aid complete the Free Application for Federal Student Aid (FAFSA) by June 30.",
                RecommendedMetrics = "Percentage of grade 12 students who complete the FAFSA by June 30",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "FAFSA completion date",
                    "Student grade level (PK)"
                ]
            },
        ["College applications"] =
            new()
            {
                Name = "College applications",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Grade 12 students submit a well-balanced portfolio of at least three college applications.",
                RecommendedMetrics = "Percentage of grade 12 students who submitted at least three college applications",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Postsecondary applications submitted",
                    "Student grade level (PK)"
                ]
            },
        ["High school graduation"] =
            new()
            {
                Name = "High school graduation",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Students graduate from high school with a regular diploma within four, five, and six years of entering high school.",
                RecommendedMetrics =
                    "Adjusted cohort graduation rate (the percentage of first-time 9th graders who graduate with a regular diploma within four, five, and six years of entering high school, regardless of whether they transferred schools)",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Cohort graduation year",
                    "Cohort year",
                    "Diploma or credential award date",
                    "Enrollment date",
                    "High school diploma type"
                ]
            },
        ["Selection of a well-matched postsecondary institution"] =
            new()
            {
                Name = "Selection of a well-matched postsecondary institution",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "High school graduates select the best \"match\" college among the institutions to which they were admitted, based on the institutional graduation rate of similar students.",
                RecommendedMetrics =
                    "Percentage of high school seniors who select a college within 10 percentage points of the best matched postsecondary institution to which they were admitted, based on the institution's graduation rate for similar students by race, ethnicity, or income status (as measured by Pell Grant receipt).",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "High school graduation indicator",
                    "Institution graduation rate",
                    "Postsecondary enrollment date",
                    "Postsecondary Institution ID"
                ]
            },
        ["Senior summer on track"] =
            new()
            {
                Name = "Senior summer on track",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "High school graduates intending to enroll in postsecondary education in the fall after high school graduation complete the registration, financial, and logistic deadlines over the summer necessary to successfully enroll in the fall.",
                RecommendedMetrics =
                    "Percentage of high school graduates reporting intentions to enroll in postsecondary education in the fall who successfully enroll in a postsecondary institution by October 31 following their high school graduation",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "High school graduation indicator",
                    "Postsecondary enrollment date",
                    "Reported intent to enroll in postsecondary education"
                ]
            },
        ["Postsecondary enrollment directly after high school graduation"] =
            new()
            {
                Name = "Postsecondary enrollment directly after high school graduation",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "High school graduates enroll in a postsecondary institution by October 31 following their high school graduation.",
                RecommendedMetrics =
                    "Percentage of high school graduates who enroll in a postsecondary institution by October 31 following their high school graduation",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "High school graduation date",
                    "Postsecondary enrollment date"
                ]
            },
        ["First-year credit accumulation"] =
            new()
            {
                Name = "First-year credit accumulation",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Students attempt and complete sufficient credits during their first undergraduate year to be on track for on-time degree completion.",
                RecommendedMetrics =
                    "Percentage of students attempting and completing sufficient credits toward on-time completion in their first year: 30 credits for full-time and 15 credits for part-time students",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Number of credits attempted",
                    "Number of credits earned",
                    "Postsecondary enrollment date",
                    "Postsecondary enrollment status (Full time/part time)"
                ]
            },
        ["First-year program of study concentration"] =
            new()
            {
                Name = "First-year program of study concentration",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Postsecondary students demonstrate selection of a program of study by completing nine credits or three courses in a meta-major during their first year.",
                RecommendedMetrics =
                    "Percentage of students completing at least nine credits (or three courses) within a meta-major during their first year in postsecondary education",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Course identifier or title",
                    "Number of credits earned",
                    "Postsecondary enrollment date"
                ]
            },
        ["Gateway course completion"] =
            new()
            {
                Name = "Gateway course completion",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Completion of college-level introductory math and English courses, as defined by each postsecondary institution, during the first year of college.",
                RecommendedMetrics =
                    "Percentage of first-year college students who complete college-level introductory math and English courses within their first year of college",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Course identifier or title",
                    "Number of credits earned",
                    "Postsecondary enrollment date"
                ]
            },
        ["Postsecondary persistence"] =
            new()
            {
                Name = "Postsecondary persistence",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students continue enrolling in college in subsequent years, including transfers to other colleges.",
                RecommendedMetrics =
                    "Percentage of students in a cohort who continue enrolling in college (including transfers to other colleges) or complete a credential the following year, captured for up to 150 percent of program length. Other time frames, such as 100 and 200 percent of program length, should also be reported for this measure.",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Enrollment status (current and prior years)",
                    "Postsecondary credential attainment date",
                    "Postsecondary degree program length"
                ]
            },
        ["Transfer (if applicable)"] =
            new()
            {
                Name = "Transfer (if applicable)",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Postsecondary students transfer to a longer program (from certificate to associate's degree, or from associate's to bachelor's degree).",
                RecommendedMetrics =
                    "Percentage of students in a certificate or associate's degree program who transfer to a longer degree program within 150 percent of the original program's intended length. Other time frames, such as 100 percent and 200 percent of program length, are also useful to track.",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Enrollment status (current and prior years)",
                    "Postsecondary degree program length",
                    "Postsecondary institution ID (current and prior years)",
                    "Transfer indicator or transfer student status"
                ]
            },
        ["Postsecondary certificate or degree completion"] =
            new()
            {
                Name = "Postsecondary certificate or degree completion",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Students complete a certificate, associate's, or bachelor's degree within a specified time frame after entering college.",
                RecommendedMetrics =
                    "Percentage of students completing a certificate, associate's, or bachelor's degree within 150 percent of the program's intended length. Other time frames, such as 100 percent and 200 percent of program length, should also be reported for this measure.",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Graduate credential attainment date",
                    "Graduate program enrollment date",
                    "Postsecondary credential attainment date",
                    "Postsecondary degree program length",
                    "Postsecondary enrollment date"
                ]
            },
        ["Enrollment in graduate education"] =
            new()
            {
                Name = "Enrollment in graduate education",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students enroll in a graduate education program after completing an undergraduate degree.",
                RecommendedMetrics =
                    "Percentage of bachelor's degree recipients enrolling in post-baccalaureate or graduate programs within one to five years of completion. Other time frames, such as within 10 years of completion, should also be reported for this measure.",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Bachelor's degree completion date",
                    "Graduate program enrollment date",
                    "Post-baccalaureate program enrollment date"
                ]
            },
        ["Graduate degree completion"] =
            new()
            {
                Name = "Graduate degree completion",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Students complete a graduate degree (master's degree or higher) within a specified time frame after entering graduate school.",
                RecommendedMetrics =
                    "Percentage of graduate students completing a graduate degree within 150 percent of their current program's length. Other time frames, such as 100 percent and 200 percent of program length, should also be reported for this measure.",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    "Postsecondary enrollment date",
                    // "Enrollment type (postsecondary)",
                    "Postsecondary credential earned",
                    // "Postsecondary program length"
                ]
            },
        ["Kindergarten readiness: social-emotional development"] = // a.k.a Developmental progress: social-emotional development
            new()
            {
                Name = "Kindergarten readiness: social-emotional development",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition =
                    "Children develop and demonstrate the skills to form positive relationships with adults and peers, emotional functioning, and a sense of identity and belonging.",
                RecommendedMetrics =
                    "Percentage of students meeting benchmarks on teacher-reported kindergarten readiness assessment, such as the DRDP Social and Emotional Development domain, R4K ELA Social Foundations domain, TS GOLD Social-Emotional subscale, or percentage of students meeting benchmarks on teacher reports, such as the Child Behavior Rating Scale (CBRS) and Devereaux Early Childhood Assessment Preschool Program (DECA-P2)",
                DataNeeded = [DataCategory.Assessments],
                DataElementNames =
                [
                    // "Developmental assessments"
                    /* ECS data elements */
                    "Reported kindergarten readiness (social-emotional skills)",
                    "Teacher reports of social-emotional development"
                ]
            },
        ["Kindergarten readiness: approaches to learning"] = // a.k.a Developmental progress: approaches to learning
            new()
            {
                Name = "Kindergarten readiness: approaches to learning",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition =
                    "Children develop and demonstrate emotional and behavioral self-regulation, cognitive self-regulation (executive functioning), initiative and curiosity, and creativity.",
                RecommendedMetrics =
                    "Percentage of students meeting benchmarks on teacher-reported kindergarten readiness assessment, such as the DRDP Approaches to Learning - Self-Regulation domain and TS GOLD Cognitive subscale, or percentage of students meeting benchmarks on teacher reports of children's executive function, such as the CBRS, or percentage of students meeting benchmarks on a direct child assessment, such as the Heads Toes Knees Shoulders (HTKS) task administered by teachers and the Minnesota Executive Function Scale (MEFS), self-administered on a tablet",
                DataNeeded = [DataCategory.Assessments],
                DataElementNames =
                [
                    // "Developmental assessments"
                    /* ECS data elements */
                    "Direct child assessments of executive function",
                    "Reported kindergarten readiness (behavioral skills)",
                    "Teacher reports of executive function"
                ]
            },
        ["Kindergarten readiness: perceptual, motor, and physical development"] = // a.k.a. Developmental progress: perceptual, motor, and physical development
            new()
            {
                Name = "Kindergarten readiness: perceptual, motor, and physical development",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition =
                    "Children develop and demonstrate gross and fine motor skills, and an understanding of health, safety, and nutrition.",
                RecommendedMetrics =
                    "Percentage of children meeting benchmarks on teacher-reported kindergarten readiness assessment, such as the DRDP Physical Development - Health domain, R4K ELA Physical Well-Being and Motor Development domain, TS GOLD Physical subscale, or percentage of students meeting benchmarks on direct child assessment administered by teachers, healthcare professionals, or other qualified adults, such as the Peabody Developmental Motor Scale",
                DataNeeded = [DataCategory.Assessments],
                DataElementNames = [
                    /* ECS data elements */
                    "Direct child assessments of physical development",
                    "Reported kindergarten readiness (physical development)"
                ]
            },
        ["Self-management"] =
            new()
            {
                Name = "Self-management",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition =
                    "Students are able to regulate their emotions, thoughts, and behaviors effectively in different situations.",
                RecommendedMetrics =
                    "Percentage of students reporting a high level of self-management on surveys such as the CORE Districts SEL Survey self-management scale (grades 5-12) or Shift and Persist scale for children, Percentage of individuals reporting a high level of self-management on surveys such as the Shift and Persist scale for teens and adults",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Self-management surveys (K-12)",
                    "Self-management surveys (Postsecondary)",
                    "Self-management surveys (Workforce)"
                ]
            },
        ["Growth mindset"] =
            new()
            {
                Name = "Growth mindset",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Students believe that their abilities can grow with effort.",
                RecommendedMetrics =
                    "Percentage of students reporting a high level of growth mindset on surveys such as the CORE Districts SEL Survey Growth Mindset Scale (grades 5-12) or the Growth Mindset Scale developed by Carol Dweck, Percentage of students reporting a high level of growth mindset on surveys such as the Growth Mindset Scale developed by Carol Dweck",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Growth mindset surveys (K-12)",
                    "Growth mindset surveys (Postsecondary)",
                    "Growth mindset surveys (Workforce)"
                ]
            },
        ["Self-efficacy"] =
            new()
            {
                Name = "Self-efficacy",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Students believe in their ability to achieve an outcome or reach a goal.",
                RecommendedMetrics =
                    "Percentage of students reporting a high level of self-efficacy on surveys such as the CORE Districts SEL Survey self-efficacy scale, Percentage of individuals reporting a high level of self-efficacy on surveys such as the New General Self-Efficacy Scale or Ascend survey's Self-Efficacy Scale",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Self-efficacy surveys (K-12)",
                    "Self-efficacy surveys (Postsecondary)",
                    "Self-efficacy surveys (Workforce)"
                ]
            },
        ["Social awareness"] =
            new()
            {
                Name = "Social awareness",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition =
                    "Students are able understand others' perspectives; understand social and ethical norms for behavior; and recognize family, school, and community resources and supports.",
                RecommendedMetrics =
                    "Percentage of students reporting a high level of social awareness on surveys such as the CORE Districts SEL Survey social awareness scale, or percentage of students meeting benchmarks on teacher ratings of social skills drawn from Elliott and Gresham's Social Skills Rating Scale, Percentage of individuals demonstrating social proficiency on a performance assessment, such as the National Work Readiness Credential Essential Soft Skills assessment",
                DataNeeded = [DataCategory.Surveys, DataCategory.Assessments],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Self-management surveys (K-12)",
                    "Social awareness teacher ratings",
                    "Social proficiency performance assessments (Postsecondary)",
                    "Social proficiency performance assessments (Workforce)"
                ]
            },
        ["Cultural competency"] =
            new()
            {
                Name = "Cultural competency",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition =
                    "Individuals are able to understand the perspectives of and empathize with others from diverse backgrounds and cultures.",
                RecommendedMetrics =
                    "Percentage of students demonstrating proficiency on an assessment of cultural competency, such as the HEIghten Outcomes Assessment for Intercultural Competency & Diversity or The Intercultural Development Inventory®, Percentage of individuals demonstrating proficiency on an assessment of cultural competency, such as The Intercultural Development Inventory®",
                DataNeeded = [DataCategory.Surveys, DataCategory.Assessments],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Cultural competency assessments (K-12)",
                    "Cultural competency assessments (Postsecondary)",
                    "Cultural competency assessments (Workforce)"
                ]
            },
        ["Civic engagement"] =
            new()
            {
                Name = "Civic engagement",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition =
                    "Individuals exhibit the knowledge, skills, values, motivation, and activities that promote quality of life within a community and society at large through political and nonpolitical processes.",
                RecommendedMetrics =
                    "Percentage of students reporting a high level of civic engagement on surveys such as the Youth Civic and Character Measures Toolkit Survey and Youth Civic Engagement Indicators Project Survey, Percentage of individuals reporting a high level of civic engagement on surveys such as the Index of Civic and Political Engagement",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Civic engagement surveys (K-12)",
                    "Civic engagement surveys (Postsecondary)",
                    "Civic engagement surveys (Workforce)"
                ]
            },
        ["Social capital"] =
            new()
            {
                Name = "Social capital",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition =
                    "Individuals have access to and are able to mobilize relationships that help them further their goals.",
                RecommendedMetrics =
                    "Percentage of students or individuals reporting a high level of social capital on surveys such as the Social Capital Assessment + Learning for Equity (SCALE) Social Capital, Network Diversity, and Network Strength scales; Percentage of individuals reporting a high level of social capital on surveys such as the Social Capital Community Benchmark Survey",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Social capital surveys (K-12)",
                    "Social capital surveys (Postsecondary)",
                    "Social capital surveys (Workforce)"
                ]
            },
        ["Mental and emotional well-being"] =
            new()
            {
                Name = "Mental and emotional well-being",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Individuals possess mental and emotional well-being.",
                RecommendedMetrics =
                    "Percentage of children with identified health or developmental concerns as identified by a developmental screening tool. For a list of screening tools that may be appropriate for children younger than age 5, see the following guide from the Head Start Early Childhood Learning and Knowledge Center: \"Birth to 5: Watch Me Thrive! A Compendium of Screening Measures for Young Children.\"; Percentage of youth with mental or emotional health needs as identified by a universal screening tool. For a list of mental health screening tools that may be appropriate for school-based use, see the following guide from the National Center on Safe Supportive Learning Environments: \"Mental Health Screening Tools for Grades K-12.\"; Psychological well-being scale.",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Age",
                    "Developmental screening results",
                    "Mental and emotional well-being assessments",
                    "Universal screening results"
                ]
            },
        ["Physical development and well-being"] =
            new()
            {
                Name = "Physical development and well-being",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Individuals exhibit positive physical development and health.",
                RecommendedMetrics =
                    "Pre-K: See kindergarten readiness: perceptual, motor, and physical development indicator; K-12: Percentage of students meeting benchmarks on self-rated surveys of physical health, such as the California Healthy Kids Survey Physical Health & Nutrition module; Postsecondary and workforce: Percentage of adults who rate their own health as good, very good, or excellent on the Self-Rated Health scale, or percentage of individuals meeting benchmarks on the Health-Related Quality of Life Scale",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Health-Related Quality of Life Scale scores",
                    "Physical health surveys (K-12)",
                    "Physical health surveys (Postsecondary)",
                    "Physical health surveys (Workforce)"
                ]
            },
        ["Successful career transition after high school"] =
            new()
            {
                Name = "Successful career transition after high school",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition =
                    "High school graduates transition to training, military service, or employment in the fall after graduating high school (if they do not matriculate to postsecondary education).",
                RecommendedMetrics =
                    "Percentage of high school graduates enlisted in the military, enrolled in an apprenticeship program, enrolled in noncredit career and technical education (CTE) courses, or employed and earning at least the median annual full-time earnings for high school graduates ($35,000 year) before October 31 following graduation.",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Apprenticeship program enrollment date",
                    "Earnings",
                    "Employment date",
                    "Enlistment in the military",
                    "Enrollment in noncredit CTE date",
                    "High school graduation date"
                ]
            },
        ["CTE pathway concentration"] =
            new()
            {
                Name = "CTE pathway concentration",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Students participating in CTE concentrate in a single chosen pathway or program of study.",
                RecommendedMetrics =
                    "Percentage of 12th-grade students enrolled in CTE who complete three or more CTE courses in a single pathway; Percentage of CTE students who earn at least 12 credits within a CTE program or complete such a program if it encompasses fewer than 12 credits in total",
                DataNeeded = [DataCategory.StudentTranscripts],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Course identifier or title",
                    "CTE course completion",
                    "CTE course ID or course title",
                    "CTE pathway or career cluster associated with CTE course",
                    "Student course enrollment record",
                    "Student grade level (PK)"
                ]
            },
        ["Industry-recognized credential"] =
            new()
            {
                Name = "Industry-recognized credential",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Individuals complete at least one industry-recognized credential, as defined by each state.",
                RecommendedMetrics =
                    "Percentage of 12th-grade students enrolled in CTE who earn at least one industry-recognized credential; Percentage of students enrolled in a CTE program who earn at least one industry-recognized credential; Percentage of program participants who have completed at least one industry-recognized credential",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    //"Indicator of current enrollment in postsecondary",
                    "Industry-recognized credential attainment",
                    "Student course enrollment record",
                    "Student grade level (PK)",
                    "Workforce development program participation"
                ]
            },
        ["Participation in work-based learning"] =
            new()
            {
                Name = "Participation in work-based learning",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition =
                    "Credential seekers participate in an internship, work study, cooperative education, apprenticeship program, or other work-based learning opportunities.",
                RecommendedMetrics =
                    "Percentage of students who participate in a work-based learning opportunity before graduation; Percentage of students who participate in a work-based learning opportunity before graduation; Percentage of workforce training program participants who participate in a work-based learning opportunity before program completion",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts, DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Enrollment in workforce training program",
                    "Participation in work-based learning",
                    "Student grade level (K-12)",
                    "Student grade level (PK)"
                ]
            },
        ["Digital skills"] =
            new()
            {
                Name = "Digital skills",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition =
                    "Students and workers can use digital technology tools effectively to access, manage, evaluate, and communicate information.",
                RecommendedMetrics =
                    "Percentage of individuals demonstrating proficiency on a performance assessment that measures digital skills required for workforce success, such as the Problem Solving in Technology-Rich Environments assessment within the Education & Skills Online assessment suite, which can be used by researchers and institutions to gather individual-level results based on Organisation for Economic Co-operation and Development (OECD) Survey of Adult Skills (Programme for the International Assessment of Adult Competencies [PIAAC]) domains",
                DataNeeded = [DataCategory.Assessments],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Digital skills assessments (K-12)",
                    "Digital skills assessments (Postsecondary)",
                    "Digital skills assessments (Workforce)"
                ]
            },
        ["Communication skills"] =
            new()
            {
                Name = "Communication skills",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition =
                    "Individuals have the oral, written, nonverbal, and listening skills required for success in school and at work.",
                RecommendedMetrics =
                    "Percentage of students demonstrating proficiency on assessments such as the College and Career Readiness Assessment (CCRA+), an assessment for grades 6-12 that measures critical thinking, problem solving, and written communications; Percentage of students demonstrating proficiency on assessments such as the Collegiate Learning Assessment (CLA+) or Success Skills Assessment (SSA+) for postsecondary students that measure critical thinking, problem solving, and written communications, or the HEIghten Outcomes Assessment for Written Communication; Percentage of individuals demonstrating proficiency on a performance assessment, such as the National Work Readiness Credential Essential Soft Skills assessment",
                DataNeeded = [DataCategory.Assessments],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Communication skills performance assessments (K-12)",
                    "Communication skills performance assessments (Postsecondary)",
                    "Communication skills performance assessments (Workforce)"
                ]
            },
        ["Higher-order thinking skills"] =
            new()
            {
                Name = "Higher-order thinking skills",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition =
                    "Individuals have the problem solving, critical thinking, and decision-making skills needed in the workplace.",
                RecommendedMetrics =
                    "Percentage of students demonstrating proficiency on assessments such as the College and Career Readiness Assessment (CLA+), an assessment for grades 6-12 that measures critical thinking, problem solving, and written communications; Percentage of students demonstrating proficiency on assessments such as the CLA+ or Success Skills Assessment (SSA+), assessments for postsecondary students that measure critical thinking, problem solving, and written communications, or the HEIghten Outcomes Assessment for Critical Thinking; Percentage of individuals demonstrating proficiency on assessments such as the Watson Glaser Critical Thinking Appraisal, a scenario-based assessment used by employers to evaluate candidates or identify areas of opportunity for growth",
                DataNeeded = [DataCategory.Assessments],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Higher-order thinking skills performance assessments (K-12)",
                    "Higher-order thinking skills performance assessments (Postsecondary)",
                    "Higher-order thinking skills performance assessments (Workforce)"
                ]
            },
        ["Minimum economic return"] =
            new()
            {
                Name = "Minimum economic return",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Individuals earn enough after completing their education to recover the costs of their investment.",
                RecommendedMetrics =
                    "Percentage of individuals that earn at least as much as the median high school graduate in their state plus enough to recoup their total net price plus interest within 10 years of completing their highest degree or leaving education (high school or postsecondary)",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Earnings",
                    "High school graduation date",
                    "Graduate credential attainment date",
                    "Total net price of education plus interest"
                ]
            },
        ["Student loan repayment"] =
            new()
            {
                Name = "Student loan repayment",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Individuals pay student loans on time and make progress toward paying down their debt.",
                RecommendedMetrics =
                    "Percentage of student borrowers in the following repayment categories, as defined on the College Scorecard—making progress, paid in full, and deferment—1, 2, 3, 5, and 10 years into the repayment phase of the loans. \"Making progress\" is defined as making regular payments such that the total of outstanding loan balances is less than the total of the original loan balances. \"Paid in full\" is defined as the outstanding loan balance being $0 and the loan not having been discharged through bankruptcy or other means. \"Deferment\" is defined as a postponement of the loan obligations, which is common for students re-enrolling in school. Borrowers who do not meet these milestones may fall in other categories, such as delinquency, default, and not making progress, that indicate they are unable to make timely progress toward their student debt.",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Repayment phase start date",
                    "Repayment status"
                ]
            },
        ["Employment in a quality job"] =
            new()
            {
                Name = "Employment in a quality job",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition =
                    "Individuals are employed in a position that offers a living wage, benefits, stable and predictable schedules, clear and fair advancement to higher pay, safe conditions, and job security.",
                RecommendedMetrics =
                    "Percentage of individuals employed in a quality job, as defined by scores on an indexed measure, such as the Good Jobs Scorecard, which assesses pay and benefits, scheduling, potential career paths, safety, and security",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames =
                [
                    "Job quality index",
                    /* ECS data elements */
                    "Employment status"
                ]
            },
        ["Economic mobility"] =
            new()
            {
                Name = "Economic mobility",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition =
                    "Individuals reach the level of earnings needed to enter the fourth income quintile or above, regardless of field of study.",
                RecommendedMetrics =
                    "Percentage of individuals who reach the level of earnings needed to enter the fourth (60th to 80th percentile) income quintile in their state or above 1, 3, 5, 10, and 15 years after completing their highest degree or leaving education (high school or postsecondary)",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames = [
                    /* ECS data elements */
                    "Earnings",
                    "High school graduation date",
                    "Graduate credential attainment date"
                ]
            },
        ["Economic security"] =
            new()
            {
                Name = "Economic security",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Individuals reach median levels of wealth (net worth).",
                RecommendedMetrics =
                    "Percentage of individuals who reach median levels of wealth 10, 15, 20, and 30 years after completing their highest degree or leaving education (high school or postsecondary)",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "High school graduation date",
                    "Net worth",
                    "Graduate credential attainment date"
                ]
            },
        ["Access to quality public pre-K"] =
            new()
            {
                Name = "Access to quality public pre-K",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Children have access to a high-quality public pre-K program.",
                RecommendedMetrics =
                    "Percentage of public pre-K programs that meet Quality Rating and Improvement Systems (QRIS) state benchmarks of quality",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.ClassroomObservations],
                DataElementNames = [
                    /* ECS data elements */
                    "Pre-K program QRIS rating"
                ]
            },
        ["Access to full-day pre-K"] =
            new()
            {
                Name = "Access to full-day pre-K",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Children have access to full-day, publicly funded pre-K programs.",
                RecommendedMetrics = "Percentage of public pre-K programs that are six hours per day for five days per week",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    "Kindergarten enrollment date",
                    /* ECS data elements */
                    "Pre-K program days per week",
                    "Pre-K program hours per day"
                ]
            },
        ["Access to child care subsidies"] =
            new()
            {
                Name = "Access to child care subsidies",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Eligible families have access to child care by using subsidies to pay for care.",
                RecommendedMetrics =
                    "Percentage of eligible families receiving assistance to pay for child care through subsidies",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Family eligibility for child care subsidies",
                    "Receipt of child care subsidies"
                ]
            },
        ["School-family engagement"] =
            new()
            {
                Name = "School-family engagement",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "There are effective partnerships between schools and families, such that parents have access to school systems and are meaningfully included in school processes and student learning.",
                RecommendedMetrics =
                    "Percentage of families and percentage of teachers or caregivers reporting positive relationship quality with one another, using a tool such as the Family and Provider/Teacher Relationship Quality (FPTRQ) parent survey; Mean scores on family surveys, such as the Panorama Family-School Relationships Survey or CORE Districts School Culture & Climate Survey parent assessment of school-community engagement",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Family engagement surveys (K-12)",
                    "Family engagement surveys (PK)"
                ]
            },
        ["Equitable discipline practices"] =
            new()
            {
                Name = "Equitable discipline practices",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Schools treat students similarly and appropriately for disciplinary infractions.",
                RecommendedMetrics =
                    "Differences in the rates at which students from key demographic subgroups ever experience different forms of school discipline (office referrals, suspensions, expulsions, restraint, and exclusion) relative to those students' representation in their school population as a whole; Disproportionalities along the lines of key demographic characteristics in the level of school discipline experienced (for example, number of days suspended).",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    "Number of days suspended (PK)",
                    "Office referrals (PK)",
                    /* ECS data elements */
                    "Number of days suspended (K-12)",
                    "Office referrals (K-12)",
                    "Restraint and seclusion for safety (K-12)",
                    "Suspensions and expulsions (K-12)"
                ]
            },
        ["Access to full-day kindergarten"] =
            new()
            {
                Name = "Access to full-day kindergarten",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Children have access to full-day kindergarten programs taught by the same certificated staff member in a day.",
                RecommendedMetrics =
                    "Percentage of schools and districts offering kindergarten programs that are six hours per day for five days per week",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Kindergarten program days per week",
                    "Kindergarten program hours per day"
                ]
            },
        ["English learner progress"] =
            new()
            {
                Name = "English learner progress",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Emerging multilingual students achieve English proficiency within five years of being classified as English learners.",
                RecommendedMetrics =
                    "Percentage of English learner students who are reclassified in five years or less, based on local reclassification criteria",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "English learner classification date",
                    "English learner status"
                ]
            },
        ["Teacher credentials"] =
            new()
            {
                Name = "Teacher credentials",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Students have access to teachers who have earned credentials demonstrating their knowledge and preparation for teaching.",
                RecommendedMetrics =
                    "• Pre-K: Percentage of lead teachers with at least a bachelor's degree • Pre-K: Percentage of lead teachers with specialized training in pre-K • K-12: Percentage of courses taught by full-time equivalent (FTE) teachers (that is, teachers other than substitutes or those with emergency or provisional licenses) • K-12: Percentage of courses taught by teachers certified to teach the given subject or grade level",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    "Credential or certification type",
                    "Highest level of education completed",
                    "Teacher qualification or certification type",
                    /* ECS data elements */
                    "Course identifier or title",
                    "Job title or position type",
                    "Staff FTE status",
                    "Teaching assignment"
                ]
            },
        ["Teacher experience"] =
            new()
            {
                Name = "Teacher experience",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students have equitable access to experienced teachers.",
                RecommendedMetrics =
                    "• Pre-K: Percentage of teachers with < 1 year, 1-5 years, and 5+ years of experience • K-12: Percentage of teachers with < 1 year, 1-5 years, and 5+ years of experience",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    "Years of teaching experience"
                ]
            },
        ["Educator retention"] =
            new()
            {
                Name = "Educator retention",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Teachers and school leaders return to the same school in consecutive years.",
                RecommendedMetrics =
                    "• Teacher retention: Percentage of teachers who return to teaching in the same school from year to year • School leader tenure: Percentage of school leaders who have served in their current positions for < 2 years, 2-3 years, and 4+ years",
                DataNeeded = [DataCategory.EducatorAdministrativeData],
                DataElementNames = ["Years in current position"]
            },
        ["Classroom observations of instructional practice"] =
            new()
            {
                Name = "Classroom observations of instructional practice",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Teachers demonstrate high-quality instructional practices and interactions with students.",
                RecommendedMetrics =
                    "• Pre-K: Scores on measures of teacher-child interactions, such as CLASS, the Early Childhood Environment Rating Scale (ECERS) Interactions subscale, or the Assessing Classroom Sociocultural Equity Scale (ACSES) (which assesses equitable classroom interactions) • K-12: Teachers' overall and subscale scores on an observation rubric associated with an educator observation system; examples of common frameworks include the Danielson's Framework for Teaching and the Marzano Causal Teacher Evaluation Model • Postsecondary: There are currently no widely used standardized rubrics for peer observations of college teaching, though multiple researchers and universities have produced guidance surrounding the peer observation process",
                DataNeeded = [DataCategory.ClassroomObservations],
                DataElementNames =
                [
                    "Teacher-child interaction measure (PK)",
                    /* ECS data elements */
                    "Instructor observations",
                    "Overall teacher observation score",
                    "Subscale observation scores",
                    "Teacher-child interaction measure (K-12)"
                ]
            },
        ["Student perceptions of teaching"] =
            new()
            {
                Name = "Student perceptions of teaching",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Students report having a supportive, engaging teacher who sets clear, fair, and high expectations, and helps them learn.",
                RecommendedMetrics =
                    "• K-12: Students' perceptions of their teacher's effectiveness, using a survey instrument such as the Pedagogical Effectiveness subscale of the Panorama Student Survey, the Tripod Student Survey, or the Ambitious Instruction and Supportive Environment domains of the 5Essentials Survey • Postsecondary: Students' perceptions of whether college instructors implement effective teaching practices, using a survey instrument such as the National Survey of Student Engagement",
                DataNeeded = [DataCategory.ClassroomObservations, DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Teacher effectiveness student surveys (K-12)",
                    "Teacher effectiveness student surveys (Postsecondary)"
                ]
            },
        ["Teachers' contributions to student learning growth"] =
            new()
            {
                Name = "Teachers' contributions to student learning growth",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Teachers contribute to students' learning growth.",
                RecommendedMetrics =
                    "• K-12 and postsecondary: Percentage of instructors demonstrating above average contributions to student learning, as measured by student growth on state standardized tests or other outcomes (for example, using value-added models or student growth percentiles)",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Assessments],
                DataElementNames =
                [
                    /* ECS data elements */
                    "SGP for standardized assessments",
                    "VAM for subject specific assessment"
                ]
            },
        ["Effective program and school leadership"] =
            new()
            {
                Name = "Effective program and school leadership",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Schools are led by effective principals and school leaders.",
                RecommendedMetrics =
                    "• Percentage of school leaders rated as effective, using an evaluation system that includes multiple measures, such as the Administrator Evaluation component of the Tennessee Educator Acceleration Model (TEAM)",
                DataNeeded =
                    [DataCategory.Assessments, DataCategory.Surveys, DataCategory.ClassroomObservations, DataCategory.Rubrics],
                DataElementNames = [
                    /* ECS data elements */
                    "Leader effectiveness assessments"
                ]
            },
        ["Institutions' contributions to student outcomes"] =
            new()
            {
                Name = "Institutions' contributions to student outcomes",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Schools and colleges contribute to students' short- and long-term outcomes.",
                RecommendedMetrics =
                    "• K-12: Schools' contributions to student outcomes, including achievement, attendance, social-emotional learning, college enrollment, and earnings, using value-added models • Postsecondary: Colleges' contributions to student outcomes, including graduation rates, earnings, and student loan repayment, using value-added models",
                DataNeeded =
                [
                    DataCategory.AdministrativeData, DataCategory.Assessments, DataCategory.StudentTranscripts,
                    DataCategory.Surveys
                ],
                DataElementNames =
                [
                    /* ECS data elements */
                    "College value-added",
                    "School value-added"
                ]
            },
        ["Access to college preparatory coursework"] =
            new()
            {
                Name = "Access to college preparatory coursework",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Students have access to the full set of courses needed to meet the requirements for admission at a majority of colleges.",
                RecommendedMetrics =
                    "• Percentage of high schools offering each of the following sets of college preparatory courses: - Four years of English - Four years of math (including at least four of the following: pre-algebra, algebra, geometry, Algebra II or trigonometry, precalculus, calculus, statistics, quantitative reasoning, and data science) - Three years of laboratory science (including biology, chemistry, physics) - Two years of social science - Two years of foreign language - One year of visual or performing arts  • Percentage of middle schools offering Algebra I",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Course department",
                    "Course identifier or title",
                    "Course offering by grade level"
                ]
            },
        ["Access to early college coursework"] =
            new()
            {
                Name = "Access to early college coursework",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students have access to AP, IB, and dual enrollment courses.",
                RecommendedMetrics =
                    "• Number of AP, IB, and dual enrollment courses offered, overall and by subject • Percentage of students in an early college course who take the relevant end-of-course test needed to earn credit (for example, AP or IB test), overall and by subject",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "AP course designation",
                    "AP, IB, or Dual Credit course credits",
                    "Course identifier or title",
                    "Dual credit course designation",
                    "IB course designation",
                    "Student course enrollment record"
                ]
            },
        ["Equitable placement in rigorous coursework"] =
            new()
            {
                Name = "Equitable placement in rigorous coursework",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Students from various demographic subgroups are proportionally represented in rigorous courses and programs.",
                RecommendedMetrics =
                    "Differences in the participation rates for students from key demographic subgroups in rigorous courses and programs relative to those students' representation in their school population as a whole, including opportunities, such as the following: • Gifted and talented programs • Algebra I in middle school • Higher-level math courses in high school (that is, Algebra II, calculus) • Early college courses (AP, IB, and dual enrollment)",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts],
                DataElementNames = [
                    /* ECS data elements */
                    "Gifted and talented participation",
                    "Student course enrollment record"
                ]
            },
        ["Access to quality, culturally responsive curricula"] =
            new()
            {
                Name = "Access to quality, culturally responsive curricula",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition =
                    "Schools and instructors use a standards-aligned core course curriculum that meets quality standards (as defined by EdReports) and is culturally relevant, centering the lived experiences and heritage of students' ethnic or racial backgrounds.",
                RecommendedMetrics = "No specific measures or tools identified",
                DataNeeded = [DataCategory.CurriculumMaterials],
                DataElementNames = [
                    "Percentage of teachers regularly using standards-aligned; culturally responsive curricula",
                    /* ECS data elements */
                    "Expenditures per student (K-12)",
                    "Expenditures per student (PK)",
                    "Expenditures per student (Postsecondary)",
                    "Student FTE status"
                ]
            },
        ["Expenditures per student"] =
            new()
            {
                Name = "Expenditures per student",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "The amount of education and related expenditures per student.",
                RecommendedMetrics =
                    "- Pre-K: State expenditures per child enrolled - K-12: Per pupil expenditures - K-12: Equity Factor, a measure that indicates variance in per-pupil funding within a state (see this brief by New America for more information) - Postsecondary: Total instruction and student service expenditures per FTE student based on 12-month enrollment",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Expenditures per student (K-12)",
                    "Expenditures per student (PK)",
                    "Expenditures per student (Postsecondary)",
                    "Student FTE status"
                ]
            },
        ["Access to early intervention screening"] =
            new()
            {
                Name = "Access to early intervention screening",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition =
                    "Children receive early intervention screening for any developmental, sensory, and behavioral concerns to determine whether services are needed.",
                RecommendedMetrics =
                    "Percentage of children with identified concerns who are connected to services; Percentage of children needing selected special education services in kindergarten who were not identified and connected to services before kindergarten",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Date of services provided",
                    "Early intervention screening results",
                    "Early intervention screening services referral status",
                    "Indicator of whether services were provided",
                    "Kindergarten enrollment date"
                ]
            },
        ["School safety"] =
            new()
            {
                Name = "School safety",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition =
                    "Students feel physically, mentally, and emotionally safe at school or campus (that is, safe from both physical threats and violence, as well as bullying and cyberbullying).",
                RecommendedMetrics =
                    "Percentage of students reporting high levels of physical, mental, and emotional safety in school climate surveys, such as the U.S. Department of Education ED School Climate Surveys (EDSCLS), the Sense of Safety subscale within the CORE Districts school culture and climate survey, or the School Safety subscale within the Panorama Student Survey. Percentage of students reporting physical safety and freedom from harassment and discrimination in campus surveys, such as the National Survey of Student Engagement.",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Campus climate surveys (K-12)",
                    "Campus climate surveys (Postsecondary)"
                ]
            },
        ["Inclusive environments"] =
            new()
            {
                Name = "Inclusive environments",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition =
                    "Individuals feel they belong and feel connected to their peers in their schools, postsecondary institutions, and workplaces.",
                RecommendedMetrics =
                    "Percentage of children reporting positive feelings toward their school, as measured by questionnaires such as the Collaborative for Academic, Social, and Emotional Learning's (CASEL) How I Feel About My School questionnaire, or percentage of classrooms demonstrating equitable sociocultural interactions, as measured by observational assessments, such as ACSES. Percentage of students reporting belonging in school, as measured by surveys such as the Sense of Belonging subscale of the CORE Districts school culture and climate survey or the Classroom Belonging subscale of the Panorama Student Survey. Percentage of students reporting belonging on campus, as measured by surveys such as the Higher Education Research Institute (HERI) Diverse Learning Environments Survey or the National Institute for Transformation and Equity (NITE) Culturally Engaging Campus Environments Survey. Percentage of employees reporting belonging at work, as measured by surveys such as the Association of American Medical Colleges (AAMC) Diversity Engagement Survey.",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Sense of belonging surveys",
                    "Sense of belonging surveys (K-12)",
                    "Sense of belonging surveys (PK)",
                    "Sense of belonging surveys (Postsecondary)",
                    "Sociocultural observational assessments"
                ]
            },
        ["Representational racial and ethnic diversity of educators"] =
            new()
            {
                Name = "Representational racial and ethnic diversity of educators",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Educators reflect the racial and ethnic diversity of the student body.",
                RecommendedMetrics =
                    "- Pre-K: Educational staff composition by race and ethnicity (%) compared to student composition by race and ethnicity (%) - K-12: Educational staff composition by race and ethnicity (%) compared to student composition by race and ethnicity (%) - Postsecondary: Educational staff composition by race and ethnicity (%) compared to student composition by race and ethnicity (%) - Additional possible measure: Same-race student-teacher ratio by race and ethnicity",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames = [
                    /* ECS data elements */
                    "Staff race/ethnicity",
                    "Student race/ethnicity"
                ]
            },
        ["School and workplace racial and ethnic diversity"] =
            new()
            {
                Name = "School and workplace racial and ethnic diversity",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition =
                    "Individuals are exposed to racial and ethnic diversity within their schools, postsecondary institutions, and workplaces.",
                RecommendedMetrics =
                    "Student body composition by race and ethnicity (%); Employee composition by race and ethnicity (%).",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames = [
                    /* ECS data elements */
                    "Employee race/ethnicity",
                    "Student race/ethnicity"
                ]
            },
        ["School and workplace socioeconomic diversity"] =
            new()
            {
                Name = "School and workplace socioeconomic diversity",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition =
                    "Individuals are exposed to socioeconomic diversity within their schools, postsecondary institutions, and workplaces.",
                RecommendedMetrics = "Student body composition by income; Employee composition by income.",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Employee income level",
                    "Student or family socioeconomic status",
                    "Student socioeconomic status"
                ]
            },
        ["Access to health, mental health, and social supports"] =
            new()
            {
                Name = "Access to health, mental health, and social supports",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition =
                    "Individuals have access to health, mental health, and social services provided by educational institutions and employers.",
                RecommendedMetrics =
                    "Percentage of programs offering health, mental health, and social services, or staff or consultants providing infant and early childhood mental health consultation (IECMHC) services; Ratio of number of students to number of health, mental health, and social services FTE staff (for example, school nurses, psychologists, and social workers); Percentage of employers offering an employee assistance program or mental health access through health care plans or other services, as measured by employer surveys.",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "EAP or mental health services provided",
                    "Health services offered",
                    "IECMHC services offered",
                    "Job title or position type",
                    "Mental health services offered",
                    "Social services offered",
                    "Staff FTE status"
                ]
            },
        ["Access to college and career advising"] =
            new()
            {
                Name = "Access to college and career advising",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "College and career counseling services are available in high schools and college campuses.",
                RecommendedMetrics =
                    "Ratio of number of students to number of FTE counselors; Percentage of students using academic advising and career counseling services.",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Advising and counseling service utilization",
                    "Job title or position type",
                    "Staff FTE status"
                ]
            },
        ["Access to in-demand CTE pathways"] =
            new()
            {
                Name = "Access to in-demand CTE pathways",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition =
                    "CTE pathway offerings are aligned to in-demand occupations, as defined by regional labor market data.",
                RecommendedMetrics = "Number and percentage of CTE program offerings considered \"in demand.\"",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "CTE program",
                    "In-demand status"
                ]
            },
        ["Unmet financial need"] =
            new()
            {
                Name = "Unmet financial need",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "The cost of college attendance students must pay out of pocket or finance through loans.",
                RecommendedMetrics =
                    "Average net price (cost of attendance minus grants, scholarships, or tuition waivers from all sources) minus average expected family contribution (EFC), as calculated by FAFSA.",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Average cost of attendance",
                    "Average expected family contribution (EFC)",
                    "Average financial aid amount (including grants, scholarships, and tuition waivers)"
                ]
            },
        ["Cumulative student debt"] =
            new()
            {
                Name = "Cumulative student debt",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "The total amount of student loans individuals take out while enrolled in college.",
                RecommendedMetrics = "Median student debt",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames = [
                    /* ECS data elements */
                    "Median student debt"
                ]
            },
        ["Expenditures on workforce development programs"] =
            new()
            {
                Name = "Expenditures on workforce development programs",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition =
                    "The amount of government funding dedicated to workforce development programs, including apprenticeships and job training programs, in a state.",
                RecommendedMetrics =
                    "The amount of funding dedicated to workforce development programs as a percentage of total educational funding in a state",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Funding dedicated to workforce development programs",
                    "Total educational funding"
                ]
            },
        ["Access to jobs paying a living wage"] =
            new()
            {
                Name = "Access to jobs paying a living wage",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Jobs that pay enough to meet basic family needs are available in a community.",
                RecommendedMetrics =
                    "Percentage of jobs in a county or metropolitan statistical area (MSA) for which the ratio of average pay to the location-adjusted cost of living is greater than one",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Average pay in county or MSA",
                    "Location-adjusted cost of living in county or MSA"
                ]
            },
        ["Access to ongoing career skills development"] =
            new()
            {
                Name = "Access to ongoing career skills development",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition =
                    "Workers are employed in jobs that provide on-the-job training or a professional learning and development path.",
                RecommendedMetrics =
                    "Percentage of employees who have access to on-the-job training or a professional learning and development plan directly from their employer",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Learning and development plan offered",
                    "On-the-job training offered"
                ]
            },
        ["Childhood experiences"] =
            new()
            {
                Name = "Childhood experiences",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "Individuals have not experienced repeated traumatic events within home environments.",
                RecommendedMetrics = "Percentage of individuals with fewer than three ACEs",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames = [
                    /* ECS data elements */
                    "Number of ACEs"
                ]
            },
        ["Health insurance coverage"] =
            new()
            {
                Name = "Health insurance coverage",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "Individuals have health insurance coverage for preventative and emergency care.",
                RecommendedMetrics =
                    "- Percentage of individuals with health insurance - Percentage of eligible individuals (children or adults) enrolled in Medicaid or CHIP",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "CHIP eligibility status",
                    "CHIP enrollment",
                    "Insured status",
                    "Medicaid eligibility status",
                    "Medicaid enrollment"
                ]
            },
        ["Food security"] =
            new()
            {
                Name = "Food security",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "Individuals have access to enough affordable, nutritious food.",
                RecommendedMetrics =
                    "- Percentage of individuals with high or marginal food security, as measured by the U.S. Department of Agriculture's (USDA) Food Security Survey Module - Percentage of eligible individuals participating in SNAP - Percentage of individuals living in a census tract with low access to healthy food, as defined by the USDA's Food Access Research Atlas",
                DataNeeded = [DataCategory.Surveys, DataCategory.AdministrativeData],
                DataElementNames = [
                    /* ECS data elements */
                    "SNAP eligibility",
                    "SNAP participation",
                    "USDA Food Access Research Atlas Access Level Flag",
                    "USDA Food Security Survey ratings"
                ]
            },
        ["Access to affordable housing"] =
            new()
            {
                Name = "Access to affordable housing",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition =
                    "There is sufficient availability of affordable housing for the number of families with low incomes in an area (city or county).",
                RecommendedMetrics =
                    "- Ratio of (1) the number of affordable housing units to (2) the number of households with low and very low incomes in an area (city or county). Housing units are defined as affordable if the monthly costs do not exceed 30 percent of a household's income. Households with low incomes are defined as those earning below 80 percent of area median income (AMI), and very low-income households are defined as those earning below 50 percent of AMI - Percentage of eligible households receiving federal rental assistance",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Eligibility for federal rental assistance",
                    "Number of affordable housing units in city or county",
                    "Number of households with low income in city or county",
                    "Number of households with very low income in city or county",
                    "Receipt of federal rental assistance"
                ]
            },
        ["Access to technology"] =
            new()
            {
                Name = "Access to technology",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition =
                    "Individuals have access to a reliable Internet connection and a personal desktop or laptop computer.",
                RecommendedMetrics =
                    "Percentage of individuals who have both (1) access to at least one desktop or laptop computer owned by someone in the home and (2) reliable broadband Internet",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Indicator of access to desktop or laptop at home",
                    "Indicator of access to reliable broadband internet"
                ]
            },
        ["Access to transportation"] =
            new()
            {
                Name = "Access to transportation",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "Individuals have access to low-cost and timely transportation to commute to school or work.",
                RecommendedMetrics =
                    "- Average commute time to work, school, or college - The Low Transportation Cost Index, from the U.S. Department of Housing and Urban Development",
                DataNeeded = [DataCategory.Surveys, DataCategory.AdministrativeData],
                DataElementNames =
                [
                    "Low Transportation Cost Index",
                    /* ECS data elements */
                    "Commute time"
                ]
            },
        ["Exposure to neighborhood crime"] =
            new()
            {
                Name = "Exposure to neighborhood crime",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "The rate of violent and property crimes in a city or county.",
                RecommendedMetrics =
                    "Rate of violent felonies and property felonies by city or county (number of incidents per 100,000 residents)",
                DataNeeded = [DataCategory.AdministrativeData],
                DataElementNames =
                [
                    /* ECS data elements */
                    "City or county population",
                    "Number of property felonies in city or county",
                    "Number of violent felonies in city or county"
                ]
            },
        ["Neighborhood economic diversity"] =
            new()
            {
                Name = "Neighborhood economic diversity",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "The concentration of poverty within a city or county.",
                RecommendedMetrics =
                    "Percentage of city or county residents experiencing poverty who live in a high-poverty neighborhood (defined as a neighborhood in which more than 40 percent of residents experience poverty)",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames =
                [
                    /* ECS data elements */
                    "Number of city or county residents experiencing poverty",
                    "Number of city or county residents living in a high poverty neighborhood"
                ]
            },
        ["Neighborhood racial diversity"] =
            new()
            {
                Name = "Neighborhood racial diversity",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "The share of an individual's neighbors who are people of other races and ethnicities.",
                RecommendedMetrics =
                    "Percentage of an individual's neighbors who are members of other racial or ethnic groups, calculated as a Neighborhood Exposure Index",
                DataNeeded = [DataCategory.Surveys],
                DataElementNames = [
                    /* ECS data elements */
                    "Geographical indicator",
                    "Student race/ethnicity"
                ]
            },
        ["Neighborhood juvenile arrests"] = new()
        {
            Name = "Neighborhood juvenile arrests",
            Type = IndicatorType.AdjacentSystemConditions,
            Domain = IndicatorDomain.CrossDomain,
            Definition = "The rate of juveniles arrested in a city or county.",
            RecommendedMetrics = "Rate of juvenile arrests by city or county (number of arrests per 100,000 residents)",
            DataNeeded = [DataCategory.AdministrativeData],
            DataElementNames =
            [
                /* ECS data elements */
                "City or county population",
                "Number of juvenile arrests in city or county"
            ]
        },
    };

    public static readonly Dictionary<string, DataElement> Elements = new()
    {
        ["ACT completion"] =
            new DataElement
            {
                Name = "ACT completion",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["ACT score"] =
            new DataElement
            {
                Name = "ACT score",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Advising and counseling service utilization"] =
            new DataElement
            {
                Name = "Advising and counseling service utilization",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Age"] =
            new DataElement
            {
                Name = "Age",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Numerical age as of point in time, or birth date"
            },
        ["AP course designation"] = // Should this be combined with IB and Dual Credit similar to the following element?
            new DataElement
            {
                Name = "AP course designation",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["AP, IB, or Dual Credit course credits"] =
            new DataElement
            {
                Name = "AP, IB, or Dual Credit course credits",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Apprenticeship program enrollment date"] =
            new DataElement
            {
                Name = "Apprenticeship program enrollment date",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Average cost of attendance"] =
            new DataElement
            {
                Name = "Average cost of attendance",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Average expected family contribution (EFC)"] =
            new DataElement
            {
                Name = "Average expected family contribution (EFC)",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Average financial aid amount (including grants, scholarships, and tuition waivers)"] =
            new DataElement
            {
                Name = "Average financial aid amount (including grants, scholarships, and tuition waivers)",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Average pay in county or MSA"] =
            new DataElement
            {
                Name = "Average pay in county or MSA",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Bachelor's degree completion date"] = // should this just be "credential attainment date"?
            new DataElement
            {
                Name = "Bachelor's degree completion date",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Basic skills level"] =
            new DataElement
            {
                Name = "Basic skills level",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Benefits availability"] =
            new DataElement
            {
                Name = "Benefits availability",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Campus climate surveys (K-12)"] =
            new DataElement
            {
                Name = "Campus climate surveys (K-12)",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Campus climate surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Campus climate surveys (Postsecondary)",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["CHIP eligibility status"] =
            new DataElement
            {
                Name = "CHIP eligibility status",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["CHIP enrollment"] =
            new DataElement
            {
                Name = "CHIP enrollment",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["City or county population"] =
            new DataElement
            {
                Name = "City or county population",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Civic engagement surveys (K-12)"] =
            new DataElement
            {
                Name = "Civic engagement surveys (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Civic engagement surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Civic engagement surveys (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Civic engagement surveys (Workforce)"] =
            new DataElement
            {
                Name = "Civic engagement surveys (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Cohort graduation year"] =
            new DataElement
            {
                Name = "Cohort graduation year",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Cohort year"] =
            new DataElement
            {
                Name = "Cohort year",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["College selectivity level"] =
            new DataElement
            {
                Name = "College selectivity level",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["College value-added"] =
            new DataElement
            {
                Name = "College value-added",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Communication skills performance assessments (K-12)"] =
            new DataElement
            {
                Name = "Communication skills performance assessments (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Communication skills performance assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Communication skills performance assessments (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Communication skills performance assessments (Workforce)"] =
            new DataElement
            {
                Name = "Communication skills performance assessments (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Commute time"] =
            new DataElement
            {
                Name = "Commute time",
                DataElementCategory = "Transportation",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Course department"] =
            new DataElement
            {
                Name = "Course department",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Course identifier or title"] =
            new DataElement
            {
                Name = "Course identifier or title",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Course offering by grade level"] =
            new DataElement
            {
                Name = "Course offering by grade level",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Course outcome"] =
            new DataElement
            {
                Name = "Course outcome",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "For example, completion, failure, passage"
            },
        ["Course performance (English and Math)"] = // Is "(English and Math)" reasonable or should this be "Course performance by subject area"
            new DataElement
            {
                Name = "Course performance (English and Math)",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Credential or certification type"] = // maybe more specifically "teacher credential", similar to "Teacher qualification or certification type"
            new DataElement
            {
                Name = "Credential or certification type",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Credential-seeking status"] =
            new DataElement
            {
                Name = "Credential-seeking status",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Credits earned in first year"] = // is "first year" appropriate here? or just "PS credits by year"?
            new DataElement
            {
                Name = "Credits earned in first year",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["CTE course completion"] = // Similar to "Course outcome", just a specific variety of course
            new DataElement
            {
                Name = "CTE course completion",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["CTE course ID or course title"] = // similar to "Course identifier or title" just for CTE... maybe this should be CTE course indicator?
            new DataElement
            {
                Name = "CTE course ID or course title",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["CTE pathway or career cluster associated with CTE course"] =
            new DataElement
            {
                Name = "CTE pathway or career cluster associated with CTE course",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["CTE program"] =
            new DataElement
            {
                Name = "CTE program",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Cultural competency assessments (K-12)"] =
            new DataElement
            {
                Name = "Cultural competency assessments (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Cultural competency assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Cultural competency assessments (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Cultural competency assessments (Workforce)"] =
            new DataElement
            {
                Name = "Cultural competency assessments (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Date of services provided"] =
            new DataElement
            {
                Name = "Date of services provided",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Developmental screening results"] =
            new DataElement
            {
                Name = "Developmental screening results",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Digital skills assessments (K-12)"] =
            new DataElement
            {
                Name = "Digital skills assessments (K-12)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Digital skills assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Digital skills assessments (Postsecondary)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Digital skills assessments (Workforce)"] =
            new DataElement
            {
                Name = "Digital skills assessments (Workforce)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Diploma or credential award date"] = // related to High school graduation date, which we already have -- this seems to be exclusively about ACGR metric for HS, skipping
            new DataElement
            {
                Name = "Diploma or credential award date",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Direct child assessments (cognition)"] =
            new DataElement
            {
                Name = "Direct child assessments (cognition)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Direct child assessments (language and literacy)"] =
            new DataElement
            {
                Name = "Direct child assessments (language and literacy)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Direct child assessments of executive function"] =
            new DataElement
            {
                Name = "Direct child assessments of executive function",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Direct child assessments of physical development"] =
            new DataElement
            {
                Name = "Direct child assessments of physical development",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Disability status"] =
            new DataElement
            {
                Name = "Disability status",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Dislocated worker status"] =
            new DataElement
            {
                Name = "Dislocated worker status",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Dual credit course designation"] = // May want to combine with AP / IB
            new DataElement
            {
                Name = "Dual credit course designation",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["EAP or mental health services provided"] =
            new DataElement
            {
                Name = "EAP or mental health services provided",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Early intervention screening results"] =
            new DataElement
            {
                Name = "Early intervention screening results",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Early intervention screening services referral status"] =
            new DataElement
            {
                Name = "Early intervention screening services referral status",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Earnings"] =
            new DataElement
            {
                Name = "Earnings",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Eligibility for federal rental assistance"] =
            new DataElement
            {
                Name = "Eligibility for federal rental assistance",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Employee income level"] = // This one is similar to "Earnings" but is more about diversity in a workplace
            new DataElement
            {
                Name = "Employee income level",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Employee race/ethnicity"] = // This one is similar to Staff race/ethnicity... but more oriented around workplace rather than school employees
            new DataElement
            {
                Name = "Employee race/ethnicity",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Employment date"] = // specifically in the context of someone in the workforce, not staff
            new DataElement
            {
                Name = "Employment date",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Employment status"] = // specifically in the context of someone in the workforce, not staff
            new DataElement
            {
                Name = "Employment status",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["English learner classification date"] =
            new DataElement
            {
                Name = "English learner classification date",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["English learner status"] =
            new DataElement
            {
                Name = "English learner status",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Enlistment in the military"] =
            new DataElement
            {
                Name = "Enlistment in the military",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Enrollment date"] = // Related to other enrollment dates, pre-K, kindergarten, postsecondary, graduate program, etc., where this is defined in the metrics too it seems "Cohort year" is actually more precise
            new DataElement
            {
                Name = "Enrollment date",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Enrollment in noncredit CTE date"] =
            new DataElement
            {
                Name = "Enrollment in noncredit CTE date",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Enrollment in public pre-K"] =
            new DataElement
            {
                Name = "Enrollment in public pre-K",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Enrollment in workforce training program"] =
            new DataElement
            {
                Name = "Enrollment in workforce training program",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Enrollment status (current and prior years)"] = // should this be split into K-12 vs PS? Indicator for question 15 seems to be PS oriented specifically, there are separate tables for enrollment
            new DataElement
            {
                Name = "Enrollment status (current and prior years)",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Expenditures per student (K-12)"] = // overlaps with Institutional expenditure per student?
            new DataElement
            {
                Name = "Expenditures per student (K-12)",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Expenditures per student (PK)"] =
            new DataElement
            {
                Name = "Expenditures per student (PK)",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Expenditures per student (Postsecondary)"] =
            new DataElement
            {
                Name = "Expenditures per student (Postsecondary)",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["FAFSA completion date"] =
            new DataElement
            {
                Name = "FAFSA completion date",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Family eligibility for child care subsidies"] =
            new DataElement
            {
                Name = "Family eligibility for child care subsidies",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Family engagement surveys (K-12)"] =
            new DataElement
            {
                Name = "Family engagement surveys (K-12)",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Family engagement surveys (PK)"] =
            new DataElement
            {
                Name = "Family engagement surveys (PK)",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["First-generation college student"] =
            new DataElement
            {
                Name = "First-generation college student",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["First-time 9th grade student status"] = // can't find this
            new DataElement
            {
                Name = "First-time 9th grade student status",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Funding dedicated to workforce development programs"] =
            new DataElement
            {
                Name = "Funding dedicated to workforce development programs",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Gateway course completion"] =
            new DataElement
            {
                Name = "Gateway course completion",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Gender"] = // Should this be split up by sector similar to other elements with "(K-12)" type sucffixes in the names?
            new DataElement
            {
                Name = "Gender",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Geographical indicator"] = // Should this be Student place of residence? The goal is for this to be relatable to census data for racial/ethnic diversity
            new DataElement
            {
                Name = "Geographical indicator",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "For example, census tract, city, or county"
            },
        ["Gifted and talented participation"] =
            new DataElement
            {
                Name = "Gifted and talented participation",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Grade point average (K-12)"] =
            new DataElement
            {
                Name = "Grade point average (K-12)",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Grade point average (Postsecondary)"] =
            new DataElement
            {
                Name = "Grade point average (Postsecondary)",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Graduate credential attainment date"] = // Should this just be "credential attainment date"
            new DataElement
            {
                Name = "Graduate credential attainment date",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Graduate program enrollment date"] =
            new DataElement
            {
                Name = "Graduate program enrollment date",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Growth mindset surveys (K-12)"] =
            new DataElement
            {
                Name = "Growth mindset surveys (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Growth mindset surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Growth mindset surveys (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Growth mindset surveys (Workforce)"] =
            new DataElement
            {
                Name = "Growth mindset surveys (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Health services offered"] = // seems to only be used in PK context
            new DataElement
            {
                Name = "Health services offered",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Health-Related Quality of Life Scale scores"] =
            new DataElement
            {
                Name = "Health-Related Quality of Life Scale scores",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["High school diploma type"] =
            new DataElement
            {
                Name = "High school diploma type",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["High school graduation date"] =
            new DataElement
            {
                Name = "High school graduation date",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["High school graduation indicator"] =
            new DataElement
            {
                Name = "High school graduation indicator",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Higher-order thinking skills performance assessments (K-12)"] =
            new DataElement
            {
                Name = "Higher-order thinking skills performance assessments (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Higher-order thinking skills performance assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Higher-order thinking skills performance assessments (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Higher-order thinking skills performance assessments (Workforce)"] =
            new DataElement
            {
                Name = "Higher-order thinking skills performance assessments (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Highest level of education completed"] = // This is in reference to teachers, so related to instruction, not individual longitudinally.
            new DataElement
            {
                Name = "Highest level of education completed",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Home language"] =
            new DataElement
            {
                Name = "Home language",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["IB course designation"] = // maybe common course designation element instead with a distribution?
            new DataElement
            {
                Name = "IB course designation",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["IECMHC services offered"] =
            new DataElement
            {
                Name = "IECMHC services offered",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["In-demand status"] =
            new DataElement
            {
                Name = "In-demand status",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Income level"] = // as a disaggregate, probably the "target" individual is the person moving through the E-W continuum, not staff or instructors
            new DataElement
            {
                Name = "Income level",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Indicator of access to desktop or laptop at home"] =
            new DataElement
            {
                Name = "Indicator of access to desktop or laptop at home",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Indicator of access to reliable broadband internet"] =
            new DataElement
            {
                Name = "Indicator of access to reliable broadband internet",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Indicator of whether services were provided"] =
            new DataElement
            {
                Name = "Indicator of whether services were provided",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Individual or family military status"] = // disaggregate
            new DataElement
            {
                Name = "Individual or family military status",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Individual with current or past child welfare involvement"] = // disaggregate
            new DataElement
            {
                Name = "Individual with current or past child welfare involvement",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Individuals experiencing homelessness"] = // disaggregate
            new DataElement
            {
                Name = "Individuals experiencing homelessness",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Industry-recognized credential attainment"] =
            new DataElement
            {
                Name = "Industry-recognized credential attainment",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Institution graduation rate"] = // Should we look at primary elements that could derive this as well?
            new DataElement
            {
                Name = "Institution graduation rate",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Institutional expenditure per student"] = // Seems same as the "Expenditure per puil" elements?
            new DataElement
            {
                Name = "Institutional expenditure per student",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Instructor observations"] =
            new DataElement
            {
                Name = "Instructor observations",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Insured status"] =
            new DataElement
            {
                Name = "Insured status",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Job quality index"] =
            new DataElement
            {
                Name = "Job quality index",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Job title or position type"] =
            new DataElement
            {
                Name = "Job title or position type",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Justice involvement"] =
            new DataElement
            {
                Name = "Justice involvement",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["K-12 school type"] =
            new DataElement
            {
                Name = "K-12 school type",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Kindergarten enrollment date"] =
            new DataElement
            {
                Name = "Kindergarten enrollment date",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Kindergarten program days per week"] =
            new DataElement
            {
                Name = "Kindergarten program days per week",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Kindergarten program hours per day"] =
            new DataElement
            {
                Name = "Kindergarten program hours per day",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Kindergarten readiness assessments (cognition)"] =
            new DataElement
            {
                Name = "Kindergarten readiness assessments (cognition)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Leader effectiveness assessments"] =
            new DataElement
            {
                Name = "Leader effectiveness assessments",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Learning and development plan offered"] = // Maybe make it clear this is talking about WF specifically in the data element name?
            new DataElement
            {
                Name = "Learning and development plan offered",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["LGBT status"] =
            new DataElement
            {
                Name = "LGBT status",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Location-adjusted cost of living in county or MSA"] =
            new DataElement
            {
                Name = "Location-adjusted cost of living in county or MSA",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Low Transportation Cost Index"] =
            new DataElement
            {
                Name = "Low Transportation Cost Index",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Math proficiency (Grades 1 and 2)"] =
            new DataElement
            {
                Name = "Math proficiency (Grades 1 and 2)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Median student debt"] =
            new DataElement
            {
                Name = "Median student debt",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Medicaid eligibility status"] =
            new DataElement
            {
                Name = "Medicaid eligibility status",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Medicaid enrollment"] =
            new DataElement
            {
                Name = "Medicaid enrollment",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Mental and emotional well-being assessments"] =
            new DataElement
            {
                Name = "Mental and emotional well-being assessments",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Mental health services offered"] =
            new DataElement
            {
                Name = "Mental health services offered",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Net worth"] =
            new DataElement
            {
                Name = "Net worth",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Number of ACEs"] =
            new DataElement
            {
                Name = "Number of ACEs",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "ACE is an abbreviation for \"adverse childhood experiences\""
            },
        ["Number of affordable housing units in city or county"] =
            new DataElement
            {
                Name = "Number of affordable housing units in city or county",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Number of city or county residents experiencing poverty"] =
            new DataElement
            {
                Name = "Number of city or county residents experiencing poverty",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Number of city or county residents living in a high poverty neighborhood"] =
            new DataElement
            {
                Name = "Number of city or county residents living in a high poverty neighborhood",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Number of credits attempted"] =
            new DataElement
            {
                Name = "Number of credits attempted",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Number of credits earned"] =
            new DataElement
            {
                Name = "Number of credits earned",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Number of days suspended (K-12)"] =
            new DataElement
            {
                Name = "Number of days suspended (K-12)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Number of days suspended (PK)"] =
            new DataElement
            {
                Name = "Number of days suspended (PK)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Number of households with low income in city or county"] =
            new DataElement
            {
                Name = "Number of households with low income in city or county",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Number of households with very low income in city or county"] =
            new DataElement
            {
                Name = "Number of households with very low income in city or county",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Number of juvenile arrests in city or county"] =
            new DataElement
            {
                Name = "Number of juvenile arrests in city or county",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Number of property felonies in city or county"] =
            new DataElement
            {
                Name = "Number of property felonies in city or county",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Number of violent felonies in city or county"] =
            new DataElement
            {
                Name = "Number of violent felonies in city or county",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Occupation category"] =
            new DataElement
            {
                Name = "Occupation category",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Office referrals (K-12)"] =
            new DataElement
            {
                Name = "Office referrals (K-12)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Office referrals (PK)"] =
            new DataElement
            {
                Name = "Office referrals (PK)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["On-the-job training offered"] =
            new DataElement
            {
                Name = "On-the-job training offered",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Overall teacher observation score"] =
            new DataElement
            {
                Name = "Overall teacher observation score",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Parental education level"] =
            new DataElement
            {
                Name = "Parental education level",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Participation in work-based learning"] =
            new DataElement
            {
                Name = "Participation in work-based learning",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Pell grant receipt"] =
            new DataElement
            {
                Name = "Pell grant receipt",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Percentage of teachers regularly using standards-aligned; culturally responsive curricula"] = // quite a wordy and wide scope
            new DataElement
            {
                Name = "Percentage of teachers regularly using standards-aligned; culturally responsive curricula",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Physical health surveys (K-12)"] =
            new DataElement
            {
                Name = "Physical health surveys (K-12)",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Physical health surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Physical health surveys (Postsecondary)",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Physical health surveys (Workforce)"] =
            new DataElement
            {
                Name = "Physical health surveys (Workforce)",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Post-baccalaureate program enrollment date"] =
            new DataElement
            {
                Name = "Post-baccalaureate program enrollment date",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary applications submitted"] =
            new DataElement
            {
                Name = "Postsecondary applications submitted",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary credential attainment date"] =
            new DataElement
            {
                Name = "Postsecondary credential attainment date",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary credential earned"] =
            new DataElement
            {
                Name = "Postsecondary credential earned",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary degree program length"] =
            new DataElement
            {
                Name = "Postsecondary degree program length",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary enrollment date"] =
            new DataElement
            {
                Name = "Postsecondary enrollment date",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary enrollment status (Full time/part time)"] =
            new DataElement
            {
                Name = "Postsecondary enrollment status (Full time/part time)",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary institution classification"] =
            new DataElement
            {
                Name = "Postsecondary institution classification",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary Institution ID"] =
            new DataElement
            {
                Name = "Postsecondary Institution ID",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary institution ID (current and prior years)"] = // This is fairly duplicative
            new DataElement
            {
                Name = "Postsecondary institution ID (current and prior years)",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary major"] =
            new DataElement
            {
                Name = "Postsecondary major",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Pre-K eligibility status"] =
            new DataElement
            {
                Name = "Pre-K eligibility status",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Pre-K program days per week"] =
            new DataElement
            {
                Name = "Pre-K program days per week",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Pre-K program hours per day"] =
            new DataElement
            {
                Name = "Pre-K program hours per day",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Pre-K program QRIS rating"] =
            new DataElement
            {
                Name = "Pre-K program QRIS rating",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Reading proficiency (Grades 1 and 2)"] =
            new DataElement
            {
                Name = "Reading proficiency (Grades 1 and 2)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Receipt of child care subsidies"] =
            new DataElement
            {
                Name = "Receipt of child care subsidies",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Receipt of federal rental assistance"] =
            new DataElement
            {
                Name = "Receipt of federal rental assistance",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Repayment phase start date"] =
            new DataElement
            {
                Name = "Repayment phase start date",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Repayment status"] =
            new DataElement
            {
                Name = "Repayment status",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Reported intent to enroll in postsecondary education"] =
            new DataElement
            {
                Name = "Reported intent to enroll in postsecondary education",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Reported kindergarten readiness (behavioral skills)"] =
            new DataElement
            {
                Name = "Reported kindergarten readiness (behavioral skills)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes =
                    "Teacher-reported kindergarten readiness assessments (self-regulation; approaches to learning; executive function)"
            },
        ["Reported kindergarten readiness (language and literacy)"] =
            new DataElement
            {
                Name = "Reported kindergarten readiness (language and literacy)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported kindergarten readiness assessments (language and literacy)"
            },
        ["Reported kindergarten readiness (physical development)"] =
            new DataElement
            {
                Name = "Reported kindergarten readiness (physical development)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported kindergarten readiness assessments (physical development)"
            },
        ["Reported kindergarten readiness (social-emotional skills)"] =
            new DataElement
            {
                Name = "Reported kindergarten readiness (social-emotional skills)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported kindergarten readiness assessments (social-emotional development)"
            },
        ["Restraint and seclusion for discipline (K-12)"] =
            new DataElement
            {
                Name = "Restraint and seclusion for discipline (K-12)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Restraint and seclusion for discipline (PK)"] =
            new DataElement
            {
                Name = "Restraint and seclusion for discipline (PK)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Restraint and seclusion for safety (K-12)"] =
            new DataElement
            {
                Name = "Restraint and seclusion for safety (K-12)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Restraint and seclusion for safety (PK)"] =
            new DataElement
            {
                Name = "Restraint and seclusion for safety (PK)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["SAT completion"] =
            new DataElement
            {
                Name = "SAT completion",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["SAT score"] =
            new DataElement
            {
                Name = "SAT score",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["School assignment (prior and current year)"] =
            new DataElement
            {
                Name = "School assignment (prior and current year)",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["School value-added"] =
            new DataElement
            {
                Name = "School value-added",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Self-efficacy surveys (K-12)"] =
            new DataElement
            {
                Name = "Self-efficacy surveys (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Self-efficacy surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Self-efficacy surveys (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Self-efficacy surveys (Workforce)"] =
            new DataElement
            {
                Name = "Self-efficacy surveys (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Self-management surveys (K-12)"] =
            new DataElement
            {
                Name = "Self-management surveys (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Self-management surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Self-management surveys (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Self-management surveys (Workforce)"] =
            new DataElement
            {
                Name = "Self-management surveys (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Sense of belonging surveys"] = // missing WF identifier?
            new DataElement
            {
                Name = "Sense of belonging surveys",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Sense of belonging surveys (K-12)"] =
            new DataElement
            {
                Name = "Sense of belonging surveys (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Sense of belonging surveys (PK)"] =
            new DataElement
            {
                Name = "Sense of belonging surveys (PK)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Sense of belonging surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Sense of belonging surveys (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["SGP for standardized assessments"] =
            new DataElement
            {
                Name = "SGP for standardized assessments",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student growth percentile, subject specific for reading/literacy, math, and science"
            },
        ["SNAP eligibility"] =
            new DataElement
            {
                Name = "SNAP eligibility",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["SNAP participation"] =
            new DataElement
            {
                Name = "SNAP participation",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Social awareness teacher ratings"] =
            new DataElement
            {
                Name = "Social awareness teacher ratings",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Social capital surveys (K-12)"] =
            new DataElement
            {
                Name = "Social capital surveys (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Social capital surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Social capital surveys (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Social capital surveys (Workforce)"] =
            new DataElement
            {
                Name = "Social capital surveys (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Social proficiency performance assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Social proficiency performance assessments (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Social proficiency performance assessments (Workforce)"] =
            new DataElement
            {
                Name = "Social proficiency performance assessments (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Social services offered"] =
            new DataElement
            {
                Name = "Social services offered",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Sociocultural observational assessments"] =
            new DataElement
            {
                Name = "Sociocultural observational assessments",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Staff FTE status"] =
            new DataElement
            {
                Name = "Staff FTE status",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Staff race/ethnicity"] =
            new DataElement
            {
                Name = "Staff race/ethnicity",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["State standardized test (Math proficiency)"] =
            new DataElement
            {
                Name = "State standardized test (Math proficiency)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["State standardized test (Reading proficiency)"] =
            new DataElement
            {
                Name = "State standardized test (Reading proficiency)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Student attendance rate (K-12)"] =
            new DataElement
            {
                Name = "Student attendance rate (K-12)",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Student attendance rate (PK)"] =
            new DataElement
            {
                Name = "Student attendance rate (PK)",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Student course enrollment record"] = // This one replaces individual subjects, but we should probably split this between K-12 and PS...
            new DataElement
            {
                Name = "Student course enrollment record",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Student from migrant family household"] =
            new DataElement
            {
                Name = "Student from migrant family household",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Student FTE status"] = // This seems to already be covered by PostsecondaryEnrollmentStatusFullTimePartTime
            new DataElement
            {
                Name = "Student FTE status",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Student grade level"] = // This is duplicative of the one below
            new DataElement
            {
                Name = "Student grade level",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Student grade level (K-12)"] =
            new DataElement
            {
                Name = "Student grade level (K-12)",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Student grade level (PK)"] =
            new DataElement
            {
                Name = "Student grade level (PK)",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Student or family socioeconomic status"] = // May want to split this to income level AND "economic disadvantage status" so we could essentially mark this as partially available for systems like CEDS DW. Also we have "Income Level" as a disaggregate which is a bit of an overlap...
            new DataElement
            {
                Name = "Student or family socioeconomic status",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Student parenting status"] =
            new DataElement
            {
                Name = "Student parenting status",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Student race/ethnicity"] = // not used strictly as a disaggregate, likely need to track PK vs K12 vs PS here
            new DataElement
            {
                Name = "Student race/ethnicity",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Student socioeconomic status"] =
            new DataElement
            {
                Name = "Student socioeconomic status", // seems to overlap with "Student or family" version above...
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS], // this is listed as PS metric only
                AdditionalNotes = null
            },
        ["Subscale observation scores"] = // Not very descriptive -- this refers to teacher observation subscale scores in the context of teacher effectiveness
            new DataElement
            {
                Name = "Subscale observation scores",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Suspensions and expulsions (K-12)"] =
            new DataElement
            {
                Name = "Suspensions and expulsions (K-12)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Suspensions and expulsions (PK)"] =
            new DataElement
            {
                Name = "Suspensions and expulsions (PK)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Teacher effectiveness student surveys (K-12)"] =
            new DataElement
            {
                Name = "Teacher effectiveness student surveys (K-12)",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Teacher effectiveness student surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Teacher effectiveness student surveys (Postsecondary)",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Teacher qualification or certification type"] =
            new DataElement
            {
                Name = "Teacher qualification or certification type",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Teacher reports of executive function"] =
            new DataElement
            {
                Name = "Teacher reports of executive function",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Teacher reports of social-emotional development"] =
            new DataElement
            {
                Name = "Teacher reports of social-emotional development",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Teacher-child interaction measure (K-12)"] =
            new DataElement
            {
                Name = "Teacher-child interaction measure (K-12)",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Teacher-child interaction measure (PK)"] =
            new DataElement
            {
                Name = "Teacher-child interaction measure (PK)",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Teaching assignment"] = // May need to break out and include "course teaching requirements" or something, the spirit is really about "qualification to teach a particular course"
            new DataElement
            {
                Name = "Teaching assignment",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Total educational funding"] =
            new DataElement
            {
                Name = "Total educational funding",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Total net price of education plus interest"] =
            new DataElement
            {
                Name = "Total net price of education plus interest",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Transfer enrollment status"] =
            new DataElement
            {
                Name = "Transfer enrollment status",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Transfer indicator or transfer student status"] =
            new DataElement
            {
                Name = "Transfer indicator or transfer student status",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Universal screening results"] =
            new DataElement
            {
                Name = "Universal screening results",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Urbanicity"] =
            new DataElement
            {
                Name = "Urbanicity",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["USDA Food Access Research Atlas Access Level Flag"] =
            new DataElement
            {
                Name = "USDA Food Access Research Atlas Access Level Flag",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["USDA Food Security Survey ratings"] =
            new DataElement
            {
                Name = "USDA Food Security Survey ratings",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["VAM for subject specific assessment"] =
            new DataElement
            {
                Name = "VAM for subject specific assessment",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Workforce development program participation"] =
            new DataElement
            {
                Name = "Workforce development program participation",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Years in current position"] =
            new DataElement
            {
                Name = "Years in current position",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Years of teaching experience"] = new DataElement
        {
            Name = "Years of teaching experience",
            DataElementCategory = "Staff & Educators",
            RelatedSectors = [Sector.K12],
            AdditionalNotes = null
        },
    };

    public static IReadOnlyList<Disaggregate> Disaggregates { get; } =
    [
        new Disaggregate
        {
            Name = "Race and ethnicity",
            Description = "Self-reported race and ethnicity used to identify disparities and inequities across systems and outcomes.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames = ["Student race/ethnicity"]
        },
        new Disaggregate
        {
            Name = "Gender",
            Description = "Self-identified gender used to analyze gender-based disparities in education and workforce outcomes.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames = ["Gender"]
        },
        new Disaggregate
        {
            Name = "LGBT status",
            Description = "Sexual orientation and gender identity information used to understand disparities affecting LGBT individuals.",
            Sectors = [Sector.K12, Sector.PS, Sector.WF],
            DataElementNames = ["LGBT status"]
        },
        new Disaggregate
        {
            Name = "Disability status",
            Description = "Whether an individual has a disability or receives disability-related supports or accommodations.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames = ["Disability status"]
        },
        new Disaggregate
        {
            Name = "Income level",
            Description = "Household or individual income level, often used as a proxy for socioeconomic status.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames = ["Income level"]
        },
        new Disaggregate
        {
            Name = "Parental education level",
            Description = "Highest educational attainment of either parent or guardian.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames = ["Parental education level"]
        },
        new Disaggregate
        {
            Name = "First-generation college student",
            Description = "Whether a student's parents or guardians have completed a postsecondary degree.",
            Sectors = [Sector.K12, Sector.PS],
            DataElementNames = ["First-generation college student"]
        },
        new Disaggregate
        {
            Name = "Student from migrant family household",
            Description = "Whether a student belongs to a migrant family household, often associated with migratory agricultural or fishing work.",
            Sectors = [Sector.PK, Sector.K12],
            DataElementNames = ["Student from migrant family household"]
        },
        new Disaggregate
        {
            Name = "Home language",
            Description = "Primary language spoken at home or by the individual.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames = ["Home language"]
        },
        new Disaggregate
        {
            Name = "English learner",
            Description = "Whether an individual is identified as needing support to develop English proficiency.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames = ["English learner status"]
        },
        new Disaggregate
        {
            Name = "Attendance intensity",
            Description = "The extent of participation or enrollment, such as full-time versus part-time attendance.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS],
            DataElementNames = []
        },
        new Disaggregate
        {
            Name = "K-12 school type",
            Description = "The type of school attended, such as traditional public, charter, magnet, private, or alternative school.",
            Sectors = [Sector.K12],
            DataElementNames = ["K-12 school type"]
        },
        new Disaggregate
        {
            Name = "Postsecondary institution classification",
            Description = "The classification or type of postsecondary institution attended, such as two-year, four-year, public, or private.",
            Sectors = [Sector.PS],
            DataElementNames = ["Postsecondary institution classification"]
        },
        new Disaggregate
        {
            Name = "Transfer enrollment status",
            Description = "Whether a postsecondary student transferred from another institution.",
            Sectors = [Sector.PS],
            DataElementNames = ["Transfer enrollment status"]
        },
        new Disaggregate
        {
            Name = "Credential-seeking status",
            Description = "Whether a postsecondary student is formally seeking a credential, degree, or certificate.",
            Sectors = [Sector.PS],
            DataElementNames = ["Credential-seeking status"]
        },
        new Disaggregate
        {
            Name = "Student parenting status",
            Description = "Whether a postsecondary student is a parent or has dependent children.",
            Sectors = [Sector.PS],
            DataElementNames = ["Student parenting status"]
        },
        new Disaggregate
        {
            Name = "Postsecondary major",
            Description = "The academic field or program of study pursued in postsecondary education.",
            Sectors = [Sector.PS, Sector.WF],
            DataElementNames = ["Postsecondary major"]
        },
        new Disaggregate
        {
            Name = "Occupation category",
            Description = "Occupational grouping or job category associated with employment.",
            Sectors = [Sector.WF],
            DataElementNames = ["Occupation category"]
        },
        new Disaggregate
        {
            Name = "Dislocated worker status",
            Description = "Whether an individual lost employment because of layoffs, closures, or economic conditions.",
            Sectors = [Sector.PS, Sector.WF],
            DataElementNames = ["Dislocated worker status"]
        },
        new Disaggregate
        {
            Name = "Basic skills level",
            Description = "Literacy, numeracy, or foundational skill proficiency relevant to workforce participation.",
            Sectors = [Sector.WF],
            DataElementNames = ["Basic skills level"]
        },
        new Disaggregate
        {
            Name = "Age group",
            Description = "Age-based grouping used to analyze trends and outcomes across developmental or workforce stages.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames = ["Age"]
        },
        new Disaggregate
        {
            Name = "Urbanicity",
            Description = "Whether an individual or institution is located in an urban, suburban, town, or rural area.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames = ["Urbanicity"]
        },
        new Disaggregate
        {
            Name = "Individuals experiencing homelessness",
            Description = "Whether an individual lacks stable, permanent, or adequate housing.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames = ["Individuals experiencing homelessness"]
        },
        new Disaggregate
        {
            Name = "Individual or family military status",
            Description = "Whether an individual or family member serves or has served in the military.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames = ["Individual or family military status"]
        },
        new Disaggregate
        {
            Name = "Individual with current or past child welfare involvement",
            Description = "Whether an individual has current or prior involvement with foster care or child welfare systems.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS],
            DataElementNames = ["Individual with current or past child welfare involvement"]
        },
        new Disaggregate
        {
            Name = "Justice involvement",
            Description = "Whether an individual has interacted with the juvenile or criminal justice system in any capacity.",
            Sectors = [Sector.K12, Sector.PS, Sector.WF],
            DataElementNames = ["Justice involvement"]
        }
    ];
}