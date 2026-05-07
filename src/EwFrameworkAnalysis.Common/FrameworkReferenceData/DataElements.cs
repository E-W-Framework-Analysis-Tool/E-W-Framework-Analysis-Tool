using EwFrameworkAnalysis.Common.Models.Framework;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public static class EwFrameworkDataElements
{
    public static readonly Dictionary<string, DataElement> Elements = new()
    {
        ["ACT completion"] =
            new DataElement
            {
                Name = "ACT completion",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Binary indicator of whether a student has taken the ACT. Used in conjunction with ACT score to measure both participation and college readiness. Indicator source targets grades 11-12."
            },
        ["ACT score"] =
            new DataElement
            {
                Name = "ACT score",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Composite or section ACT score. \"College-ready\" threshold is defined by ACT benchmark scores. Should be interpreted alongside ACT completion; a missing score may mean non-participation rather than low performance."
            },
        ["Advising and counseling service utilization"] =
            new DataElement
            {
                Name = "Advising and counseling service utilization",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator of whether a student has used academic advising or career counseling services. In the K-12 context this relates to college and career counselor access (student-to-counselor ratio is the derived metric); in the PS context it relates to campus advising utilization rates."
            },
        ["Age"] =
            new DataElement
            {
                Name = "Age",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Numerical age as of point in time, or birth date. In the PK context, age is particularly important for determining eligibility for public pre-K programs (typically 3- and 4-year-olds) and for interpreting developmental screening results."
            },
        ["AP course designation"] =
            new DataElement
            {
                Name = "AP course designation",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Flag indicating a course is an Advanced Placement offering. Used to measure access to and participation in early college coursework. Distinct from AP credit earned — a student may be enrolled in an AP course but not sit the end-of-course exam or earn a qualifying score (3+). See also: AP, IB, or Dual Credit course credits."
            },
        ["AP, IB, or Dual Credit course credits"] =
            new DataElement
            {
                Name = "AP, IB, or Dual Credit course credits",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Credits earned through AP (score of 3+), IB (score of 5+), or dual enrollment courses. Captures the completion/success side of early college participation, as opposed to enrollment or course designation flags. In the PS context, tracks credits that transferred from high school."
            },
        ["Apprenticeship program enrollment date"] =
            new DataElement
            {
                Name = "Apprenticeship program enrollment date",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Date a student or worker enrolled in a registered apprenticeship program. Used as an indicator of successful career transition after high school (alongside employment, military enlistment, and noncredit CTE enrollment). The relevant post-graduation window for the associated indicator is before October 31 following graduation."
            },
        ["Average cost of attendance"] =
            new DataElement
            {
                Name = "Average cost of attendance",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Institution-level average total cost of attendance (tuition, fees, housing, etc.). Used as an input to the Unmet Financial Need indicator: net price = cost of attendance minus grants, scholarships, and tuition waivers. This is an institutional aggregate, not a student-level value."
            },
        ["Average expected family contribution (EFC)"] =
            new DataElement
            {
                Name = "Average expected family contribution (EFC)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Institution-level average EFC as calculated via FAFSA. Used alongside average cost of attendance and average financial aid to derive unmet financial need. This is an institutional aggregate. Note: EFC has been replaced by the Student Aid Index (SAI) under the FAFSA Simplification Act; confirm which term/value the source system uses."
            },
        ["Average financial aid amount (including grants, scholarships, and tuition waivers)"] =
            new DataElement
            {
                Name = "Average financial aid amount (including grants, scholarships, and tuition waivers)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Institution-level average of all grant-type aid received (excludes loans). Used in computing net price for the Unmet Financial Need indicator. This is an institutional aggregate, not a student-level value."
            },
        ["Average pay in county or MSA"] =
            new DataElement
            {
                Name = "Average pay in county or MSA",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Community-level average wage used in the Access to Jobs Paying a Living Wage indicator. The metric computes the ratio of average pay to location-adjusted cost of living; a ratio greater than 1 indicates jobs paying a living wage. This is an area-level aggregate sourced from labor market data, not an individual earnings value."
            },
        ["Bachelor's degree completion date"] =
            new DataElement
            {
                Name = "Bachelor's degree completion date",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Date a student completed a bachelor's degree. Used as the reference point for measuring subsequent graduate program enrollment (within 1-5 years) and for minimum economic return and economic mobility calculations. Consider whether this should be unified with Postsecondary credential attainment date filtered to degree level, or kept distinct."
            },
        ["Basic skills level"] =
            new DataElement
            {
                Name = "Basic skills level",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Assessment result indicating a worker's foundational reading, math, or language proficiency level. Relevant in the context of workforce training program eligibility and placement. The specific assessment instrument may vary by program (e.g., TABE, Accuplacer)."
            },
        ["Benefits availability"] =
            new DataElement
            {
                Name = "Benefits availability",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator of whether a job offers benefits (e.g., health insurance, retirement, paid leave). One component of the Employment in a Quality Job indicator, which uses an indexed measure such as the Good Jobs Scorecard assessing pay, benefits, scheduling, career paths, safety, and security."
            },
        ["Campus climate surveys (K-12)"] =
            new DataElement
            {
                Name = "Campus climate surveys (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student-reported perceptions of physical, mental, and emotional safety at school. Example instruments: ED School Climate Surveys (EDSCLS), CORE Districts School Culture & Climate Survey (Sense of Safety subscale), Panorama Student Survey (School Safety subscale). Distinct from family engagement or teacher effectiveness surveys."
            },
        ["Campus climate surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Campus climate surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student-reported perceptions of physical safety and freedom from harassment and discrimination on campus. Example instrument: National Survey of Student Engagement (NSSE). Used for the School Safety indicator in the PS context."
            },
        ["CHIP eligibility status"] =
            new DataElement
            {
                Name = "CHIP eligibility status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Indicator of whether a child meets income and residency requirements for the Children's Health Insurance Program. Used in the Health Insurance Coverage indicator alongside Medicaid eligibility and actual enrollment."
            },
        ["CHIP enrollment"] =
            new DataElement
            {
                Name = "CHIP enrollment",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Indicator of whether a child is actively enrolled in CHIP. The Health Insurance Coverage indicator measures the percentage of eligible individuals enrolled, so both eligibility and enrollment are needed to compute uptake rates."
            },
        ["City or county population"] =
            new DataElement
            {
                Name = "City or county population",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Total population of a city or county. Used as the denominator for community-level crime rate metrics (violent felonies per 100,000 residents; juvenile arrests per 100,000 residents). Sourced from census or similar area-level data."
            },
        ["Civic engagement surveys (K-12)"] =
            new DataElement
            {
                Name = "Civic engagement surveys (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student self-report of civic knowledge, values, and participation. Example instruments: Youth Civic and Character Measures Toolkit Survey, Youth Civic Engagement Indicators Project Survey."
            },
        ["Civic engagement surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Civic engagement surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student self-report of civic engagement at the postsecondary level. Same construct as K-12 version; instrument may differ (e.g., Index of Civic and Political Engagement)."
            },
        ["Civic engagement surveys (Workforce)"] =
            new DataElement
            {
                Name = "Civic engagement surveys (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Individual self-report of civic engagement in the workforce context. Example instrument: Index of Civic and Political Engagement."
            },
        ["Cohort graduation year"] =
            new DataElement
            {
                Name = "Cohort graduation year",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "The expected on-time graduation year for a student's entering cohort. Used in the Adjusted Cohort Graduation Rate (ACGR) calculation — the primary metric for the High School Graduation indicator. Students are tracked against this year regardless of school transfers."
            },
        ["Cohort year"] =
            new DataElement
            {
                Name = "Cohort year",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "The year a student first entered a specific cohort (e.g., first-time 9th grade entry year for K-12; first postsecondary enrollment year for PS). Used alongside cohort graduation year to compute on-time completion rates and persistence metrics."
            },
        ["College selectivity level"] =
            new DataElement
            {
                Name = "College selectivity level",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Classification of a postsecondary institution by admissions selectivity. Used in the Selection of a Well-Matched Institution indicator, which assesses whether students enroll in a college within 10 percentage points of the best-matched institution based on graduation rates for similar students."
            },
        ["College value-added"] =
            new DataElement
            {
                Name = "College value-added",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Institution-level value-added measure representing a college's contribution to student outcomes (graduation rates, earnings, loan repayment) beyond what would be predicted by student characteristics. Derived metric computed using statistical models; not typically available as a raw data field."
            },
        ["Communication skills performance assessments (K-12)"] =
            new DataElement
            {
                Name = "Communication skills performance assessments (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Score or proficiency level on an assessment of oral, written, nonverbal, and listening skills. Example instrument: College and Career Readiness Assessment (CCRA+) for grades 6-12, which measures critical thinking, problem solving, and written communication."
            },
        ["Communication skills performance assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Communication skills performance assessments (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Score or proficiency on communication assessments at the PS level. Example instruments: Collegiate Learning Assessment (CLA+), Success Skills Assessment (SSA+), HEIghten Outcomes Assessment for Written Communication."
            },
        ["Communication skills performance assessments (Workforce)"] =
            new DataElement
            {
                Name = "Communication skills performance assessments (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Score or proficiency on workforce communication assessments. Example instrument: National Work Readiness Credential Essential Soft Skills assessment."
            },
        ["Commute time"] =
            new DataElement
            {
                Name = "Commute time",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Transportation",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Average commute time to work, school, or college. One of two metrics for the Access to Transportation indicator (the other being the Low Transportation Cost Index). May be individual-reported or area-level aggregate depending on data source."
            },
        ["Course department"] =
            new DataElement
            {
                Name = "Course department",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "The subject-area department offering a course (e.g., English, Math, Science). Used in the Access to College Preparatory Coursework indicator to verify that schools offer the required subject-area distribution (4 years English, 4 years math, 3 years lab science, etc.)."
            },
        ["Course identifier or title"] =
            new DataElement
            {
                Name = "Course identifier or title",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "The local course code or name used to identify a specific course. Critical for determining course type (e.g., Algebra I, AP Biology, dual enrollment English) across multiple indicators including college prep coursework completion, CTE pathway concentration, gateway course completion, and early college access."
            },
        ["Course offering by grade level"] =
            new DataElement
            {
                Name = "Course offering by grade level",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Indicates which grade levels a course is offered to. Used to measure access to rigorous coursework (e.g., Algebra I availability in middle school) and to verify the full suite of college preparatory courses is accessible at appropriate grade levels."
            },
        ["Course outcome"] =
            new DataElement
            {
                Name = "Course outcome",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Result of a student's enrollment in a course: completion, failure, or passage. Used across on-track indicators (6th, 8th, 9th grade) and college prep coursework completion. A passing outcome is typically required to count a course toward graduation or admissions requirements. Distinct from the course grade (GPA element)."
            },
        ["Course performance (English and Math)"] =
            new DataElement
            {
                Name = "Course performance (English and Math)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Grade or pass/fail result specifically in English language arts and math courses. Used in the 6th and 8th grade on-track indicators, which require no Ds or Fs in ELA or math as a criterion. Consider whether this should be broadened to all subjects or kept as a targeted ELA+Math flag to match the indicator definition precisely."
            },
        ["Credential or certification type"] =
            new DataElement
            {
                Name = "Credential or certification type",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Type of teaching credential or certification held by an educator. Used in the Teacher Credentials indicator, which measures the percentage of courses taught by certified/credentialed teachers and the percentage of pre-K lead teachers with specialized early childhood training. Distinct from Teacher qualification or certification type — consider consolidating."
            },
        ["Credential-seeking status"] =
            new DataElement
            {
                Name = "Credential-seeking status",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS, Sector.WF],
                AdditionalNotes = "Indicates whether a student or worker is enrolled with the intent to earn a credential (degree, certificate, or industry certification). Relevant to persistence and completion metrics; non-credential-seeking students are often excluded from graduation rate calculations."
            },
        ["Credits earned in first year"] =
            new DataElement
            {
                Name = "Credits earned in first year",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Number of credits successfully completed during a student's first year of postsecondary enrollment. The First-Year Credit Accumulation indicator targets 30 credits for full-time and 15 for part-time students. Should be evaluated in conjunction with credits attempted and enrollment status (full/part time). Consider whether this overlaps with Number of credits earned and whether that element can serve the same purpose."
            },
        ["CTE course completion"] =
            new DataElement
            {
                Name = "CTE course completion",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator that a student successfully completed a CTE course. Used in the CTE Pathway Concentration indicator, which requires completing three or more CTE courses in a single pathway (for 12th-graders) or 12+ credits within a CTE program. Similar to Course outcome but scoped specifically to CTE courses."
            },
        ["CTE course ID or course title"] =
            new DataElement
            {
                Name = "CTE course ID or course title",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "The local identifier or name for a CTE-specific course. Enables grouping of courses by CTE pathway or career cluster. Functionally similar to Course identifier or title but scoped to CTE offerings; consider whether a CTE flag on the general course identifier element would be sufficient."
            },
        ["CTE pathway or career cluster associated with CTE course"] =
            new DataElement
            {
                Name = "CTE pathway or career cluster associated with CTE course",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "The state-defined CTE pathway (e.g., Health Sciences, Information Technology) or federal career cluster associated with a CTE course. Essential for determining pathway concentration — a student must complete multiple courses within the same pathway to meet concentration criteria."
            },
        ["CTE program"] =
            new DataElement
            {
                Name = "CTE program",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "The CTE program a student is enrolled in. Used in the Access to In-Demand CTE Pathways indicator, which evaluates whether CTE program offerings align to in-demand occupations. Also used to determine enrollment for industry-recognized credential and work-based learning metrics."
            },
        ["Cultural competency assessments (K-12)"] =
            new DataElement
            {
                Name = "Cultural competency assessments (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Score or proficiency level on an assessment measuring intercultural competency. Example instruments: HEIghten Outcomes Assessment for Intercultural Competency & Diversity, Intercultural Development Inventory (IDI)."
            },
        ["Cultural competency assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Cultural competency assessments (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Score or proficiency on intercultural competency assessments at the PS level. Example instrument: Intercultural Development Inventory (IDI)."
            },
        ["Cultural competency assessments (Workforce)"] =
            new DataElement
            {
                Name = "Cultural competency assessments (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Score or proficiency on intercultural competency assessments in the workforce context. Example instrument: Intercultural Development Inventory (IDI)."
            },
        ["Date of services provided"] =
            new DataElement
            {
                Name = "Date of services provided",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Date on which a support service was delivered to a student or child. In the PK context, used to verify that children with identified early intervention needs were connected to services (and the timeliness of that connection). In K-12 and PS, tracks student support service delivery."
            },
        ["Developmental screening results"] =
            new DataElement
            {
                Name = "Developmental screening results",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Results of a standardized developmental screening tool used to identify developmental, sensory, or behavioral concerns in children under age 5. Example instruments are listed in the Head Start Early Childhood Learning and Knowledge Center guide \"Birth to 5: Watch Me Thrive!\" Used in both the Mental and Emotional Well-Being indicator and the Access to Early Intervention Screening indicator."
            },
        ["Digital skills assessments (K-12)"] =
            new DataElement
            {
                Name = "Digital skills assessments (K-12)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Score or proficiency level on an assessment of digital literacy and technology skills. Example instrument: Problem Solving in Technology-Rich Environments (PS-TRE) assessment within the Education & Skills Online suite, based on OECD PIAAC domains."
            },
        ["Digital skills assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Digital skills assessments (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Score or proficiency on digital skills assessments at the PS level. Same construct and instrument family as K-12 version (OECD PIAAC-based tools)."
            },
        ["Digital skills assessments (Workforce)"] =
            new DataElement
            {
                Name = "Digital skills assessments (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Score or proficiency on digital skills assessments in the workforce context. Measures ability to use digital tools for accessing, managing, evaluating, and communicating information required for workforce success."
            },
        ["Diploma or credential award date"] =
            new DataElement
            {
                Name = "Diploma or credential award date",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Date a diploma or credential was formally awarded. In the K-12 context, relates primarily to the ACGR metric (high school diploma award date). In the PS context, captures postsecondary credential award dates. Overlaps with High school graduation date (K-12) and Postsecondary credential attainment date (PS) — consider whether this element is redundant or serves a distinct purpose (e.g., tracking alternative credentials)."
            },
        ["Direct child assessments (cognition)"] =
            new DataElement
            {
                Name = "Direct child assessments (cognition)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Results of a direct (assessor-administered) assessment of a child's math and scientific reasoning skills. Example instruments: Woodcock-Johnson IV Tests of ECAD (Number Sense subtest), IGDIs Early Numeracy, Research Based Early Mathematics Assessment (REMA). Distinct from teacher-reported readiness assessments."
            },
        ["Direct child assessments (language and literacy)"] =
            new DataElement
            {
                Name = "Direct child assessments (language and literacy)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Results of a direct (assessor-administered) assessment of a child's language and literacy skills. Example instruments: Woodcock-Johnson IV Tests of ECAD (Letter-Word and Writing subtests), IGDIs Early Literacy. Distinct from teacher-reported readiness assessments."
            },
        ["Direct child assessments of executive function"] =
            new DataElement
            {
                Name = "Direct child assessments of executive function",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Results of a direct assessment of a child's cognitive self-regulation and executive function. Example instruments: Heads Toes Knees Shoulders (HTKS) task (teacher-administered), Minnesota Executive Function Scale (MEFS, tablet-based). Supports the Kindergarten Readiness: Approaches to Learning indicator."
            },
        ["Direct child assessments of physical development"] =
            new DataElement
            {
                Name = "Direct child assessments of physical development",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Results of a direct assessment of a child's gross and fine motor skills and physical development. Example instrument: Peabody Developmental Motor Scale. May be administered by teachers, healthcare professionals, or other qualified adults."
            },
        ["Disability status"] =
            new DataElement
            {
                Name = "Disability status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual has a documented disability. Used as a demographic disaggregate across sectors. In K-12, this typically corresponds to students with an IEP or 504 plan. Definition and categorization may differ across sectors and data systems."
            },
        ["Dislocated worker status"] =
            new DataElement
            {
                Name = "Dislocated worker status",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator of whether a worker has been laid off or displaced from their job and is unlikely to return to their previous industry. Relevant for workforce training program eligibility under WIOA and other federal programs."
            },
        ["Dual credit course designation"] =
            new DataElement
            {
                Name = "Dual credit course designation",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Flag indicating a course is offered for simultaneous high school and college credit. Used in the Access to Early College Coursework indicator alongside AP and IB course designation flags. In the PS context, tracks whether credit was transferred in from dual enrollment. Consider consolidating with AP course designation and IB course designation into a single early-college course type flag."
            },
        ["EAP or mental health services provided"] =
            new DataElement
            {
                Name = "EAP or mental health services provided",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator of whether an employer offers an Employee Assistance Program (EAP) or mental health access through health care plans or other services. Used in the Access to Health, Mental Health, and Social Supports indicator. This is an employer-level offering flag, not an individual utilization record."
            },
        ["Early intervention screening results"] =
            new DataElement
            {
                Name = "Early intervention screening results",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Results of early intervention screening for developmental, sensory, and behavioral concerns in young children. Used to determine whether a referral for services is warranted. Distinct from Developmental screening results — confirm whether these two elements capture results from different instruments or serve distinct purposes, as they are used in overlapping indicators."
            },
        ["Early intervention screening services referral status"] =
            new DataElement
            {
                Name = "Early intervention screening services referral status",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Indicates whether a child was referred to early intervention services following a screening. The Access to Early Intervention Screening indicator measures the percentage of children with identified concerns who were connected to services, making this element essential for computing uptake rates."
            },
        ["Earnings"] =
            new DataElement
            {
                Name = "Earnings",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Individual annual or quarterly earnings. Used across multiple indicators: Successful Career Transition After High School (threshold: $35,000/year, the median for HS graduates), Minimum Economic Return (earnings must exceed median HS graduate earnings plus education cost recovery), and Economic Mobility (earnings must reach the 4th income quintile). The applicable threshold varies by indicator — ensure the data element captures sufficient precision (annual vs. quarterly) and time reference."
            },
        ["Eligibility for federal rental assistance"] =
            new DataElement
            {
                Name = "Eligibility for federal rental assistance",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether a household meets income requirements for federal rental assistance programs (e.g., Section 8/Housing Choice Voucher). Used alongside receipt of assistance to compute uptake rates for the Access to Affordable Housing indicator."
            },
        ["Employee income level"] =
            new DataElement
            {
                Name = "Employee income level",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Income classification of employees within a workplace. Used in the School and Workplace Socioeconomic Diversity indicator to measure the income composition of a workforce. Distinct from the individual Earnings element — this is used as a diversity/distribution measure at the workplace level rather than an individual outcome."
            },
        ["Employee race/ethnicity"] =
            new DataElement
            {
                Name = "Employee race/ethnicity",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Race and ethnicity of employees within a workplace. Used in the School and Workplace Racial and Ethnic Diversity indicator to measure employee composition. Distinct from Staff race/ethnicity, which is used in the educational context (K-12/PS) for measuring educator diversity relative to students."
            },
        ["Employment date"] =
            new DataElement
            {
                Name = "Employment date",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Date an individual began employment. In the Successful Career Transition After High School indicator, the relevant window is before October 31 following graduation. Also used to measure time-to-employment after completing education."
            },
        ["Employment status"] =
            new DataElement
            {
                Name = "Employment status",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Current employment status of an individual (e.g., employed full-time, employed part-time, unemployed, not in labor force). Used in the Employment in a Quality Job indicator. Note that this element captures whether someone is employed; job quality attributes (benefits, schedule, pay) are captured in separate elements."
            },
        ["English learner classification date"] =
            new DataElement
            {
                Name = "English learner classification date",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "The date a student was first classified as an English learner. Used in the English Learner Progress indicator to measure time to reclassification — the target is reclassification within five years of initial EL classification."
            },
        ["English learner status"] =
            new DataElement
            {
                Name = "English learner status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Current classification of a student as an English learner or reclassified fluent English proficient (RFEP). Used alongside English learner classification date to compute reclassification rates and as a demographic disaggregate across indicators."
            },
        ["Enlistment in the military"] =
            new DataElement
            {
                Name = "Enlistment in the military",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator that a high school graduate enlisted in the military. One of several acceptable post-graduation pathways counted in the Successful Career Transition After High School indicator (alongside employment, apprenticeship, and noncredit CTE enrollment), with the relevant window being before October 31 following graduation."
            },
        ["Enrollment date"] =
            new DataElement
            {
                Name = "Enrollment date",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Date a student first enrolled in a program or institution. In the K-12 context, used in ACGR calculations as the starting point for cohort tracking. See also: Kindergarten enrollment date, Postsecondary enrollment date. Consider whether this general element is redundant with those more specific elements or serves a distinct cross-sector purpose."
            },
        ["Enrollment in noncredit CTE date"] =
            new DataElement
            {
                Name = "Enrollment in noncredit CTE date",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Date a student enrolled in a noncredit CTE course at a postsecondary institution. In the Successful Career Transition After High School indicator, this is one acceptable post-graduation pathway (before October 31 following graduation). Note: this is listed under PS sector but the indicator context is primarily about K-12 graduates transitioning to WF — confirm sector assignment."
            },
        ["Enrollment in public pre-K"] =
            new DataElement
            {
                Name = "Enrollment in public pre-K",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Indicator that a child is enrolled in a publicly funded pre-K program. The program may be delivered through Head Start, public school pre-K classrooms, or licensed family-based/community-based programs. Used to compute the percentage of eligible 3- and 4-year-olds enrolled."
            },
        ["Enrollment in workforce training program"] =
            new DataElement
            {
                Name = "Enrollment in workforce training program",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator that an individual is enrolled in a workforce training program (e.g., WIOA-funded training, apprenticeship, or employer-sponsored program). Used as a denominator context for work-based learning participation and industry-recognized credential attainment rates."
            },
        ["Enrollment status (current and prior years)"] =
            new DataElement
            {
                Name = "Enrollment status (current and prior years)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Year-by-year enrollment status of a student, including whether they are enrolled, transferred, stopped out, or completed. Used in Postsecondary Persistence and Transfer indicators, which track enrollment continuity for up to 150% of program length. The K-12 sector tag appears in the data element but the associated indicators are PS-focused — verify whether K-12 is intentional."
            },
        ["Expenditures per student (K-12)"] =
            new DataElement
            {
                Name = "Expenditures per student (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Per-pupil expenditure at the K-12 level. The Expenditures per Student indicator references both total per-pupil expenditure and equity measures such as the New America Equity Factor (variance in per-pupil funding within a state). This is a school or district-level aggregate, not a student-level value."
            },
        ["Expenditures per student (PK)"] =
            new DataElement
            {
                Name = "Expenditures per student (PK)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "State expenditures per child enrolled in public pre-K. Program-level aggregate used in the Expenditures per Student and Access to Quality Culturally Responsive Curricula indicators."
            },
        ["Expenditures per student (Postsecondary)"] =
            new DataElement
            {
                Name = "Expenditures per student (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Total instruction and student service expenditures per FTE student based on 12-month enrollment, as reported to IPEDS. Institution-level aggregate."
            },
        ["FAFSA completion date"] =
            new DataElement
            {
                Name = "FAFSA completion date",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Date a student completed the FAFSA. The FAFSA Completion indicator measures percentage of grade 12 students completing by June 30. Timeliness of completion matters because it affects financial aid eligibility; the date field enables computing whether completion occurred within the relevant window."
            },
        ["Family eligibility for child care subsidies"] =
            new DataElement
            {
                Name = "Family eligibility for child care subsidies",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Indicator of whether a family meets income and other criteria to receive child care subsidy assistance. Used alongside receipt of subsidies to compute uptake rates for the Access to Child Care Subsidies indicator."
            },
        ["Family engagement surveys (K-12)"] =
            new DataElement
            {
                Name = "Family engagement surveys (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Family-reported perceptions of the quality of their relationship with the school. Example instruments: Panorama Family-School Relationships Survey, CORE Districts School Culture & Climate Survey (parent community engagement assessment). Used in the School-Family Engagement indicator."
            },
        ["Family engagement surveys (PK)"] =
            new DataElement
            {
                Name = "Family engagement surveys (PK)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Family and caregiver-reported perceptions of relationship quality with the pre-K program. Example instrument: Family and Provider/Teacher Relationship Quality (FPTRQ) parent survey."
            },
        ["First-generation college student"] =
            new DataElement
            {
                Name = "First-generation college student",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Indicator that neither of a student's parents completed a bachelor's degree. Used as a demographic disaggregate and equity lens across postsecondary access, persistence, and completion indicators. Related to Parental education level — confirm whether both are needed or if one can be derived from the other."
            },
        ["First-time 9th grade student status"] =
            new DataElement
            {
                Name = "First-time 9th grade student status",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Indicator that a student is entering 9th grade for the first time (not a repeater). Critical for correctly constructing the Adjusted Cohort Graduation Rate (ACGR) cohort and for the Successful Completion of Algebra I by 9th Grade indicator. Students who repeat 9th grade should not be counted as first-time entries in a subsequent year's cohort."
            },
        ["Funding dedicated to workforce development programs"] =
            new DataElement
            {
                Name = "Funding dedicated to workforce development programs",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Dollar amount of government funding allocated to workforce development programs (apprenticeships, job training, etc.) in a state. Used as the numerator in the Expenditures on Workforce Development Programs indicator; the denominator is Total educational funding."
            },
        ["Gateway course completion"] =
            new DataElement
            {
                Name = "Gateway course completion",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Indicator that a student successfully completed college-level introductory math and English courses (as defined by their institution) within their first year. These gateway courses are institution-defined and may vary; the element should capture both completion status and whether the course was designated as a gateway course at the student's institution."
            },
        ["Gender"] =
            new DataElement
            {
                Name = "Gender",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Gender identity of an individual. Used as a demographic disaggregate across all sectors. Note that data systems may capture this as biological sex, self-reported gender, or both; confirm what is available in each source system."
            },
        ["Geographical indicator"] =
            new DataElement
            {
                Name = "Geographical indicator",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Geographic identifier for an individual's place of residence (e.g., census tract, city, county). Enables linking individual-level records to community-level data (crime rates, poverty concentration, racial diversity). Used in the Neighborhood Racial Diversity indicator to compute exposure indices. For example, census tract, city, or county."
            },
        ["Gifted and talented participation"] =
            new DataElement
            {
                Name = "Gifted and talented participation",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Indicator of whether a student participates in a gifted and talented program. Used in the Equitable Placement in Rigorous Coursework indicator to measure whether demographic subgroups are proportionally represented in gifted programs."
            },
        ["Grade point average (K-12)"] =
            new DataElement
            {
                Name = "Grade point average (K-12)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Cumulative GPA for K-12 students. Used across multiple on-track indicators: 8th grade on track (GPA ≥ 2.5), 9th grade on track (GPA ≥ 3.0), and the general Grade Point Average indicator (GPA ≥ 3.0 for grades 6-12). The GPA threshold differs by grade band — ensure the element captures GPA at the applicable point in time."
            },
        ["Grade point average (Postsecondary)"] =
            new DataElement
            {
                Name = "Grade point average (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Cumulative GPA for postsecondary students. The Grade Point Average indicator targets a GPA of 3.0 or higher for college students as sufficient to graduate and obtain jobs. Note: this element is categorized under Postsecondary Transitions but is used in Postsecondary Success indicators — verify cluster category assignment."
            },
        ["Graduate credential attainment date"] =
            new DataElement
            {
                Name = "Graduate credential attainment date",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Date a student completed a graduate-level credential (master's degree or higher). Used as a reference point for Minimum Economic Return and Economic Mobility indicators (earnings measured 1, 3, 5, 10, 15 years after completing highest degree). Consider whether this should be unified with Postsecondary credential attainment date filtered by credential level."
            },
        ["Graduate program enrollment date"] =
            new DataElement
            {
                Name = "Graduate program enrollment date",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Date a student enrolled in a graduate degree program. Used in the Enrollment in Graduate Education indicator, which measures whether bachelor's degree recipients enroll in post-baccalaureate or graduate programs within 1-5 years of undergraduate completion."
            },
        ["Growth mindset surveys (K-12)"] =
            new DataElement
            {
                Name = "Growth mindset surveys (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student self-report of belief that abilities can grow with effort. Example instruments: CORE Districts SEL Survey Growth Mindset Scale (grades 5-12), Growth Mindset Scale developed by Carol Dweck."
            },
        ["Growth mindset surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Growth mindset surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student self-report of growth mindset at the PS level. Example instrument: Growth Mindset Scale developed by Carol Dweck."
            },
        ["Growth mindset surveys (Workforce)"] =
            new DataElement
            {
                Name = "Growth mindset surveys (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Individual self-report of growth mindset in the workforce context. Same construct as K-12/PS versions; instrument may differ."
            },
        ["Health services offered"] =
            new DataElement
            {
                Name = "Health services offered",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator of whether health services are offered by a program or institution. Part of the Access to Health, Mental Health, and Social Supports indicator, which measures the percentage of programs offering these services and the staff ratio. This is a program/institution-level offering flag."
            },
        ["Health-Related Quality of Life Scale scores"] =
            new DataElement
            {
                Name = "Health-Related Quality of Life Scale scores",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Score on the Health-Related Quality of Life (HRQoL) Scale, a self-rated measure of physical and mental health. Used in the Physical Development and Well-Being indicator for PS and WF populations. Distinct from the Self-Rated Health scale (which is a single-item measure also referenced in the indicator)."
            },
        ["High school diploma type"] =
            new DataElement
            {
                Name = "High school diploma type",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Classification of the diploma earned (e.g., regular, honors, IEP/alternative, GED). The ACGR metric used in the High School Graduation indicator counts only students who earn a regular diploma — alternative credentials are explicitly excluded. This element is therefore essential for correctly computing ACGR."
            },
        ["High school graduation date"] =
            new DataElement
            {
                Name = "High school graduation date",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Date a student received their high school diploma. Used as the starting reference point for post-graduation transition indicators: postsecondary enrollment must occur by October 31 following graduation; career transition outcomes (employment, apprenticeship, military, noncredit CTE) must also be achieved before that date."
            },
        ["High school graduation indicator"] =
            new DataElement
            {
                Name = "High school graduation indicator",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Binary flag indicating whether a student has graduated from high school with a regular diploma. Used across multiple transition indicators (college applications, FAFSA completion, postsecondary enrollment, well-matched institution selection) as a prerequisite filter. Closely related to High school graduation date and High school diploma type — all three may be needed together."
            },
        ["Higher-order thinking skills performance assessments (K-12)"] =
            new DataElement
            {
                Name = "Higher-order thinking skills performance assessments (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Score or proficiency on assessments of critical thinking, problem solving, and decision-making. Example instrument: College and Career Readiness Assessment (CLA+) for grades 6-12."
            },
        ["Higher-order thinking skills performance assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Higher-order thinking skills performance assessments (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Score or proficiency on higher-order thinking assessments at the PS level. Example instruments: CLA+, Success Skills Assessment (SSA+), HEIghten Outcomes Assessment for Critical Thinking."
            },
        ["Higher-order thinking skills performance assessments (Workforce)"] =
            new DataElement
            {
                Name = "Higher-order thinking skills performance assessments (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Score or proficiency on higher-order thinking assessments in the workforce context. Example instrument: Watson Glaser Critical Thinking Appraisal (scenario-based, used by employers for candidate evaluation)."
            },
        ["Highest level of education completed"] =
            new DataElement
            {
                Name = "Highest level of education completed",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Highest educational credential earned. In the Teaching Effectiveness context, this refers specifically to educators — the Teacher Credentials indicator measures the percentage of pre-K lead teachers with at least a bachelor's degree. The WF sector tag appears broad; confirm whether this element is intended to capture educator credentials only or also individual student/worker attainment."
            },
        ["Home language"] =
            new DataElement
            {
                Name = "Home language",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "The primary language spoken in a student's home. Used alongside English learner status and classification date to track EL progress. Also used as a demographic disaggregate. The home language survey is typically the first step in EL identification."
            },
        ["IB course designation"] =
            new DataElement
            {
                Name = "IB course designation",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Flag indicating a course is an International Baccalaureate offering. Used in the Access to Early College Coursework indicator. A qualifying score for IB credit is 5 or higher. Similar to AP course designation — consider consolidating AP, IB, and dual credit course designation flags."
            },
        ["IECMHC services offered"] =
            new DataElement
            {
                Name = "IECMHC services offered",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Indicator of whether Infant and Early Childhood Mental Health Consultation (IECMHC) services are available through a pre-K program. Used in the Access to Health, Mental Health, and Social Supports indicator. IECMHC is a specific evidence-based model for supporting social-emotional development in early childhood settings."
            },
        ["In-demand status"] =
            new DataElement
            {
                Name = "In-demand status",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator of whether a CTE program or occupation is classified as \"in demand\" based on regional labor market data. Used in the Access to In-Demand CTE Pathways indicator. The definition of in-demand is typically determined by state workforce agencies using regional LMI data."
            },
        ["Income level"] =
            new DataElement
            {
                Name = "Income level",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Income classification of an individual or family, used as a demographic disaggregate. Overlaps conceptually with Student or family socioeconomic status and Student socioeconomic status — confirm whether these elements capture different granularities (e.g., categorical income bands vs. free/reduced lunch eligibility vs. Pell eligibility) or are redundant."
            },
        ["Indicator of access to desktop or laptop at home"] =
            new DataElement
            {
                Name = "Indicator of access to desktop or laptop at home",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Binary indicator of whether someone in the household owns at least one desktop or laptop computer. The Access to Technology indicator requires both this element AND reliable broadband access — neither alone is sufficient to count as having technology access."
            },
        ["Indicator of access to reliable broadband internet"] =
            new DataElement
            {
                Name = "Indicator of access to reliable broadband internet",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Binary indicator of whether a household has access to reliable broadband internet. Must be used in conjunction with Indicator of access to desktop or laptop at home to compute the Access to Technology indicator metric."
            },
        ["Indicator of whether services were provided"] =
            new DataElement
            {
                Name = "Indicator of whether services were provided",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Binary flag indicating whether support services were actually delivered to a student or child. Used in conjunction with referral status and date of services provided to measure the gap between identified need and service delivery in the early intervention and student support contexts."
            },
        ["Individual or family military status"] =
            new DataElement
            {
                Name = "Individual or family military status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual is active duty military, a veteran, or a dependent of a military family. Used as a demographic disaggregate and may affect eligibility for certain support programs."
            },
        ["Individual with current or past child welfare involvement"] =
            new DataElement
            {
                Name = "Individual with current or past child welfare involvement",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator of whether a student is or has been involved with the child welfare system (e.g., foster care, child protective services). Used as an equity disaggregate given the well-documented educational disadvantages faced by students with child welfare involvement. Sensitive data requiring careful data governance."
            },
        ["Individuals experiencing homelessness"] =
            new DataElement
            {
                Name = "Individuals experiencing homelessness",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual lacks fixed, regular, and adequate nighttime residence. In K-12, typically identified under the McKinney-Vento Act. Used as a demographic disaggregate; also affects housing stability indicators."
            },
        ["Industry-recognized credential attainment"] =
            new DataElement
            {
                Name = "Industry-recognized credential attainment",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator that an individual has earned at least one industry-recognized credential as defined by their state. The Industry-Recognized Credential indicator applies across K-12 (12th-grade CTE students), PS (CTE program students), and WF (training program participants). The specific credentials that qualify are state-defined."
            },
        ["Institution graduation rate"] =
            new DataElement
            {
                Name = "Institution graduation rate",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "The overall graduation rate of a postsecondary institution, disaggregated by race, ethnicity, and income (Pell receipt). Used in the Selection of a Well-Matched Institution indicator: students are compared against institutions within 10 percentage points of the best-matched institution's graduation rate for similar students."
            },
        ["Institutional expenditure per student"] =
            new DataElement
            {
                Name = "Institutional expenditure per student",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Institution-level expenditure per FTE student. Appears to overlap with Expenditures per student (Postsecondary) — confirm whether these represent different reporting sources (e.g., IPEDS vs. institutional records) or are truly redundant."
            },
        ["Instructor observations"] =
            new DataElement
            {
                Name = "Instructor observations",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Results of formal classroom observation of an instructor's teaching practice. Feeds into the Classroom Observations of Instructional Practice indicator. Note: unlike K-12, there are no widely used standardized rubrics for postsecondary peer observation, though the indicator does include PS in scope."
            },
        ["Insured status"] =
            new DataElement
            {
                Name = "Insured status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual has any form of health insurance coverage. Used as the overall health insurance indicator, with Medicaid enrollment and CHIP enrollment as more specific sub-elements. The Health Insurance Coverage metric uses all three."
            },
        ["Job quality index"] =
            new DataElement
            {
                Name = "Job quality index",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "A composite or indexed score measuring overall job quality across dimensions such as pay, benefits, scheduling, career advancement, safety, and job security. Example instrument: Good Jobs Scorecard. This is a derived/composite measure rather than a raw data field — confirm how it is operationalized in source systems."
            },
        ["Job title or position type"] =
            new DataElement
            {
                Name = "Job title or position type",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "The title or classification of a staff member's position (e.g., lead teacher, assistant teacher, counselor, school nurse). Used to compute student-to-staff ratios for specific service types (counselors, health professionals) in the Access to College and Career Advising and Access to Health, Mental Health, and Social Supports indicators."
            },
        ["Justice involvement"] =
            new DataElement
            {
                Name = "Justice involvement",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual has had involvement with the criminal justice system (arrest, conviction, incarceration). Used as an equity disaggregate. Sensitive data requiring careful data governance and legal review for use in educational and workforce contexts."
            },
        ["K-12 school type"] =
            new DataElement
            {
                Name = "K-12 school type",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Classification of a K-12 school (e.g., traditional public, charter, magnet, private). Relevant for contextualizing per-pupil expenditure and resource comparisons. Consider whether this belongs under Finance & Resources or a more general institutional characteristics category."
            },
        ["Kindergarten enrollment date"] =
            new DataElement
            {
                Name = "Kindergarten enrollment date",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Date a child first enrolled in kindergarten. Used as a reference point to determine whether a child attended a pre-K program before entering K (linked to Pre-K eligibility status and Enrollment in public pre-K elements) and for calculating whether early intervention services were connected before kindergarten entry."
            },
        ["Kindergarten program days per week"] =
            new DataElement
            {
                Name = "Kindergarten program days per week",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Number of days per week a kindergarten program operates. The Access to Full-Day Kindergarten indicator requires programs to run five days per week and six hours per day. Both this element and Kindergarten program hours per day are needed to determine full-day status."
            },
        ["Kindergarten program hours per day"] =
            new DataElement
            {
                Name = "Kindergarten program hours per day",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Number of hours per day a kindergarten program operates. The Access to Full-Day Kindergarten indicator requires programs to run six hours per day for five days per week. Must be used alongside Kindergarten program days per week."
            },
        ["Kindergarten readiness assessments (cognition)"] =
            new DataElement
            {
                Name = "Kindergarten readiness assessments (cognition)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported or direct assessment of a child's math and scientific reasoning readiness for kindergarten. Example instruments: DRDP Cognition domain, R4K ELA Mathematics and Science domains, TS GOLD Cognitive and Mathematics subscales. Note: the Kindergarten Readiness: Cognition indicator references this as a K-12 element (administered at kindergarten entry), though development occurs in PK."
            },
        ["Leader effectiveness assessments"] =
            new DataElement
            {
                Name = "Leader effectiveness assessments",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Results of a formal evaluation of a school or program leader's effectiveness. The Effective Program and School Leadership indicator measures the percentage of leaders rated as effective using multi-measure evaluation systems such as the Tennessee TEAM Administrator Evaluation component."
            },
        ["Learning and development plan offered"] =
            new DataElement
            {
                Name = "Learning and development plan offered",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator of whether an employer provides a professional learning and development plan for employees. One of two components (alongside on-the-job training) in the Access to Ongoing Career Skills Development indicator. This is an employer-level offering flag."
            },
        ["LGBT status"] =
            new DataElement
            {
                Name = "LGBT status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual identifies as lesbian, gay, bisexual, or transgender. Used as an equity disaggregate. Sensitive data requiring careful data governance; collection methods and availability vary widely across data systems."
            },
        ["Location-adjusted cost of living in county or MSA"] =
            new DataElement
            {
                Name = "Location-adjusted cost of living in county or MSA",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Area-level cost of living index adjusted for geographic location. Used in the Access to Jobs Paying a Living Wage indicator, where it serves as the denominator in a ratio with average pay. The PK/K12/PS sector tags appear broad for what is primarily a workforce metric — confirm intended use across sectors."
            },
        ["Low Transportation Cost Index"] =
            new DataElement
            {
                Name = "Low Transportation Cost Index",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "The Low Transportation Cost Index from the U.S. Department of Housing and Urban Development. An area-level measure of transportation affordability. One of two metrics for the Access to Transportation indicator (the other being average commute time)."
            },
        ["Math proficiency (Grades 1 and 2)"] =
            new DataElement
            {
                Name = "Math proficiency (Grades 1 and 2)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Assessment of math skills for students in grades 1 and 2, used in the Early Grades On Track indicator. The indicator requires meeting grade-level benchmarks in math alongside reading proficiency and attendance/discipline criteria. This element captures early-grade progress before state standardized testing begins. Distinct from State standardized test (Math proficiency), which typically applies from grade 3 onward."
            },
        ["Median student debt"] =
            new DataElement
            {
                Name = "Median student debt",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Institution-level median total student loan debt. Used as the primary metric for the Cumulative Student Debt indicator. This is an institutional aggregate typically sourced from College Scorecard data."
            },
        ["Medicaid eligibility status"] =
            new DataElement
            {
                Name = "Medicaid eligibility status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Indicator of whether an individual meets income and other criteria for Medicaid enrollment. Used alongside Medicaid enrollment to compute uptake rates for the Health Insurance Coverage indicator."
            },
        ["Medicaid enrollment"] =
            new DataElement
            {
                Name = "Medicaid enrollment",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Indicator of whether an individual is actively enrolled in Medicaid. Used in the Health Insurance Coverage indicator; the percentage of eligible individuals enrolled is one of the key metrics."
            },
        ["Mental and emotional well-being assessments"] =
            new DataElement
            {
                Name = "Mental and emotional well-being assessments",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Results of a mental or emotional health screening or well-being assessment. In the PK context, uses developmental screening tools (see Head Start \"Birth to 5: Watch Me Thrive!\" guide). In K-12, uses universal screening tools (see NCSSLE \"Mental Health Screening Tools for Grades K-12\"). In PS/WF, may use psychological well-being scales."
            },
        ["Mental health services offered"] =
            new DataElement
            {
                Name = "Mental health services offered",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator of whether mental health services are available through a program or institution. Used in the Access to Health, Mental Health, and Social Supports indicator. This is a program/institution-level offering flag. The WF version of this concept is captured under EAP or mental health services provided."
            },
        ["Net worth"] =
            new DataElement
            {
                Name = "Net worth",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.PS, Sector.WF],
                AdditionalNotes = "Total assets minus total liabilities for an individual. Used in the Economic Security indicator, which measures whether individuals reach median wealth levels 10, 15, 20, and 30 years after completing education. This is a difficult-to-capture individual-level measure; confirm whether administrative sources can provide this or if survey data is required."
            },
        ["Number of ACEs"] =
            new DataElement
            {
                Name = "Number of ACEs",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Count of adverse childhood experiences (ACEs) reported by or on behalf of an individual. ACEs include household dysfunction, abuse, neglect, and other traumatic events. The Childhood Experiences indicator targets fewer than three ACEs. Typically collected via survey. Sensitive data requiring careful data governance."
            },
        ["Number of affordable housing units in city or county"] =
            new DataElement
            {
                Name = "Number of affordable housing units in city or county",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Count of housing units in an area where monthly costs do not exceed 30% of a household's income. Used as the numerator in the Access to Affordable Housing indicator; the denominator is the number of households with low (below 80% AMI) and very low (below 50% AMI) incomes."
            },
        ["Number of city or county residents experiencing poverty"] =
            new DataElement
            {
                Name = "Number of city or county residents experiencing poverty",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Count of residents in a city or county experiencing poverty (as defined by the federal poverty level). Used in the Neighborhood Economic Diversity indicator as the denominator for computing the share living in high-poverty neighborhoods (>40% poverty rate)."
            },
        ["Number of city or county residents living in a high poverty neighborhood"] =
            new DataElement
            {
                Name = "Number of city or county residents living in a high poverty neighborhood",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Count of residents living in a neighborhood where more than 40% of residents experience poverty. Used as the numerator in the Neighborhood Economic Diversity indicator metric."
            },
        ["Number of credits attempted"] =
            new DataElement
            {
                Name = "Number of credits attempted",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Total credits a student enrolled in during a given term or year. Used alongside Number of credits earned in the First-Year Credit Accumulation indicator to compute a completion ratio and assess whether students are attempting sufficient credits for on-time graduation."
            },
        ["Number of credits earned"] =
            new DataElement
            {
                Name = "Number of credits earned",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Total credits successfully completed by a student. Used in First-Year Credit Accumulation (30 credits full-time / 15 credits part-time in year one), First-Year Program of Study Concentration (9+ credits within a meta-major), and Gateway Course Completion indicators. Distinct from Credits earned in first year — verify whether that element is a subset or redundant."
            },
        ["Number of days suspended (K-12)"] =
            new DataElement
            {
                Name = "Number of days suspended (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Total number of days a K-12 student was suspended (in-school or out-of-school). Used in the Equitable Discipline Practices indicator to measure disproportionalities in the severity of discipline experienced across demographic subgroups."
            },
        ["Number of days suspended (PK)"] =
            new DataElement
            {
                Name = "Number of days suspended (PK)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Total number of days a pre-K child was suspended. Used in the Equitable Discipline Practices indicator. Pre-K suspensions are a significant equity concern given federal guidance discouraging their use; this element enables monitoring of disproportionate use in early childhood settings."
            },
        ["Number of households with low income in city or county"] =
            new DataElement
            {
                Name = "Number of households with low income in city or county",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Count of households in an area earning below 80% of area median income (AMI), the HUD definition of \"low income.\" Used as part of the denominator in the Access to Affordable Housing indicator ratio."
            },
        ["Number of households with very low income in city or county"] =
            new DataElement
            {
                Name = "Number of households with very low income in city or county",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Count of households in an area earning below 50% of area median income (AMI), the HUD definition of \"very low income.\" Used alongside Number of households with low income as a combined denominator for the Access to Affordable Housing indicator."
            },
        ["Number of juvenile arrests in city or county"] =
            new DataElement
            {
                Name = "Number of juvenile arrests in city or county",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Count of juvenile arrests in a city or county. Used with City or county population to compute the juvenile arrest rate per 100,000 residents for the Neighborhood Juvenile Arrests indicator. Note: the indicator lists all sectors in scope but this element only tags PK and K12 — verify whether WF and PS should be included."
            },
        ["Number of property felonies in city or county"] =
            new DataElement
            {
                Name = "Number of property felonies in city or county",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Count of property felonies in a city or county. Used with City or county population to compute the property crime rate per 100,000 residents for the Exposure to Neighborhood Crime indicator."
            },
        ["Number of violent felonies in city or county"] =
            new DataElement
            {
                Name = "Number of violent felonies in city or county",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Count of violent felonies in a city or county. Used with City or county population to compute the violent crime rate per 100,000 residents for the Exposure to Neighborhood Crime indicator."
            },
        ["Occupation category"] =
            new DataElement
            {
                Name = "Occupation category",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "The occupational classification of a worker's job (e.g., SOC code or major occupation group). Used in analyzing whether employment is in an in-demand field and supports labor market alignment analysis alongside In-demand status and CTE pathway data."
            },
        ["Office referrals (K-12)"] =
            new DataElement
            {
                Name = "Office referrals (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Count of disciplinary office referrals for a K-12 student. Used in the Equitable Discipline Practices indicator to measure differences in referral rates across demographic subgroups."
            },
        ["Office referrals (PK)"] =
            new DataElement
            {
                Name = "Office referrals (PK)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Count of disciplinary office referrals for a pre-K child. Used in the Equitable Discipline Practices indicator to detect disproportionate referral patterns in early childhood settings."
            },
        ["On-the-job training offered"] =
            new DataElement
            {
                Name = "On-the-job training offered",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator of whether an employer provides structured on-the-job training. One of two components in the Access to Ongoing Career Skills Development indicator, alongside Learning and development plan offered. Employer-level offering flag."
            },
        ["Overall teacher observation score"] =
            new DataElement
            {
                Name = "Overall teacher observation score",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Composite score from a formal classroom observation evaluation. Example frameworks: Danielson's Framework for Teaching, Marzano Causal Teacher Evaluation Model (K-12); CLASS or ECERS Interactions subscale (PK). Used alongside Subscale observation scores in the Classroom Observations of Instructional Practice indicator."
            },
        ["Parental education level"] =
            new DataElement
            {
                Name = "Parental education level",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Highest level of education completed by a student's parent(s) or guardian(s). Used as a demographic disaggregate and is the basis for the First-generation college student flag (neither parent completed a bachelor's degree). Consider whether both this element and First-generation college student are needed or if one can be derived from the other."
            },
        ["Participation in work-based learning"] =
            new DataElement
            {
                Name = "Participation in work-based learning",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator that a student or worker has participated in a work-based learning opportunity (internship, work study, cooperative education, apprenticeship, or similar). The Participation in Work-Based Learning indicator measures this before graduation/program completion across K-12, PS, and WF."
            },
        ["Pell grant receipt"] =
            new DataElement
            {
                Name = "Pell grant receipt",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Indicator of whether a student received a Pell grant. Used as a proxy for low-income student status in institutional graduation rate analyses (the Selection of a Well-Matched Institution indicator references graduation rates by Pell receipt). Also used as a financial aid tracking element."
            },
        ["Percentage of teachers regularly using standards-aligned; culturally responsive curricula"] =
            new DataElement
            {
                Name = "Percentage of teachers regularly using standards-aligned; culturally responsive curricula",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "School or program-level measure of the percentage of teachers who regularly use a core curriculum that is both standards-aligned (meeting EdReports quality criteria) and culturally responsive (centering students' lived experiences and heritage). This is an aggregated/derived measure; the indicator notes no specific measurement tools have been identified. Consider how this is operationalized in automated vs. manual profiling."
            },
        ["Physical health surveys (K-12)"] =
            new DataElement
            {
                Name = "Physical health surveys (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student self-report of physical health status. Example instrument: California Healthy Kids Survey Physical Health & Nutrition module. Used in the Physical Development and Well-Being indicator for K-12."
            },
        ["Physical health surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Physical health surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Adult self-report of physical health, such as the Self-Rated Health scale (percentage rating health as good, very good, or excellent) or HRQoL Scale. Used in the Physical Development and Well-Being indicator for PS."
            },
        ["Physical health surveys (Workforce)"] =
            new DataElement
            {
                Name = "Physical health surveys (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Adult self-report of physical health in the workforce context. Same instruments as PS version (Self-Rated Health scale, HRQoL Scale). Used in the Physical Development and Well-Being indicator for WF."
            },
        ["Post-baccalaureate program enrollment date"] =
            new DataElement
            {
                Name = "Post-baccalaureate program enrollment date",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Date a student enrolled in a post-baccalaureate program (e.g., certificate program or professional preparation program for bachelor's degree holders). Used alongside Graduate program enrollment date in the Enrollment in Graduate Education indicator, distinguishing post-bac certificates from graduate degree programs."
            },
        ["Postsecondary applications submitted"] =
            new DataElement
            {
                Name = "Postsecondary applications submitted",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Count of postsecondary applications submitted by a grade 12 student. The College Applications indicator requires submission of at least three applications for a student to count as meeting the target. The PS sector tag may reflect tracking of transfer applications — confirm intended use."
            },
        ["Postsecondary credential attainment date"] =
            new DataElement
            {
                Name = "Postsecondary credential attainment date",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Date a student earned a postsecondary credential (certificate, associate's, or bachelor's degree). Used in Postsecondary Persistence (to determine if a student completed rather than dropped out) and Postsecondary Certificate or Degree Completion indicators. Closely related to Bachelor's degree completion date and Graduate credential attainment date — consider whether a single credential attainment date with a credential type field could replace all three."
            },
        ["Postsecondary credential earned"] =
            new DataElement
            {
                Name = "Postsecondary credential earned",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "The specific type of credential earned (e.g., certificate, associate's degree, bachelor's degree). Used in the Graduate Degree Completion indicator to confirm credential type. Closely related to Postsecondary credential attainment date — these two elements together provide a complete credential record."
            },
        ["Postsecondary degree program length"] =
            new DataElement
            {
                Name = "Postsecondary degree program length",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "The nominal intended length of a postsecondary degree program (e.g., 2-year associate's, 4-year bachelor's). Critical for computing on-time completion metrics — outcomes are measured at 100%, 150%, and 200% of program length. This is the denominator for all time-based completion and persistence calculations."
            },
        ["Postsecondary enrollment date"] =
            new DataElement
            {
                Name = "Postsecondary enrollment date",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Date a student first enrolled in a postsecondary institution. Used in multiple indicators: Postsecondary Enrollment Directly After High School Graduation (must be by October 31 after HS graduation), First-Year Credit Accumulation, First-Year Program of Study Concentration, Gateway Course Completion, and Postsecondary Certificate or Degree Completion."
            },
        ["Postsecondary enrollment status (Full time/part time)"] =
            new DataElement
            {
                Name = "Postsecondary enrollment status (Full time/part time)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Indicator of whether a student is enrolled full-time or part-time in a postsecondary institution. Directly affects credit accumulation targets in the First-Year Credit Accumulation indicator (30 credits full-time vs. 15 credits part-time). Also affects how persistence and completion rates should be interpreted."
            },
        ["Postsecondary institution classification"] =
            new DataElement
            {
                Name = "Postsecondary institution classification",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Classification of a postsecondary institution by type (e.g., Carnegie Classification: research university, liberal arts college, community college, for-profit). Used to contextualize outcomes and as a control variable in value-added and well-matched institution analyses."
            },
        ["Postsecondary Institution ID"] =
            new DataElement
            {
                Name = "Postsecondary Institution ID",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Unique identifier for a postsecondary institution (e.g., IPEDS Unit ID, OPE ID). Used in the Selection of a Well-Matched Institution indicator to link students to institutional graduation rate data. See also Postsecondary institution ID (current and prior years) — confirm whether these two elements are redundant."
            },
        ["Postsecondary institution ID (current and prior years)"] =
            new DataElement
            {
                Name = "Postsecondary institution ID (current and prior years)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Postsecondary institution identifier captured longitudinally (current and prior enrollment years). Used in the Transfer indicator to track institutional movement — specifically, transfer from a shorter program (certificate/associate's) to a longer one. The prior-year institution ID is essential for identifying upward transfer. Likely redundant with Postsecondary Institution ID unless a longitudinal snapshot structure is explicitly modeled."
            },
        ["Postsecondary major"] =
            new DataElement
            {
                Name = "Postsecondary major",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "A student's declared field of study or major. Related to the First-Year Program of Study Concentration indicator, which measures completion of 9+ credits within a meta-major (a broader grouping of related majors). The major itself is more specific than the meta-major concept used in the indicator."
            },
        ["Pre-K eligibility status"] =
            new DataElement
            {
                Name = "Pre-K eligibility status",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Indicator of whether a 3- or 4-year-old child meets eligibility criteria for publicly funded pre-K (typically income-based). Used in the Enrollment in Quality Public Pre-K indicator as the denominator (percentage of eligible children enrolled)."
            },
        ["Pre-K program days per week"] =
            new DataElement
            {
                Name = "Pre-K program days per week",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Number of days per week a pre-K program operates. The Access to Full-Day Pre-K indicator requires programs to run five days per week and six hours per day. Must be used alongside Pre-K program hours per day."
            },
        ["Pre-K program hours per day"] =
            new DataElement
            {
                Name = "Pre-K program hours per day",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Number of hours per day a pre-K program operates. The Access to Full-Day Pre-K indicator requires programs to run six hours per day for five days per week. Must be used alongside Pre-K program days per week."
            },
        ["Pre-K program QRIS rating"] =
            new DataElement
            {
                Name = "Pre-K program QRIS rating",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "A pre-K program's rating on the state's Quality Rating and Improvement System (QRIS). Used in both the Access to Quality Public Pre-K and Enrollment in Quality Public Pre-K indicators. QRIS rating systems vary by state, so the specific scale and quality threshold should be documented per state."
            },
        ["Reading proficiency (Grades 1 and 2)"] =
            new DataElement
            {
                Name = "Reading proficiency (Grades 1 and 2)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Assessment of reading/ELA skills for students in grades 1 and 2, used in the Early Grades On Track indicator alongside math proficiency and attendance/discipline criteria. Captures early-grade literacy before state standardized testing. Distinct from State standardized test (Reading proficiency), which applies from grade 3 onward."
            },
        ["Receipt of child care subsidies"] =
            new DataElement
            {
                Name = "Receipt of child care subsidies",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Indicator of whether a family is currently receiving child care subsidy assistance. Used alongside Family eligibility for child care subsidies to compute uptake rates. The K-12 sector tag may reflect tracking into early elementary — confirm intended scope."
            },
        ["Receipt of federal rental assistance"] =
            new DataElement
            {
                Name = "Receipt of federal rental assistance",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether a household is currently receiving federal rental assistance (e.g., Section 8/Housing Choice Voucher). Used alongside eligibility data to compute uptake rates for the Access to Affordable Housing indicator."
            },
        ["Repayment phase start date"] =
            new DataElement
            {
                Name = "Repayment phase start date",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Date a student borrower entered the repayment phase of their student loans. The Student Loan Repayment indicator measures repayment status at 1, 2, 3, 5, and 10 years after this date. Repayment typically begins six months after leaving school."
            },
        ["Repayment status"] =
            new DataElement
            {
                Name = "Repayment status",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Current status of a borrower's student loan repayment. Categories as defined by the College Scorecard: making progress (outstanding balance decreasing), paid in full, deferment (common for re-enrollees), delinquency, default, or not making progress. The indicator targets the percentage in the first three (positive) categories."
            },
        ["Reported intent to enroll in postsecondary education"] =
            new DataElement
            {
                Name = "Reported intent to enroll in postsecondary education",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student-reported intention to enroll in postsecondary education. Used as a filter in the Senior Summer On Track indicator — only students who report intent to enroll are counted in the denominator. This distinguishes between students who planned to enroll and then did not (summer melt) versus those who always intended alternative pathways."
            },
        ["Reported kindergarten readiness (behavioral skills)"] =
            new DataElement
            {
                Name = "Reported kindergarten readiness (behavioral skills)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported assessment of a child's behavioral self-regulation, approaches to learning, and executive function readiness for kindergarten. Example instruments: DRDP Approaches to Learning - Self-Regulation domain, TS GOLD Cognitive subscale, CBRS. Supports the Kindergarten Readiness: Approaches to Learning indicator. Teacher-reported; distinct from direct child assessment instruments."
            },
        ["Reported kindergarten readiness (language and literacy)"] =
            new DataElement
            {
                Name = "Reported kindergarten readiness (language and literacy)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported assessment of a child's language and literacy readiness for kindergarten. Example instruments: DRDP Language and Literacy Development domain, R4K ELA Language and Literacy domain, TS GOLD Language and Literacy subscales. Teacher-reported; distinct from Direct child assessments (language and literacy)."
            },
        ["Reported kindergarten readiness (physical development)"] =
            new DataElement
            {
                Name = "Reported kindergarten readiness (physical development)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported assessment of a child's physical development and motor skills readiness for kindergarten. Example instruments: DRDP Physical Development - Health domain, R4K ELA Physical Well-Being and Motor Development domain, TS GOLD Physical subscale. Teacher-reported; distinct from Direct child assessments of physical development."
            },
        ["Reported kindergarten readiness (social-emotional skills)"] =
            new DataElement
            {
                Name = "Reported kindergarten readiness (social-emotional skills)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported assessment of a child's social-emotional development readiness for kindergarten. Example instruments: DRDP Social and Emotional Development domain, R4K ELA Social Foundations domain, TS GOLD Social-Emotional subscale, CBRS, DECA-P2. Teacher-reported; distinct from direct child assessments."
            },
        ["Restraint and seclusion for discipline (K-12)"] =
            new DataElement
            {
                Name = "Restraint and seclusion for discipline (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Indicator or count of incidents where a K-12 student was physically restrained or secluded as a disciplinary measure. Used in the Equitable Discipline Practices and Positive Behavior indicators. Distinct from restraint and seclusion for safety — the purpose matters for both legal compliance and equity analysis."
            },
        ["Restraint and seclusion for discipline (PK)"] =
            new DataElement
            {
                Name = "Restraint and seclusion for discipline (PK)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Indicator or count of incidents where a pre-K child was physically restrained or secluded as a disciplinary measure. PK-specific element for the Equitable Discipline Practices indicator."
            },
        ["Restraint and seclusion for safety (K-12)"] =
            new DataElement
            {
                Name = "Restraint and seclusion for safety (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Indicator or count of incidents where a K-12 student was physically restrained or secluded for safety reasons (i.e., imminent danger). Used in the Equitable Discipline Practices indicator. The discipline vs. safety distinction is important: safety-based restraint is subject to different regulatory requirements and equity analysis."
            },
        ["Restraint and seclusion for safety (PK)"] =
            new DataElement
            {
                Name = "Restraint and seclusion for safety (PK)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Indicator or count of incidents where a pre-K child was physically restrained or secluded for safety reasons. PK-specific element for the Equitable Discipline Practices indicator."
            },
        ["SAT completion"] =
            new DataElement
            {
                Name = "SAT completion",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Binary indicator of whether a student has taken the SAT. Used in the SAT and ACT Participation and Performance indicator alongside SAT score to measure both participation and college readiness. Targets grades 11-12."
            },
        ["SAT score"] =
            new DataElement
            {
                Name = "SAT score",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Total or section SAT score. \"College-ready\" threshold is defined by SAT benchmark scores. Should be interpreted alongside SAT completion; a missing score may mean non-participation rather than low performance."
            },
        ["School assignment (prior and current year)"] =
            new DataElement
            {
                Name = "School assignment (prior and current year)",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "The school(s) a staff member was assigned to in the current and prior years. Used to compute educator retention — teachers who return to the same school are counted as retained. The prior-year school assignment is essential for year-over-year comparison."
            },
        ["School value-added"] =
            new DataElement
            {
                Name = "School value-added",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "School-level value-added measure representing a school's contribution to student outcomes (achievement, attendance, SEL, college enrollment, earnings) beyond what would be predicted by student characteristics. Derived metric computed using statistical models. Used in the Institutions' Contributions to Student Outcomes indicator."
            },
        ["Self-efficacy surveys (K-12)"] =
            new DataElement
            {
                Name = "Self-efficacy surveys (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student self-report of belief in their ability to achieve outcomes or reach goals. Example instrument: CORE Districts SEL Survey self-efficacy scale."
            },
        ["Self-efficacy surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Self-efficacy surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student self-report of self-efficacy at the PS level. Example instruments: New General Self-Efficacy Scale, Ascend survey's Self-Efficacy Scale."
            },
        ["Self-efficacy surveys (Workforce)"] =
            new DataElement
            {
                Name = "Self-efficacy surveys (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Individual self-report of self-efficacy in the workforce context. Example instruments: New General Self-Efficacy Scale, Ascend survey's Self-Efficacy Scale."
            },
        ["Self-management surveys (K-12)"] =
            new DataElement
            {
                Name = "Self-management surveys (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student self-report of ability to regulate emotions, thoughts, and behaviors. Example instruments: CORE Districts SEL Survey self-management scale (grades 5-12), Shift and Persist scale for children. Note: this element also appears in the Social Awareness indicator data element list — verify whether that is intentional or an error."
            },
        ["Self-management surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Self-management surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student self-report of self-management at the PS level. Example instrument: Shift and Persist scale for teens and adults."
            },
        ["Self-management surveys (Workforce)"] =
            new DataElement
            {
                Name = "Self-management surveys (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Individual self-report of self-management in the workforce context. Example instrument: Shift and Persist scale for teens and adults."
            },
        ["Sense of belonging surveys"] =
            new DataElement
            {
                Name = "Sense of belonging surveys",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Employee self-report of belonging at their workplace. Example instrument: AAMC Diversity Engagement Survey. Used in the Inclusive Environments indicator for the WF sector. The naming is inconsistent with the sector-labeled variants (K-12, PK, PS); consider renaming to \"Sense of belonging surveys (Workforce)\" for consistency."
            },
        ["Sense of belonging surveys (K-12)"] =
            new DataElement
            {
                Name = "Sense of belonging surveys (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student self-report of belonging and connection at school. Example instruments: CORE Districts school culture and climate survey (Sense of Belonging subscale), Panorama Student Survey (Classroom Belonging subscale)."
            },
        ["Sense of belonging surveys (PK)"] =
            new DataElement
            {
                Name = "Sense of belonging surveys (PK)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Assessment or observational measure of a pre-K child's sense of belonging. Example instrument: CASEL's How I Feel About My School questionnaire, or ACSES observational assessment of equitable classroom interactions."
            },
        ["Sense of belonging surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Sense of belonging surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student self-report of belonging on campus. Example instruments: HERI Diverse Learning Environments Survey, NITE Culturally Engaging Campus Environments Survey."
            },
        ["SGP for standardized assessments"] =
            new DataElement
            {
                Name = "SGP for standardized assessments",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student growth percentile (SGP) on state standardized assessments, computed separately for reading/literacy, math, and science. SGPs measure a student's growth relative to academically similar peers. Used in the Teachers' Contributions to Student Learning Growth indicator alongside VAM as a measure of educator effectiveness."
            },
        ["SNAP eligibility"] =
            new DataElement
            {
                Name = "SNAP eligibility",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual meets income and other criteria for the Supplemental Nutrition Assistance Program. Used alongside SNAP participation to compute uptake rates for the Food Security indicator."
            },
        ["SNAP participation"] =
            new DataElement
            {
                Name = "SNAP participation",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual is actively receiving SNAP benefits. The Food Security indicator measures the percentage of eligible individuals participating in SNAP."
            },
        ["Social awareness teacher ratings"] =
            new DataElement
            {
                Name = "Social awareness teacher ratings",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-rated assessment of a student's social skills and awareness. Example instruments: Elliott and Gresham's Social Skills Rating Scale. Used in the Social Awareness indicator alongside student self-report surveys. This is a teacher-reported element, distinct from student self-reports."
            },
        ["Social capital surveys (K-12)"] =
            new DataElement
            {
                Name = "Social capital surveys (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student self-report of access to and ability to mobilize supportive relationships. Example instrument: Social Capital Assessment + Learning for Equity (SCALE) Social Capital, Network Diversity, and Network Strength scales."
            },
        ["Social capital surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Social capital surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student self-report of social capital at the PS level. Example instruments: SCALE, Social Capital Community Benchmark Survey."
            },
        ["Social capital surveys (Workforce)"] =
            new DataElement
            {
                Name = "Social capital surveys (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Individual self-report of social capital in the workforce context. Example instrument: Social Capital Community Benchmark Survey."
            },
        ["Social proficiency performance assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Social proficiency performance assessments (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Score or proficiency on an assessment of social skills at the PS level. Used in the Social Awareness indicator. Note: the indicator also references this element for PS/WF but the K-12 Social Awareness metric uses teacher ratings rather than performance assessments — confirm whether a K-12 performance assessment version is also needed."
            },
        ["Social proficiency performance assessments (Workforce)"] =
            new DataElement
            {
                Name = "Social proficiency performance assessments (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Score or proficiency on a social skills performance assessment in the workforce context. Example instrument: National Work Readiness Credential Essential Soft Skills assessment."
            },
        ["Social services offered"] =
            new DataElement
            {
                Name = "Social services offered",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator of whether social services (e.g., social work, case management) are available through a program or institution. Used in the Access to Health, Mental Health, and Social Supports indicator. Program/institution-level offering flag."
            },
        ["Sociocultural observational assessments"] =
            new DataElement
            {
                Name = "Sociocultural observational assessments",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Results of an observational assessment measuring equitable sociocultural interactions. Example instrument: Assessing Classroom Sociocultural Equity Scale (ACSES). Used in the Inclusive Environments indicator to measure whether classroom interactions are equitable. The WF sector tag appears broad — confirm intended use."
            },
        ["Staff FTE status"] =
            new DataElement
            {
                Name = "Staff FTE status",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Full-time equivalent status of a staff member. Used in multiple indicators: Teacher Credentials (percentage of courses taught by FTE teachers, excluding substitutes and emergency license holders), Access to College and Career Advising (student-to-FTE-counselor ratio), and Access to Health, Mental Health, and Social Supports (student-to-FTE-staff ratio)."
            },
        ["Staff race/ethnicity"] =
            new DataElement
            {
                Name = "Staff race/ethnicity",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Race and ethnicity of educational staff. Used in the Representational Racial and Ethnic Diversity of Educators indicator to compare educator composition to student composition. Distinct from Employee race/ethnicity, which applies in the workforce context."
            },
        ["State standardized test (Math proficiency)"] =
            new DataElement
            {
                Name = "State standardized test (Math proficiency)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student score or proficiency level on the state's standardized math assessment. Used across grade-band proficiency indicators (grade 3, grade 8, high school). Proficiency is defined by the state's cut score. Distinct from Math proficiency (Grades 1 and 2), which uses different early-grade assessment instruments."
            },
        ["State standardized test (Reading proficiency)"] =
            new DataElement
            {
                Name = "State standardized test (Reading proficiency)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student score or proficiency level on the state's standardized reading/ELA assessment. Used across grade-band proficiency indicators (grade 3, grade 8, high school). Distinct from Reading proficiency (Grades 1 and 2), which uses different early-grade assessment instruments."
            },
        ["Student attendance rate (K-12)"] =
            new DataElement
            {
                Name = "Student attendance rate (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Percentage of enrolled days a K-12 student was present. The Consistent Attendance indicator defines the target as more than 90% of enrolled days (excluding students enrolled fewer than 90 days). Also used as a criterion in the 6th, 8th, and 9th grade on-track indicators (96% threshold for 8th and 9th grade)."
            },
        ["Student attendance rate (PK)"] =
            new DataElement
            {
                Name = "Student attendance rate (PK)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Percentage of enrolled days a pre-K student was present. Used in the Consistent Attendance indicator (90% threshold). Note: this element is also listed as a data element for the Early Grades On Track, 6th Grade On Track, 8th Grade On Track, and 9th Grade On Track K-12 indicators — verify whether \"Student attendance rate (PK)\" is the correct element name for those references or if \"Student attendance rate (K-12)\" was intended."
            },
        ["Student course enrollment record"] =
            new DataElement
            {
                Name = "Student course enrollment record",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "A record of all courses a student is or was enrolled in. Serves as the core transcript element underlying multiple indicators: CTE pathway concentration, industry-recognized credential, early college access, equitable placement in rigorous coursework, and work-based learning participation. Consider whether this should be split into K-12 and PS versions, as the underlying data systems and record structures typically differ."
            },
        ["Student from migrant family household"] =
            new DataElement
            {
                Name = "Student from migrant family household",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator of whether a student is from a migratory agricultural or fishing family, as defined under Title I Part C of ESEA. Used as a demographic disaggregate. This has a specific federal definition — confirm whether the data element captures the formal Migrant Education Program (MEP) designation or a broader migrant/immigrant status."
            },
        ["Student FTE status"] =
            new DataElement
            {
                Name = "Student FTE status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Full-time equivalent enrollment status of a student. Used as a weighting factor for per-student expenditure calculations. Likely overlaps with Postsecondary enrollment status (Full time/part time) for the PS context — confirm whether both are needed or if the PS-specific element is sufficient."
            },
        ["Student grade level"] =
            new DataElement
            {
                Name = "Student grade level",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "The current grade level of a K-12 student. Appears to be duplicative of Student grade level (K-12) — confirm whether both are needed or if one should be deprecated."
            },
        ["Student grade level (K-12)"] =
            new DataElement
            {
                Name = "Student grade level (K-12)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "The current grade level of a K-12 student (K through 12). Used across many indicators to filter to specific grade cohorts (e.g., grade 12 for FAFSA completion, grades 11-12 for SAT/ACT). See also: Student grade level — appears duplicative."
            },
        ["Student grade level (PK)"] =
            new DataElement
            {
                Name = "Student grade level (PK)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "The program level or age group of a pre-K student (e.g., infant/toddler, preschool age 3, preschool age 4)."
            },
        ["Student or family socioeconomic status"] =
            new DataElement
            {
                Name = "Student or family socioeconomic status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Composite or categorical indicator of a student or family's socioeconomic status. Overlaps with Student socioeconomic status and Income level — these three elements likely capture the same concept at varying granularities. Consider consolidating and documenting what specifically each captures (e.g., free/reduced lunch eligibility, Pell receipt, income quintile, economic disadvantage flag)."
            },
        ["Student parenting status"] =
            new DataElement
            {
                Name = "Student parenting status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator of whether a student is a parent or primary caregiver for a dependent child. Used as a demographic disaggregate given the known barriers parenting students face in persisting and completing education."
            },
        ["Student race/ethnicity"] =
            new DataElement
            {
                Name = "Student race/ethnicity",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Race and ethnicity of a student. Used across virtually all indicators as a primary disaggregate. Also used as a direct input in Representational Racial and Ethnic Diversity of Educators, School and Workplace Racial and Ethnic Diversity, and Neighborhood Racial Diversity indicators."
            },
        ["Student socioeconomic status"] =
            new DataElement
            {
                Name = "Student socioeconomic status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator of a student's socioeconomic status. Appears to overlap with Student or family socioeconomic status. The indicator that references this element (School and Workplace Socioeconomic Diversity) is noted as PS-metric only in the source data, though the RelatedSectors here includes PK and K12. Review for consolidation with Student or family socioeconomic status and Income level."
            },
        ["Subscale observation scores"] =
            new DataElement
            {
                Name = "Subscale observation scores",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Domain or subscale scores from a classroom observation rubric. In the PK context, example subscales include CLASS domains (Emotional Support, Classroom Organization, Instructional Support) or ECERS Interactions subscale. In K-12, refers to domain scores from frameworks like Danielson or Marzano. Used alongside Overall teacher observation score in the Classroom Observations of Instructional Practice indicator."
            },
        ["Suspensions and expulsions (K-12)"] =
            new DataElement
            {
                Name = "Suspensions and expulsions (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Indicator or count of in-school suspensions, out-of-school suspensions, and expulsions for a K-12 student. Used as a criterion in on-track indicators (must have zero suspensions/expulsions) and in the Positive Behavior and Equitable Discipline Practices indicators. Distinct from the Number of days suspended element — this captures whether any event occurred, while that element captures severity."
            },
        ["Suspensions and expulsions (PK)"] =
            new DataElement
            {
                Name = "Suspensions and expulsions (PK)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Indicator or count of suspensions and expulsions for a pre-K student. Used in the Positive Behavior indicator. Pre-K suspensions are of particular equity concern given federal guidance discouraging their use with young children."
            },
        ["Teacher effectiveness student surveys (K-12)"] =
            new DataElement
            {
                Name = "Teacher effectiveness student surveys (K-12)",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student-reported perceptions of their teacher's effectiveness. Example instruments: Panorama Student Survey (Pedagogical Effectiveness subscale), Tripod Student Survey, 5Essentials Survey (Ambitious Instruction and Supportive Environment domains). Used in the Student Perceptions of Teaching indicator."
            },
        ["Teacher effectiveness student surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Teacher effectiveness student surveys (Postsecondary)",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student-reported perceptions of whether college instructors implement effective teaching practices. Example instrument: National Survey of Student Engagement (NSSE). Used in the Student Perceptions of Teaching indicator."
            },
        ["Teacher qualification or certification type"] =
            new DataElement
            {
                Name = "Teacher qualification or certification type",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "The type of teaching qualification or state certification held by an educator (e.g., standard, emergency, provisional, subject-area endorsement). Used in the Teacher Credentials indicator. Distinct from Credential or certification type — both appear to capture teacher credentials; consider consolidating."
            },
        ["Teacher reports of executive function"] =
            new DataElement
            {
                Name = "Teacher reports of executive function",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Teacher-reported assessment of a pre-K child's executive function abilities. Example instrument: CBRS. Used in the Kindergarten Readiness: Approaches to Learning indicator. Distinct from Direct child assessments of executive function (which uses direct assessment tools like HTKS or MEFS)."
            },
        ["Teacher reports of social-emotional development"] =
            new DataElement
            {
                Name = "Teacher reports of social-emotional development",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Teacher-reported assessment of a pre-K child's social-emotional development. Example instruments: CBRS, DECA-P2. Used in the Kindergarten Readiness: Social-Emotional Development indicator."
            },
        ["Teacher-child interaction measure (K-12)"] =
            new DataElement
            {
                Name = "Teacher-child interaction measure (K-12)",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Observational measure of the quality of teacher-student interactions in K-12 classrooms. Example framework: ACSES (Assessing Classroom Sociocultural Equity Scale). Used in the Classroom Observations of Instructional Practice indicator. Distinct from the PK-specific version."
            },
        ["Teacher-child interaction measure (PK)"] =
            new DataElement
            {
                Name = "Teacher-child interaction measure (PK)",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Observational measure of the quality of teacher-child interactions in pre-K settings. Example instruments: CLASS (Classroom Assessment Scoring System), ECERS (Early Childhood Environment Rating Scale) Interactions subscale, ACSES. Used in the Classroom Observations of Instructional Practice indicator."
            },
        ["Teaching assignment"] =
            new DataElement
            {
                Name = "Teaching assignment",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "The subject area(s) and/or grade level(s) a teacher is assigned to teach. Used in the Teacher Credentials indicator to determine whether a teacher is certified to teach their assigned subject or grade level. The spirit of this element is \"qualification to teach a particular assigned course\" — a mismatch between assignment and certification is the key metric."
            },
        ["Total educational funding"] =
            new DataElement
            {
                Name = "Total educational funding",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Total state-level funding allocated to education. Used as the denominator in the Expenditures on Workforce Development Programs indicator (workforce development funding as a percentage of total educational funding). In other contexts, serves as a top-line funding figure."
            },
        ["Total net price of education plus interest"] =
            new DataElement
            {
                Name = "Total net price of education plus interest",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "The total out-of-pocket cost of postsecondary education, including accumulated loan interest. Used in the Minimum Economic Return indicator: earnings must exceed median HS graduate earnings in the state plus enough to recoup this total cost within 10 years of completion."
            },
        ["Transfer enrollment status"] =
            new DataElement
            {
                Name = "Transfer enrollment status",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Indicator of whether a student has transferred to a different postsecondary institution. Used in persistence metrics, where transfer to another institution (including upward transfer) counts as a positive outcome. Distinct from Transfer indicator or transfer student status — confirm whether both are needed or if one captures the same information."
            },
        ["Transfer indicator or transfer student status"] =
            new DataElement
            {
                Name = "Transfer indicator or transfer student status",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Flag indicating a student has transferred from one postsecondary institution to another, specifically upward transfer (from certificate to associate's, or associate's to bachelor's). Used in the Transfer indicator, which measures the percentage of students in shorter programs who transfer to longer programs within 150% of the original program's length."
            },
        ["Universal screening results"] =
            new DataElement
            {
                Name = "Universal screening results",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Results of a universal (school-wide or program-wide) mental health screening tool. For K-12, see the NCSSLE guide \"Mental Health Screening Tools for Grades K-12\" for example instruments. Used in the Mental and Emotional Well-Being indicator alongside Mental and emotional well-being assessments."
            },
        ["Urbanicity"] =
            new DataElement
            {
                Name = "Urbanicity",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Classification of a geographic area as urban, suburban, or rural (e.g., NCES locale codes). Used as a community-level contextual variable across multiple indicators."
            },
        ["USDA Food Access Research Atlas Access Level Flag"] =
            new DataElement
            {
                Name = "USDA Food Access Research Atlas Access Level Flag",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "A flag from the USDA Food Access Research Atlas indicating whether a census tract is classified as having low access to healthy food. Used in the Food Security indicator to measure the percentage of individuals living in a low-access census tract."
            },
        ["USDA Food Security Survey ratings"] =
            new DataElement
            {
                Name = "USDA Food Security Survey ratings",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Results of the USDA Food Security Survey Module, which classifies households as high, marginal, low, or very low food security. The Food Security indicator targets the percentage of individuals with high or marginal food security."
            },
        ["VAM for subject specific assessment"] =
            new DataElement
            {
                Name = "VAM for subject specific assessment",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Value-added model (VAM) score for a teacher or school on a specific subject assessment. Measures a teacher's estimated contribution to student learning on standardized tests, controlling for student background. Used in the Teachers' Contributions to Student Learning Growth indicator alongside SGP. VAM scores are derived measures; confirm how they are computed and stored."
            },
        ["Workforce development program participation"] =
            new DataElement
            {
                Name = "Workforce development program participation",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator that an individual has participated in a workforce development program. Used as a denominator context in the Industry-Recognized Credential indicator (percentage of program participants earning a credential). Distinct from Enrollment in workforce training program — confirm whether these represent different stages (enrollment vs. active participation) or are redundant."
            },
        ["Years in current position"] =
            new DataElement
            {
                Name = "Years in current position",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Number of years a staff member has served in their current role at their current school. Used in the Educator Retention indicator for school leaders: the target metric categorizes leaders by tenure (fewer than 2 years, 2-3 years, 4+ years). Also underlies teacher retention calculations alongside School assignment."
            },
        ["Years of teaching experience"] =
            new DataElement
            {
                Name = "Years of teaching experience",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Total years of teaching experience, regardless of school. Used in the Teacher Experience indicator, which categorizes teachers as having fewer than 1 year, 1-5 years, or 5+ years of experience. Distinct from Years in current position, which tracks tenure at a specific school."
            },
    };
}
