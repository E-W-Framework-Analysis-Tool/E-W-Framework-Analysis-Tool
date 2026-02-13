using EwFrameworkAnalysis.Common.Models.Framework;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public static class EwFrameworkDataElements
{
    public static readonly Dictionary<string, DataElement> Elements = new()
    {
        // Academic Assessment and Performance
        ["ACT completion"] = new("ACT completion", "Academic Assessment and Performance", [], null, []),
        ["ACT score"] = new("ACT score", "Academic Assessment and Performance", [], null, []),
        ["AP course passage"] = new("AP course passage", "Academic Assessment and Performance", [], null, []),
        ["AP credit earned (or AP test scores)"] = new("AP credit earned (or AP test scores)", "Academic Assessment and Performance", [], null, []),
        ["AP test completion"] = new("AP test completion", "Academic Assessment and Performance", [], null, []),
        ["Basic skills level"] = new("Basic skills level", "Academic Assessment and Performance", [], null, []),
        ["CTE course completion"] = new("CTE course completion", "Academic Assessment and Performance", [], null, []),
        ["Communication skills performance assessments (K-12)"] = new("Communication skills performance assessments (K-12)", "Academic Assessment and Performance", [Sector.K12], null, []),
        ["Communication skills performance assessments (Postsecondary)"] = new("Communication skills performance assessments (Postsecondary)", "Academic Assessment and Performance", [Sector.PS], null, []),
        ["Communication skills performance assessments (Workforce)"] = new("Communication skills performance assessments (Workforce)", "Academic Assessment and Performance", [Sector.WF], null, []),
        ["Course outcome"] = new("Course outcome", "Academic Assessment and Performance", [Sector.K12], "For example, completion, failure, passage", []),
        ["Course performance (English and Math)"] = new("Course performance (English and Math)", "Academic Assessment and Performance", [Sector.K12], null, []),
        ["Cultural competency assessments (K-12)"] = new("Cultural competency assessments (K-12)", "Academic Assessment and Performance", [Sector.K12], null, []),
        ["Cultural competency assessments (Postsecondary)"] = new("Cultural competency assessments (Postsecondary)", "Academic Assessment and Performance", [Sector.PS], null, []),
        ["Cultural competency assessments (Workforce)"] = new("Cultural competency assessments (Workforce)", "Academic Assessment and Performance", [Sector.WF], null, []),
        ["Digital skills assessments (K-12)"] = new("Digital skills assessments (K-12)", "Academic Assessment and Performance", [Sector.K12], null, []),
        ["Digital skills assessments (Postsecondary)"] = new("Digital skills assessments (Postsecondary)", "Academic Assessment and Performance", [Sector.PS], null, []),
        ["Digital skills assessments (Workforce)"] = new("Digital skills assessments (Workforce)", "Academic Assessment and Performance", [Sector.WF], null, []),
        ["Dual credit course passage"] = new("Dual credit course passage", "Academic Assessment and Performance", [], null, []),
        ["Dual credit earned"] = new("Dual credit earned", "Academic Assessment and Performance", [], null, []),
        ["Dual credit test completion"] = new("Dual credit test completion", "Academic Assessment and Performance", [], null, []),
        ["Grade point average (K-12)"] = new("Grade point average (K-12)", "Academic Assessment and Performance", [Sector.K12], null, []),
        ["Gateway course completion"] = new("Gateway course completion", "Academic Assessment and Performance", [], null, []),
        ["Grade point average (Postsecondary)"] = new("Grade point average (Postsecondary)", "Academic Assessment and Performance", [Sector.PS], null, []),
        ["Higher-order thinking skills performance assessments (K-12)"] = new("Higher-order thinking skills performance assessments (K-12)", "Academic Assessment and Performance", [Sector.K12], null, []),
        ["Higher-order thinking skills performance assessments (Postsecondary)"] = new("Higher-order thinking skills performance assessments (Postsecondary)", "Academic Assessment and Performance", [Sector.PS], null, []),
        ["Higher-order thinking skills performance assessments (Workforce)"] = new("Higher-order thinking skills performance assessments (Workforce)", "Academic Assessment and Performance", [Sector.WF], null, []),
        ["IB course passage"] = new("IB course passage", "Academic Assessment and Performance", [], null, []),
        ["IB credit earned (or IB test scores)"] = new("IB credit earned (or IB test scores)", "Academic Assessment and Performance", [], null, []),
        ["IB test completion"] = new("IB test completion", "Academic Assessment and Performance", [], null, []),
        ["Math proficiency (Grades 1 and 2)"] = new("Math proficiency (Grades 1 and 2)", "Academic Assessment and Performance", [Sector.K12], null, []),
        ["State standardized test (Math proficiency)"] = new("State standardized test (Math proficiency)", "Academic Assessment and Performance", [Sector.K12], null, []),
        ["Reading proficiency (Grades 1 and 2)"] = new("Reading proficiency (Grades 1 and 2)", "Academic Assessment and Performance", [Sector.K12], null, []),
        ["State standardized test (Reading proficiency)"] = new("State standardized test (Reading proficiency)", "Academic Assessment and Performance", [Sector.K12], null, []),
        ["Direct child assessments (cognition)"] = new("Direct child assessments (cognition)", "Academic Assessment and Performance", [Sector.PK], null, []),
        ["Direct child assessments (language and literacy)"] = new("Direct child assessments (language and literacy)", "Academic Assessment and Performance", [Sector.PK], null, []),
        ["Direct child assessments of executive function"] = new("Direct child assessments of executive function", "Academic Assessment and Performance", [Sector.PK], null, []),
        ["Direct child assessments of physical development"] = new("Direct child assessments of physical development", "Academic Assessment and Performance", [Sector.PK], null, []),
        ["Kindergarten readiness assessments (cognition)"] = new("Kindergarten readiness assessments (cognition)", "Academic Assessment and Performance", [Sector.PK], null, []),
        ["Teacher reports of executive function"] = new("Teacher reports of executive function", "Academic Assessment and Performance", [Sector.PK], null, []),
        ["Teacher reports of social-emotional development"] = new("Teacher reports of social-emotional development", "Academic Assessment and Performance", [Sector.PK], null, []),
        ["Teacher-reported kindergarten readiness assessments (language and literacy)"] = new("Teacher-reported kindergarten readiness assessments (language and literacy)", "Academic Assessment and Performance", [Sector.PK], null, []),
        ["Teacher-reported kindergarten readiness assessments (physical development)"] = new("Teacher-reported kindergarten readiness assessments (physical development)", "Academic Assessment and Performance", [Sector.PK], null, []),
        ["Teacher-reported kindergarten readiness assessments (self-regulation; approaches to learning; executive function)"] = new("Teacher-reported kindergarten readiness assessments (self-regulation; approaches to learning; executive function)", "Academic Assessment and Performance", [Sector.PK], null, []),
        ["Teacher-reported kindergarten readiness assessments (social-emotional development)"] = new("Teacher-reported kindergarten readiness assessments (social-emotional development)", "Academic Assessment and Performance", [Sector.PK], null, []),
        ["SAT completion"] = new("SAT completion", "Academic Assessment and Performance", [], null, []),
        ["SAT score"] = new("SAT score", "Academic Assessment and Performance", [], null, []),
        ["SGP for standardized assessments (subject specific for reading/literacy; math; and science)"] = new("SGP for standardized assessments (subject specific for reading/literacy; math; and science)", "Academic Assessment and Performance", [], null, []),
        ["Social proficiency performance assessments (Postsecondary)"] = new("Social proficiency performance assessments (Postsecondary)", "Academic Assessment and Performance", [Sector.PS], null, []),
        ["Social proficiency performance assessments (Workforce)"] = new("Social proficiency performance assessments (Workforce)", "Academic Assessment and Performance", [Sector.WF], null, []),
        ["VAM for subject specific assessment"] = new("VAM for subject specific assessment", "Academic Assessment and Performance", [], null, []),

        // Attendance and Engagement
        ["Student attendance rate (PK)"] = new("Student attendance rate (PK)", "Attendance and Engagement", [Sector.PK], null, []),
        ["Student attendance rate (K-12)"] = new("Student attendance rate (K-12)", "Attendance and Engagement", [Sector.K12], null, []),

        // Behavior and Discipline
        ["Restraint and seclusion (K-12)"] = new("Restraint and seclusion (K-12)", "Behavior and Discipline", [Sector.K12], null, []),
        ["Restraint and seclusion (PK)"] = new("Restraint and seclusion (PK)", "Behavior and Discipline", [Sector.PK], null, []),
        ["Suspensions and expulsions (K-12)"] = new("Suspensions and expulsions (K-12)", "Behavior and Discipline", [Sector.K12], null, []),
        ["Suspensions and expulsions (PK)"] = new("Suspensions and expulsions (PK)", "Behavior and Discipline", [Sector.PK], null, []),
        ["Number of days suspended (K-12)"] = new("Number of days suspended (K-12)", "Behavior and Discipline", [Sector.K12], null, []),
        ["Number of days suspended (PK)"] = new("Number of days suspended (PK)", "Behavior and Discipline", [Sector.PK], null, []),
        ["Office referrals (K-12)"] = new("Office referrals (K-12)", "Behavior and Discipline", [Sector.K12], null, []),
        ["Office referrals (PK)"] = new("Office referrals (PK)", "Behavior and Discipline", [Sector.PK], null, []),

        // Community and Environmental Conditions
        ["City or county population"] = new("City or county population", "Community and Environmental Conditions", [], null, []),
        ["Commute time"] = new("Commute time", "Community and Environmental Conditions", [], null, []),
        ["Geographical indicator"] = new("Geographical indicator", "Community and Environmental Conditions", [], "For example, census tract, city, or county", []),
        ["Indicator of access to desktop or laptop at home"] = new("Indicator of access to desktop or laptop at home", "Community and Environmental Conditions", [], null, []),
        ["Indicator of access to reliable broadband internet"] = new("Indicator of access to reliable broadband internet", "Community and Environmental Conditions", [], null, []),
        ["Low Transportation Cost Index"] = new("Low Transportation Cost Index", "Community and Environmental Conditions", [], null, []),
        ["Number of affordable housing units in city or county"] = new("Number of affordable housing units in city or county", "Community and Environmental Conditions", [], null, []),
        ["Number of city or county residents experiencing poverty"] = new("Number of city or county residents experiencing poverty", "Community and Environmental Conditions", [], null, []),
        ["Number of city or county residents living in a high poverty neighborhood"] = new("Number of city or county residents living in a high poverty neighborhood", "Community and Environmental Conditions", [], null, []),
        ["Number of households with low income in city or county"] = new("Number of households with low income in city or county", "Community and Environmental Conditions", [], null, []),
        ["Number of households with very low income in city or county"] = new("Number of households with very low income in city or county", "Community and Environmental Conditions", [], null, []),
        ["Number of juvenile arrests in city or county"] = new("Number of juvenile arrests in city or county", "Community and Environmental Conditions", [], null, []),
        ["Number of property felonies in city or county"] = new("Number of property felonies in city or county", "Community and Environmental Conditions", [], null, []),
        ["Number of violent felonies in city or county"] = new("Number of violent felonies in city or county", "Community and Environmental Conditions", [], null, []),
        ["USDA Food Access Research Atlas Access Level Flag"] = new("USDA Food Access Research Atlas Access Level Flag", "Community and Environmental Conditions", [], null, []),

        // Course and Curriculum
        ["AP course designation"] = new("AP course designation", "Course and Curriculum", [], null, []),
        ["Student course enrollment record      "] = new("Student course enrollment record", "Course and Curriculum", [], null, []),
        ["CTE course ID or course title"] = new("CTE course ID or course title", "Course and Curriculum", [], null, []),
        ["CTE pathway or career cluster associated with CTE course"] = new("CTE pathway or career cluster associated with CTE course", "Course and Curriculum", [], null, []),
        ["CTE program"] = new("CTE program", "Course and Curriculum", [], null, []),
        ["Course identifier or title"] = new("Course identifier or title", "Course and Curriculum", [], null, []),
        ["Course department"] = new("Course department", "Course and Curriculum", [], null, []),
        ["Teaching assignment"] = new("Teaching assignment", "Course and Curriculum", [], null, []),
        ["Dual credit course designation"] = new("Dual credit course designation", "Course and Curriculum", [], null, []),
        ["IB course designation"] = new("IB course designation", "Course and Curriculum", [], null, []),
        ["Course offering by grade level"] = new("Course offering by grade level", "Course and Curriculum", [], null, []),
        ["Postsecondary major"] = new("Postsecondary major", "Course and Curriculum", [Sector.PS], null, []),

        // Educator Employment and Retention
        ["Job title or position type"] = new("Job title or position type", "Educator Employment and Retention", [], null, []),
        ["Years of teaching experience"] = new("Years of teaching experience", "Educator Employment and Retention", [], null, []),
        ["School assignment (prior and current year)"] = new("School assignment (prior and current year)", "Educator Employment and Retention", [], null, []),
        ["Staff FTE status"] = new("Staff FTE status", "Educator Employment and Retention", [], null, []),
        ["Years in current position"] = new("Years in current position", "Educator Employment and Retention", [], null, []),

        // Educator Qualifications and Credentials
        ["Credential or certification type"] = new("Credential or certification type", "Educator Qualifications and Credentials", [], null, []),
        ["Highest level of education completed"] = new("Highest level of education completed", "Educator Qualifications and Credentials", [], null, []),
        ["Leader effectiveness assessments"] = new("Leader effectiveness assessments", "Educator Qualifications and Credentials", [], null, []),
        ["Staff race/ethnicity"] = new("Staff race/ethnicity", "Educator Qualifications and Credentials", [], null, []),
        ["Teacher qualification or certification type"] = new("Teacher qualification or certification type", "Educator Qualifications and Credentials", [], null, []),

        // Financial Support and Resources
        ["Average cost of attendance"] = new("Average cost of attendance", "Financial Support and Resources", [], null, []),
        ["Average expected family contribution (EFC)"] = new("Average expected family contribution (EFC)", "Financial Support and Resources", [], null, []),
        ["Average financial aid amount (including grants, scholarships, and tuition waivers)"] = new("Average financial aid amount (including grants, scholarships, and tuition waivers)", "Financial Support and Resources", [], null, []),
        ["CHIP eligibility status"] = new("CHIP eligibility status", "Financial Support and Resources", [], null, []),
        ["CHIP enrollment"] = new("CHIP enrollment", "Financial Support and Resources", [], null, []),
        ["Eligibility for federal rental assistance"] = new("Eligibility for federal rental assistance", "Financial Support and Resources", [], null, []),
        ["FAFSA completion date"] = new("FAFSA completion date", "Financial Support and Resources", [], null, []),
        ["Family eligibility for child care subsidies"] = new("Family eligibility for child care subsidies", "Financial Support and Resources", [], null, []),
        ["Median student debt"] = new("Median student debt", "Financial Support and Resources", [], null, []),
        ["Medicaid eligibility status"] = new("Medicaid eligibility status", "Financial Support and Resources", [], null, []),
        ["Medicaid enrollment"] = new("Medicaid enrollment", "Financial Support and Resources", [], null, []),
        ["Receipt of child care subsidies"] = new("Receipt of child care subsidies", "Financial Support and Resources", [], null, []),
        ["Receipt of federal rental assistance"] = new("Receipt of federal rental assistance", "Financial Support and Resources", [], null, []),
        ["Repayment phase start date"] = new("Repayment phase start date", "Financial Support and Resources", [], null, []),
        ["Repayment status"] = new("Repayment status", "Financial Support and Resources", [], null, []),
        ["SNAP eligibility"] = new("SNAP eligibility", "Financial Support and Resources", [], null, []),
        ["SNAP participation"] = new("SNAP participation", "Financial Support and Resources", [], null, []),
        ["Total net price of education plus interest"] = new("Total net price of education plus interest", "Financial Support and Resources", [], null, []),

        // Health and Well-being
        ["Insured status"] = new("Insured status", "Health and Well-being", [], null, []),
        ["Number of ACEs"] = new("Number of ACEs", "Health and Well-being", [], null, []),
        ["Physical health surveys (K-12)"] = new("Physical health surveys (K-12)", "Health and Well-being", [Sector.K12], null, []),
        ["Physical health surveys (Postsecondary)"] = new("Physical health surveys (Postsecondary)", "Health and Well-being", [Sector.PS], null, []),
        ["Physical health surveys (Workforce)"] = new("Physical health surveys (Workforce)", "Health and Well-being", [Sector.WF], null, []),
        ["USDA Food Security Survey ratings"] = new("USDA Food Security Survey ratings", "Health and Well-being", [], null, []),
        ["Mental and emotional well-being assessments"] = new("Mental and emotional well-being assessments", "Health and Well-being", [], null, []),
        ["Health-Related Quality of Life Scale scores"] = new("Health-Related Quality of Life Scale scores", "Health and Well-being", [], null, []),

        // Instructional Quality and Practice
        ["Instructor observations"] = new("Instructor observations", "Instructional Quality and Practice", [], null, []),
        ["Overall teacher observation score"] = new("Overall teacher observation score", "Instructional Quality and Practice", [], null, []),
        ["Percentage of teachers regularly using standards-aligned; culturally responsive curricula"] = new("Percentage of teachers regularly using standards-aligned; culturally responsive curricula", "Instructional Quality and Practice", [], null, []),
        ["Teacher effectiveness student surveys (Postsecondary)"] = new("Teacher effectiveness student surveys (Postsecondary)", "Instructional Quality and Practice", [Sector.PS], null, []),
        ["Teacher effectiveness student surveys (K-12)"] = new("Teacher effectiveness student surveys (K-12)", "Instructional Quality and Practice", [Sector.K12], null, []),
        ["Teacher-child interaction measure (K-12)"] = new("Teacher-child interaction measure (K-12)", "Instructional Quality and Practice", [Sector.K12], null, []),
        ["Teacher-child interaction measure (PK)"] = new("Teacher-child interaction measure (PK)", "Instructional Quality and Practice", [Sector.PK], null, []),
        ["Subscale observation scores"] = new("Subscale observation scores", "Instructional Quality and Practice", [], null, []),

        // Postsecondary Transition and Success
        ["Bachelor's degree completion date"] = new("Bachelor's degree completion date", "Postsecondary Transition and Success", [], null, []),
        ["College selectivity level"] = new("College selectivity level", "Postsecondary Transition and Success", [], null, []),
        ["Credential-seeking status"] = new("Credential-seeking status", "Postsecondary Transition and Success", [], null, []),
        ["Credits earned in first year"] = new("Credits earned in first year", "Postsecondary Transition and Success", [], null, []),
        ["Diploma or credential award date"] = new("Diploma or credential award date", "Postsecondary Transition and Success", [], null, []),
        ["Enrollment status (current and prior years)"] = new("Enrollment status (current and prior years)", "Postsecondary Transition and Success", [], null, []),
        ["Graduate credential attainment date"] = new("Graduate credential attainment date", "Postsecondary Transition and Success", [], null, []),
        ["Graduate program enrollment date"] = new("Graduate program enrollment date", "Postsecondary Transition and Success", [], null, []),
        ["High school diploma type"] = new("High school diploma type", "Postsecondary Transition and Success", [], null, []),
        ["High school graduation date"] = new("High school graduation date", "Postsecondary Transition and Success", [], null, []),
        ["High school graduation indicator"] = new("High school graduation indicator", "Postsecondary Transition and Success", [], null, []),
        ["Institution graduation rate"] = new("Institution graduation rate", "Postsecondary Transition and Success", [], null, []),
        ["Number of credits attempted"] = new("Number of credits attempted", "Postsecondary Transition and Success", [], null, []),
        ["Number of credits earned"] = new("Number of credits earned", "Postsecondary Transition and Success", [], null, []),
        ["Post-baccalaureate program enrollment date"] = new("Post-baccalaureate program enrollment date", "Postsecondary Transition and Success", [], null, []),
        ["Postsecondary Institution ID"] = new("Postsecondary Institution ID", "Postsecondary Transition and Success", [Sector.PS], null, []),
        ["Postsecondary applications submitted"] = new("Postsecondary applications submitted", "Postsecondary Transition and Success", [Sector.PS], null, []),
        ["Postsecondary credential attainment date"] = new("Postsecondary credential attainment date", "Postsecondary Transition and Success", [Sector.PS], null, []),
        ["Postsecondary credential earned"] = new("Postsecondary credential earned", "Postsecondary Transition and Success", [Sector.PS], null, []),
        ["Postsecondary degree program length"] = new("Postsecondary degree program length", "Postsecondary Transition and Success", [Sector.PS], null, []),
        ["Postsecondary enrollment date"] = new("Postsecondary enrollment date", "Postsecondary Transition and Success", [Sector.PS], null, []),
        ["Postsecondary enrollment status (Full time/part time)"] = new("Postsecondary enrollment status (Full time/part time)", "Postsecondary Transition and Success", [Sector.PS], null, []),
        ["Postsecondary institution ID (current and prior years)"] = new("Postsecondary institution ID (current and prior years)", "Postsecondary Transition and Success", [Sector.PS], null, []),
        ["Reported intent to enroll in postsecondary education"] = new("Reported intent to enroll in postsecondary education", "Postsecondary Transition and Success", [], null, []),
        ["Transfer enrollment status"] = new("Transfer enrollment status", "Postsecondary Transition and Success", [], null, []),
        ["Transfer indicator or transfer student status"] = new("Transfer indicator or transfer student status", "Postsecondary Transition and Success", [], null, []),

        // Program Eligibility and Enrollment
        ["Enrollment in public pre-K"] = new("Enrollment in public pre-K", "Program Eligibility and Enrollment", [], null, []),
        ["Gifted and talented participation"] = new("Gifted and talented participation", "Program Eligibility and Enrollment", [], null, []),
        ["Kindergarten enrollment date"] = new("Kindergarten enrollment date", "Program Eligibility and Enrollment", [], null, []),
        ["Pre-K eligbility status"] = new("Pre-K eligbility status", "Program Eligibility and Enrollment", [Sector.PK], null, []),

        // Program Quality and Standards
        ["K-12 school type"] = new("K-12 school type", "Program Quality and Standards", [], null, []),
        ["Kindergarten program days per week"] = new("Kindergarten program days per week", "Program Quality and Standards", [], null, []),
        ["Kindergarten program hours per day"] = new("Kindergarten program hours per day", "Program Quality and Standards", [], null, []),
        ["Postsecondary institution classification"] = new("Postsecondary institution classification", "Program Quality and Standards", [Sector.PS], null, []),
        ["Pre-K program QRIS rating"] = new("Pre-K program QRIS rating", "Program Quality and Standards", [Sector.PK], null, []),
        ["Pre-K program days per week"] = new("Pre-K program days per week", "Program Quality and Standards", [Sector.PK], null, []),
        ["Pre-K program hours per day"] = new("Pre-K program hours per day", "Program Quality and Standards", [Sector.PK], null, []),

        // School Climate and Engagement
        ["Civic engagement surveys (K-12)"] = new("Civic engagement surveys (K-12)", "School Climate and Engagement", [Sector.K12], null, []),
        ["Civic engagement surveys (Postsecondary)"] = new("Civic engagement surveys (Postsecondary)", "School Climate and Engagement", [Sector.PS], null, []),
        ["Civic engagement surveys (Workforce)"] = new("Civic engagement surveys (Workforce)", "School Climate and Engagement", [Sector.WF], null, []),
        ["Growth mindset surveys (K-12)"] = new("Growth mindset surveys (K-12)", "School Climate and Engagement", [Sector.K12], null, []),
        ["Growth mindset surveys (Postsecondary)"] = new("Growth mindset surveys (Postsecondary)", "School Climate and Engagement", [Sector.PS], null, []),
        ["Growth mindset surveys (Workforce)"] = new("Growth mindset surveys (Workforce)", "School Climate and Engagement", [Sector.WF], null, []),
        ["Sense of belonging surveys (K-12)"] = new("Sense of belonging surveys (K-12)", "School Climate and Engagement", [Sector.K12], null, []),
        ["Sense of belonging surveys (PK)"] = new("Sense of belonging surveys (PK)", "School Climate and Engagement", [Sector.PK], null, []),
        ["Campus climate surveys (Postsecondary)"] = new("Campus climate surveys (Postsecondary)", "School Climate and Engagement", [Sector.PS], null, []),
        ["Sense of belonging surveys (Postsecondary)"] = new("Sense of belonging surveys (Postsecondary)", "School Climate and Engagement", [Sector.PS], null, []),
        ["Family engagement surveys (K-12)"] = new("Family engagement surveys (K-12)", "School Climate and Engagement", [Sector.K12], null, []),
        ["Family engagement surveys (PK)"] = new("Family engagement surveys (PK)", "School Climate and Engagement", [Sector.PK], null, []),
        ["Campus climate surveys (K-12)"] = new("Campus climate surveys (K-12)", "School Climate and Engagement", [Sector.K12], null, []),
        ["Self-management surveys (K-12)"] = new("Self-management surveys (K-12)", "School Climate and Engagement", [Sector.K12], null, []),
        ["Social awareness teacher ratings"] = new("Social awareness teacher ratings", "School Climate and Engagement", [], null, []),
        ["Sociocultural observational assessments"] = new("Sociocultural observational assessments", "School Climate and Engagement", [], null, []),
        ["Sense of belonging surveys"] = new("Sense of belonging surveys", "School Climate and Engagement", [], null, []),
        ["Self-efficacy surveys (K-12)"] = new("Self-efficacy surveys (K-12)", "School Climate and Engagement", [Sector.K12], null, []),
        ["Self-efficacy surveys (Postsecondary)"] = new("Self-efficacy surveys (Postsecondary)", "School Climate and Engagement", [Sector.PS], null, []),
        ["Self-efficacy surveys (Workforce)"] = new("Self-efficacy surveys (Workforce)", "School Climate and Engagement", [Sector.WF], null, []),
        ["Self-management surveys (Postsecondary)"] = new("Self-management surveys (Postsecondary)", "School Climate and Engagement", [Sector.PS], null, []),
        ["Self-management surveys (Workforce)"] = new("Self-management surveys (Workforce)", "School Climate and Engagement", [Sector.WF], null, []),
        ["Social capital surveys (K-12)"] = new("Social capital surveys (K-12)", "School Climate and Engagement", [Sector.K12], null, []),
        ["Social capital surveys (Postsecondary)"] = new("Social capital surveys (Postsecondary)", "School Climate and Engagement", [Sector.PS], null, []),
        ["Social capital surveys (Workforce)"] = new("Social capital surveys (Workforce)", "School Climate and Engagement", [Sector.WF], null, []),

        // Student Demographics
        ["Student grade level"] = new("Student grade level", "Student Demographics", [], null, []),
        ["Age"] = new("Age", "Student Demographics", [], "Numerical age as of point in time, or birth date", []),
        ["Age group"] = new("Age group", "Student Demographics", [], "Generalized category of age, e.g. adult or minor", []),
        ["Cohort graduation year"] = new("Cohort graduation year", "Student Demographics", [], null, []),
        ["Cohort year"] = new("Cohort year", "Student Demographics", [], null, []),
        ["Disability status"] = new("Disability status", "Student Demographics", [], null, []),
        ["English learner status"] = new("English learner status", "Student Demographics", [], null, []),
        ["English learner classification date"] = new("English learner classification date", "Student Demographics", [], null, []),
        ["Enrollment date"] = new("Enrollment date", "Student Demographics", [], null, []),
        ["First-generation college student"] = new("First-generation college student", "Student Demographics", [], null, []),
        ["First-time 9th grade student status"] = new("First-time 9th grade student status", "Student Demographics", [], null, []),
        ["Gender"] = new("Gender", "Student Demographics", [], null, []),
        ["Home language"] = new("Home language", "Student Demographics", [], null, []),
        ["Income level"] = new("Income level", "Student Demographics", [], null, []),
        ["Individual or family military status"] = new("Individual or family military status", "Student Demographics", [], null, []),
        ["Individual with current or past child welfare involvement"] = new("Individual with current or past child welfare involvement", "Student Demographics", [], null, []),
        ["Individuals experiencing homelessness"] = new("Individuals experiencing homelessness", "Student Demographics", [], null, []),
        ["Justice involvement"] = new("Justice involvement", "Student Demographics", [], null, []),
        ["LGBT status"] = new("LGBT status", "Student Demographics", [], null, []),
        ["Parental education level"] = new("Parental education level", "Student Demographics", [], null, []),
        ["Student race/ethnicity"] = new("Student race/ethnicity", "Student Demographics", [], null, []),
        ["Student FTE status"] = new("Student FTE status", "Student Demographics", [], null, []),
        ["Student from migrant family household"] = new("Student from migrant family household", "Student Demographics", [], null, []),
        ["Student grade level (PK)"] = new("Student grade level (PK)", "Student Demographics", [Sector.PK], null, []),
        ["Student grade level (K-12)"] = new("Student grade level (K-12)", "Student Demographics", [Sector.K12], null, []),
        ["Student or family socioeconomic status"] = new("Student or family socioeconomic status", "Student Demographics", [], null, []),
        ["Student parenting status"] = new("Student parenting status", "Student Demographics", [], null, []),
        ["Student socioeconomic status"] = new("Student socioeconomic status", "Student Demographics", [], null, []),
        ["Urbanicity"] = new("Urbanicity", "Student Demographics", [], null, []),

        // Student Support Services
        ["Advising and counseling service utilization"] = new("Advising and counseling service utilization", "Student Support Services", [], null, []),
        ["Date of services provided"] = new("Date of services provided", "Student Support Services", [], null, []),
        ["Developmental screening results"] = new("Developmental screening results", "Student Support Services", [], null, []),
        ["Early intervention screening support services referral"] = new("Early intervention screening support services referral", "Student Support Services", [], null, []),
        ["Early intervention screening results"] = new("Early intervention screening results", "Student Support Services", [], null, []),
        ["Health services offered"] = new("Health services offered", "Student Support Services", [], null, []),
        ["IECMHC services offered"] = new("IECMHC services offered", "Student Support Services", [], null, []),
        ["Indicator of whether services were provided"] = new("Indicator of whether services were provided", "Student Support Services", [], null, []),
        ["Mental health services offered"] = new("Mental health services offered", "Student Support Services", [], null, []),
        ["Social services offered"] = new("Social services offered", "Student Support Services", [], null, []),
        ["Universal screening results"] = new("Universal screening results", "Student Support Services", [], null, []),

        // System Economic Conditions
        ["College value-added"] = new("College value-added", "System Economic Conditions", [], null, []),
        ["Expenditures per student (K-12)"] = new("Expenditures per student (K-12)", "System Economic Conditions", [Sector.K12], null, []),
        ["Expenditures per student (PK)"] = new("Expenditures per student (PK)", "System Economic Conditions", [Sector.PK], null, []),
        ["Expenditures per student (Postsecondary)"] = new("Expenditures per student (Postsecondary)", "System Economic Conditions", [Sector.PS], null, []),
        ["Funding dedicated to workforce development programs"] = new("Funding dedicated to workforce development programs", "System Economic Conditions", [], null, []),
        ["Institutional expenditure per student"] = new("Institutional expenditure per student", "System Economic Conditions", [], null, []),
        ["PS institution graduation rate; by race/ethnicity and Pell grant receipt"] = new("PS institution graduation rate; by race/ethnicity and Pell grant receipt", "System Economic Conditions", [], null, []),
        ["School value-added"] = new("School value-added", "System Economic Conditions", [], null, []),
        ["Total educational funding"] = new("Total educational funding", "System Economic Conditions", [], null, []),

        // Workforce and Career Outcomes
        ["Apprenticeship program enrollment date"] = new("Apprenticeship program enrollment date", "Workforce and Career Outcomes", [], null, []),
        ["Average pay in county or MSA"] = new("Average pay in county or MSA", "Workforce and Career Outcomes", [], null, []),
        ["Benefits availability"] = new("Benefits availability", "Workforce and Career Outcomes", [], null, []),
        ["Dislocated worker status"] = new("Dislocated worker status", "Workforce and Career Outcomes", [], null, []),
        ["EAP or mental health services provided"] = new("EAP or mental health services provided", "Workforce and Career Outcomes", [], null, []),
        ["Earnings"] = new("Earnings", "Workforce and Career Outcomes", [], null, []),
        ["Employee income level"] = new("Employee income level", "Workforce and Career Outcomes", [], null, []),
        ["Employee race/ethnicity"] = new("Employee race/ethnicity", "Workforce and Career Outcomes", [], null, []),
        ["Employment date"] = new("Employment date", "Workforce and Career Outcomes", [], null, []),
        ["Employment status"] = new("Employment status", "Workforce and Career Outcomes", [], null, []),
        ["Enlistment in the military"] = new("Enlistment in the military", "Workforce and Career Outcomes", [], null, []),
        ["Enrollment in noncredit CTE date"] = new("Enrollment in noncredit CTE date", "Workforce and Career Outcomes", [], null, []),
        ["Enrollment in workforce training program"] = new("Enrollment in workforce training program", "Workforce and Career Outcomes", [], null, []),
        ["In-demand status"] = new("In-demand status", "Workforce and Career Outcomes", [], null, []),
        ["Industry-recognized credential attainment"] = new("Industry-recognized credential attainment", "Workforce and Career Outcomes", [], null, []),
        ["Learning and development plan offered"] = new("Learning and development plan offered", "Workforce and Career Outcomes", [], null, []),
        ["Location-adjusted cost of living in county or MSA"] = new("Location-adjusted cost of living in county or MSA", "Workforce and Career Outcomes", [], null, []),
        ["Net worth"] = new("Net worth", "Workforce and Career Outcomes", [], null, []),
        ["Occupation category"] = new("Occupation category", "Workforce and Career Outcomes", [], null, []),
        ["On-the-job training offered"] = new("On-the-job training offered", "Workforce and Career Outcomes", [], null, []),
        ["Participation in work-based learning"] = new("Participation in work-based learning", "Workforce and Career Outcomes", [], null, []),
        ["Job quality index"] = new("Job quality index", "Workforce and Career Outcomes", [], null, []),
        ["Workforce development program participation"] = new("Workforce development program participation", "Workforce and Career Outcomes", [Sector.WF], null, [])
    };
}

public class DataElement
{
    public string Name { get; }
    public string Category { get; }
    public List<Sector> RelatedSectors { get; }
    public string? AdditionalNotes { get; }
    public List<string> RelatedIndicatorNames { get; }
    public string? ScoringRuleName { get; }

    public DataElement(
        string name,
        string category,
        List<Sector> relatedSectors,
        string? additionalNotes,
        List<string> relatedIndicatorNames,
        string? scoringRuleName = null)
    {
        Name = name;
        Category = category;
        RelatedSectors = relatedSectors;
        AdditionalNotes = additionalNotes;
        RelatedIndicatorNames = relatedIndicatorNames;
        ScoringRuleName = scoringRuleName;
    }
}
