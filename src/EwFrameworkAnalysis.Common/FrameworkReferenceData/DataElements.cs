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
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Binary indicator of whether a student has taken the ACT. Used in conjunction with ACT score to measure both participation and college readiness. Indicator source targets grades 11-12."
            },
        ["ACT score"] =
            new DataElement
            {
                Name = "ACT score",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Composite or section ACT score. \"College-ready\" threshold is defined by ACT benchmark scores. Should be interpreted alongside ACT completion; a missing score may mean non-participation rather than low performance."
            },
        ["Age"] =
            new DataElement
            {
                Name = "Age",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "May be stored as date of birth or a calculated age at a defined point in time. In the PK context, age is particularly important for determining program eligibility (typically 3- and 4-year-olds) and interpreting developmental screening results."
            },
        ["AP, IB, or Dual Credit course designation"] =
            new DataElement
            {
                Name = "AP, IB, or Dual Credit course designation",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Indicates whether a course is designated as Advanced Placement (AP), International Baccalaureate (IB), or dual credit/dual enrollment. Used to identify access to and participation in early college coursework."
            },
        ["Apprenticeship program enrollment date"] =
            new DataElement
            {
                Name = "Apprenticeship program enrollment date",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Date a participant enrolled in a registered apprenticeship program. Used to identify successful career transitions following high school graduation; the relevant window closes October 31 of the graduation year."
            },
        ["Average cost of attendance"] =
            new DataElement
            {
                Name = "Average cost of attendance",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Institution-level average total cost of attendance (tuition, fees, housing, etc.). Used as an input to the Unmet Financial Need indicator: net price = cost of attendance minus grants, scholarships, and tuition waivers. This is an institutional aggregate, not a student-level value."
            },
        ["Average expected family contribution (EFC)"] =
            new DataElement
            {
                Name = "Average expected family contribution (EFC)",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Institution-level average EFC as calculated via FAFSA. Used alongside average cost of attendance and average financial aid to derive unmet financial need. This is an institutional aggregate. Note: EFC has been replaced by the Student Aid Index (SAI) under the FAFSA Simplification Act; confirm which term/value the source system uses."
            },
        ["Average financial aid amount (including grants, scholarships, and tuition waivers)"] =
            new DataElement
            {
                Name = "Average financial aid amount (including grants, scholarships, and tuition waivers)",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Institution-level average of all grant-type aid received (excludes loans). Used in computing net price for the Unmet Financial Need indicator. This is an institutional aggregate, not a student-level value."
            },
        ["Average pay in county or MSA"] =
            new DataElement
            {
                Name = "Average pay in county or MSA",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Community-level average wage at the county or MSA level, sourced from labor market data. Used in computing access to jobs paying a living wage relative to local cost of living. This is an area-level aggregate, not an individual earnings value."
            },
        ["Basic skills level"] =
            new DataElement
            {
                Name = "Basic skills level",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Assessment result indicating a worker's foundational reading, math, or language proficiency level. Relevant in the context of workforce training program eligibility and placement. The specific assessment instrument may vary by program (e.g., TABE, Accuplacer)."
            },
        ["Campus climate surveys (K-12)"] =
            new DataElement
            {
                Name = "Campus climate surveys (K-12)",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student-reported perceptions of physical, mental, and emotional safety at school. Example instruments: ED School Climate Surveys (EDSCLS), CORE Districts School Culture & Climate Survey (Sense of Safety subscale), Panorama Student Survey (School Safety subscale). Distinct from family engagement or teacher effectiveness surveys."
            },
        ["Campus climate surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Campus climate surveys (Postsecondary)",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student-reported perceptions of physical safety and freedom from harassment and discrimination on campus. Example instrument: National Survey of Student Engagement (NSSE)."
            },
        ["Campus safety incident reports (Postsecondary)"] =
            new DataElement
            {
                Name = "Campus safety incident reports (Postsecondary)",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Count of reported on-campus crimes, sourced from institutional reporting to the U.S. Department of Education's Campus Safety and Security Reporting System (Clery Act data). Used in the School Safety indicator as an administrative complement to campus climate surveys. Note that administrative records typically underreport victimization; anonymous survey data is recommended alongside this element for a more complete picture."
            },
        ["Census tract or neighborhood identifier"] =
            new DataElement
            {
                Name = "Census tract or neighborhood identifier",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Geographic identifier for an individual's place of residence, such as census tract, city, or county. Enables linking individual-level records to community-level data including crime rates, poverty concentration, and racial and ethnic composition."
            },
        ["CHIP eligibility status"] =
            new DataElement
            {
                Name = "CHIP eligibility status",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Indicator of whether a child meets income and residency requirements for the Children's Health Insurance Program. Used in the Health Insurance Coverage indicator alongside Medicaid eligibility and actual enrollment."
            },
        ["CHIP enrollment"] =
            new DataElement
            {
                Name = "CHIP enrollment",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Indicator of whether a child is actively enrolled in CHIP. The Health Insurance Coverage indicator measures the percentage of eligible individuals enrolled, so both eligibility and enrollment are needed to compute uptake rates."
            },
        ["City or county population"] =
            new DataElement
            {
                Name = "City or county population",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Total population of a city or county. Used as the denominator for community-level crime rate metrics (violent felonies per 100,000 residents; juvenile arrests per 100,000 residents). Sourced from census or similar area-level data."
            },
        ["Civic engagement surveys (K-12)"] =
            new DataElement
            {
                Name = "Civic engagement surveys (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student self-report of civic knowledge, values, and participation. Example instruments: Youth Civic and Character Measures Toolkit Survey, Youth Civic Engagement Indicators Project Survey."
            },
        ["Civic engagement surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Civic engagement surveys (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student self-report of civic engagement at the postsecondary level. Same construct as K-12 version; instrument may differ (e.g., Index of Civic and Political Engagement)."
            },
        ["Civic engagement surveys (Workforce)"] =
            new DataElement
            {
                Name = "Civic engagement surveys (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Individual self-report of civic engagement in the workforce context. Example instrument: Index of Civic and Political Engagement."
            },
        ["Cohort graduation year"] =
            new DataElement
            {
                Name = "Cohort graduation year",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "The expected on-time graduation year for a student's entering cohort. Used in the Adjusted Cohort Graduation Rate (ACGR) calculation — the primary metric for the High School Graduation indicator. Students are tracked against this year regardless of school transfers."
            },
        ["Cohort year"] =
            new DataElement
            {
                Name = "Cohort year",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "The year a student first entered a cohort (e.g., first-time 9th grade entry year). Used alongside cohort graduation year to compute on-time completion rates."
            },
        ["College value-added"] =
            new DataElement
            {
                Name = "College value-added",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Institution-level value-added measure representing a college's contribution to student outcomes (graduation rates, earnings, loan repayment) beyond what would be predicted by student characteristics. Derived metric computed using statistical models; not typically available as a raw data field."
            },
        ["Communication skills performance assessments (K-12)"] =
            new DataElement
            {
                Name = "Communication skills performance assessments (K-12)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Score or proficiency level on an assessment of oral, written, nonverbal, and listening skills. Example instrument: College and Career Readiness Assessment (CCRA+), available for grades 6–12."
            },
        ["Communication skills performance assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Communication skills performance assessments (Postsecondary)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Score or proficiency on communication assessments at the PS level. Example instruments: Collegiate Learning Assessment (CLA+), Success Skills Assessment (SSA+), HEIghten Outcomes Assessment for Written Communication."
            },
        ["Communication skills performance assessments (Workforce)"] =
            new DataElement
            {
                Name = "Communication skills performance assessments (Workforce)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Score or proficiency on workforce communication assessments. Example instrument: National Work Readiness Credential Essential Soft Skills assessment."
            },
        ["Commute time"] =
            new DataElement
            {
                Name = "Commute time",
                DataElementCategory = "Transportation",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Average commute time to work, school, or college. One of two metrics for the Access to Transportation indicator (the other being the Low Transportation Cost Index). May be individual-reported or area-level aggregate depending on data source."
            },
        ["Course offering by grade level"] =
            new DataElement
            {
                Name = "Course offering by grade level",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Indicates which grade levels a course is offered to. Used to measure access to rigorous coursework (e.g., Algebra I availability in middle school) and to verify the full suite of college preparatory courses is accessible at appropriate grade levels."
            },
        ["Course outcome"] =
            new DataElement
            {
                Name = "Course outcome",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Result of a student's enrollment in a course: completion, failure, or passage. Used across on-track indicators (6th, 8th, 9th grade) and college prep coursework completion. A passing outcome is typically required to count a course toward graduation or admissions requirements. Distinct from the course grade (GPA element)."
            },
        ["Course performance (English and Math)"] =
            new DataElement
            {
                Name = "Course performance (English and Math)",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Grade or pass/fail result specifically in English language arts and math courses. Used in the 6th and 8th grade on-track indicators, which require no Ds or Fs in ELA or math as a criterion."
            },
        ["Course subject area"] =
            new DataElement
            {
                Name = "Course subject area",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Standardized classification of a course by subject area, using a local or state taxonomy (e.g., SCED course codes). Used to identify course type across multiple indicators including college prep completion, CTE pathway concentration, gateway course completion, and early college access. Distinct from a local course name or section identifier."
            },
        ["Credential-seeking status"] =
            new DataElement
            {
                Name = "Credential-seeking status",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS, Sector.WF],
                AdditionalNotes = "Indicates whether a student or worker is enrolled with the intent to earn a credential (degree, certificate, or industry certification). Relevant to persistence and completion metrics; non-credential-seeking students are often excluded from graduation rate calculations."
            },
        ["CTE pathway or career cluster associated with CTE course"] =
            new DataElement
            {
                Name = "CTE pathway or career cluster associated with CTE course",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "The state-defined CTE pathway (e.g., Health Sciences, Information Technology) or federal career cluster associated with a CTE course. Essential for determining pathway concentration — a student must complete multiple courses within the same pathway to meet concentration criteria."
            },
        ["CTE program"] =
            new DataElement
            {
                Name = "CTE program",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Name or identifier of the CTE program in which a student is enrolled. Used to evaluate whether program offerings align to in-demand occupations, and to determine enrollment for industry-recognized credential and work-based learning metrics."
            },
        ["Cultural competency assessments (K-12)"] =
            new DataElement
            {
                Name = "Cultural competency assessments (K-12)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Score or proficiency level on an assessment measuring intercultural competency. No widely adopted K–12 specific instruments currently exist; adult tools may be adapted for use with youth. Example instruments (developed for PS/WF contexts): Intercultural Development Inventory (IDI), HEIghten Outcomes Assessment for Intercultural Competency & Diversity."
            },
        ["Cultural competency assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Cultural competency assessments (Postsecondary)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Score or proficiency level on an assessment measuring intercultural competency. Example instruments: Intercultural Development Inventory (IDI), HEIghten Outcomes Assessment for Intercultural Competency & Diversity."
            },
        ["Cultural competency assessments (Workforce)"] =
            new DataElement
            {
                Name = "Cultural competency assessments (Workforce)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Score or proficiency on intercultural competency assessments in the workforce context. Example instrument: Intercultural Development Inventory (IDI)."
            },
        ["Culturally responsive curriculum assessment"] =
            new DataElement
            {
                Name = "Culturally responsive curriculum assessment",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "A rating or evaluation of the degree to which a curriculum reflects the lived experiences, heritage, and cultural backgrounds of students from diverse ethnic and racial groups. Typically survey-derived or rubric-based; no universal standardized tool exists across sectors. Available tools include the Culturally Responsive Curriculum Scorecards and Mathematica's math-specific review tool. Ratings are subjective and may vary across raters. Systematic data collection in this area is limited; many institutions may not yet have a formal assessment on record."
            },
        ["Curriculum adoption"] =
            new DataElement
            {
                Name = "Curriculum adoption",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Records which curriculum materials have been formally adopted for use at the school, district, or institution level. Serves as the foundational linking record for curriculum quality and cultural responsiveness assessments — ratings and assessments cannot be meaningfully applied without first establishing which curricula are in use. Administrative tracking of curriculum adoption is not yet systematic in most pre-K, K–12, or postsecondary contexts; data collection in this area is emerging."
            },
        ["Curriculum quality rating"] =
            new DataElement
            {
                Name = "Curriculum quality rating",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PK, Sector.PS],
                AdditionalNotes = "A rating or classification of a curriculum's alignment to academic standards, usability, and coherence, as assessed by an external review tool such as EdReports (K–12) or equivalent frameworks for pre-K and postsecondary contexts. Used to determine whether curricula in use meet quality benchmarks. Ratings are typically assigned at the curriculum material level rather than the school or classroom level, and must be linked to curriculum adoption records to assess coverage across programs or institutions. No universal standard exists across sectors; rating tools and criteria vary."
            },
        ["Date of early intervention services provided"] =
            new DataElement
            {
                Name = "Date of early intervention services provided",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "The date on which early intervention services were provided to a child following screening and referral. Serves as confirmation that services were delivered (presence of date indicates service provision; absence indicates services were not provided). Used to assess timeliness of connection to services following identification of developmental, sensory, or behavioral concerns. Data is typically held across fragmented systems (state Pre-K programs, Head Start, pediatric records) and may not be consistently available through a single administrative source; survey data may be required to supplement."
            },
        ["Developmental screening results (PK)"] =
            new DataElement
            {
                Name = "Developmental screening results (PK)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Results of a standardized developmental screening tool used to identify developmental, sensory, or behavioral concerns in children under age 5. Example instruments are listed in the Head Start Early Childhood Learning and Knowledge Center guide \"Birth to 5: Watch Me Thrive!\" Used in both the Mental and Emotional Well-Being indicator and the Access to Early Intervention Screening indicator."
            },
        ["Digital skills assessments (K-12)"] =
            new DataElement
            {
                Name = "Digital skills assessments (K-12)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Score or proficiency level on an assessment of digital literacy and technology skills. Example instrument: Problem Solving in Technology-Rich Environments (PS-TRE) assessment within the Education & Skills Online suite, based on OECD PIAAC domains."
            },
        ["Digital skills assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Digital skills assessments (Postsecondary)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Score or proficiency on digital skills assessments at the PS level. Same construct and instrument family as K-12 version (OECD PIAAC-based tools)."
            },
        ["Digital skills assessments (Workforce)"] =
            new DataElement
            {
                Name = "Digital skills assessments (Workforce)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Score or proficiency on digital skills assessments in the workforce context. Measures ability to use digital tools for accessing, managing, evaluating, and communicating information required for workforce success."
            },
        ["Direct child assessments (cognition)"] =
            new DataElement
            {
                Name = "Direct child assessments (cognition)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Results of a direct (assessor-administered) assessment of a child's math and scientific reasoning skills. Example instruments: Woodcock-Johnson IV Tests of ECAD (Number Sense subtest), IGDIs Early Numeracy, Research Based Early Mathematics Assessment (REMA). Distinct from teacher-reported readiness assessments."
            },
        ["Direct child assessments (executive function)"] =
            new DataElement
            {
                Name = "Direct child assessments (executive function)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Results of a direct assessment of a child's cognitive self-regulation and executive function. Example instruments: Heads Toes Knees Shoulders (HTKS) task (teacher-administered), Minnesota Executive Function Scale (MEFS, tablet-based). Supports the Kindergarten Readiness: Approaches to Learning indicator."
            },
        ["Direct child assessments (language and literacy)"] =
            new DataElement
            {
                Name = "Direct child assessments (language and literacy)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Results of a direct (assessor-administered) assessment of a child's language and literacy skills. Example instruments: Woodcock-Johnson IV Tests of ECAD (Letter-Word and Writing subtests), IGDIs Early Literacy. Distinct from teacher-reported readiness assessments."
            },
        ["Direct child assessments (physical development)"] =
            new DataElement
            {
                Name = "Direct child assessments (physical development)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Results of a direct assessment of a child's gross and fine motor skills and physical development. Example instrument: Peabody Developmental Motor Scale. May be administered by teachers, healthcare professionals, or other qualified adults."
            },
        ["Disability status"] =
            new DataElement
            {
                Name = "Disability status",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual has a documented disability. Used as a demographic disaggregate across sectors. In K-12, this typically corresponds to students with an IEP or 504 plan. Definition and categorization may differ across sectors and data systems."
            },
        ["Dislocated worker status"] =
            new DataElement
            {
                Name = "Dislocated worker status",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator of whether a worker has been laid off or displaced from their job and is unlikely to return to their previous industry. Relevant for workforce training program eligibility under WIOA and other federal programs."
            },
        ["EAP or mental health services provided"] =
            new DataElement
            {
                Name = "EAP or mental health services provided",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator of whether an employer offers an Employee Assistance Program (EAP) or mental health access through health care plans or other services. Used in the Access to Health, Mental Health, and Social Supports indicator. This is an employer-level offering flag, not an individual utilization record."
            },
        ["Early intervention screening results"] =
            new DataElement
            {
                Name = "Early intervention screening results",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Results of a screening process used to identify developmental, sensory, and behavioral concerns in young children. Distinct from Developmental screening results, which capture scores from standardized instruments; this element records the outcome determination of the screening process (e.g., concern identified or not). See also: Early intervention screening services referral status, which captures whether a referral was subsequently made."
            },
        ["Early intervention screening services referral status"] =
            new DataElement
            {
                Name = "Early intervention screening services referral status",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Indicates whether a child was referred to early intervention services following a screening. The Access to Early Intervention Screening indicator measures the percentage of children with identified concerns who were connected to services, making this element essential for computing uptake rates."
            },
        ["Earnings"] =
            new DataElement
            {
                Name = "Earnings",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Individual annual or quarterly earnings. Used across multiple indicators: Successful Career Transition After High School (threshold: $35,000/year, the median for HS graduates), Minimum Economic Return (earnings must exceed median HS graduate earnings plus education cost recovery), and Economic Mobility (earnings must reach the 4th income quintile). The applicable threshold varies by indicator — ensure the data element captures sufficient precision (annual vs. quarterly) and time reference."
            },
        ["Educator tenure in current position"] =
            new DataElement
            {
                Name = "Educator tenure in current position",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Number of years a staff member has served in their current role at their current school or institution. Resets when a staff member changes schools or roles. Used to measure teacher retention (year-over-year return to the same school) and to categorize school leaders by tenure length (fewer than 2 years, 2–3 years, 4+ years)."
            },
        ["Eligibility for federal rental assistance"] =
            new DataElement
            {
                Name = "Eligibility for federal rental assistance",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether a household meets income requirements for federal rental assistance programs (e.g., Section 8/Housing Choice Voucher). Used alongside receipt of assistance to compute uptake rates for the Access to Affordable Housing indicator."
            },
        ["Employee learning and development plan offered"] =
            new DataElement
            {
                Name = "Employee learning and development plan offered",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator of whether an employer provides a professional learning and development plan for employees. One of two components (alongside on-the-job training) in the Access to Ongoing Career Skills Development indicator. This is an employer-level offering flag."
            },
        ["Employment date"] =
            new DataElement
            {
                Name = "Employment date",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Date an individual began employment. In the Successful Career Transition After High School indicator, the relevant window is before October 31 following graduation. Also used to measure time-to-employment after completing education."
            },
        ["Employment status"] =
            new DataElement
            {
                Name = "Employment status",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Current employment status of an individual (e.g., employed full-time, employed part-time, unemployed, not in labor force). Used in the Employment in a Quality Job indicator. Note that this element captures whether someone is employed; job quality attributes (benefits, schedule, pay) are captured in separate elements."
            },
        ["End-of-course exam participation"] =
            new DataElement
            {
                Name = "End-of-course exam participation",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Indicates whether a student participated in an end-of-course exam, such as an AP or IB qualifying exam, following enrollment in a corresponding course. Captures exam-taking as a distinct act from course enrollment or outcome, enabling measurement of access to and uptake of credit-bearing assessment opportunities."
            },
        ["English learner classification date"] =
            new DataElement
            {
                Name = "English learner classification date",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "The date a student was first classified as an English learner. Used in the English Learner Progress indicator to measure time to reclassification — the target is reclassification within five years of initial EL classification."
            },
        ["English learner status"] =
            new DataElement
            {
                Name = "English learner status",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Current classification of a student as an English learner or reclassified fluent English proficient (RFEP). Used alongside English learner classification date to compute reclassification rates and as a demographic disaggregate across indicators."
            },
        ["Enlistment in the military"] =
            new DataElement
            {
                Name = "Enlistment in the military",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator that a high school graduate enlisted in the military. One of several acceptable post-graduation pathways counted in the Successful Career Transition After High School indicator (alongside employment, apprenticeship, and noncredit CTE enrollment), with the relevant window being before October 31 following graduation."
            },
        ["Enrollment in noncredit CTE date"] =
            new DataElement
            {
                Name = "Enrollment in noncredit CTE date",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.PS, Sector.WF],
                AdditionalNotes = "Date a student enrolled in a noncredit CTE course at a postsecondary institution. In the Successful Career Transition After High School indicator, this is one acceptable post-graduation pathway (before October 31 following graduation)."
            },
        ["Expenditures per student (K-12)"] =
            new DataElement
            {
                Name = "Expenditures per student (K-12)",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Per-pupil expenditure at the K-12 level. The Expenditures per Student indicator references both total per-pupil expenditure and equity measures such as the New America Equity Factor (variance in per-pupil funding within a state). This is a school or district-level aggregate, not a student-level value."
            },
        ["Expenditures per student (PK)"] =
            new DataElement
            {
                Name = "Expenditures per student (PK)",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "State expenditures per child enrolled in public pre-K. Program-level aggregate used in the Expenditures per Student indicator."
            },
        ["Expenditures per student (Postsecondary)"] =
            new DataElement
            {
                Name = "Expenditures per student (Postsecondary)",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Total instruction and student service expenditures per FTE student based on 12-month enrollment, as reported to IPEDS. Institution-level aggregate."
            },
        ["FAFSA completion date"] =
            new DataElement
            {
                Name = "FAFSA completion date",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Date a student completed the FAFSA. The FAFSA Completion indicator measures percentage of grade 12 students completing by June 30. Timeliness of completion matters because it affects financial aid eligibility; the date field enables computing whether completion occurred within the relevant window."
            },
        ["Family eligibility for child care subsidies"] =
            new DataElement
            {
                Name = "Family eligibility for child care subsidies",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Indicator of whether a family meets income and other criteria to receive child care subsidy assistance. Used alongside receipt of subsidies to compute uptake rates for the Access to Child Care Subsidies indicator."
            },
        ["Family engagement surveys (K-12)"] =
            new DataElement
            {
                Name = "Family engagement surveys (K-12)",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Family-reported perceptions of the quality of their relationship with the school. Example instruments: Panorama Family-School Relationships Survey, CORE Districts School Culture & Climate Survey (parent community engagement assessment). Used in the School-Family Engagement indicator."
            },
        ["Family engagement surveys (PK)"] =
            new DataElement
            {
                Name = "Family engagement surveys (PK)",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Family and caregiver-reported perceptions of relationship quality with the pre-K program. Example instrument: Family and Provider/Teacher Relationship Quality (FPTRQ) parent survey."
            },
        ["First-generation college student"] =
            new DataElement
            {
                Name = "First-generation college student",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Indicator that neither of a student's parents completed a bachelor's degree. Used as a demographic disaggregate and equity lens across postsecondary access, persistence, and completion indicators. May be derivable from Parental education level if that element is available in the same system."
            },
        ["Food access level by census tract"] =
            new DataElement
            {
                Name = "Food access level by census tract",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "A flag from the USDA Food Access Research Atlas indicating whether a census tract is classified as having low access to healthy food. Used in the Food Security indicator to measure the percentage of individuals living in a low-access census tract."
            },
        ["Food security survey rating"] =
            new DataElement
            {
                Name = "Food security survey rating",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Results of the USDA Food Security Survey Module, which classifies households as high, marginal, low, or very low food security. The Food Security indicator targets the percentage of individuals with high or marginal food security."
            },
        ["Funding dedicated to workforce development programs"] =
            new DataElement
            {
                Name = "Funding dedicated to workforce development programs",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Dollar amount of government funding allocated to workforce development programs (apprenticeships, job training, etc.) in a state. Used as the numerator in the Expenditures on Workforce Development Programs indicator; the denominator is Total educational funding."
            },
        ["Gender"] =
            new DataElement
            {
                Name = "Gender",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Gender identity of an individual. Used as a demographic disaggregate across all sectors. Note that data systems may capture this as biological sex, self-reported gender, or both; confirm what is available in each source system."
            },
        ["Gifted and talented participation"] =
            new DataElement
            {
                Name = "Gifted and talented participation",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Indicator of whether a student participates in a gifted and talented program. Used in the Equitable Placement in Rigorous Coursework indicator to measure whether demographic subgroups are proportionally represented in gifted programs."
            },
        ["Grade point average (K-12)"] =
            new DataElement
            {
                Name = "Grade point average (K-12)",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Cumulative GPA for K-12 students. Used across multiple on-track indicators: 8th grade on track (GPA ≥ 2.5), 9th grade on track (GPA ≥ 3.0), and the general Grade Point Average indicator (GPA ≥ 3.0 for grades 6-12). The GPA threshold differs by grade band — ensure the element captures GPA at the applicable point in time."
            },
        ["Grade point average (Postsecondary)"] =
            new DataElement
            {
                Name = "Grade point average (Postsecondary)",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Cumulative GPA for postsecondary students. The Grade Point Average indicator targets a GPA of 3.0 or higher as sufficient to graduate and obtain employment."
            },
        ["Growth mindset surveys (K-12)"] =
            new DataElement
            {
                Name = "Growth mindset surveys (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student self-report of belief that abilities can grow with effort. Example instruments: CORE Districts SEL Survey Growth Mindset Scale (grades 5-12), Growth Mindset Scale developed by Carol Dweck."
            },
        ["Growth mindset surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Growth mindset surveys (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student self-report of growth mindset at the PS level. Example instrument: Growth Mindset Scale developed by Carol Dweck."
            },
        ["Growth mindset surveys (Workforce)"] =
            new DataElement
            {
                Name = "Growth mindset surveys (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Individual self-report of growth mindset in the workforce context. Same construct as K-12/PS versions; instrument may differ."
            },
        ["Health insurance coverage status"] =
            new DataElement
            {
                Name = "Health insurance coverage status",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual has any form of health insurance coverage. Used as the overall health insurance indicator, with Medicaid enrollment and CHIP enrollment as more specific sub-elements. The Health Insurance Coverage metric uses all three."
            },
        ["Health services offered"] =
            new DataElement
            {
                Name = "Health services offered",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator of whether health services are offered by a program or institution. Part of the Access to Health, Mental Health, and Social Supports indicator, which measures the percentage of programs offering these services and the staff ratio. This is a program/institution-level offering flag."
            },
        ["Health-Related Quality of Life Scale scores"] =
            new DataElement
            {
                Name = "Health-Related Quality of Life Scale scores",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PS, Sector.WF],
                AdditionalNotes = "Score on the Health-Related Quality of Life (HRQoL) Scale, a self-rated measure of physical and mental health. Used in the Physical Development and Well-Being indicator for PS and WF populations. Distinct from the Self-Rated Health scale (which is a single-item measure also referenced in the indicator)."
            },
        ["High school diploma type"] =
            new DataElement
            {
                Name = "High school diploma type",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Classification of the diploma earned (e.g., regular, honors, IEP/alternative, GED). The ACGR metric used in the High School Graduation indicator counts only students who earn a regular diploma — alternative credentials are explicitly excluded. This element is therefore essential for correctly computing ACGR."
            },
        ["High school graduation date"] =
            new DataElement
            {
                Name = "High school graduation date",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Date a student received their high school diploma. Used as the starting reference point for post-graduation transition indicators: postsecondary enrollment must occur by October 31 following graduation; career transition outcomes (employment, apprenticeship, military, noncredit CTE) must also be achieved before that date."
            },
        ["High school graduation indicator"] =
            new DataElement
            {
                Name = "High school graduation indicator",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Binary flag indicating whether a student has graduated from high school with a regular diploma. Used across multiple transition indicators (college applications, FAFSA completion, postsecondary enrollment, well-matched institution selection) as a prerequisite filter. Closely related to High school graduation date and High school diploma type — all three may be needed together."
            },
        ["Higher-order thinking skills performance assessments (K-12)"] =
            new DataElement
            {
                Name = "Higher-order thinking skills performance assessments (K-12)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Score or proficiency on assessments of critical thinking, problem solving, and decision-making. Example instrument: College and Career Readiness Assessment (CLA+), designed for grades 6–12."
            },
        ["Higher-order thinking skills performance assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Higher-order thinking skills performance assessments (Postsecondary)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Score or proficiency on higher-order thinking assessments at the PS level. Example instruments: CLA+, Success Skills Assessment (SSA+), HEIghten Outcomes Assessment for Critical Thinking."
            },
        ["Higher-order thinking skills performance assessments (Workforce)"] =
            new DataElement
            {
                Name = "Higher-order thinking skills performance assessments (Workforce)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Score or proficiency on higher-order thinking assessments in the workforce context. Example instrument: Watson Glaser Critical Thinking Appraisal (scenario-based, used by employers for candidate evaluation)."
            },
        ["Highest level of education completed (staff/educator)"] =
            new DataElement
            {
                Name = "Highest level of education completed (staff/educator)",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Highest educational credential earned by a staff member or educator. The Teacher Credentials indicator measures the percentage of pre-K lead teachers with at least a bachelor's degree."
            },
        ["Home language"] =
            new DataElement
            {
                Name = "Home language",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "The primary language spoken in a student's home. Used alongside English learner status and classification date to track EL progress. Also used as a demographic disaggregate. The home language survey is typically the first step in EL identification."
            },
        ["IECMHC services offered"] =
            new DataElement
            {
                Name = "IECMHC services offered",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Indicator of whether Infant and Early Childhood Mental Health Consultation (IECMHC) services are available through a pre-K program. Used in the Access to Health, Mental Health, and Social Supports indicator. IECMHC is a specific evidence-based model for supporting social-emotional development in early childhood settings."
            },
        ["Income level (individual/family)"] =
            new DataElement
            {
                Name = "Income level (individual/family)",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Income classification of an individual or family, used as a demographic disaggregate across sectors. May be captured as categorical income bands, a poverty level threshold, or derived from program eligibility (e.g., free/reduced lunch, Pell Grant). Confirm which form is available in each source system, as granularity varies."
            },
        ["Indicator of access to desktop or laptop at home"] =
            new DataElement
            {
                Name = "Indicator of access to desktop or laptop at home",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Binary indicator of whether someone in the household owns at least one desktop or laptop computer. The Access to Technology indicator requires both this element AND reliable broadband access — neither alone is sufficient to count as having technology access."
            },
        ["Indicator of access to reliable broadband internet"] =
            new DataElement
            {
                Name = "Indicator of access to reliable broadband internet",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Binary indicator of whether a household has access to reliable broadband internet. Must be used in conjunction with Indicator of access to desktop or laptop at home to compute the Access to Technology indicator metric."
            },
        ["Individual with current or past child welfare involvement"] =
            new DataElement
            {
                Name = "Individual with current or past child welfare involvement",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator of whether a student is or has been involved with the child welfare system (e.g., foster care, child protective services). Used as an equity disaggregate given the well-documented educational disadvantages faced by students with child welfare involvement. Sensitive data requiring careful data governance."
            },
        ["Individuals experiencing homelessness"] =
            new DataElement
            {
                Name = "Individuals experiencing homelessness",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual lacks fixed, regular, and adequate nighttime residence. In K-12, typically identified under the McKinney-Vento Act. Used as a demographic disaggregate; also affects housing stability indicators."
            },
        ["Industry-recognized credential indicator"] =
            new DataElement
            {
                Name = "Industry-recognized credential indicator",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator that an individual has earned at least one industry-recognized credential as defined by their state. The Industry-Recognized Credential indicator applies across K-12 (12th-grade CTE students), PS (CTE program students), and WF (training program participants). The specific credentials that qualify are state-defined."
            },
        ["Institution graduation rate"] =
            new DataElement
            {
                Name = "Institution graduation rate",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "The overall graduation rate of a postsecondary institution, disaggregated by race, ethnicity, and income (Pell receipt). Used in the Selection of a Well-Matched Institution indicator: students are compared against institutions within 10 percentage points of the best-matched institution's graduation rate for similar students."
            },
        ["Job quality index"] =
            new DataElement
            {
                Name = "Job quality index",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "A composite or indexed score measuring overall job quality across dimensions such as pay, benefits, scheduling, career advancement, safety, and job security. Example instrument: Good Jobs Scorecard. This is a derived/composite measure rather than a raw data field — confirm how it is operationalized in source systems."
            },
        ["Justice involvement"] =
            new DataElement
            {
                Name = "Justice involvement",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual has had involvement with the criminal justice system (arrest, conviction, incarceration). Used as an equity disaggregate. Sensitive data requiring careful data governance and legal review for use in educational and workforce contexts."
            },
        ["K-12 school type"] =
            new DataElement
            {
                Name = "K-12 school type",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Classification of a K-12 school by governance or program type (e.g., traditional public, charter, magnet, private). Used as a disaggregate to contextualize per-pupil expenditure and resource comparisons across school types."
            },
        ["Kindergarten enrollment date"] =
            new DataElement
            {
                Name = "Kindergarten enrollment date",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Date a child first enrolled in kindergarten. Used as a reference point to determine whether a child attended a pre-K program before entering K (linked to Pre-K eligibility status and Enrollment in public pre-K elements) and for calculating whether early intervention services were connected before kindergarten entry."
            },
        ["Kindergarten program schedule"] =
            new DataElement
            {
                Name = "Kindergarten program schedule",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Records the number of days per week and hours per day of a kindergarten program. Used to determine whether a program meets the full-day threshold of six hours per day, five days per week. Typically stored at the school or district level."
            },
        ["LGBT status"] =
            new DataElement
            {
                Name = "LGBT status",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual identifies as lesbian, gay, bisexual, or transgender. Used as an equity disaggregate. Sensitive data requiring careful data governance; collection methods and availability vary widely across data systems."
            },
        ["Location-adjusted cost of living in county or MSA"] =
            new DataElement
            {
                Name = "Location-adjusted cost of living in county or MSA",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Area-level cost of living index adjusted for geographic location, at the county or MSA level. Used as the denominator in the ratio with average pay to determine whether jobs in an area pay a living wage."
            },
        ["Low Transportation Cost Index"] =
            new DataElement
            {
                Name = "Low Transportation Cost Index",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "The Low Transportation Cost Index from the U.S. Department of Housing and Urban Development. An area-level measure of transportation affordability. One of two metrics for the Access to Transportation indicator (the other being average commute time)."
            },
        ["Math proficiency (Grades 1 and 2)"] =
            new DataElement
            {
                Name = "Math proficiency (Grades 1 and 2)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Assessment of math skills for students in grades 1 and 2, used in the Early Grades On Track indicator. The indicator requires meeting grade-level benchmarks in math alongside reading proficiency and attendance/discipline criteria. This element captures early-grade progress before state standardized testing begins. Distinct from State standardized test (Math proficiency), which typically applies from grade 3 onward."
            },
        ["Medicaid eligibility status"] =
            new DataElement
            {
                Name = "Medicaid eligibility status",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Indicator of whether an individual meets income and other criteria for Medicaid enrollment. Used alongside Medicaid enrollment to compute uptake rates for the Health Insurance Coverage indicator."
            },
        ["Medicaid enrollment"] =
            new DataElement
            {
                Name = "Medicaid enrollment",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Indicator of whether an individual is actively enrolled in Medicaid. Used in the Health Insurance Coverage indicator; the percentage of eligible individuals enrolled is one of the key metrics."
            },
        ["Mental and emotional well-being assessments"] =
            new DataElement
            {
                Name = "Mental and emotional well-being assessments",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PS, Sector.WF],
                AdditionalNotes = "Self-report survey measure of mental and emotional well-being among postsecondary and workforce populations. Used to calculate the percentage of individuals reporting a high level of mental and emotional well-being. Distinct from the screening-based approaches used in PK (developmental screening) and K-12 (universal mental health screening); survey instruments are voluntary and confidential. Specific instruments vary; selection should prioritize tools with an established evidence base."
            },
        ["Mental health services offered"] =
            new DataElement
            {
                Name = "Mental health services offered",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator of whether mental health services are available through a program or institution. Used in the Access to Health, Mental Health, and Social Supports indicator. This is a program/institution-level offering flag. The WF version of this concept is captured under EAP or mental health services provided."
            },
        ["Meta-major classifications"] =
            new DataElement
            {
                Name = "Meta-major classifications",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "A mapping of courses or programs of study to broad interdisciplinary groupings used to determine whether a student's first-year course-taking is concentrated within a single area of study. Meta-major definitions are institution-defined and derived from course data and degree requirements rather than stored as a native administrative field; this element functions as a reference crosswalk joined to course enrollment records at the time of metric calculation. Institutions may reference the Nguyen et al. methodology or the National Student Clearinghouse Postsecondary Data Partnership for consistent classification guidance. Many institutions will need to define and maintain this mapping before this metric can be calculated."
            },
        ["Military status (individual/family)"] =
            new DataElement
            {
                Name = "Military status (individual/family)",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual is active duty military, a veteran, or a dependent of a military family. Used as a demographic disaggregate and may affect eligibility for certain support programs."
            },
        ["Neighborhood exposure index"] =
            new DataElement
            {
                Name = "Neighborhood exposure index",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "A derived measure of neighborhood racial and ethnic diversity from the perspective of an individual resident, calculated as the percentage of neighbors belonging to racial or ethnic groups other than the individual's own. Computed by linking individual race/ethnicity records to census tract or neighborhood demographic data. Sourced from Census or ACS data."
            },
        ["Net worth"] =
            new DataElement
            {
                Name = "Net worth",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PS, Sector.WF],
                AdditionalNotes = "Total assets minus total liabilities for an individual. Used in the Economic Security indicator, which measures whether individuals reach median wealth levels 10, 15, 20, and 30 years after completing education. This is a difficult-to-capture individual-level measure; confirm whether administrative sources can provide this or if survey data is required."
            },
        ["Number of Adverse Childhood Experiences (ACEs)"] =
            new DataElement
            {
                Name = "Number of Adverse Childhood Experiences (ACEs)",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Count of adverse childhood experiences (ACEs) reported by or on behalf of an individual. ACEs include household dysfunction, abuse, neglect, and other traumatic events. The Childhood Experiences indicator targets fewer than three ACEs. Typically collected via survey. Sensitive data requiring careful data governance."
            },
        ["Number of affordable housing units in city or county"] =
            new DataElement
            {
                Name = "Number of affordable housing units in city or county",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Count of housing units in an area where monthly costs do not exceed 30% of a household's income. Used as the numerator in the Access to Affordable Housing indicator; the denominator is the number of households with low (below 80% AMI) and very low (below 50% AMI) incomes."
            },
        ["Number of city or county residents experiencing poverty"] =
            new DataElement
            {
                Name = "Number of city or county residents experiencing poverty",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Count of residents in a city or county experiencing poverty (as defined by the federal poverty level). Used in the Neighborhood Economic Diversity indicator as the denominator for computing the share living in high-poverty neighborhoods (>40% poverty rate)."
            },
        ["Number of city or county residents living in a high poverty neighborhood"] =
            new DataElement
            {
                Name = "Number of city or county residents living in a high poverty neighborhood",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Count of residents living in a neighborhood where more than 40% of residents experience poverty. Used as the numerator in the Neighborhood Economic Diversity indicator metric."
            },
        ["Number of credits attempted"] =
            new DataElement
            {
                Name = "Number of credits attempted",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Total credits a student enrolled in during a given term or year. Used alongside Number of credits earned in the First-Year Credit Accumulation indicator to compute a completion ratio and assess whether students are attempting sufficient credits for on-time graduation."
            },
        ["Number of credits earned"] =
            new DataElement
            {
                Name = "Number of credits earned",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Total credits successfully completed by a student. Used in First-Year Credit Accumulation (30 credits for full-time / 15 credits for part-time students in year one), First-Year Program of Study Concentration (9+ credits within a meta-major), and Gateway Course Completion indicators. See also: Credits earned in first year, which may capture a time-bounded subset of this element depending on how source systems store credit history."
            },
        ["Number of days suspended (K-12)"] =
            new DataElement
            {
                Name = "Number of days suspended (K-12)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Total number of days a K-12 student was suspended (in-school or out-of-school). Used in the Equitable Discipline Practices indicator to measure disproportionalities in the severity of discipline experienced across demographic subgroups."
            },
        ["Number of days suspended (PK)"] =
            new DataElement
            {
                Name = "Number of days suspended (PK)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Total number of days a pre-K child was suspended. Used in the Equitable Discipline Practices indicator. Pre-K suspensions are a significant equity concern given federal guidance discouraging their use; this element enables monitoring of disproportionate use in early childhood settings."
            },
        ["Number of households with low income in city or county"] =
            new DataElement
            {
                Name = "Number of households with low income in city or county",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Count of households in an area earning below 80% of area median income (AMI), the HUD definition of \"low income.\" Used as part of the denominator in the Access to Affordable Housing indicator ratio."
            },
        ["Number of households with very low income in city or county"] =
            new DataElement
            {
                Name = "Number of households with very low income in city or county",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Count of households in an area earning below 50% of area median income (AMI), the HUD definition of \"very low income.\" Used alongside Number of households with low income as a combined denominator for the Access to Affordable Housing indicator."
            },
        ["Number of juvenile arrests in city or county"] =
            new DataElement
            {
                Name = "Number of juvenile arrests in city or county",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Count of juvenile arrests in a city or county. Used with City or county population to compute the juvenile arrest rate per 100,000 residents for the Neighborhood Juvenile Arrests indicator."
            },
        ["Number of property felonies in city or county"] =
            new DataElement
            {
                Name = "Number of property felonies in city or county",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Count of property felonies in a city or county. Used with City or county population to compute the property crime rate per 100,000 residents for the Exposure to Neighborhood Crime indicator."
            },
        ["Number of violent felonies in city or county"] =
            new DataElement
            {
                Name = "Number of violent felonies in city or county",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Count of violent felonies in a city or county. Used with City or county population to compute the violent crime rate per 100,000 residents for the Exposure to Neighborhood Crime indicator."
            },
        ["Occupation category"] =
            new DataElement
            {
                Name = "Occupation category",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "The occupational classification of a worker's job (e.g., SOC code or major occupation group). Used in analyzing whether employment is in an in-demand field and supports labor market alignment analysis alongside In-demand status and CTE pathway data."
            },
        ["Occupational demand by region"] =
            new DataElement
            {
                Name = "Occupational demand by region",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "A classification of occupations by demand level within a specific geographic region, used to determine whether a CTE program is aligned to in-demand work. Typically derived from O*NET Bright Outlook criteria — occupations projected to have rapid growth, a large number of openings, or classified as new and emerging within a state or regional labor market. Requires external data linkage via a CIP-to-SOC crosswalk to connect CTE program offerings to occupational classifications. Not a native field in Ed-Fi or CEDS; represents an external labor market data input."
            },
        ["Office referrals (K-12)"] =
            new DataElement
            {
                Name = "Office referrals (K-12)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Count of disciplinary office referrals for a K-12 student. Used in the Equitable Discipline Practices indicator to measure differences in referral rates across demographic subgroups."
            },
        ["Office referrals (PK)"] =
            new DataElement
            {
                Name = "Office referrals (PK)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Count of disciplinary office referrals for a pre-K child. Used in the Equitable Discipline Practices indicator to detect disproportionate referral patterns in early childhood settings."
            },
        ["On-the-job training offered"] =
            new DataElement
            {
                Name = "On-the-job training offered",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator of whether an employer provides structured on-the-job training. One of two components in the Access to Ongoing Career Skills Development indicator, alongside Learning and development plan offered. Employer-level offering flag."
            },
        ["Parental education level"] =
            new DataElement
            {
                Name = "Parental education level",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Highest level of education completed by a student's parent(s) or guardian(s). Used as a demographic disaggregate. The First-generation college student flag is derivable from this element when parental education is stored in sufficient detail; both may be needed if source systems store only the derived binary flag rather than the underlying education level."
            },
        ["Participation in work-based learning"] =
            new DataElement
            {
                Name = "Participation in work-based learning",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator that a student or worker has participated in a work-based learning opportunity (internship, work study, cooperative education, apprenticeship, or similar). The Participation in Work-Based Learning indicator measures this before graduation/program completion across K-12, PS, and WF."
            },
        ["Pell grant receipt"] =
            new DataElement
            {
                Name = "Pell grant receipt",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Indicator of whether a student received a Pell grant. Used as a proxy for low-income student status in institutional graduation rate analyses (the Selection of a Well-Matched Institution indicator references graduation rates by Pell receipt). Also used as a financial aid tracking element."
            },
        ["Physical health surveys (K-12)"] =
            new DataElement
            {
                Name = "Physical health surveys (K-12)",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student self-report of physical health status. Example instrument: California Healthy Kids Survey Physical Health & Nutrition module. Used in the Physical Development and Well-Being indicator for K-12."
            },
        ["Postsecondary admissions decision"] =
            new DataElement
            {
                Name = "Postsecondary admissions decision",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [],
                AdditionalNotes = "Record of an institution's admissions decision for a prospective student (e.g., accepted, denied, waitlisted). Used in the Selection of a Well-Matched Postsecondary Institution indicator to establish the set of institutions to which a student was admitted, against which their eventual enrollment can be evaluated for match quality. Typically held by the receiving institution in admissions/CRM systems such as Slate. Note that cross-institutional admissions data is rarely centralized, making this a likely gap in most administrative data systems."
            },
        ["Postsecondary applications submitted"] =
            new DataElement
            {
                Name = "Postsecondary applications submitted",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Count of postsecondary applications submitted by a student. The College Applications indicator requires submission of at least three applications to meet the target. In the PS context, this element may capture transfer applications; confirm availability in source systems."
            },
        ["Postsecondary credential award date"] =
            new DataElement
            {
                Name = "Postsecondary credential award date",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Date a student earned a postsecondary credential (certificate, associate's, or bachelor's degree). Used to determine completion vs. dropout in persistence metrics and to calculate time-to-completion. See also: Bachelor's degree completion date and Graduate credential attainment date, which capture credential-specific milestones and may overlap with this element depending on how source systems record credential history."
            },
        ["Postsecondary credential type"] =
            new DataElement
            {
                Name = "Postsecondary credential type",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "The specific type of credential earned (e.g., certificate, associate's degree, bachelor's degree). Used in the Graduate Degree Completion indicator to confirm credential type. Closely related to Postsecondary credential attainment date — these two elements together provide a complete credential record."
            },
        ["Postsecondary degree program length"] =
            new DataElement
            {
                Name = "Postsecondary degree program length",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "The nominal intended length of a postsecondary degree program (e.g., 2-year associate's, 4-year bachelor's). Critical for computing on-time completion metrics — outcomes are measured at 100%, 150%, and 200% of program length. This is the denominator for all time-based completion and persistence calculations."
            },
        ["Postsecondary enrollment date"] =
            new DataElement
            {
                Name = "Postsecondary enrollment date",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Date a student first enrolled in a postsecondary institution. Used across multiple persistence, completion, and transition indicators. For the Postsecondary Enrollment Directly After High School Graduation indicator, enrollment must occur by October 31 following high school graduation."
            },
        ["Postsecondary enrollment status (Full time/part time)"] =
            new DataElement
            {
                Name = "Postsecondary enrollment status (Full time/part time)",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Indicator of whether a student is enrolled full-time or part-time in a postsecondary institution. Directly affects credit accumulation targets in the First-Year Credit Accumulation indicator (30 credits full-time vs. 15 credits part-time). Also affects how persistence and completion rates should be interpreted."
            },
        ["Postsecondary institution classification"] =
            new DataElement
            {
                Name = "Postsecondary institution classification",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Classification of a postsecondary institution by type (e.g., Carnegie Classification: research university, liberal arts college, community college, for-profit). Used to contextualize outcomes and as a control variable in value-added and well-matched institution analyses."
            },
        ["Postsecondary major"] =
            new DataElement
            {
                Name = "Postsecondary major",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "A student's declared field of study or major. Related to the First-Year Program of Study Concentration indicator, which measures completion of 9+ credits within a meta-major (a broader grouping of related majors). The major itself is more specific than the meta-major concept used in the indicator."
            },
        ["Pre-K eligibility status"] =
            new DataElement
            {
                Name = "Pre-K eligibility status",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Indicator of whether a 3- or 4-year-old child meets eligibility criteria for publicly funded pre-K (typically income-based). Used in the Enrollment in Quality Public Pre-K indicator as the denominator (percentage of eligible children enrolled)."
            },
        ["Pre-K enrollment date"] =
            new DataElement
            {
                Name = "Pre-K enrollment date",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "The date on which a child enrolled in a pre-K program. Used to confirm enrollment and to verify age at time of enrollment for eligibility purposes (3- and 4-year-olds). Programs may be delivered through Head Start, public school pre-K classrooms, or licensed family-based or community-based programs. Used to compute the percentage of eligible 3- and 4-year-olds enrolled in public pre-K when combined with Pre-K program funding source to scope to publicly funded programs."
            },
        ["Pre-K program funding source"] =
            new DataElement
            {
                Name = "Pre-K program funding source",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Indicates the funding source(s) for a pre-K program, such as federal, state, local, or private funding. Used to identify publicly funded programs for inclusion in access metrics. A program may have multiple funding sources."
            },
        ["Pre-K program QRIS rating"] =
            new DataElement
            {
                Name = "Pre-K program QRIS rating",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "A pre-K program's rating on the state's Quality Rating and Improvement System (QRIS). Used in both the Access to Quality Public Pre-K and Enrollment in Quality Public Pre-K indicators. QRIS rating systems vary by state, so the specific scale and quality threshold should be documented per state."
            },
        ["Pre-K program schedule"] =
            new DataElement
            {
                Name = "Pre-K program schedule",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Records the number of days per week and hours per day of a pre-K program. Used to determine whether a program meets the full-day threshold of six hours per day, five days per week. Typically stored at the program or district level."
            },
        ["Program CIP code"] =
            new DataElement
            {
                Name = "Program CIP code",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.PS, Sector.K12],
                AdditionalNotes = "A six-digit Classification of Instructional Programs (CIP) code assigned to a program offering, used to classify educational programs by subject matter and content. Provides a standardized reference for linking program offerings to occupational classifications via CIP-to-SOC crosswalks, enabling alignment analysis against labor market demand data. Maintained by the National Center for Education Statistics (NCES)."
            },
        ["Race and ethnicity (individual)"] =
            new DataElement
            {
                Name = "Race and ethnicity (individual)",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Race and ethnicity of the individual being tracked through the education-to-workforce pipeline. Spans PK, K12, postsecondary, and workforce contexts. Used across virtually all indicators as a primary disaggregate, and as a direct input in the Representational Racial and Ethnic Diversity of Educators, School and Workplace Racial and Ethnic Diversity, and Neighborhood Racial Diversity indicators."
            },
        ["Race and ethnicity (staff/educator)"] =
            new DataElement
            {
                Name = "Race and ethnicity (staff/educator)",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Race and ethnicity of educational or workforce staff. Used as a system condition measure in the Representational Racial and Ethnic Diversity of Educators and School and Workplace Racial and Ethnic Diversity indicators, where staff composition is compared against the individuals those systems serve. Distinct from Race and ethnicity (individual), which tracks the E-W pipeline participant."
            },
        ["Reading proficiency (Grades 1 and 2)"] =
            new DataElement
            {
                Name = "Reading proficiency (Grades 1 and 2)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Assessment of reading/ELA skills for students in grades 1 and 2, used in the Early Grades On Track indicator alongside math proficiency and attendance/discipline criteria. Captures early-grade literacy before state standardized testing. Distinct from State standardized test (Reading proficiency), which applies from grade 3 onward."
            },
        ["Receipt of child care subsidies"] =
            new DataElement
            {
                Name = "Receipt of child care subsidies",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Indicator of whether a family is currently receiving child care subsidy assistance. Used alongside Family eligibility for child care subsidies to compute uptake rates for the Access to Child Care Subsidies indicator."
            },
        ["Receipt of federal rental assistance"] =
            new DataElement
            {
                Name = "Receipt of federal rental assistance",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether a household is currently receiving federal rental assistance (e.g., Section 8/Housing Choice Voucher). Used alongside eligibility data to compute uptake rates for the Access to Affordable Housing indicator."
            },
        ["Reported intent to enroll in postsecondary education"] =
            new DataElement
            {
                Name = "Reported intent to enroll in postsecondary education",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student-reported intention to enroll in postsecondary education. Used as a filter in the Senior Summer On Track indicator — only students who report intent to enroll are counted in the denominator. This distinguishes between students who planned to enroll and then did not (summer melt) versus those who always intended alternative pathways."
            },
        ["Restraint and seclusion for discipline (K-12)"] =
            new DataElement
            {
                Name = "Restraint and seclusion for discipline (K-12)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Indicator or count of incidents where a K-12 student was physically restrained or secluded as a disciplinary measure. Used in the Equitable Discipline Practices and Positive Behavior indicators. Distinct from restraint and seclusion for safety — the purpose matters for both legal compliance and equity analysis."
            },
        ["Restraint and seclusion for discipline (PK)"] =
            new DataElement
            {
                Name = "Restraint and seclusion for discipline (PK)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Indicator or count of incidents where a pre-K child was physically restrained or secluded as a disciplinary measure. PK-specific element for the Equitable Discipline Practices indicator."
            },
        ["SAT completion"] =
            new DataElement
            {
                Name = "SAT completion",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Binary indicator of whether a student has taken the SAT. Used in the SAT and ACT Participation and Performance indicator alongside SAT score to measure both participation and college readiness. Targets grades 11-12."
            },
        ["SAT score"] =
            new DataElement
            {
                Name = "SAT score",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Total or section SAT score. \"College-ready\" threshold is defined by SAT benchmark scores. Should be interpreted alongside SAT completion; a missing score may mean non-participation rather than low performance."
            },
        ["School leader evaluation rating"] =
            new DataElement
            {
                Name = "School leader evaluation rating",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Results of a formal evaluation of a school or program leader's effectiveness. The Effective Program and School Leadership indicator measures the percentage of leaders rated as effective using multi-measure evaluation systems such as the Tennessee TEAM Administrator Evaluation component."
            },
        ["School value-added"] =
            new DataElement
            {
                Name = "School value-added",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "School-level value-added measure representing a school's contribution to student outcomes (achievement, attendance, SEL, college enrollment, earnings) beyond what would be predicted by student characteristics. Derived metric computed using statistical models. Used in the Institutions' Contributions to Student Outcomes indicator."
            },
        ["Self-efficacy surveys (K-12)"] =
            new DataElement
            {
                Name = "Self-efficacy surveys (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student self-report of belief in their ability to achieve outcomes or reach goals. Example instrument: CORE Districts SEL Survey self-efficacy scale."
            },
        ["Self-efficacy surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Self-efficacy surveys (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student self-report of self-efficacy at the PS level. Example instruments: New General Self-Efficacy Scale, Ascend survey's Self-Efficacy Scale."
            },
        ["Self-efficacy surveys (Workforce)"] =
            new DataElement
            {
                Name = "Self-efficacy surveys (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Individual self-report of self-efficacy in the workforce context. Example instruments: New General Self-Efficacy Scale, Ascend survey's Self-Efficacy Scale."
            },
        ["Self-management surveys (K-12)"] =
            new DataElement
            {
                Name = "Self-management surveys (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student self-report of ability to regulate emotions, thoughts, and behaviors. Example instruments: CORE Districts SEL Survey self-management scale (grades 5–12), Shift and Persist scale for children."
            },
        ["Self-management surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Self-management surveys (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student self-report of self-management at the PS level. Example instrument: Shift and Persist scale for teens and adults."
            },
        ["Self-management surveys (Workforce)"] =
            new DataElement
            {
                Name = "Self-management surveys (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Individual self-report of self-management in the workforce context. Example instrument: Shift and Persist scale for teens and adults."
            },
        ["Sense of belonging surveys (K-12)"] =
            new DataElement
            {
                Name = "Sense of belonging surveys (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student self-report of belonging and connection at school. Example instruments: CORE Districts school culture and climate survey (Sense of Belonging subscale), Panorama Student Survey (Classroom Belonging subscale)."
            },
        ["Sense of belonging surveys (PK)"] =
            new DataElement
            {
                Name = "Sense of belonging surveys (PK)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Assessment or observational measure of a pre-K child's sense of belonging. Example instrument: CASEL's How I Feel About My School questionnaire, or ACSES observational assessment of equitable classroom interactions."
            },
        ["Sense of belonging surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Sense of belonging surveys (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student self-report of belonging on campus. Example instruments: HERI Diverse Learning Environments Survey, NITE Culturally Engaging Campus Environments Survey."
            },
        ["Sense of belonging surveys (Workforce)"] =
            new DataElement
            {
                Name = "Sense of belonging surveys (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Employee self-report of belonging in the workplace. Example instruments: AAMC Diversity Engagement Survey."
            },
        ["SGP for standardized assessments"] =
            new DataElement
            {
                Name = "SGP for standardized assessments",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student growth percentile (SGP) on state standardized assessments, computed separately for reading/literacy, math, and science. SGPs measure a student's growth relative to academically similar peers. Used in the Teachers' Contributions to Student Learning Growth indicator alongside VAM as a measure of educator effectiveness."
            },
        ["SNAP eligibility"] =
            new DataElement
            {
                Name = "SNAP eligibility",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual meets income and other criteria for the Supplemental Nutrition Assistance Program. Used alongside SNAP participation to compute uptake rates for the Food Security indicator."
            },
        ["SNAP participation"] =
            new DataElement
            {
                Name = "SNAP participation",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Indicator of whether an individual is actively receiving SNAP benefits. The Food Security indicator measures the percentage of eligible individuals participating in SNAP."
            },
        ["Social awareness surveys (K-12)"] =
            new DataElement
            {
                Name = "Social awareness surveys (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Self-reported survey measure of social awareness in K-12 students, capturing the ability to understand others' perspectives, recognize social and ethical norms, and identify community resources and supports. Example instruments include the CORE Districts SEL Survey social awareness scale and Elliott and Gresham's Social Skills Rating Scale self-report form. Used in the Social Awareness indicator."
            },
        ["Social awareness teacher ratings"] =
            new DataElement
            {
                Name = "Social awareness teacher ratings",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Teacher-rated assessment of a student's social skills and awareness. Example instruments: Elliott and Gresham's Social Skills Rating Scale. Used in the Social Awareness indicator alongside student self-report surveys. This is a teacher-reported element, distinct from student self-reports."
            },
        ["Social capital surveys (K-12)"] =
            new DataElement
            {
                Name = "Social capital surveys (K-12)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student self-report of access to and ability to mobilize supportive relationships. Example instrument: Social Capital Assessment + Learning for Equity (SCALE) Social Capital, Network Diversity, and Network Strength scales."
            },
        ["Social capital surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Social capital surveys (Postsecondary)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student self-report of social capital at the PS level. Example instruments: SCALE, Social Capital Community Benchmark Survey."
            },
        ["Social capital surveys (Workforce)"] =
            new DataElement
            {
                Name = "Social capital surveys (Workforce)",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Individual self-report of social capital in the workforce context. Example instrument: Social Capital Community Benchmark Survey."
            },
        ["Social proficiency performance assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Social proficiency performance assessments (Postsecondary)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Score or proficiency on an assessment of social skills at the postsecondary level. Used in the Social Awareness indicator alongside other social awareness measures."
            },
        ["Social proficiency performance assessments (Workforce)"] =
            new DataElement
            {
                Name = "Social proficiency performance assessments (Workforce)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Score or proficiency on a social skills performance assessment in the workforce context. Example instrument: National Work Readiness Credential Essential Soft Skills assessment."
            },
        ["Social services offered"] =
            new DataElement
            {
                Name = "Social services offered",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator of whether social services (e.g., social work, case management) are available through a program or institution. Used in the Access to Health, Mental Health, and Social Supports indicator. Program/institution-level offering flag."
            },
        ["Sociocultural observational assessments (PK)"] =
            new DataElement
            {
                Name = "Sociocultural observational assessments (PK)",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Results of an observational assessment measuring equitable sociocultural interactions. Example instrument: Assessing Classroom Sociocultural Equity Scale (ACSES). Used in the Inclusive Environments indicator to measure whether classroom interactions are equitable."
            },
        ["Staff FTE status"] =
            new DataElement
            {
                Name = "Staff FTE status",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Full-time equivalent status of a staff member. Used in multiple indicators: Teacher Credentials (percentage of courses taught by FTE teachers, excluding substitutes and emergency license holders), Access to College and Career Advising (student-to-FTE-counselor ratio), and Access to Health, Mental Health, and Social Supports (student-to-FTE-staff ratio)."
            },
        ["Staff position type"] =
            new DataElement
            {
                Name = "Staff position type",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "The classification of a staff member's role or position (e.g., lead teacher, substitute teacher, school nurse, psychologist, social worker, employee assistance program counselor). Used to identify staff role distinctions in the Teacher Credentials indicator (e.g., lead vs. substitute teachers, certified vs. emergency license holders) and to compute student-to-staff ratios for specific service types in the Access to Health, Mental Health, and Social Supports indicator."
            },
        ["State standardized test (Math proficiency)"] =
            new DataElement
            {
                Name = "State standardized test (Math proficiency)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student score or proficiency level on the state's standardized math assessment. Used across grade-band proficiency indicators (grade 3, grade 8, high school). Proficiency is defined by the state's cut score. Distinct from Math proficiency (Grades 1 and 2), which uses different early-grade assessment instruments."
            },
        ["State standardized test (Reading proficiency)"] =
            new DataElement
            {
                Name = "State standardized test (Reading proficiency)",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student score or proficiency level on the state's standardized reading/ELA assessment. Used across grade-band proficiency indicators (grade 3, grade 8, high school). Distinct from Reading proficiency (Grades 1 and 2), which uses different early-grade assessment instruments."
            },
        ["Student attendance rate (K-12)"] =
            new DataElement
            {
                Name = "Student attendance rate (K-12)",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Percentage of enrolled days a K-12 student was present. The Consistent Attendance indicator defines the target as more than 90% of enrolled days (excluding students enrolled fewer than 90 days). Also used as a criterion in the 6th, 8th, and 9th grade on-track indicators (96% threshold for 8th and 9th grade)."
            },
        ["Student attendance rate (PK)"] =
            new DataElement
            {
                Name = "Student attendance rate (PK)",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Percentage of enrolled days a pre-K student was present. Used in the Consistent Attendance indicator, which targets attendance above 90% of enrolled days."
            },
        ["Student attendance rate (Postsecondary)"] =
            new DataElement
            {
                Name = "Student attendance rate (Postsecondary)",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "The percentage of enrolled days or scheduled class sessions attended by a postsecondary student within a given term. Used to identify students present for more than 90 percent of enrolled time. Tracking methods vary by institution and program type; attendance data may be more consistently available in contexts where it is required for financial aid compliance or in structured programs such as clinical or vocational training."
            },
        ["Student course enrollment record"] =
            new DataElement
            {
                Name = "Student course enrollment record",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "A record of all courses a student is or was enrolled in, serving as the core transcript element underlying multiple indicators including CTE pathway concentration, early college access, equitable placement in rigorous coursework, and work-based learning participation. Record structure and source systems typically differ between K12 and PS contexts; confirm availability and format in each."
            },
        ["Student course enrollment record (K-12)"] =
            new DataElement
            {
                Name = "Student course enrollment record (K-12)",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "A K-12 record of all courses a student is or was enrolled in, serving as the core transcript element underlying multiple indicators including CTE pathway concentration, early college access, equitable placement in rigorous coursework, and work-based learning participation."
            },
        ["Student from migrant family household"] =
            new DataElement
            {
                Name = "Student from migrant family household",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator of whether a student is from a migratory agricultural or fishing family, as defined under Title I Part C of ESEA. Used as a demographic disaggregate. This has a specific federal definition — confirm whether the data element captures the formal Migrant Education Program (MEP) designation or a broader migrant/immigrant status."
            },
        ["Student grade level (K-12)"] =
            new DataElement
            {
                Name = "Student grade level (K-12)",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "The current grade level of a K-12 student (kindergarten through grade 12). Used across many indicators to filter to specific grade cohorts (e.g., grade 12 for FAFSA completion, grades 11–12 for SAT/ACT participation)."
            },
        ["Student loan debt balance"] =
            new DataElement
            {
                Name = "Student loan debt balance",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "The total cumulative amount of student loan debt held by an individual at a given point in time, typically at the time of graduation or program exit. Used to calculate median student debt across a population of postsecondary completers or leavers. Includes federal and private loans; may not capture parent PLUS loans or other debt incurred on behalf of the student."
            },
        ["Student loan repayment phase start date"] =
            new DataElement
            {
                Name = "Student loan repayment phase start date",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Date a student borrower entered the repayment phase of their student loans. The Student Loan Repayment indicator measures repayment status at 1, 2, 3, 5, and 10 years after this date. Repayment typically begins six months after leaving school."
            },
        ["Student loan repayment status"] =
            new DataElement
            {
                Name = "Student loan repayment status",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Current status of a borrower's student loan repayment. Categories as defined by the College Scorecard: making progress (outstanding balance decreasing), paid in full, deferment (common for re-enrollees), delinquency, default, or not making progress. The indicator targets the percentage in the first three (positive) categories."
            },
        ["Student parenting status"] =
            new DataElement
            {
                Name = "Student parenting status",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "Indicator of whether a student is a parent or primary caregiver for a dependent child. Used as a demographic disaggregate given the known barriers parenting students face in persisting and completing education."
            },
        ["Student utilization of academic advising services (Postsecondary)"] =
            new DataElement
            {
                Name = "Student utilization of academic advising services (Postsecondary)",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Indicator of whether a postsecondary student has used academic advising services at their institution during a given term or academic year. Typically survey-derived; administrative tracking of individual advising utilization varies by institution."
            },
        ["Student utilization of career counseling services (Postsecondary)"] =
            new DataElement
            {
                Name = "Student utilization of career counseling services (Postsecondary)",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Indicator of whether a postsecondary student has used career counseling services at their institution during a given term or academic year. Typically survey-derived; administrative tracking of individual utilization varies by institution."
            },
        ["Suspensions and expulsions (K-12)"] =
            new DataElement
            {
                Name = "Suspensions and expulsions (K-12)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Indicator or count of in-school suspensions, out-of-school suspensions, and expulsions for a K-12 student. Used as a criterion in on-track indicators (must have zero suspensions/expulsions) and in the Positive Behavior and Equitable Discipline Practices indicators. Distinct from the Number of days suspended element — this captures whether any event occurred, while that element captures severity."
            },
        ["Suspensions and expulsions (PK)"] =
            new DataElement
            {
                Name = "Suspensions and expulsions (PK)",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Indicator or count of suspensions and expulsions for a pre-K student. Used in the Positive Behavior indicator. Pre-K suspensions are of particular equity concern given federal guidance discouraging their use with young children."
            },
        ["Teacher effectiveness student surveys (K-12)"] =
            new DataElement
            {
                Name = "Teacher effectiveness student surveys (K-12)",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Student-reported perceptions of their teacher's effectiveness. Example instruments: Panorama Student Survey (Pedagogical Effectiveness subscale), Tripod Student Survey, 5Essentials Survey (Ambitious Instruction and Supportive Environment domains). Used in the Student Perceptions of Teaching indicator."
            },
        ["Teacher effectiveness student surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Teacher effectiveness student surveys (Postsecondary)",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Student-reported perceptions of whether college instructors implement effective teaching practices. Example instrument: National Survey of Student Engagement (NSSE). Used in the Student Perceptions of Teaching indicator."
            },
        ["Teacher observation scores (K-12)"] =
            new DataElement
            {
                Name = "Teacher observation scores (K-12)",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "An overall score from a formal classroom observation rubric assessing the quality of instructional practice. Typically derived from a structured educator evaluation framework such as Danielson's Framework for Teaching or the Marzano Causal Teacher Evaluation Model. Represents the composite rating across all observed domains."
            },
        ["Teacher observation scores (Postsecondary)"] =
            new DataElement
            {
                Name = "Teacher observation scores (Postsecondary)",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "A score or rating from a peer or administrative observation of instructional practice at the postsecondary level. No widely adopted standardized rubric currently exists for college teaching observation; instruments and criteria vary by institution. Where available, scores may be derived from locally developed rubrics or emerging frameworks for peer observation of college teaching. Many institutions may not have formal observation data on record."
            },
        ["Teacher observation subscale scores (K-12)"] =
            new DataElement
            {
                Name = "Teacher observation subscale scores (K-12)",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Domain or subscale-level scores from a formal classroom observation rubric, providing a more granular breakdown of instructional practice than the overall score. Subscales vary by framework but commonly include dimensions such as classroom environment, instruction, and professional responsibilities."
            },
        ["Teacher qualification or certification type"] =
            new DataElement
            {
                Name = "Teacher qualification or certification type",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "The type of teaching qualification or state certification held by an educator (e.g., standard, emergency, provisional, subject-area endorsement). Used in the Teacher Credentials indicator to distinguish fully certified teachers from those holding emergency or provisional licenses."
            },
        ["Teacher reports of executive function"] =
            new DataElement
            {
                Name = "Teacher reports of executive function",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Teacher-reported assessment of a pre-K child's executive function abilities. Example instrument: Comprehensive Behavior Rating Scale (CBRS). Used in the Kindergarten Readiness: Approaches to Learning indicator. Distinct from Direct child assessments of executive function (which uses direct assessment tools like HTKS or MEFS)."
            },
        ["Teacher reports of social-emotional development"] =
            new DataElement
            {
                Name = "Teacher reports of social-emotional development",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Teacher-reported assessment of a pre-K child's social-emotional development. Example instruments: Comprehensive Behavior Rating Scale (CBRS), Devereux Early Childhood Assessment for Preschoolers (DECA-P2). Used in the Kindergarten Readiness: Social-Emotional Development indicator."
            },
        ["Teacher value-added"] =
            new DataElement
            {
                Name = "Teacher value-added",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "A statistical estimate of an individual teacher's contribution to student learning growth, derived from student assessment outcomes while controlling for prior achievement and student characteristics. Distinct from School value-added, which aggregates to the institutional level. Used in the Teachers' Contributions to Student Learning Growth indicator. Note that value-added models vary in methodology across states and institutions, which can limit comparability across contexts."
            },
        ["Teacher-child interaction measure (PK)"] =
            new DataElement
            {
                Name = "Teacher-child interaction measure (PK)",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = "Observational measure of the quality of teacher-child interactions in pre-K settings. Example instruments: CLASS (Classroom Assessment Scoring System), ECERS (Early Childhood Environment Rating Scale) Interactions subscale, ACSES. Used in the Classroom Observations of Instructional Practice indicator."
            },
        ["Teacher-reported kindergarten readiness (behavioral skills)"] =
            new DataElement
            {
                Name = "Teacher-reported kindergarten readiness (behavioral skills)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported assessment of a child's behavioral self-regulation, approaches to learning, and executive function readiness for kindergarten. Example instruments: DRDP Approaches to Learning - Self-Regulation domain, TS GOLD Cognitive subscale. Teacher-reported; distinct from direct child assessment instruments."
            },
        ["Teacher-reported kindergarten readiness (cognition)"] =
            new DataElement
            {
                Name = "Teacher-reported kindergarten readiness (cognition)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported assessment of a child's math and scientific reasoning readiness for kindergarten. Example instruments: DRDP Cognition domain, R4K ELA Mathematics and Science domains, TS GOLD Cognitive and Mathematics subscales. Teacher-reported; distinct from direct child assessment instruments."
            },
        ["Teacher-reported kindergarten readiness (language and literacy)"] =
            new DataElement
            {
                Name = "Teacher-reported kindergarten readiness (language and literacy)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported assessment of a child's language and literacy readiness for kindergarten. Example instruments: DRDP Language and Literacy Development domain, R4K ELA Language and Literacy domain, TS GOLD Language and Literacy subscales. Teacher-reported; distinct from direct child assessment instruments."
            },
        ["Teacher-reported kindergarten readiness (physical development)"] =
            new DataElement
            {
                Name = "Teacher-reported kindergarten readiness (physical development)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported assessment of a child's physical development and motor skills readiness for kindergarten. Example instruments: DRDP Physical Development - Health domain, R4K ELA Physical Well-Being and Motor Development domain, TS GOLD Physical subscale. Teacher-reported; distinct from direct child assessment instruments."
            },
        ["Teacher-reported kindergarten readiness (social-emotional skills)"] =
            new DataElement
            {
                Name = "Teacher-reported kindergarten readiness (social-emotional skills)",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported assessment of a child's social-emotional development readiness for kindergarten. Example instruments: DRDP Social and Emotional Development domain, R4K ELA Social Foundations domain, TS GOLD Social-Emotional subscale. Teacher-reported; distinct from direct child assessment instruments."
            },
        ["Teaching assignment"] =
            new DataElement
            {
                Name = "Teaching assignment",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "The subject area(s) and grade level(s) a teacher is assigned to teach. Used in the Teacher Credentials indicator to determine whether a teacher holds appropriate certification for their assigned subject or grade level; a mismatch between assignment and certification is the key metric of interest."
            },
        ["Total net price of education plus interest"] =
            new DataElement
            {
                Name = "Total net price of education plus interest",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "The total out-of-pocket cost of postsecondary education, including accumulated loan interest. Used in the Minimum Economic Return indicator: earnings must exceed median HS graduate earnings in the state plus enough to recoup this total cost within 10 years of completion."
            },
        ["Total state educational funding"] =
            new DataElement
            {
                Name = "Total state educational funding",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Total state-level funding allocated to education. Used as the denominator in the Expenditures on Workforce Development Programs indicator (workforce development funding as a percentage of total educational funding). In other contexts, serves as a top-line funding figure."
            },
        ["Transfer enrollment status"] =
            new DataElement
            {
                Name = "Transfer enrollment status",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = "Indicator of whether a student has transferred to a different postsecondary institution. In persistence metrics, transfer to another institution (including upward transfer) counts as a positive outcome rather than dropout. See also: Transfer (if applicable) indicator and transfer student status elements, which may capture overlapping information depending on source system."
            },
        ["Universal mental health screening results (K-12)"] =
            new DataElement
            {
                Name = "Universal mental health screening results (K-12)",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Results of a universal (school-wide or program-wide) mental health screening tool. For K-12, see the NCSSLE guide \"Mental Health Screening Tools for Grades K-12\" for example instruments. Used in the Mental and Emotional Well-Being indicator alongside Mental and emotional well-being assessments."
            },
        ["Urbanicity"] =
            new DataElement
            {
                Name = "Urbanicity",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Classification of a geographic area as urban, suburban, or rural (e.g., NCES locale codes). Used as a community-level contextual variable across multiple indicators."
            },
        ["Workforce development program participation"] =
            new DataElement
            {
                Name = "Workforce development program participation",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = "Indicator that an individual has participated in a workforce development program. Used as a population denominator in the Industry-Recognized Credential indicator (percentage of participants earning a credential) and in work-based learning participation metrics. Distinct from program enrollment — participation typically implies active engagement rather than initial registration; confirm how source systems distinguish these stages."
            },
        ["Years of teaching experience"] =
            new DataElement
            {
                Name = "Years of teaching experience",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Total years of teaching experience, regardless of school. Used in the Teacher Experience indicator, which categorizes teachers as having fewer than 1 year, 1-5 years, or 5+ years of experience. Distinct from Years in current position, which tracks tenure at a specific school."
            },
    };
}
