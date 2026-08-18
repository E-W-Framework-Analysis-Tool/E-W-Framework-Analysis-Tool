using EwFrameworkAnalysis.Common.Models.Framework;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public static class EwFrameworkIndicators
{
    public static readonly Dictionary<string, Indicator> Indicators = new()
    {
        ["6th grade on track"] =
            new()
            {
                Name = "6th grade on track",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Grade 6 students are on track to graduate high school on time.",
                RecommendedMetrics = "Percentage of students in grade 6 with passing grades in English language arts and math, attendance of 90 percent or higher, and no in- or out-of-school suspensions or expulsions",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Course performance (English and Math)",
                    "Suspensions and expulsions (K-12)",
                    "Student attendance rate (K-12)",
                    "Student grade level (K-12)",
                ]
            },
        ["8th grade on track"] =
            new()
            {
                Name = "8th grade on track",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Grade 8 students are prepared to transition to high school and are on track to graduate on time.",
                RecommendedMetrics = "Percentage of students in grade 8 with a GPA of 2.5 or higher, no Ds or Fs in English language arts or math, attendance of 96 percent or higher, and no in- or out-of-school suspensions or expulsions.",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Course performance (English and Math)",
                    "Grade point average (K-12)",
                    "Suspensions and expulsions (K-12)",
                    "Student grade level (K-12)",
                    "Student attendance rate (K-12)",
                ]
            },
        ["9th grade on track"] =
            new()
            {
                Name = "9th grade on track",
                Cluster = [IndicatorCluster.PostsecondaryTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Grade 9 students are on track to graduate high school in four years, enroll in postsecondary education, and succeed in their first year of postsecondary education.",
                RecommendedMetrics = "Percentage of students in grade 9 with a GPA of 3.0 or higher, no Ds or Fs in English language arts or math, attendance of 96 percent or higher, and no in- or out-of-school suspensions or expulsions.",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Course outcome",
                    "Grade point average (K-12)",
                    "Suspensions and expulsions (K-12)",
                    "Student attendance rate (K-12)",
                    "Student grade level (K-12)",
                ]
            },
        ["Access to affordable housing"] =
            new()
            {
                Name = "Access to affordable housing",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "There is sufficient availability of affordable housing for the number of families with low incomes in an area (city or county).",
                RecommendedMetrics = "- Ratio of (1) the number of affordable housing units to (2) the number of households with low and very low incomes in an area (city or county). Housing units are defined as affordable if the monthly costs do not exceed 30 percent of a household's income. Households with low incomes are defined as those earning below 80 percent of area median income (AMI), and very low-income households are defined as those earning below 50 percent of AMI - Percentage of eligible households receiving federal rental assistance",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Eligibility for federal rental assistance",
                    "Number of affordable housing units in city or county",
                    "Number of households with low income in city or county",
                    "Number of households with very low income in city or county",
                    "Receipt of federal rental assistance",
                ]
            },
        ["Access to child care subsidies"] =
            new()
            {
                Name = "Access to child care subsidies",
                Cluster = [IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Eligible families have access to child care by using subsidies to pay for care.",
                RecommendedMetrics = "Percentage of eligible families receiving assistance to pay for child care through subsidies",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Family eligibility for child care subsidies",
                    "Receipt of child care subsidies",
                ]
            },
        ["Access to college and career advising"] =
            new()
            {
                Name = "Access to college and career advising",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.PostsecondaryTransitions, IndicatorCluster.WorkforceTransitions],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "College and career counseling services are available in high schools and college campuses.",
                RecommendedMetrics = "Ratio of number of students to number of FTE counselors; Percentage of students using academic advising and career counseling services.",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Student utilization of academic advising services (Postsecondary)",
                    "Staff position type",
                    "Staff FTE status",
                    "Student grade level (K-12)",
                ]
            },
        ["Access to college preparatory coursework"] =
            new()
            {
                Name = "Access to college preparatory coursework",
                Cluster = [IndicatorCluster.PostsecondaryTransitions],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students have access to the full set of courses needed to meet the requirements for admission at a majority of colleges.",
                RecommendedMetrics = "• Percentage of high schools offering each of the following sets of college preparatory courses: - Four years of English - Four years of math (including at least four of the following: pre-algebra, algebra, geometry, Algebra II or trigonometry, precalculus, calculus, statistics, quantitative reasoning, and data science) - Three years of laboratory science (including biology, chemistry, physics) - Two years of social science - Two years of foreign language - One year of visual or performing arts  • Percentage of middle schools offering Algebra I",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Course subject area",
                    "Course offering by grade level",
                ]
            },
        ["Access to early college coursework"] =
            new()
            {
                Name = "Access to early college coursework",
                Cluster = [IndicatorCluster.PostsecondaryTransitions],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students have access to AP, IB, and dual enrollment courses.",
                RecommendedMetrics = "• Number of AP, IB, and dual enrollment courses offered, overall and by subject • Percentage of students in an early college course who take the relevant end-of-course test needed to earn credit (for example, AP or IB test), overall and by subject",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "AP, IB, or Dual Credit course designation",
                    "Course subject area",
                    "Student course enrollment record (K-12)",
                    "End-of-course exam participation",
                ]
            },
        ["Access to early intervention screening"] =
            new()
            {
                Name = "Access to early intervention screening",
                Cluster = [IndicatorCluster.KindergartenReadiness],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Children receive early intervention screening for any developmental, sensory, and behavioral concerns to determine whether services are needed.",
                RecommendedMetrics = "Percentage of children with identified concerns who are connected to services; Percentage of children needing selected special education services in kindergarten who were not identified and connected to services before kindergarten",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                SectorReadiness = [new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Evolving }],
                DataElementNames =
                [
                    "Date of early intervention services provided",
                    "Early intervention screening results",
                    "Early intervention screening services referral status",
                    "Kindergarten enrollment date",
                ]
            },
        ["Access to full-day kindergarten"] =
            new()
            {
                Name = "Access to full-day kindergarten",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Children have access to full-day kindergarten programs taught by the same certificated staff member in a day.",
                RecommendedMetrics = "Percentage of schools and districts offering kindergarten programs that are six hours per day for five days per week",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Kindergarten program schedule",
                ]
            },
        ["Access to full-day pre-K"] =
            new()
            {
                Name = "Access to full-day pre-K",
                Cluster = [IndicatorCluster.KindergartenReadiness],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Children have access to full-day, publicly funded pre-K programs.",
                RecommendedMetrics = "Percentage of public pre-K programs that are six hours per day for five days per week",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Pre-K program schedule",
                    "Pre-K program funding source",
                ]
            },
        ["Access to health, mental health, and social supports"] =
            new()
            {
                Name = "Access to health, mental health, and social supports",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.SchoolClimate, IndicatorCluster.SocialEmotionalLearning, IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Individuals have access to health, mental health, and social services provided by educational institutions and employers.",
                RecommendedMetrics = "Percentage of programs offering health, mental health, and social services, or staff or consultants providing infant and early childhood mental health consultation (IECMHC) services; Ratio of number of students to number of health, mental health, and social services FTE staff (for example, school nurses, psychologists, and social workers); Percentage of employers offering an employee assistance program or mental health access through health care plans or other services, as measured by employer surveys.",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "EAP or mental health services provided",
                    "Health services offered",
                    "IECMHC services offered",
                    "Staff position type",
                    "Mental health services offered",
                    "Social services offered",
                    "Staff FTE status",
                ]
            },
        ["Access to in-demand CTE pathways"] =
            new()
            {
                Name = "Access to in-demand CTE pathways",
                Cluster = [IndicatorCluster.PostsecondaryTransitions, IndicatorCluster.WorkforceTransitions],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "CTE pathway offerings are aligned to in-demand occupations, as defined by regional labor market data.",
                RecommendedMetrics = "Number and percentage of CTE program offerings considered \"in demand.\"",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "CTE program",
                    "Occupational demand by region",
                    "Program CIP code",
                ]
            },
        ["Access to jobs paying a living wage"] =
            new()
            {
                Name = "Access to jobs paying a living wage",
                Cluster = [IndicatorCluster.WorkforceSuccess, IndicatorCluster.WorkforceTransitions],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Jobs that pay enough to meet basic family needs are available in a community.",
                RecommendedMetrics = "Percentage of jobs in a county or metropolitan statistical area (MSA) for which the ratio of average pay to the location-adjusted cost of living is greater than one",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving }],
                DataElementNames =
                [
                    "Average pay in county or MSA",
                    "Location-adjusted cost of living in county or MSA",
                ]
            },
        ["Access to ongoing career skills development"] =
            new()
            {
                Name = "Access to ongoing career skills development",
                Cluster = [IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Workers are employed in jobs that provide on-the-job training or a professional learning and development path.",
                RecommendedMetrics = "Percentage of employees who have access to on-the-job training or a professional learning and development plan directly from their employer",
                DataNeeded = [DataCategory.Surveys],
                SectorReadiness = [new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving }],
                DataElementNames =
                [
                    "Employee learning and development plan offered",
                    "On-the-job training offered",
                ]
            },
        ["Access to quality public pre-K"] =
            new()
            {
                Name = "Access to quality public pre-K",
                Cluster = [IndicatorCluster.KindergartenReadiness],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Children have access to a high-quality public pre-K program.",
                RecommendedMetrics = "Percentage of public pre-K programs that meet Quality Rating and Improvement Systems (QRIS) state benchmarks of quality",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.ClassroomObservations],
                SectorReadiness = [new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Pre-K program QRIS rating",
                    "Pre-K program funding source",
                ]
            },
        ["Access to quality, culturally responsive curricula"] =
            new()
            {
                Name = "Access to quality, culturally responsive curricula",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Schools and instructors use a standards-aligned core course curriculum that meets quality standards (as defined by EdReports) and is culturally relevant, centering the lived experiences and heritage of students' ethnic or racial backgrounds.",
                RecommendedMetrics = "Percentage of teachers regularly using standards-aligned, culturally responsive curricula",
                DataNeeded = [DataCategory.CurriculumMaterials],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Emerging },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Emerging },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Emerging },
                ],
                DataElementNames =
                [
                    "Curriculum quality rating",
                    "Curriculum adoption",
                    "Culturally responsive curriculum assessment",
                ]
            },
        ["Access to technology"] =
            new()
            {
                Name = "Access to technology",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "Individuals have access to a reliable Internet connection and a personal desktop or laptop computer.",
                RecommendedMetrics = "Percentage of individuals who have both (1) access to at least one desktop or laptop computer owned by someone in the home and (2) reliable broadband Internet",
                DataNeeded = [DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Indicator of access to desktop or laptop at home",
                    "Indicator of access to reliable broadband internet",
                ]
            },
        ["Access to transportation"] =
            new()
            {
                Name = "Access to transportation",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "Individuals have access to low-cost and timely transportation to commute to school or work.",
                RecommendedMetrics = "- Average commute time to work, school, or college - The Low Transportation Cost Index, from the U.S. Department of Housing and Urban Development",
                DataNeeded = [DataCategory.Surveys, DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Low Transportation Cost Index",
                    "Commute time",
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
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Number of Adverse Childhood Experiences (ACEs)",
                ]
            },
        ["Civic engagement"] =
            new()
            {
                Name = "Civic engagement",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.SchoolClimate, IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Individuals exhibit the knowledge, skills, values, motivation, and activities that promote quality of life within a community and society at large through political and nonpolitical processes.",
                RecommendedMetrics = "Percentage of students reporting a high level of civic engagement on surveys such as the Youth Civic and Character Measures Toolkit Survey and Youth Civic Engagement Indicators Project Survey, Percentage of individuals reporting a high level of civic engagement on surveys such as the Index of Civic and Political Engagement",
                DataNeeded = [DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Emerging },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Emerging },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Emerging },
                ],
                DataElementNames =
                [
                    "Civic engagement surveys (K-12)",
                    "Civic engagement surveys (Postsecondary)",
                    "Civic engagement surveys (Workforce)",
                ]
            },
        ["Classroom observations of instructional practice"] =
            new()
            {
                Name = "Classroom observations of instructional practice",
                Cluster = [IndicatorCluster.TeachingEffectiveness],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Teachers demonstrate high-quality instructional practices and interactions with students.",
                RecommendedMetrics = "• Pre-K: Scores on measures of teacher-child interactions, such as CLASS, the Early Childhood Environment Rating Scale (ECERS) Interactions subscale, or the Assessing Classroom Sociocultural Equity Scale (ACSES) (which assesses equitable classroom interactions) • K-12: Teachers' overall and subscale scores on an observation rubric associated with an educator observation system; examples of common frameworks include the Danielson's Framework for Teaching and the Marzano Causal Teacher Evaluation Model • Postsecondary: There are currently no widely used standardized rubrics for peer observations of college teaching, though multiple researchers and universities have produced guidance surrounding the peer observation process",
                DataNeeded = [DataCategory.ClassroomObservations],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Emerging },
                ],
                DataElementNames =
                [
                    "Teacher-child interaction measure (PK)",
                    "Teacher observation scores (K-12)",
                    "Teacher observation subscale scores (K-12)",
                    "Teacher observation scores (Postsecondary)",
                ]
            },
        ["College applications"] =
            new()
            {
                Name = "College applications",
                Cluster = [IndicatorCluster.PostsecondaryTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Grade 12 students submit a well-balanced portfolio of at least three college applications.",
                RecommendedMetrics = "Percentage of grade 12 students who submitted at least three college applications",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving }],
                DataElementNames =
                [
                    "Postsecondary applications submitted",
                    "Student grade level (K-12)",
                ]
            },
        ["College preparatory coursework completion"] =
            new()
            {
                Name = "College preparatory coursework completion",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "High school students meet typical coursework requirements for admission in a four-year college.",
                RecommendedMetrics = "Percentage of high school graduates who successfully complete the coursework required for admission at a four-year college or university, which includes: - Four years of English classes - Four years of math classes (including at least four of the following: pre-algebra, algebra, geometry, Algebra II or trigonometry, precalculus, calculus, statistics, quantitative reasoning, and data science) - Three years of laboratory science (including biology, chemistry, and physics) - Two years of social sciences - Two years of foreign language - One year of visual or performing arts",
                DataNeeded = [DataCategory.StudentTranscripts],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Course subject area",
                    "Course outcome",
                    "High school graduation indicator",
                ]
            },
        ["Communication skills"] =
            new()
            {
                Name = "Communication skills",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.PostsecondaryTransitions, IndicatorCluster.SocialEmotionalLearning, IndicatorCluster.WorkforceSuccess, IndicatorCluster.WorkforceTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Individuals have the oral, written, nonverbal, and listening skills required for success in school and at work.",
                RecommendedMetrics = "Percentage of students demonstrating proficiency on assessments such as the College and Career Readiness Assessment (CCRA+), an assessment for grades 6-12 that measures critical thinking, problem solving, and written communications; Percentage of students demonstrating proficiency on assessments such as the Collegiate Learning Assessment (CLA+) or Success Skills Assessment (SSA+) for postsecondary students that measure critical thinking, problem solving, and written communications, or the HEIghten Outcomes Assessment for Written Communication; Percentage of individuals demonstrating proficiency on a performance assessment, such as the National Work Readiness Credential Essential Soft Skills assessment",
                DataNeeded = [DataCategory.Assessments],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Emerging },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Emerging },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Emerging },
                ],
                DataElementNames =
                [
                    "Communication skills performance assessments (K-12)",
                    "Communication skills performance assessments (Postsecondary)",
                    "Communication skills performance assessments (Workforce)",
                ]
            },
        ["Consistent attendance"] =
            new()
            {
                Name = "Consistent attendance",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.SchoolClimate],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students are present for more than 90 percent of enrolled days.",
                RecommendedMetrics = "Percentage of students who are present for more than 90 percent of their enrolled days, excluding students enrolled for fewer than 90 days",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Student attendance rate (K-12)",
                    "Student attendance rate (PK)",
                    "Student attendance rate (Postsecondary)",
                ]
            },
        ["CTE pathway concentration"] =
            new()
            {
                Name = "CTE pathway concentration",
                Cluster = [IndicatorCluster.PostsecondaryTransitions, IndicatorCluster.WorkforceTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Students participating in CTE concentrate in a single chosen pathway or program of study.",
                RecommendedMetrics = "Percentage of 12th-grade students enrolled in CTE who complete three or more CTE courses in a single pathway; Percentage of CTE students who earn at least 12 credits within a CTE program or complete such a program if it encompasses fewer than 12 credits in total",
                DataNeeded = [DataCategory.StudentTranscripts],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Course subject area",
                    "CTE pathway or career cluster associated with CTE course",
                    "Student course enrollment record (K-12)",
                    "Student grade level (K-12)",
                    "Number of credits earned",
                    "CTE program",
                ]
            },
        ["Cultural competency"] =
            new()
            {
                Name = "Cultural competency",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.SocialEmotionalLearning, IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Individuals are able to understand the perspectives of and empathize with others from diverse backgrounds and cultures.",
                RecommendedMetrics = "Percentage of students demonstrating proficiency on an assessment of cultural competency, such as the HEIghten Outcomes Assessment for Intercultural Competency & Diversity or The Intercultural Development Inventory®, Percentage of individuals demonstrating proficiency on an assessment of cultural competency, such as The Intercultural Development Inventory®",
                DataNeeded = [DataCategory.Surveys, DataCategory.Assessments],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Emerging },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Emerging },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Emerging },
                ],
                DataElementNames =
                [
                    "Cultural competency assessments (K-12)",
                    "Cultural competency assessments (Postsecondary)",
                    "Cultural competency assessments (Workforce)",
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
                SectorReadiness =
                [
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Student loan debt balance",
                ]
            },
        ["Digital skills"] =
            new()
            {
                Name = "Digital skills",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.PostsecondaryTransitions, IndicatorCluster.WorkforceSuccess, IndicatorCluster.WorkforceTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Students and workers can use digital technology tools effectively to access, manage, evaluate, and communicate information.",
                RecommendedMetrics = "Percentage of individuals demonstrating proficiency on a performance assessment that measures digital skills required for workforce success, such as the Problem Solving in Technology-Rich Environments assessment within the Education & Skills Online assessment suite, which can be used by researchers and institutions to gather individual-level results based on Organisation for Economic Co-operation and Development (OECD) Survey of Adult Skills (Programme for the International Assessment of Adult Competencies [PIAAC]) domains",
                DataNeeded = [DataCategory.Assessments],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Emerging },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Emerging },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Emerging },
                ],
                DataElementNames =
                [
                    "Digital skills assessments (K-12)",
                    "Digital skills assessments (Postsecondary)",
                    "Digital skills assessments (Workforce)",
                ]
            },
        ["Early college coursework completion"] =
            new()
            {
                Name = "Early college coursework completion",
                Cluster = [IndicatorCluster.PostsecondaryTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "High school students successfully complete early college coursework (Advanced Placement [AP], International Baccalaureate [IB], or dual credit).",
                RecommendedMetrics = "Percentage of high school students who enroll in and pass at least one early college course (AP, IB, or dual credit) Percentage of students enrolled in early college coursework who earn credit-bearing scores on end-of-course tests (for example, a score of 3 or higher on AP tests or 5 or higher on IB tests) or earn postsecondary credit within their dual enrollment courses",
                DataNeeded = [DataCategory.StudentTranscripts, DataCategory.Assessments],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Student course enrollment record (K-12)",
                    "AP, IB, or Dual Credit course designation",
                    "Course outcome",
                    "End-of-course exam participation",
                ]
            },
        ["Early grades on track"] =
            new()
            {
                Name = "Early grades on track",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students in grades 1 and 2 are on track to achieve academic proficiency in grade 3.",
                RecommendedMetrics = "Percentage of students in grades 1 and 2 meeting grade-level math and reading benchmarks, with an attendance rate of 90 percent or higher, and no in- or out-of-school suspensions or expulsions",
                DataNeeded = [DataCategory.Assessments, DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Emerging }],
                DataElementNames =
                [
                    "Suspensions and expulsions (K-12)",
                    "Student grade level (K-12)",
                    "Student attendance rate (K-12)",
                    "Math proficiency (Grades 1 and 2)",
                    "Reading proficiency (Grades 1 and 2)",
                ]
            },
        ["Economic mobility"] =
            new()
            {
                Name = "Economic mobility",
                Cluster = [IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Individuals reach the level of earnings needed to enter the fourth income quintile or above, regardless of field of study.",
                RecommendedMetrics = "Percentage of individuals who reach the level of earnings needed to enter the fourth (60th to 80th percentile) income quintile in their state or above 1, 3, 5, 10, and 15 years after completing their highest degree or leaving education (high school or postsecondary)",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving }],
                DataElementNames =
                [
                    "Earnings",
                    "High school graduation date",
                    "Postsecondary credential award date",
                ]
            },
        ["Economic security"] =
            new()
            {
                Name = "Economic security",
                Cluster = [IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Individuals reach median levels of wealth (net worth).",
                RecommendedMetrics = "Percentage of individuals who reach median levels of wealth 10, 15, 20, and 30 years after completing their highest degree or leaving education (high school or postsecondary)",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                SectorReadiness = [new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving }],
                DataElementNames =
                [
                    "High school graduation date",
                    "Net worth",
                    "Postsecondary credential award date",
                ]
            },
        ["Educator retention"] =
            new()
            {
                Name = "Educator retention",
                Cluster = [IndicatorCluster.SchoolClimate],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Teachers and school leaders return to the same school in consecutive years.",
                RecommendedMetrics = "• Teacher retention: Percentage of teachers who return to teaching in the same school from year to year • School leader tenure: Percentage of school leaders who have served in their current positions for < 2 years, 2-3 years, and 4+ years",
                DataNeeded = [DataCategory.EducatorAdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Educator tenure in current position",
                ]
            },
        ["Effective program and school leadership"] =
            new()
            {
                Name = "Effective program and school leadership",
                Cluster = [IndicatorCluster.SchoolClimate],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Schools are led by effective principals and school leaders.",
                RecommendedMetrics = "• Percentage of school leaders rated as effective, using an evaluation system that includes multiple measures, such as the Administrator Evaluation component of the Tennessee Educator Acceleration Model (TEAM)",
                DataNeeded = [DataCategory.Assessments, DataCategory.Surveys, DataCategory.ClassroomObservations, DataCategory.Rubrics],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "School leader evaluation rating",
                ]
            },
        ["Employment in a quality job"] =
            new()
            {
                Name = "Employment in a quality job",
                Cluster = [IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Individuals are employed in a position that offers a living wage, benefits, stable and predictable schedules, clear and fair advancement to higher pay, safe conditions, and job security.",
                RecommendedMetrics = "Percentage of individuals employed in a quality job, as defined by scores on an indexed measure, such as the Good Jobs Scorecard, which assesses pay and benefits, scheduling, potential career paths, safety, and security",
                DataNeeded = [DataCategory.Surveys],
                SectorReadiness = [new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Emerging }],
                DataElementNames =
                [
                    "Job quality index",
                    "Employment status",
                ]
            },
        ["English learner progress"] =
            new()
            {
                Name = "English learner progress",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Emerging multilingual students achieve English proficiency within five years of being classified as English learners.",
                RecommendedMetrics = "Percentage of English learner students who are reclassified in five years or less, based on local reclassification criteria",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "English learner classification date",
                    "English learner status",
                ]
            },
        ["Enrollment in graduate education"] =
            new()
            {
                Name = "Enrollment in graduate education",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students enroll in a graduate education program after completing an undergraduate degree.",
                RecommendedMetrics = "Percentage of bachelor's degree recipients enrolling in post-baccalaureate or graduate programs within one to five years of completion. Other time frames, such as within 10 years of completion, should also be reported for this measure.",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Postsecondary enrollment date",
                    "Postsecondary credential award date",
                ]
            },
        ["Enrollment in public pre-K"] =
            new()
            {
                Name = "Enrollment in public pre-K",
                Cluster = [IndicatorCluster.KindergartenReadiness],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Eligible children are enrolled in a publicly funded pre-K program, which can be administered through mixed delivery systems that include Head Start, pre-K classrooms in public schools, and licensed family-based child care programs and community-based organizations.",
                RecommendedMetrics = "Percentage of eligible 3- and 4-year-olds enrolled in public pre-K",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Age",
                    "Pre-K enrollment date",
                    "Pre-K eligibility status",
                    "Pre-K program funding source",
                ]
            },
        ["Equitable discipline practices"] =
            new()
            {
                Name = "Equitable discipline practices",
                Cluster = [IndicatorCluster.SchoolClimate],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Schools treat students similarly and appropriately for disciplinary infractions.",
                RecommendedMetrics = "Differences in the rates at which students from key demographic subgroups ever experience different forms of school discipline (office referrals, suspensions, expulsions, restraint, and exclusion) relative to those students' representation in their school population as a whole; Disproportionalities along the lines of key demographic characteristics in the level of school discipline experienced (for example, number of days suspended).",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Number of days suspended (PK)",
                    "Office referrals (PK)",
                    "Number of days suspended (K-12)",
                    "Office referrals (K-12)",
                    "Suspensions and expulsions (K-12)",
                    "Restraint and seclusion for discipline (PK)",
                    "Restraint and seclusion for discipline (K-12)",
                    "Suspensions and expulsions (PK)",
                ]
            },
        ["Equitable placement in rigorous coursework"] =
            new()
            {
                Name = "Equitable placement in rigorous coursework",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students from various demographic subgroups are proportionally represented in rigorous courses and programs.",
                RecommendedMetrics = "Differences in the participation rates for students from key demographic subgroups in rigorous courses and programs relative to those students' representation in their school population as a whole, including opportunities, such as the following: • Gifted and talented programs • Algebra I in middle school • Higher-level math courses in high school (that is, Algebra II, calculus) • Early college courses (AP, IB, and dual enrollment)",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Gifted and talented participation",
                    "Student course enrollment record (K-12)",
                    "Course subject area",
                    "AP, IB, or Dual Credit course designation",
                ]
            },
        ["Expenditures on workforce development programs"] =
            new()
            {
                Name = "Expenditures on workforce development programs",
                Cluster = [IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "The amount of government funding dedicated to workforce development programs, including apprenticeships and job training programs, in a state.",
                RecommendedMetrics = "The amount of funding dedicated to workforce development programs as a percentage of total educational funding in a state",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.WF, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Funding dedicated to workforce development programs",
                    "Total state educational funding",
                ]
            },
        ["Expenditures per student"] =
            new()
            {
                Name = "Expenditures per student",
                Cluster = [IndicatorCluster.PostsecondarySuccess],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "The amount of education and related expenditures per student.",
                RecommendedMetrics = "- Pre-K: State expenditures per child enrolled - K-12: Per pupil expenditures - K-12: Equity Factor, a measure that indicates variance in per-pupil funding within a state (see this brief by New America for more information) - Postsecondary: Total instruction and student service expenditures per FTE student based on 12-month enrollment",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Expenditures per student (K-12)",
                    "Expenditures per student (PK)",
                    "Expenditures per student (Postsecondary)",
                    "Postsecondary enrollment status (Full time/part time)",
                ]
            },
        ["Exposure to neighborhood crime"] =
            new()
            {
                Name = "Exposure to neighborhood crime",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "The rate of violent and property crimes in a city or county.",
                RecommendedMetrics = "Rate of violent felonies and property felonies by city or county (number of incidents per 100,000 residents)",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "City or county population",
                    "Number of property felonies in city or county",
                    "Number of violent felonies in city or county",
                ]
            },
        ["FAFSA completion"] =
            new()
            {
                Name = "FAFSA completion",
                Cluster = [IndicatorCluster.PostsecondaryTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Grade 12 students eligible for federal financial aid complete the Free Application for Federal Student Aid (FAFSA) by June 30.",
                RecommendedMetrics = "Percentage of grade 12 students who complete the FAFSA by June 30",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "FAFSA completion date",
                    "Student grade level (K-12)",
                ]
            },
        ["First-year credit accumulation"] =
            new()
            {
                Name = "First-year credit accumulation",
                Cluster = [IndicatorCluster.PostsecondarySuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students attempt and complete sufficient credits during their first undergraduate year to be on track for on-time degree completion.",
                RecommendedMetrics = "Percentage of students attempting and completing sufficient credits toward on-time completion in their first year: 30 credits for full-time and 15 credits for part-time students",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts],
                SectorReadiness = [new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Number of credits attempted",
                    "Number of credits earned",
                    "Postsecondary enrollment date",
                    "Postsecondary enrollment status (Full time/part time)",
                ]
            },
        ["First-year program of study concentration"] =
            new()
            {
                Name = "First-year program of study concentration",
                Cluster = [IndicatorCluster.PostsecondarySuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Postsecondary students demonstrate selection of a program of study by completing nine credits or three courses in a meta-major during their first year.",
                RecommendedMetrics = "Percentage of students completing at least nine credits (or three courses) within a meta-major during their first year in postsecondary education",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts],
                SectorReadiness = [new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Course subject area",
                    "Number of credits earned",
                    "Postsecondary enrollment date",
                    "Meta-major classifications",
                ]
            },
        ["Food security"] =
            new()
            {
                Name = "Food security",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "Individuals have access to enough affordable, nutritious food.",
                RecommendedMetrics = "- Percentage of individuals with high or marginal food security, as measured by the U.S. Department of Agriculture's (USDA) Food Security Survey Module - Percentage of eligible individuals participating in SNAP - Percentage of individuals living in a census tract with low access to healthy food, as defined by the USDA's Food Access Research Atlas",
                DataNeeded = [DataCategory.Surveys, DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "SNAP eligibility",
                    "SNAP participation",
                    "Food access level by census tract",
                    "Food security survey rating",
                ]
            },
        ["Gateway course completion"] =
            new()
            {
                Name = "Gateway course completion",
                Cluster = [IndicatorCluster.PostsecondarySuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Completion of college-level introductory math and English courses, as defined by each postsecondary institution, during the first year of college.",
                RecommendedMetrics = "Percentage of first-year college students who complete college-level introductory math and English courses within their first year of college",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts],
                SectorReadiness = [new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Course subject area",
                    "Postsecondary enrollment date",
                    "Course outcome",
                ]
            },
        ["Grade point average"] =
            new()
            {
                Name = "Grade point average",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.PostsecondaryTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Middle school students earn course grades that demonstrate high school readiness; high school students earn course grades necessary to gain admission to college; and college students earn grades high enough to graduate and obtain jobs.",
                RecommendedMetrics = "Percentage of students in grades 6-8 with a GPA of 3.0 or higher Percentage of students in grades 9-12 with a GPA of 3.0 or higher Percent of college students with a GPA of 3.0 or higher",
                DataNeeded = [DataCategory.StudentTranscripts],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Grade point average (K-12)",
                    "Grade point average (Postsecondary)",
                    "Student grade level (K-12)",
                ]
            },
        ["Graduate degree completion"] =
            new()
            {
                Name = "Graduate degree completion",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students complete a graduate degree (master's degree or higher) within a specified time frame after entering graduate school.",
                RecommendedMetrics = "Percentage of graduate students completing a graduate degree within 150 percent of their current program's length. Other time frames, such as 100 percent and 200 percent of program length, should also be reported for this measure.",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Postsecondary enrollment date",
                    "Postsecondary credential type",
                    "Postsecondary degree program length",
                    "Postsecondary credential award date",
                ]
            },
        ["Growth mindset"] =
            new()
            {
                Name = "Growth mindset",
                Cluster = [IndicatorCluster.SocialEmotionalLearning],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Students believe that their abilities can grow with effort.",
                RecommendedMetrics = "Percentage of students reporting a high level of growth mindset on surveys such as the CORE Districts SEL Survey Growth Mindset Scale (grades 5-12) or the Growth Mindset Scale developed by Carol Dweck, Percentage of students reporting a high level of growth mindset on surveys such as the Growth Mindset Scale developed by Carol Dweck",
                DataNeeded = [DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Growth mindset surveys (K-12)",
                    "Growth mindset surveys (Postsecondary)",
                    "Growth mindset surveys (Workforce)",
                ]
            },
        ["Health insurance coverage"] =
            new()
            {
                Name = "Health insurance coverage",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "Individuals have health insurance coverage for preventative and emergency care.",
                RecommendedMetrics = "- Percentage of individuals with health insurance - Percentage of eligible individuals (children or adults) enrolled in Medicaid or CHIP",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "CHIP eligibility status",
                    "CHIP enrollment",
                    "Health insurance coverage status",
                    "Medicaid eligibility status",
                    "Medicaid enrollment",
                ]
            },
        ["High school graduation"] =
            new()
            {
                Name = "High school graduation",
                Cluster = [IndicatorCluster.PostsecondaryTransitions, IndicatorCluster.WorkforceTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students graduate from high school with a regular diploma within four, five, and six years of entering high school.",
                RecommendedMetrics = "Adjusted cohort graduation rate (the percentage of first-time 9th graders who graduate with a regular diploma within four, five, and six years of entering high school, regardless of whether they transferred schools)",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Cohort graduation year",
                    "Cohort year",
                    "High school diploma type",
                    "High school graduation date",
                ]
            },
        ["Higher-order thinking skills"] =
            new()
            {
                Name = "Higher-order thinking skills",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.PostsecondaryTransitions, IndicatorCluster.SocialEmotionalLearning, IndicatorCluster.WorkforceSuccess, IndicatorCluster.WorkforceTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Individuals have the problem solving, critical thinking, and decision-making skills needed in the workplace.",
                RecommendedMetrics = "Percentage of students demonstrating proficiency on assessments such as the College and Career Readiness Assessment (CLA+), an assessment for grades 6-12 that measures critical thinking, problem solving, and written communications; Percentage of students demonstrating proficiency on assessments such as the CLA+ or Success Skills Assessment (SSA+), assessments for postsecondary students that measure critical thinking, problem solving, and written communications, or the HEIghten Outcomes Assessment for Critical Thinking; Percentage of individuals demonstrating proficiency on assessments such as the Watson Glaser Critical Thinking Appraisal, a scenario-based assessment used by employers to evaluate candidates or identify areas of opportunity for growth",
                DataNeeded = [DataCategory.Assessments],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Emerging },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Emerging },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Emerging },
                ],
                DataElementNames =
                [
                    "Higher-order thinking skills performance assessments (K-12)",
                    "Higher-order thinking skills performance assessments (Postsecondary)",
                    "Higher-order thinking skills performance assessments (Workforce)",
                ]
            },
        ["Inclusive environments"] =
            new()
            {
                Name = "Inclusive environments",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.SchoolClimate, IndicatorCluster.SocialEmotionalLearning, IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Individuals feel they belong and feel connected to their peers in their schools, postsecondary institutions, and workplaces.",
                RecommendedMetrics = "Percentage of children reporting positive feelings toward their school, as measured by questionnaires such as the Collaborative for Academic, Social, and Emotional Learning's (CASEL) How I Feel About My School questionnaire, or percentage of classrooms demonstrating equitable sociocultural interactions, as measured by observational assessments, such as ACSES. Percentage of students reporting belonging in school, as measured by surveys such as the Sense of Belonging subscale of the CORE Districts school culture and climate survey or the Classroom Belonging subscale of the Panorama Student Survey. Percentage of students reporting belonging on campus, as measured by surveys such as the Higher Education Research Institute (HERI) Diverse Learning Environments Survey or the National Institute for Transformation and Equity (NITE) Culturally Engaging Campus Environments Survey. Percentage of employees reporting belonging at work, as measured by surveys such as the Association of American Medical Colleges (AAMC) Diversity Engagement Survey.",
                DataNeeded = [DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Emerging },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Sense of belonging surveys (K-12)",
                    "Sense of belonging surveys (PK)",
                    "Sense of belonging surveys (Postsecondary)",
                    "Sociocultural observational assessments (PK)",
                    "Sense of belonging surveys (Workforce)",
                ]
            },
        ["Industry-recognized credential"] =
            new()
            {
                Name = "Industry-recognized credential",
                Cluster = [IndicatorCluster.WorkforceTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Individuals complete at least one industry-recognized credential, as defined by each state.",
                RecommendedMetrics = "Percentage of 12th-grade students enrolled in CTE who earn at least one industry-recognized credential; Percentage of students enrolled in a CTE program who earn at least one industry-recognized credential; Percentage of program participants who have completed at least one industry-recognized credential",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Industry-recognized credential indicator",
                    "Student course enrollment record (K-12)",
                    "Workforce development program participation",
                    "Student grade level (K-12)",
                    "CTE program",
                    "Postsecondary credential type",
                ]
            },
        ["Institutions' contributions to student outcomes"] =
            new()
            {
                Name = "Institutions' contributions to student outcomes",
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Schools and colleges contribute to students' short- and long-term outcomes.",
                RecommendedMetrics = "• K-12: Schools' contributions to student outcomes, including achievement, attendance, social-emotional learning, college enrollment, and earnings, using value-added models • Postsecondary: Colleges' contributions to student outcomes, including graduation rates, earnings, and student loan repayment, using value-added models",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Assessments, DataCategory.StudentTranscripts, DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Emerging },
                ],
                DataElementNames =
                [
                    "College value-added",
                    "School value-added",
                ]
            },
        ["Kindergarten readiness: approaches to learning"] =
            new()
            {
                Name = "Kindergarten readiness: approaches to learning",
                Cluster = [IndicatorCluster.KindergartenReadiness],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Children develop and demonstrate emotional and behavioral self-regulation, cognitive self-regulation (executive functioning), initiative and curiosity, and creativity.",
                RecommendedMetrics = "Percentage of students meeting benchmarks on teacher-reported kindergarten readiness assessment, such as the DRDP Approaches to Learning - Self-Regulation domain and TS GOLD Cognitive subscale, or percentage of students meeting benchmarks on teacher reports of children's executive function, such as the CBRS, or percentage of students meeting benchmarks on a direct child assessment, such as the Heads Toes Knees Shoulders (HTKS) task administered by teachers and the Minnesota Executive Function Scale (MEFS), self-administered on a tablet",
                DataNeeded = [DataCategory.Assessments],
                SectorReadiness = [new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Direct child assessments (executive function)",
                    "Teacher-reported kindergarten readiness (behavioral skills)",
                    "Teacher reports of executive function",
                ]
            },
        ["Kindergarten readiness: cognition"] =
            new()
            {
                Name = "Kindergarten readiness: cognition",
                Cluster = [IndicatorCluster.KindergartenReadiness],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Children develop and demonstrate foundational math and scientific reasoning skills.",
                RecommendedMetrics = "Percentage of children meeting benchmarks on teacher-reported kindergarten readiness assessment, such as: - DRDP Cognition domain - R4K ELA Mathematics and Science domains - TS GOLD Cognitive and Mathematics subscales Or, percentage of children meeting benchmarks on direct child assessments, such as: - Woodcock-Johnson IV Tests of ECAD Number Sense subtest - IGDIs Early Numeracy assessment - Research Based Early Mathematics Assessment (REMA)",
                DataNeeded = [DataCategory.Assessments],
                SectorReadiness = [new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Direct child assessments (cognition)",
                    "Teacher-reported kindergarten readiness (cognition)",
                ]
            },
        ["Kindergarten readiness: language and literacy"] =
            new()
            {
                Name = "Kindergarten readiness: language and literacy",
                Cluster = [IndicatorCluster.KindergartenReadiness],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Children develop and demonstrate foundational language and literacy skills.",
                RecommendedMetrics = "Percentage of children meeting benchmarks on a teacher-reported kindergarten readiness assessment, such as: - Desired Results Developmental Profile (DRDP) Language and Literacy Development domain - Ready 4 Kindergarten Early Learning Assessment (R4K ELA) Language and Literacy domain - Teaching Strategies GOLD (TS GOLD) Language and Literacy subscales Or, percentage of children meeting benchmarks on direct child assessments administered by trained assessors, such as: - Woodcock-Johnson IV Tests of Early Cognition and Academic Development (ECAD) Letter-Word and Writing subtests - Individual Growth and Development Indicators (IGDIs) Early Literacy assessment",
                DataNeeded = [DataCategory.Assessments],
                SectorReadiness = [new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Direct child assessments (language and literacy)",
                    "Teacher-reported kindergarten readiness (language and literacy)",
                ]
            },
        ["Kindergarten readiness: perceptual, motor, and physical development"] =
            new()
            {
                Name = "Kindergarten readiness: perceptual, motor, and physical development",
                Cluster = [IndicatorCluster.KindergartenReadiness],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Children develop and demonstrate gross and fine motor skills, and an understanding of health, safety, and nutrition.",
                RecommendedMetrics = "Percentage of children meeting benchmarks on teacher-reported kindergarten readiness assessment, such as the DRDP Physical Development - Health domain, R4K ELA Physical Well-Being and Motor Development domain, TS GOLD Physical subscale, or percentage of students meeting benchmarks on direct child assessment administered by teachers, healthcare professionals, or other qualified adults, such as the Peabody Developmental Motor Scale",
                DataNeeded = [DataCategory.Assessments],
                SectorReadiness = [new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Direct child assessments (physical development)",
                    "Teacher-reported kindergarten readiness (physical development)",
                ]
            },
        ["Kindergarten readiness: social-emotional development"] =
            new()
            {
                Name = "Kindergarten readiness: social-emotional development",
                Cluster = [IndicatorCluster.KindergartenReadiness, IndicatorCluster.SocialEmotionalLearning],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Children develop and demonstrate the skills to form positive relationships with adults and peers, emotional functioning, and a sense of identity and belonging.",
                RecommendedMetrics = "Percentage of students meeting benchmarks on teacher-reported kindergarten readiness assessment, such as the DRDP Social and Emotional Development domain, R4K ELA Social Foundations domain, TS GOLD Social-Emotional subscale, or percentage of students meeting benchmarks on teacher reports, such as the Child Behavior Rating Scale (CBRS) and Devereaux Early Childhood Assessment Preschool Program (DECA-P2)",
                DataNeeded = [DataCategory.Assessments],
                SectorReadiness = [new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Teacher-reported kindergarten readiness (social-emotional skills)",
                    "Teacher reports of social-emotional development",
                ]
            },
        ["Math and reading proficiency in grade 3"] =
            new()
            {
                Name = "Math and reading proficiency in grade 3",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students demonstrate proficiency in math and ELA according to high-quality state standards.",
                RecommendedMetrics = "Percentage of students in grade 3 who meet grade-level standards in reading/English language arts and math as measured by state standardized tests.",
                DataNeeded = [DataCategory.Assessments],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "State standardized test (Math proficiency)",
                    "State standardized test (Reading proficiency)",
                    "Student grade level (K-12)",
                ]
            },
        ["Math and reading proficiency in grade 8"] =
            new()
            {
                Name = "Math and reading proficiency in grade 8",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students demonstrate proficiency in math and reading/English language arts according to high-quality state standards.",
                RecommendedMetrics = "Percentage of students in grade 8 who meet grade-level standards in reading/English language arts and math as measured by state standardized tests",
                DataNeeded = [DataCategory.Assessments],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "State standardized test (Math proficiency)",
                    "State standardized test (Reading proficiency)",
                    "Student grade level (K-12)",
                ]
            },
        ["Math and reading proficiency in high school"] =
            new()
            {
                Name = "Math and reading proficiency in high school",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students demonstrate proficiency in math and reading/English language arts according to high-quality state standards.",
                RecommendedMetrics = "Percentage of tested students who meet grade-level standards in reading/English language arts and math as measured by state standardized tests",
                DataNeeded = [DataCategory.Assessments],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "State standardized test (Math proficiency)",
                    "State standardized test (Reading proficiency)",
                    "Student grade level (K-12)",
                ]
            },
        ["Mental and emotional well-being"] =
            new()
            {
                Name = "Mental and emotional well-being",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.SchoolClimate, IndicatorCluster.SocialEmotionalLearning, IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Individuals possess mental and emotional well-being.",
                RecommendedMetrics = "Percentage of children with identified health or developmental concerns as identified by a developmental screening tool. For a list of screening tools that may be appropriate for children younger than age 5, see the following guide from the Head Start Early Childhood Learning and Knowledge Center: \"Birth to 5: Watch Me Thrive! A Compendium of Screening Measures for Young Children.\"; Percentage of youth with mental or emotional health needs as identified by a universal screening tool. For a list of mental health screening tools that may be appropriate for school-based use, see the following guide from the National Center on Safe Supportive Learning Environments: \"Mental Health Screening Tools for Grades K-12.\"; Psychological well-being scale.",
                DataNeeded = [DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Developmental screening results (PK)",
                    "Mental and emotional well-being assessments",
                    "Universal mental health screening results (K-12)",
                ]
            },
        ["Minimum economic return"] =
            new()
            {
                Name = "Minimum economic return",
                Cluster = [IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Individuals earn enough after completing their education to recover the costs of their investment.",
                RecommendedMetrics = "Percentage of individuals that earn at least as much as the median high school graduate in their state plus enough to recoup their total net price plus interest within 10 years of completing their highest degree or leaving education (high school or postsecondary)",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Earnings",
                    "High school graduation date",
                    "Total net price of education plus interest",
                    "Postsecondary credential award date",
                ]
            },
        ["Neighborhood economic diversity"] =
            new()
            {
                Name = "Neighborhood economic diversity",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "The concentration of poverty within a city or county.",
                RecommendedMetrics = "Percentage of city or county residents experiencing poverty who live in a high-poverty neighborhood (defined as a neighborhood in which more than 40 percent of residents experience poverty)",
                DataNeeded = [DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Number of city or county residents experiencing poverty",
                    "Number of city or county residents living in a high poverty neighborhood",
                ]
            },
        ["Neighborhood juvenile arrests"] =
            new()
            {
                Name = "Neighborhood juvenile arrests",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "The rate of juveniles arrested in a city or county.",
                RecommendedMetrics = "Rate of juvenile arrests by city or county (number of arrests per 100,000 residents)",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "City or county population",
                    "Number of juvenile arrests in city or county",
                ]
            },
        ["Neighborhood racial diversity"] =
            new()
            {
                Name = "Neighborhood racial diversity",
                Type = IndicatorType.AdjacentSystemConditions,
                Domain = IndicatorDomain.CrossDomain,
                Definition = "The share of an individual's neighbors who are people of other races and ethnicities.",
                RecommendedMetrics = "Percentage of an individual's neighbors who are members of other racial or ethnic groups, calculated as a Neighborhood Exposure Index",
                DataNeeded = [DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Census tract or neighborhood identifier",
                    "Race and ethnicity (individual)",
                    "Neighborhood exposure index",
                ]
            },
        ["Participation in work-based learning"] =
            new()
            {
                Name = "Participation in work-based learning",
                Cluster = [IndicatorCluster.WorkforceTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Credential seekers participate in an internship, work study, cooperative education, apprenticeship program, or other work-based learning opportunities.",
                RecommendedMetrics = "K–12 and postsecondary: Percentage of students who participate in a work-based learning opportunity before graduation.  Workforce: Percentage of workforce training program participants who participate in a work-based learning opportunity before program completion",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.StudentTranscripts, DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Participation in work-based learning",
                    "Student grade level (K-12)",
                    "Workforce development program participation",
                    "Postsecondary enrollment status (Full time/part time)",
                ]
            },
        ["Physical development and well-being"] =
            new()
            {
                Name = "Physical development and well-being",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Individuals exhibit positive physical development and health.",
                RecommendedMetrics = "Pre-K: See kindergarten readiness: perceptual, motor, and physical development indicator; K-12: Percentage of students meeting benchmarks on self-rated surveys of physical health, such as the California Healthy Kids Survey Physical Health & Nutrition module; Postsecondary and workforce: Percentage of adults who rate their own health as good, very good, or excellent on the Self-Rated Health scale, or percentage of individuals meeting benchmarks on the Health-Related Quality of Life Scale",
                DataNeeded = [DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Health-Related Quality of Life Scale scores",
                    "Physical health surveys (K-12)",
                    "Teacher-reported kindergarten readiness (physical development)",
                    "Direct child assessments (physical development)",
                ]
            },
        ["Positive behavior"] =
            new()
            {
                Name = "Positive behavior",
                Cluster = [IndicatorCluster.SchoolClimate],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students are not suspended or expelled from school and do not experience other types of exclusionary discipline, such as restraint and seclusion.",
                RecommendedMetrics = "Pre-K, K-12: Percentage of children who do not experience any of the following: in-school suspensions, out-of-school suspensions, disciplinary use of restraint and seclusion, or expulsions",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Restraint and seclusion for discipline (K-12)",
                    "Suspensions and expulsions (K-12)",
                    "Restraint and seclusion for discipline (PK)",
                    "Suspensions and expulsions (PK)",
                ]
            },
        ["Postsecondary certificate or degree completion"] =
            new()
            {
                Name = "Postsecondary certificate or degree completion",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.WorkforceTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students complete a certificate, associate's, or bachelor's degree within a specified time frame after entering college.",
                RecommendedMetrics = "Percentage of students completing a certificate, associate's, or bachelor's degree within 150 percent of the program's intended length. Other time frames, such as 100 percent and 200 percent of program length, should also be reported for this measure.",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Postsecondary credential award date",
                    "Postsecondary degree program length",
                    "Postsecondary enrollment date",
                ]
            },
        ["Postsecondary enrollment directly after high school graduation"] =
            new()
            {
                Name = "Postsecondary enrollment directly after high school graduation",
                Cluster = [IndicatorCluster.PostsecondaryTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "High school graduates enroll in a postsecondary institution by October 31 following their high school graduation.",
                RecommendedMetrics = "Percentage of high school graduates who enroll in a postsecondary institution by October 31 following their high school graduation",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "High school graduation date",
                    "Postsecondary enrollment date",
                ]
            },
        ["Postsecondary persistence"] =
            new()
            {
                Name = "Postsecondary persistence",
                Cluster = [IndicatorCluster.PostsecondarySuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students continue enrolling in college in subsequent years, including transfers to other colleges.",
                RecommendedMetrics = "Percentage of students in a cohort who continue enrolling in college (including transfers to other colleges) or complete a credential the following year, captured for up to 150 percent of program length. Other time frames, such as 100 and 200 percent of program length, should also be reported for this measure.",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Postsecondary credential award date",
                    "Postsecondary degree program length",
                    "Postsecondary enrollment date",
                ]
            },
        ["Representational racial and ethnic diversity of educators"] =
            new()
            {
                Name = "Representational racial and ethnic diversity of educators",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.SchoolClimate, IndicatorCluster.SocialEmotionalLearning],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Educators reflect the racial and ethnic diversity of the student body.",
                RecommendedMetrics = "- Pre-K: Educational staff composition by race and ethnicity (%) compared to student composition by race and ethnicity (%) - K-12: Educational staff composition by race and ethnicity (%) compared to student composition by race and ethnicity (%) - Postsecondary: Educational staff composition by race and ethnicity (%) compared to student composition by race and ethnicity (%) - Additional possible measure: Same-race student-teacher ratio by race and ethnicity",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Race and ethnicity (staff/educator)",
                    "Race and ethnicity (individual)",
                ]
            },
        ["SAT and ACT participation and performance"] =
            new()
            {
                Name = "SAT and ACT participation and performance",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "High school students take and earn a \"college-ready\" score on the ACT or SAT before graduating high school.",
                RecommendedMetrics = "Percentage of grade 11-12 students who take the SAT/ACT Percentage of grade 11-12 students who earn a \"college-ready\" score, based on the benchmarks set by the SAT and ACT",
                DataNeeded = [DataCategory.Assessments],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "ACT completion",
                    "ACT score",
                    "SAT completion",
                    "SAT score",
                    "Student grade level (K-12)",
                ]
            },
        ["School and workplace racial and ethnic diversity"] =
            new()
            {
                Name = "School and workplace racial and ethnic diversity",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.SchoolClimate, IndicatorCluster.SocialEmotionalLearning, IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Individuals are exposed to racial and ethnic diversity within their schools, postsecondary institutions, and workplaces.",
                RecommendedMetrics = "Student body composition by race and ethnicity (%); Employee composition by race and ethnicity (%).",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Race and ethnicity (individual)",
                ]
            },
        ["School and workplace socioeconomic diversity"] =
            new()
            {
                Name = "School and workplace socioeconomic diversity",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.SchoolClimate, IndicatorCluster.SocialEmotionalLearning, IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Individuals are exposed to socioeconomic diversity within their schools, postsecondary institutions, and workplaces.",
                RecommendedMetrics = "Student body composition by income; Employee composition by income.",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Income level (individual/family)",
                ]
            },
        ["School safety"] =
            new()
            {
                Name = "School safety",
                Cluster = [IndicatorCluster.SchoolClimate, IndicatorCluster.SocialEmotionalLearning],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Students feel physically, mentally, and emotionally safe at school or campus (that is, safe from both physical threats and violence, as well as bullying and cyberbullying).",
                RecommendedMetrics = "Percentage of students reporting high levels of physical, mental, and emotional safety in school climate surveys, such as the U.S. Department of Education ED School Climate Surveys (EDSCLS), the Sense of Safety subscale within the CORE Districts school culture and climate survey, or the School Safety subscale within the Panorama Student Survey. Percentage of students reporting physical safety and freedom from harassment and discrimination in campus surveys, such as the National Survey of Student Engagement.",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Campus climate surveys (K-12)",
                    "Campus climate surveys (Postsecondary)",
                    "Campus safety incident reports (Postsecondary)",
                ]
            },
        ["School-family engagement"] =
            new()
            {
                Name = "School-family engagement",
                Cluster = [IndicatorCluster.SchoolClimate],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "There are effective partnerships between schools and families, such that parents have access to school systems and are meaningfully included in school processes and student learning.",
                RecommendedMetrics = "Percentage of families and percentage of teachers or caregivers reporting positive relationship quality with one another, using a tool such as the Family and Provider/Teacher Relationship Quality (FPTRQ) parent survey; Mean scores on family surveys, such as the Panorama Family-School Relationships Survey or CORE Districts School Culture & Climate Survey parent assessment of school-community engagement",
                DataNeeded = [DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Family engagement surveys (K-12)",
                    "Family engagement surveys (PK)",
                ]
            },
        ["Selection of a well-matched postsecondary institution"] =
            new()
            {
                Name = "Selection of a well-matched postsecondary institution",
                Cluster = [IndicatorCluster.PostsecondaryTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "High school graduates select the best \"match\" college among the institutions to which they were admitted, based on the institutional graduation rate of similar students.",
                RecommendedMetrics = "Percentage of high school seniors who select a college within 10 percentage points of the best matched postsecondary institution to which they were admitted, based on the institution's graduation rate for similar students by race, ethnicity, or income status (as measured by Pell Grant receipt).",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Institution graduation rate",
                    "Cohort graduation year",
                    "Race and ethnicity (individual)",
                    "Pell grant receipt",
                    "Postsecondary enrollment status (Full time/part time)",
                    "Postsecondary admissions decision",
                ]
            },
        ["Self-efficacy"] =
            new()
            {
                Name = "Self-efficacy",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.SocialEmotionalLearning, IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Students believe in their ability to achieve an outcome or reach a goal.",
                RecommendedMetrics = "Percentage of students reporting a high level of self-efficacy on surveys such as the CORE Districts SEL Survey self-efficacy scale, Percentage of individuals reporting a high level of self-efficacy on surveys such as the New General Self-Efficacy Scale or Ascend survey's Self-Efficacy Scale",
                DataNeeded = [DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Self-efficacy surveys (K-12)",
                    "Self-efficacy surveys (Postsecondary)",
                    "Self-efficacy surveys (Workforce)",
                ]
            },
        ["Self-management"] =
            new()
            {
                Name = "Self-management",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.SocialEmotionalLearning, IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Students are able to regulate their emotions, thoughts, and behaviors effectively in different situations.",
                RecommendedMetrics = "Percentage of students reporting a high level of self-management on surveys such as the CORE Districts SEL Survey self-management scale (grades 5-12) or Shift and Persist scale for children, Percentage of individuals reporting a high level of self-management on surveys such as the Shift and Persist scale for teens and adults",
                DataNeeded = [DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Self-management surveys (K-12)",
                    "Self-management surveys (Postsecondary)",
                    "Self-management surveys (Workforce)",
                ]
            },
        ["Senior summer on track"] =
            new()
            {
                Name = "Senior summer on track",
                Cluster = [IndicatorCluster.PostsecondaryTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "High school graduates intending to enroll in postsecondary education in the fall after high school graduation complete the registration, financial, and logistic deadlines over the summer necessary to successfully enroll in the fall.",
                RecommendedMetrics = "Percentage of high school graduates reporting intentions to enroll in postsecondary education in the fall who successfully enroll in a postsecondary institution by October 31 following their high school graduation",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Postsecondary enrollment date",
                    "Reported intent to enroll in postsecondary education",
                    "High school graduation date",
                ]
            },
        ["Social awareness"] =
            new()
            {
                Name = "Social awareness",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.SocialEmotionalLearning, IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Students are able understand others' perspectives; understand social and ethical norms for behavior; and recognize family, school, and community resources and supports.",
                RecommendedMetrics = "Percentage of students reporting a high level of social awareness on surveys such as the CORE Districts SEL Survey social awareness scale, or percentage of students meeting benchmarks on teacher ratings of social skills drawn from Elliott and Gresham's Social Skills Rating Scale, Percentage of individuals demonstrating social proficiency on a performance assessment, such as the National Work Readiness Credential Essential Soft Skills assessment",
                DataNeeded = [DataCategory.Surveys, DataCategory.Assessments],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Evolving },
                ],
                DataElementNames =
                [
                    "Social awareness teacher ratings",
                    "Social proficiency performance assessments (Postsecondary)",
                    "Social proficiency performance assessments (Workforce)",
                    "Social awareness surveys (K-12)",
                ]
            },
        ["Social capital"] =
            new()
            {
                Name = "Social capital",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.SocialEmotionalPhysicalWellbeing,
                Definition = "Individuals have access to and are able to mobilize relationships that help them further their goals.",
                RecommendedMetrics = "Percentage of students or individuals reporting a high level of social capital on surveys such as the Social Capital Assessment + Learning for Equity (SCALE) Social Capital, Network Diversity, and Network Strength scales; Percentage of individuals reporting a high level of social capital on surveys such as the Social Capital Community Benchmark Survey",
                DataNeeded = [DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Emerging },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.Emerging },
                ],
                DataElementNames =
                [
                    "Social capital surveys (K-12)",
                    "Social capital surveys (Postsecondary)",
                    "Social capital surveys (Workforce)",
                ]
            },
        ["Student loan repayment"] =
            new()
            {
                Name = "Student loan repayment",
                Cluster = [IndicatorCluster.WorkforceSuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "Individuals pay student loans on time and make progress toward paying down their debt.",
                RecommendedMetrics = "Percentage of student borrowers in the following repayment categories, as defined on the College Scorecard—making progress, paid in full, and deferment—1, 2, 3, 5, and 10 years into the repayment phase of the loans. \"Making progress\" is defined as making regular payments such that the total of outstanding loan balances is less than the total of the original loan balances. \"Paid in full\" is defined as the outstanding loan balance being $0 and the loan not having been discharged through bankruptcy or other means. \"Deferment\" is defined as a postponement of the loan obligations, which is common for students re-enrolling in school. Borrowers who do not meet these milestones may fall in other categories, such as delinquency, default, and not making progress, that indicate they are unable to make timely progress toward their student debt.",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.WF, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Student loan repayment phase start date",
                    "Student loan repayment status",
                ]
            },
        ["Student perceptions of teaching"] =
            new()
            {
                Name = "Student perceptions of teaching",
                Cluster = [IndicatorCluster.SchoolClimate, IndicatorCluster.TeachingEffectiveness],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students report having a supportive, engaging teacher who sets clear, fair, and high expectations, and helps them learn.",
                RecommendedMetrics = "• K-12: Students' perceptions of their teacher's effectiveness, using a survey instrument such as the Pedagogical Effectiveness subscale of the Panorama Student Survey, the Tripod Student Survey, or the Ambitious Instruction and Supportive Environment domains of the 5Essentials Survey • Postsecondary: Students' perceptions of whether college instructors implement effective teaching practices, using a survey instrument such as the National Survey of Student Engagement",
                DataNeeded = [DataCategory.ClassroomObservations, DataCategory.Surveys],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Emerging },
                ],
                DataElementNames =
                [
                    "Teacher effectiveness student surveys (K-12)",
                    "Teacher effectiveness student surveys (Postsecondary)",
                ]
            },
        ["Successful career transition after high school"] =
            new()
            {
                Name = "Successful career transition after high school",
                Cluster = [IndicatorCluster.WorkforceTransitions],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "High school graduates transition to training, military service, or employment in the fall after graduating high school (if they do not matriculate to postsecondary education).",
                RecommendedMetrics = "Percentage of high school graduates enlisted in the military, enrolled in an apprenticeship program, enrolled in noncredit career and technical education (CTE) courses, or employed and earning at least the median annual full-time earnings for high school graduates ($35,000 year) before October 31 following graduation.",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Surveys],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving }],
                DataElementNames =
                [
                    "Apprenticeship program enrollment date",
                    "Earnings",
                    "Employment date",
                    "Enlistment in the military",
                    "Enrollment in noncredit CTE date",
                    "High school graduation date",
                ]
            },
        ["Successful completion of Algebra I by 9th grade"] =
            new()
            {
                Name = "Successful completion of Algebra I by 9th grade",
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students successfully complete Algebra I or an equivalent course before or during grade 9.",
                RecommendedMetrics = "Percentage of first-time grade 9 students who complete Algebra I or an equivalent course by the end of their 9th-grade year",
                DataNeeded = [DataCategory.StudentTranscripts],
                SectorReadiness = [new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Course subject area",
                    "Course outcome",
                    "Student grade level (K-12)",
                ]
            },
        ["Teacher credentials"] =
            new()
            {
                Name = "Teacher credentials",
                Cluster = [IndicatorCluster.TeachingEffectiveness],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students have access to teachers who have earned credentials demonstrating their knowledge and preparation for teaching.",
                RecommendedMetrics = "• Pre-K: Percentage of lead teachers with at least a bachelor's degree • Pre-K: Percentage of lead teachers with specialized training in pre-K • K-12: Percentage of courses taught by full-time equivalent (FTE) teachers (that is, teachers other than substitutes or those with emergency or provisional licenses) • K-12: Percentage of courses taught by teachers certified to teach the given subject or grade level",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Highest level of education completed (staff/educator)",
                    "Teacher qualification or certification type",
                    "Course subject area",
                    "Staff position type",
                    "Staff FTE status",
                    "Teaching assignment",
                ]
            },
        ["Teacher experience"] =
            new()
            {
                Name = "Teacher experience",
                Cluster = [IndicatorCluster.TeachingEffectiveness],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Students have equitable access to experienced teachers.",
                RecommendedMetrics = "• Pre-K: Percentage of teachers with < 1 year, 1-5 years, and 5+ years of experience • K-12: Percentage of teachers with < 1 year, 1-5 years, and 5+ years of experience",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness =
                [
                    new() { Sector = Sector.PK, Readiness = IndicatorReadiness.WellEstablished },
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.WellEstablished },
                ],
                DataElementNames =
                [
                    "Years of teaching experience",
                    "Staff position type",
                ]
            },
        ["Teachers' contributions to student learning growth"] =
            new()
            {
                Name = "Teachers' contributions to student learning growth",
                Cluster = [IndicatorCluster.TeachingEffectiveness],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Teachers contribute to students' learning growth.",
                RecommendedMetrics = "• K-12 and postsecondary: Percentage of instructors demonstrating above average contributions to student learning, as measured by student growth on state standardized tests or other outcomes (for example, using value-added models or student growth percentiles)",
                DataNeeded = [DataCategory.AdministrativeData, DataCategory.Assessments],
                SectorReadiness =
                [
                    new() { Sector = Sector.K12, Readiness = IndicatorReadiness.Evolving },
                    new() { Sector = Sector.PS, Readiness = IndicatorReadiness.Emerging },
                ],
                DataElementNames =
                [
                    "SGP for standardized assessments",
                    "Teacher value-added",
                    "Teaching assignment",
                ]
            },
        ["Transfer (if applicable)"] =
            new()
            {
                Name = "Transfer (if applicable)",
                Cluster = [IndicatorCluster.PostsecondarySuccess],
                Type = IndicatorType.OutcomesMilestones,
                Domain = IndicatorDomain.AcademicProgressCompletion,
                Definition = "Postsecondary students transfer to a longer program (from certificate to associate's degree, or from associate's to bachelor's degree).",
                RecommendedMetrics = "Percentage of students in a certificate or associate's degree program who transfer to a longer degree program within 150 percent of the original program's intended length. Other time frames, such as 100 percent and 200 percent of program length, are also useful to track.",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Postsecondary degree program length",
                    "Postsecondary enrollment date",
                    "Postsecondary credential type",
                ]
            },
        ["Unmet financial need"] =
            new()
            {
                Name = "Unmet financial need",
                Cluster = [IndicatorCluster.PostsecondarySuccess, IndicatorCluster.PostsecondaryTransitions],
                Type = IndicatorType.EWSystemConditions,
                Domain = IndicatorDomain.CareerReadinessEconomicSuccess,
                Definition = "The cost of college attendance students must pay out of pocket or finance through loans.",
                RecommendedMetrics = "Average net price (cost of attendance minus grants, scholarships, or tuition waivers from all sources) minus average expected family contribution (EFC), as calculated by FAFSA.",
                DataNeeded = [DataCategory.AdministrativeData],
                SectorReadiness = [new() { Sector = Sector.PS, Readiness = IndicatorReadiness.WellEstablished }],
                DataElementNames =
                [
                    "Average cost of attendance",
                    "Average expected family contribution (EFC)",
                    "Average financial aid amount (including grants, scholarships, and tuition waivers)",
                ]
            },
    };
}
