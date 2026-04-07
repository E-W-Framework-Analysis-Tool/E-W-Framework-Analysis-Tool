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
                AdditionalNotes = null
            },
        ["ACT score"] =
            new DataElement
            {
                Name = "ACT score",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Advising and counseling service utilization"] =
            new DataElement
            {
                Name = "Advising and counseling service utilization",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Age"] =
            new DataElement
            {
                Name = "Age",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "Numerical age as of point in time, or birth date"
            },
        ["AP course designation"] = // Should this be combined with IB and Dual Credit similar to the following element?
            new DataElement
            {
                Name = "AP course designation",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["AP, IB, or Dual Credit course credits"] =
            new DataElement
            {
                Name = "AP, IB, or Dual Credit course credits",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Apprenticeship program enrollment date"] =
            new DataElement
            {
                Name = "Apprenticeship program enrollment date",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Average cost of attendance"] =
            new DataElement
            {
                Name = "Average cost of attendance",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Average expected family contribution (EFC)"] =
            new DataElement
            {
                Name = "Average expected family contribution (EFC)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Average financial aid amount (including grants, scholarships, and tuition waivers)"] =
            new DataElement
            {
                Name = "Average financial aid amount (including grants, scholarships, and tuition waivers)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Average pay in county or MSA"] =
            new DataElement
            {
                Name = "Average pay in county or MSA",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Bachelor's degree completion date"] = // should this just be "credential attainment date"?
            new DataElement
            {
                Name = "Bachelor's degree completion date",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Basic skills level"] =
            new DataElement
            {
                Name = "Basic skills level",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Benefits availability"] =
            new DataElement
            {
                Name = "Benefits availability",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Campus climate surveys (K-12)"] =
            new DataElement
            {
                Name = "Campus climate surveys (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Campus climate surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Campus climate surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["CHIP eligibility status"] =
            new DataElement
            {
                Name = "CHIP eligibility status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["CHIP enrollment"] =
            new DataElement
            {
                Name = "CHIP enrollment",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["City or county population"] =
            new DataElement
            {
                Name = "City or county population",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Civic engagement surveys (K-12)"] =
            new DataElement
            {
                Name = "Civic engagement surveys (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Civic engagement surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Civic engagement surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Civic engagement surveys (Workforce)"] =
            new DataElement
            {
                Name = "Civic engagement surveys (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Cohort graduation year"] =
            new DataElement
            {
                Name = "Cohort graduation year",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Cohort year"] =
            new DataElement
            {
                Name = "Cohort year",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["College selectivity level"] =
            new DataElement
            {
                Name = "College selectivity level",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["College value-added"] =
            new DataElement
            {
                Name = "College value-added",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Communication skills performance assessments (K-12)"] =
            new DataElement
            {
                Name = "Communication skills performance assessments (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Communication skills performance assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Communication skills performance assessments (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Communication skills performance assessments (Workforce)"] =
            new DataElement
            {
                Name = "Communication skills performance assessments (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Commute time"] =
            new DataElement
            {
                Name = "Commute time",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Transportation",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Course department"] =
            new DataElement
            {
                Name = "Course department",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Course identifier or title"] =
            new DataElement
            {
                Name = "Course identifier or title",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Course offering by grade level"] =
            new DataElement
            {
                Name = "Course offering by grade level",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Course outcome"] =
            new DataElement
            {
                Name = "Course outcome",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = "For example, completion, failure, passage"
            },
        ["Course performance (English and Math)"] = // Is "(English and Math)" reasonable or should this be "Course performance by subject area"
            new DataElement
            {
                Name = "Course performance (English and Math)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Credential or certification type"] = // maybe more specifically "teacher credential", similar to "Teacher qualification or certification type"
            new DataElement
            {
                Name = "Credential or certification type",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Credential-seeking status"] =
            new DataElement
            {
                Name = "Credential-seeking status",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Credits earned in first year"] = // is "first year" appropriate here? or just "PS credits by year"?
            new DataElement
            {
                Name = "Credits earned in first year",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["CTE course completion"] = // Similar to "Course outcome", just a specific variety of course
            new DataElement
            {
                Name = "CTE course completion",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["CTE course ID or course title"] = // similar to "Course identifier or title" just for CTE... maybe this should be CTE course indicator?
            new DataElement
            {
                Name = "CTE course ID or course title",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["CTE pathway or career cluster associated with CTE course"] =
            new DataElement
            {
                Name = "CTE pathway or career cluster associated with CTE course",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["CTE program"] =
            new DataElement
            {
                Name = "CTE program",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Cultural competency assessments (K-12)"] =
            new DataElement
            {
                Name = "Cultural competency assessments (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Cultural competency assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Cultural competency assessments (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Cultural competency assessments (Workforce)"] =
            new DataElement
            {
                Name = "Cultural competency assessments (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Date of services provided"] =
            new DataElement
            {
                Name = "Date of services provided",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Developmental screening results"] =
            new DataElement
            {
                Name = "Developmental screening results",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Digital skills assessments (K-12)"] =
            new DataElement
            {
                Name = "Digital skills assessments (K-12)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Digital skills assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Digital skills assessments (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Digital skills assessments (Workforce)"] =
            new DataElement
            {
                Name = "Digital skills assessments (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Diploma or credential award date"] = // related to High school graduation date, which we already have -- this seems to be exclusively about ACGR metric for HS, skipping
            new DataElement
            {
                Name = "Diploma or credential award date",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Direct child assessments (cognition)"] =
            new DataElement
            {
                Name = "Direct child assessments (cognition)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Direct child assessments (language and literacy)"] =
            new DataElement
            {
                Name = "Direct child assessments (language and literacy)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Direct child assessments of executive function"] =
            new DataElement
            {
                Name = "Direct child assessments of executive function",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Direct child assessments of physical development"] =
            new DataElement
            {
                Name = "Direct child assessments of physical development",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Disability status"] =
            new DataElement
            {
                Name = "Disability status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Dislocated worker status"] =
            new DataElement
            {
                Name = "Dislocated worker status",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Dual credit course designation"] = // May want to combine with AP / IB
            new DataElement
            {
                Name = "Dual credit course designation",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["EAP or mental health services provided"] =
            new DataElement
            {
                Name = "EAP or mental health services provided",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Early intervention screening results"] =
            new DataElement
            {
                Name = "Early intervention screening results",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Early intervention screening services referral status"] =
            new DataElement
            {
                Name = "Early intervention screening services referral status",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Earnings"] =
            new DataElement
            {
                Name = "Earnings",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Eligibility for federal rental assistance"] =
            new DataElement
            {
                Name = "Eligibility for federal rental assistance",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Employee income level"] = // This one is similar to "Earnings" but is more about diversity in a workplace
            new DataElement
            {
                Name = "Employee income level",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Employee race/ethnicity"] = // This one is similar to Staff race/ethnicity... but more oriented around workplace rather than school employees
            new DataElement
            {
                Name = "Employee race/ethnicity",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Employment date"] = // specifically in the context of someone in the workforce, not staff
            new DataElement
            {
                Name = "Employment date",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Employment status"] = // specifically in the context of someone in the workforce, not staff
            new DataElement
            {
                Name = "Employment status",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["English learner classification date"] =
            new DataElement
            {
                Name = "English learner classification date",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["English learner status"] =
            new DataElement
            {
                Name = "English learner status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Enlistment in the military"] =
            new DataElement
            {
                Name = "Enlistment in the military",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Enrollment date"] = // Related to other enrollment dates, pre-K, kindergarten, postsecondary, graduate program, etc., where this is defined in the metrics too it seems "Cohort year" is actually more precise
            new DataElement
            {
                Name = "Enrollment date",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Enrollment in noncredit CTE date"] =
            new DataElement
            {
                Name = "Enrollment in noncredit CTE date",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Enrollment in public pre-K"] =
            new DataElement
            {
                Name = "Enrollment in public pre-K",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Enrollment in workforce training program"] =
            new DataElement
            {
                Name = "Enrollment in workforce training program",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Enrollment status (current and prior years)"] = // should this be split into K-12 vs PS? Indicator for question 15 seems to be PS oriented specifically, there are separate tables for enrollment
            new DataElement
            {
                Name = "Enrollment status (current and prior years)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Expenditures per student (K-12)"] =
            new DataElement
            {
                Name = "Expenditures per student (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Expenditures per student (PK)"] =
            new DataElement
            {
                Name = "Expenditures per student (PK)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Expenditures per student (Postsecondary)"] =
            new DataElement
            {
                Name = "Expenditures per student (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["FAFSA completion date"] =
            new DataElement
            {
                Name = "FAFSA completion date",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Family eligibility for child care subsidies"] =
            new DataElement
            {
                Name = "Family eligibility for child care subsidies",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Family engagement surveys (K-12)"] =
            new DataElement
            {
                Name = "Family engagement surveys (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Family engagement surveys (PK)"] =
            new DataElement
            {
                Name = "Family engagement surveys (PK)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "School Climate",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["First-generation college student"] =
            new DataElement
            {
                Name = "First-generation college student",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["First-time 9th grade student status"] = // can't find this
            new DataElement
            {
                Name = "First-time 9th grade student status",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Funding dedicated to workforce development programs"] =
            new DataElement
            {
                Name = "Funding dedicated to workforce development programs",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Gateway course completion"] =
            new DataElement
            {
                Name = "Gateway course completion",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Gender"] =
            new DataElement
            {
                Name = "Gender",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Geographical indicator"] =
            new DataElement
            {
                Name = "Geographical indicator",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = "For example, census tract, city, or county"
            },
        ["Gifted and talented participation"] =
            new DataElement
            {
                Name = "Gifted and talented participation",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Grade point average (K-12)"] =
            new DataElement
            {
                Name = "Grade point average (K-12)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Grade point average (Postsecondary)"] =
            new DataElement
            {
                Name = "Grade point average (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Graduate credential attainment date"] =
            new DataElement
            {
                Name = "Graduate credential attainment date",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Graduate program enrollment date"] =
            new DataElement
            {
                Name = "Graduate program enrollment date",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Growth mindset surveys (K-12)"] =
            new DataElement
            {
                Name = "Growth mindset surveys (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Growth mindset surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Growth mindset surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Growth mindset surveys (Workforce)"] =
            new DataElement
            {
                Name = "Growth mindset surveys (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Health services offered"] =
            new DataElement
            {
                Name = "Health services offered",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Health-Related Quality of Life Scale scores"] =
            new DataElement
            {
                Name = "Health-Related Quality of Life Scale scores",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["High school diploma type"] =
            new DataElement
            {
                Name = "High school diploma type",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["High school graduation date"] =
            new DataElement
            {
                Name = "High school graduation date",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["High school graduation indicator"] =
            new DataElement
            {
                Name = "High school graduation indicator",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Higher-order thinking skills performance assessments (K-12)"] =
            new DataElement
            {
                Name = "Higher-order thinking skills performance assessments (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Higher-order thinking skills performance assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Higher-order thinking skills performance assessments (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Higher-order thinking skills performance assessments (Workforce)"] =
            new DataElement
            {
                Name = "Higher-order thinking skills performance assessments (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Highest level of education completed"] =
            new DataElement
            {
                Name = "Highest level of education completed",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Home language"] =
            new DataElement
            {
                Name = "Home language",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["IB course designation"] =
            new DataElement
            {
                Name = "IB course designation",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["IECMHC services offered"] =
            new DataElement
            {
                Name = "IECMHC services offered",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["In-demand status"] =
            new DataElement
            {
                Name = "In-demand status",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Income level"] =
            new DataElement
            {
                Name = "Income level",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Indicator of access to desktop or laptop at home"] =
            new DataElement
            {
                Name = "Indicator of access to desktop or laptop at home",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Indicator of access to reliable broadband internet"] =
            new DataElement
            {
                Name = "Indicator of access to reliable broadband internet",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Indicator of whether services were provided"] =
            new DataElement
            {
                Name = "Indicator of whether services were provided",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Individual or family military status"] =
            new DataElement
            {
                Name = "Individual or family military status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Individual with current or past child welfare involvement"] =
            new DataElement
            {
                Name = "Individual with current or past child welfare involvement",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Individuals experiencing homelessness"] =
            new DataElement
            {
                Name = "Individuals experiencing homelessness",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Industry-recognized credential attainment"] =
            new DataElement
            {
                Name = "Industry-recognized credential attainment",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Institution graduation rate"] =
            new DataElement
            {
                Name = "Institution graduation rate",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Institutional expenditure per student"] =
            new DataElement
            {
                Name = "Institutional expenditure per student",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Instructor observations"] =
            new DataElement
            {
                Name = "Instructor observations",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Insured status"] =
            new DataElement
            {
                Name = "Insured status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Job quality index"] =
            new DataElement
            {
                Name = "Job quality index",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Job title or position type"] =
            new DataElement
            {
                Name = "Job title or position type",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Justice involvement"] =
            new DataElement
            {
                Name = "Justice involvement",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["K-12 school type"] =
            new DataElement
            {
                Name = "K-12 school type",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Kindergarten enrollment date"] =
            new DataElement
            {
                Name = "Kindergarten enrollment date",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Kindergarten program days per week"] =
            new DataElement
            {
                Name = "Kindergarten program days per week",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Kindergarten program hours per day"] =
            new DataElement
            {
                Name = "Kindergarten program hours per day",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Kindergarten readiness assessments (cognition)"] =
            new DataElement
            {
                Name = "Kindergarten readiness assessments (cognition)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Leader effectiveness assessments"] =
            new DataElement
            {
                Name = "Leader effectiveness assessments",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Learning and development plan offered"] =
            new DataElement
            {
                Name = "Learning and development plan offered",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["LGBT status"] =
            new DataElement
            {
                Name = "LGBT status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Location-adjusted cost of living in county or MSA"] =
            new DataElement
            {
                Name = "Location-adjusted cost of living in county or MSA",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Low Transportation Cost Index"] =
            new DataElement
            {
                Name = "Low Transportation Cost Index",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Math proficiency (Grades 1 and 2)"] =
            new DataElement
            {
                Name = "Math proficiency (Grades 1 and 2)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Median student debt"] =
            new DataElement
            {
                Name = "Median student debt",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Medicaid eligibility status"] =
            new DataElement
            {
                Name = "Medicaid eligibility status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Medicaid enrollment"] =
            new DataElement
            {
                Name = "Medicaid enrollment",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Mental and emotional well-being assessments"] =
            new DataElement
            {
                Name = "Mental and emotional well-being assessments",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Mental health services offered"] =
            new DataElement
            {
                Name = "Mental health services offered",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Net worth"] =
            new DataElement
            {
                Name = "Net worth",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Number of ACEs"] =
            new DataElement
            {
                Name = "Number of ACEs",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Number of affordable housing units in city or county"] =
            new DataElement
            {
                Name = "Number of affordable housing units in city or county",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Number of city or county residents experiencing poverty"] =
            new DataElement
            {
                Name = "Number of city or county residents experiencing poverty",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Number of city or county residents living in a high poverty neighborhood"] =
            new DataElement
            {
                Name = "Number of city or county residents living in a high poverty neighborhood",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Number of credits attempted"] =
            new DataElement
            {
                Name = "Number of credits attempted",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Number of credits earned"] =
            new DataElement
            {
                Name = "Number of credits earned",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Number of days suspended (K-12)"] =
            new DataElement
            {
                Name = "Number of days suspended (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Number of days suspended (PK)"] =
            new DataElement
            {
                Name = "Number of days suspended (PK)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Number of households with low income in city or county"] =
            new DataElement
            {
                Name = "Number of households with low income in city or county",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Number of households with very low income in city or county"] =
            new DataElement
            {
                Name = "Number of households with very low income in city or county",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Number of juvenile arrests in city or county"] =
            new DataElement
            {
                Name = "Number of juvenile arrests in city or county",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Number of property felonies in city or county"] =
            new DataElement
            {
                Name = "Number of property felonies in city or county",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Number of violent felonies in city or county"] =
            new DataElement
            {
                Name = "Number of violent felonies in city or county",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Occupation category"] =
            new DataElement
            {
                Name = "Occupation category",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Office referrals (K-12)"] =
            new DataElement
            {
                Name = "Office referrals (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Office referrals (PK)"] =
            new DataElement
            {
                Name = "Office referrals (PK)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["On-the-job training offered"] =
            new DataElement
            {
                Name = "On-the-job training offered",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Employment & Earnings",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Overall teacher observation score"] =
            new DataElement
            {
                Name = "Overall teacher observation score",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Parental education level"] =
            new DataElement
            {
                Name = "Parental education level",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Participation in work-based learning"] =
            new DataElement
            {
                Name = "Participation in work-based learning",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Pell grant receipt"] =
            new DataElement
            {
                Name = "Pell grant receipt",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Percentage of teachers regularly using standards-aligned; culturally responsive curricula"] =
            new DataElement
            {
                Name = "Percentage of teachers regularly using standards-aligned; culturally responsive curricula",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Physical health surveys (K-12)"] =
            new DataElement
            {
                Name = "Physical health surveys (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Physical health surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Physical health surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Physical health surveys (Workforce)"] =
            new DataElement
            {
                Name = "Physical health surveys (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Post-baccalaureate program enrollment date"] =
            new DataElement
            {
                Name = "Post-baccalaureate program enrollment date",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary applications submitted"] =
            new DataElement
            {
                Name = "Postsecondary applications submitted",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary credential attainment date"] =
            new DataElement
            {
                Name = "Postsecondary credential attainment date",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary credential earned"] =
            new DataElement
            {
                Name = "Postsecondary credential earned",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary degree program length"] =
            new DataElement
            {
                Name = "Postsecondary degree program length",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary enrollment date"] =
            new DataElement
            {
                Name = "Postsecondary enrollment date",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary enrollment status (Full time/part time)"] =
            new DataElement
            {
                Name = "Postsecondary enrollment status (Full time/part time)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary institution classification"] =
            new DataElement
            {
                Name = "Postsecondary institution classification",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary Institution ID"] =
            new DataElement
            {
                Name = "Postsecondary Institution ID",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary institution ID (current and prior years)"] =
            new DataElement
            {
                Name = "Postsecondary institution ID (current and prior years)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Postsecondary major"] =
            new DataElement
            {
                Name = "Postsecondary major",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Pre-K eligibility status"] =
            new DataElement
            {
                Name = "Pre-K eligibility status",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Pre-K program days per week"] =
            new DataElement
            {
                Name = "Pre-K program days per week",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Pre-K program hours per day"] =
            new DataElement
            {
                Name = "Pre-K program hours per day",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Pre-K program QRIS rating"] =
            new DataElement
            {
                Name = "Pre-K program QRIS rating",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Reading proficiency (Grades 1 and 2)"] =
            new DataElement
            {
                Name = "Reading proficiency (Grades 1 and 2)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Receipt of child care subsidies"] =
            new DataElement
            {
                Name = "Receipt of child care subsidies",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Receipt of federal rental assistance"] =
            new DataElement
            {
                Name = "Receipt of federal rental assistance",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Repayment phase start date"] =
            new DataElement
            {
                Name = "Repayment phase start date",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Repayment status"] =
            new DataElement
            {
                Name = "Repayment status",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Reported intent to enroll in postsecondary education"] =
            new DataElement
            {
                Name = "Reported intent to enroll in postsecondary education",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Reported kindergarten readiness (behavioral skills)"] =
            new DataElement
            {
                Name = "Reported kindergarten readiness (behavioral skills)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes =
                    "Teacher-reported kindergarten readiness assessments (self-regulation; approaches to learning; executive function)"
            },
        ["Reported kindergarten readiness (language and literacy)"] =
            new DataElement
            {
                Name = "Reported kindergarten readiness (language and literacy)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported kindergarten readiness assessments (language and literacy)"
            },
        ["Reported kindergarten readiness (physical development)"] =
            new DataElement
            {
                Name = "Reported kindergarten readiness (physical development)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported kindergarten readiness assessments (physical development)"
            },
        ["Reported kindergarten readiness (social-emotional skills)"] =
            new DataElement
            {
                Name = "Reported kindergarten readiness (social-emotional skills)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = "Teacher-reported kindergarten readiness assessments (social-emotional development)"
            },
        ["Restraint and seclusion for discipline (K-12)"] =
            new DataElement
            {
                Name = "Restraint and seclusion for discipline (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Restraint and seclusion for discipline (PK)"] =
            new DataElement
            {
                Name = "Restraint and seclusion for discipline (PK)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Restraint and seclusion for safety (K-12)"] =
            new DataElement
            {
                Name = "Restraint and seclusion for safety (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Restraint and seclusion for safety (PK)"] =
            new DataElement
            {
                Name = "Restraint and seclusion for safety (PK)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["SAT completion"] =
            new DataElement
            {
                Name = "SAT completion",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["SAT score"] =
            new DataElement
            {
                Name = "SAT score",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["School assignment (prior and current year)"] =
            new DataElement
            {
                Name = "School assignment (prior and current year)",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["School value-added"] =
            new DataElement
            {
                Name = "School value-added",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Academic Performance",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Self-efficacy surveys (K-12)"] =
            new DataElement
            {
                Name = "Self-efficacy surveys (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Self-efficacy surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Self-efficacy surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Self-efficacy surveys (Workforce)"] =
            new DataElement
            {
                Name = "Self-efficacy surveys (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Self-management surveys (K-12)"] =
            new DataElement
            {
                Name = "Self-management surveys (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Self-management surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Self-management surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Self-management surveys (Workforce)"] =
            new DataElement
            {
                Name = "Self-management surveys (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Sense of belonging surveys"] =
            new DataElement
            {
                Name = "Sense of belonging surveys",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Sense of belonging surveys (K-12)"] =
            new DataElement
            {
                Name = "Sense of belonging surveys (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Sense of belonging surveys (PK)"] =
            new DataElement
            {
                Name = "Sense of belonging surveys (PK)",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Sense of belonging surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Sense of belonging surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["SGP for standardized assessments"] =
            new DataElement
            {
                Name = "SGP for standardized assessments",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = "Subject specific for reading/literacy; math; and science"
            },
        ["SNAP eligibility"] =
            new DataElement
            {
                Name = "SNAP eligibility",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["SNAP participation"] =
            new DataElement
            {
                Name = "SNAP participation",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Social awareness teacher ratings"] =
            new DataElement
            {
                Name = "Social awareness teacher ratings",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Social capital surveys (K-12)"] =
            new DataElement
            {
                Name = "Social capital surveys (K-12)",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Social capital surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Social capital surveys (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Social capital surveys (Workforce)"] =
            new DataElement
            {
                Name = "Social capital surveys (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Social proficiency performance assessments (Postsecondary)"] =
            new DataElement
            {
                Name = "Social proficiency performance assessments (Postsecondary)",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Social proficiency performance assessments (Workforce)"] =
            new DataElement
            {
                Name = "Social proficiency performance assessments (Workforce)",
                ClusterOnlyCategory = "Workforce Success",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Social services offered"] =
            new DataElement
            {
                Name = "Social services offered",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Student Support Services",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Sociocultural observational assessments"] =
            new DataElement
            {
                Name = "Sociocultural observational assessments",
                ClusterOnlyCategory = "Social-Emotional Learning",
                DataElementCategory = "Social-Emotional Learning (SEL)",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Staff FTE status"] =
            new DataElement
            {
                Name = "Staff FTE status",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Staff race/ethnicity"] =
            new DataElement
            {
                Name = "Staff race/ethnicity",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["State standardized test (Math proficiency)"] =
            new DataElement
            {
                Name = "State standardized test (Math proficiency)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["State standardized test (Reading proficiency)"] =
            new DataElement
            {
                Name = "State standardized test (Reading proficiency)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Assessments",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Student attendance rate (K-12)"] =
            new DataElement
            {
                Name = "Student attendance rate (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Student attendance rate (PK)"] =
            new DataElement
            {
                Name = "Student attendance rate (PK)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Student course enrollment record"] =
            new DataElement
            {
                Name = "Student course enrollment record",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Academic Coursework",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Student from migrant family household"] =
            new DataElement
            {
                Name = "Student from migrant family household",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Student FTE status"] =
            new DataElement
            {
                Name = "Student FTE status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Student grade level"] =
            new DataElement
            {
                Name = "Student grade level",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Student grade level (K-12)"] =
            new DataElement
            {
                Name = "Student grade level (K-12)",
                ClusterOnlyCategory = "Postsecondary Transitions",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Student grade level (PK)"] =
            new DataElement
            {
                Name = "Student grade level (PK)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Attendance & Enrollment",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Student or family socioeconomic status"] =
            new DataElement
            {
                Name = "Student or family socioeconomic status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["Student parenting status"] =
            new DataElement
            {
                Name = "Student parenting status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Student race/ethnicity"] =
            new DataElement
            {
                Name = "Student race/ethnicity",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Student socioeconomic status"] =
            new DataElement
            {
                Name = "Student socioeconomic status",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Demographics",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Subscale observation scores"] =
            new DataElement
            {
                Name = "Subscale observation scores",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Suspensions and expulsions (K-12)"] =
            new DataElement
            {
                Name = "Suspensions and expulsions (K-12)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Suspensions and expulsions (PK)"] =
            new DataElement
            {
                Name = "Suspensions and expulsions (PK)",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Discipline & Behavior",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Teacher effectiveness student surveys (K-12)"] =
            new DataElement
            {
                Name = "Teacher effectiveness student surveys (K-12)",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Teacher effectiveness student surveys (Postsecondary)"] =
            new DataElement
            {
                Name = "Teacher effectiveness student surveys (Postsecondary)",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Teacher qualification or certification type"] =
            new DataElement
            {
                Name = "Teacher qualification or certification type",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Teacher reports of executive function"] =
            new DataElement
            {
                Name = "Teacher reports of executive function",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Teacher reports of social-emotional development"] =
            new DataElement
            {
                Name = "Teacher reports of social-emotional development",
                ClusterOnlyCategory = "Kindergarten Readiness",
                DataElementCategory = "Early Learning",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Teacher-child interaction measure (K-12)"] =
            new DataElement
            {
                Name = "Teacher-child interaction measure (K-12)",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Teacher-child interaction measure (PK)"] =
            new DataElement
            {
                Name = "Teacher-child interaction measure (PK)",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK],
                AdditionalNotes = null
            },
        ["Teaching assignment"] =
            new DataElement
            {
                Name = "Teaching assignment",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Total educational funding"] =
            new DataElement
            {
                Name = "Total educational funding",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Finance & Resources",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS],
                AdditionalNotes = null
            },
        ["Total net price of education plus interest"] =
            new DataElement
            {
                Name = "Total net price of education plus interest",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Financial Aid & Affordability",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Transfer enrollment status"] =
            new DataElement
            {
                Name = "Transfer enrollment status",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Transfer indicator or transfer student status"] =
            new DataElement
            {
                Name = "Transfer indicator or transfer student status",
                ClusterOnlyCategory = "Postsecondary Success",
                DataElementCategory = "Postsecondary Outcomes",
                RelatedSectors = [Sector.PS],
                AdditionalNotes = null
            },
        ["Universal screening results"] =
            new DataElement
            {
                Name = "Universal screening results",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Urbanicity"] =
            new DataElement
            {
                Name = "Urbanicity",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["USDA Food Access Research Atlas Access Level Flag"] =
            new DataElement
            {
                Name = "USDA Food Access Research Atlas Access Level Flag",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Community Context",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["USDA Food Security Survey ratings"] =
            new DataElement
            {
                Name = "USDA Food Security Survey ratings",
                ClusterOnlyCategory = "School Climate",
                DataElementCategory = "Health & Wellness",
                RelatedSectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
                AdditionalNotes = null
            },
        ["VAM for subject specific assessment"] =
            new DataElement
            {
                Name = "VAM for subject specific assessment",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.K12],
                AdditionalNotes = null
            },
        ["Workforce development program participation"] =
            new DataElement
            {
                Name = "Workforce development program participation",
                ClusterOnlyCategory = "Workforce Transitions",
                DataElementCategory = "Career & Technical Education (CTE)",
                RelatedSectors = [Sector.WF],
                AdditionalNotes = null
            },
        ["Years in current position"] =
            new DataElement
            {
                Name = "Years in current position",
                ClusterOnlyCategory = "Teaching Effectiveness",
                DataElementCategory = "Staff & Educators",
                RelatedSectors = [Sector.PK, Sector.K12],
                AdditionalNotes = null
            },
        ["Years of teaching experience"] = new DataElement
        {
            Name = "Years of teaching experience",
            ClusterOnlyCategory = "Teaching Effectiveness",
            DataElementCategory = "Staff & Educators",
            RelatedSectors = [Sector.K12],
            AdditionalNotes = null
        },
    };
}

public class DataElement
{
    public required string Name { get; init; }
    public required string ClusterOnlyCategory { get; init; }
    public required string DataElementCategory { get; init; }
    public required List<Sector> RelatedSectors { get; init; }
    public string? AdditionalNotes { get; init; }
    public string? ScoringRuleName { get; init; }
}
