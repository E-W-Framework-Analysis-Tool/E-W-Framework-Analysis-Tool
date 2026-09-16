using EwFrameworkAnalysis.Common.Models.Framework;

namespace EwFrameworkAnalysis.Common.FrameworkReferenceData;

public static class EwFrameworkDisaggregates
{
    public static IReadOnlyList<Disaggregate> Disaggregates { get; } =
    [
        new Disaggregate
        {
            Name = "Age group",
            Description = "Age-based grouping used to analyze trends and outcomes across developmental or workforce stages.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames =
            [
                "Age",
            ]
        },
        new Disaggregate
        {
            Name = "Attendance intensity",
            Description = "The extent of participation or enrollment, such as full-time versus part-time attendance.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS],
            DataElementNames =
            [
                "Student attendance rate (K-12)",
                "Student attendance rate (PK)",
                "Postsecondary enrollment status (Full time/part time)",
            ]
        },
        new Disaggregate
        {
            Name = "Basic skills level",
            Description = "Literacy, numeracy, or foundational skill proficiency relevant to workforce participation.",
            Sectors = [Sector.WF],
            DataElementNames =
            [
                "Basic skills level",
            ]
        },
        new Disaggregate
        {
            Name = "Credential-seeking status",
            Description = "Whether a postsecondary student is formally seeking a credential, degree, or certificate.",
            Sectors = [Sector.PS],
            DataElementNames =
            [
                "Credential-seeking status",
            ]
        },
        new Disaggregate
        {
            Name = "Disability status",
            Description = "Whether an individual has a disability or receives disability-related supports or accommodations.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames =
            [
                "Disability status",
            ]
        },
        new Disaggregate
        {
            Name = "Dislocated worker status",
            Description = "Whether an individual lost employment because of layoffs, closures, or economic conditions.",
            Sectors = [Sector.PS, Sector.WF],
            DataElementNames =
            [
                "Dislocated worker status",
            ]
        },
        new Disaggregate
        {
            Name = "English learner",
            Description = "Whether an individual is identified as needing support to develop English proficiency.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames =
            [
                "English learner status",
            ]
        },
        new Disaggregate
        {
            Name = "First-generation college student",
            Description = "Whether a student's parents or guardians have completed a postsecondary degree.",
            Sectors = [Sector.K12, Sector.PS],
            DataElementNames =
            [
                "First-generation college student",
            ]
        },
        new Disaggregate
        {
            Name = "Gender",
            Description = "Self-identified gender used to analyze gender-based disparities in education and workforce outcomes.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames =
            [
                "Gender",
            ]
        },
        new Disaggregate
        {
            Name = "Home language",
            Description = "Primary language spoken at home or by the individual.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames =
            [
                "Home language",
            ]
        },
        new Disaggregate
        {
            Name = "Income level",
            Description = "Household or individual income level, often used as a proxy for socioeconomic status.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames =
            [
                "Income level (individual/family)",
            ]
        },
        new Disaggregate
        {
            Name = "Individual or family military status",
            Description = "Whether an individual or family member serves or has served in the military.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames =
            [
                "Military status (individual/family)",
            ]
        },
        new Disaggregate
        {
            Name = "Individual with current or past child welfare involvement",
            Description = "Whether an individual has current or prior involvement with foster care or child welfare systems.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS],
            DataElementNames =
            [
                "Individual with current or past child welfare involvement",
            ]
        },
        new Disaggregate
        {
            Name = "Individuals experiencing homelessness",
            Description = "Whether an individual lacks stable, permanent, or adequate housing.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames =
            [
                "Individuals experiencing homelessness",
            ]
        },
        new Disaggregate
        {
            Name = "Justice involvement",
            Description = "Whether an individual has interacted with the juvenile or criminal justice system in any capacity.",
            Sectors = [Sector.K12, Sector.PS, Sector.WF],
            DataElementNames =
            [
                "Justice involvement",
            ]
        },
        new Disaggregate
        {
            Name = "K-12 school type",
            Description = "The type of school attended, such as traditional public, charter, magnet, private, or alternative school.",
            Sectors = [Sector.K12],
            DataElementNames =
            [
                "K-12 school type",
            ]
        },
        new Disaggregate
        {
            Name = "LGBT status",
            Description = "Sexual orientation and gender identity information used to understand disparities affecting LGBT individuals.",
            Sectors = [Sector.K12, Sector.PS, Sector.WF],
            DataElementNames =
            [
                "LGBT status",
            ]
        },
        new Disaggregate
        {
            Name = "Occupation category",
            Description = "Occupational grouping or job category associated with employment.",
            Sectors = [Sector.WF],
            DataElementNames =
            [
                "Occupation category",
            ]
        },
        new Disaggregate
        {
            Name = "Parental education level",
            Description = "Highest educational attainment of either parent or guardian.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames =
            [
                "Parental education level",
            ]
        },
        new Disaggregate
        {
            Name = "Postsecondary institution classification",
            Description = "The classification or type of postsecondary institution attended, such as two-year, four-year, public, or private.",
            Sectors = [Sector.PS],
            DataElementNames =
            [
                "Postsecondary institution classification",
            ]
        },
        new Disaggregate
        {
            Name = "Postsecondary major",
            Description = "The academic field or program of study pursued in postsecondary education.",
            Sectors = [Sector.PS, Sector.WF],
            DataElementNames =
            [
                "Postsecondary major",
            ]
        },
        new Disaggregate
        {
            Name = "Race and ethnicity",
            Description = "Self-reported race and ethnicity used to identify disparities and inequities across systems and outcomes.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames =
            [
                "Race and ethnicity (individual)",
            ]
        },
        new Disaggregate
        {
            Name = "Student from migrant family household",
            Description = "Whether a student belongs to a migrant family household, often associated with migratory agricultural or fishing work.",
            Sectors = [Sector.PK, Sector.K12],
            DataElementNames =
            [
                "Student from migrant family household",
            ]
        },
        new Disaggregate
        {
            Name = "Student parenting status",
            Description = "Whether a postsecondary student is a parent or has dependent children.",
            Sectors = [Sector.PS],
            DataElementNames =
            [
                "Student parenting status",
            ]
        },
        new Disaggregate
        {
            Name = "Transfer enrollment status",
            Description = "Whether a postsecondary student transferred from another institution.",
            Sectors = [Sector.PS],
            DataElementNames =
            [
                "Transfer enrollment status",
            ]
        },
        new Disaggregate
        {
            Name = "Urbanicity",
            Description = "Whether an individual or institution is located in an urban, suburban, town, or rural area.",
            Sectors = [Sector.PK, Sector.K12, Sector.PS, Sector.WF],
            DataElementNames =
            [
                "Urbanicity",
            ]
        },
    ];
}
