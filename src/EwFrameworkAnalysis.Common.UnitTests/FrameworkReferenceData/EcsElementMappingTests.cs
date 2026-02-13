using EwFrameworkAnalysis.Common.FrameworkReferenceData;

namespace EwFrameworkAnalysis.Common.UnitTests.FrameworkReferenceData;

public class EcsElementMappingTests
{
    [Theory]
    [InlineData("In school suspension", "K-12", "Suspensions and expulsions (K-12)")]
    [InlineData("Out of school suspension", "K-12", "Suspensions and expulsions (K-12)")]
    [InlineData("Expulsions", "K-12", "Suspensions and expulsions (K-12)")]
    [InlineData("Expulsion", "Pre-K", "Suspensions and expulsions (PK)")]
    [InlineData("Suspension", "K-12", "Suspensions and expulsions (K-12)")]
    [InlineData("In school suspension", "Pre-K", "Suspensions and expulsions (PK)")]
    [InlineData("Disciplinary use of restraint and seclusion", "K-12", "Restraint and seclusion (K-12)")]
    [InlineData("Disciplinary use of restraint and seclusion", "Pre-K", "Restraint and seclusion (PK)")]
    [InlineData("Student attendance rate", "K-12", "Student attendance rate (K-12)")]
    [InlineData("Student attendance rate", "Pre-K", "Student attendance rate (PK)")]
    [InlineData("Student grade level", "K-12", "Student grade level (K-12)")]
    [InlineData("Student grade level", "Pre-K", "Student grade level (PK)")]
    [InlineData("GPA", "K-12", "Grade point average (K-12)")]
    [InlineData("Grade point average", "K-12", "Grade point average (K-12)")]
    public void Should_MapSectorSpecificElements_When_SectorProvided(string ecsName, string sector, string expectedFrameworkName)
    {
        var result = EcsElementMapping.MapToFrameworkElement(ecsName, sector);
        Assert.Equal(expectedFrameworkName, result);
    }

    [Theory]
    [InlineData("Math proficiency on state standardized test", "State standardized test (Math proficiency)")]
    [InlineData("Math proficiency (or math assessment score)", "State standardized test (Math proficiency)")]
    [InlineData("Reading proficiency on state standardized test", "State standardized test (Reading proficiency)")]
    [InlineData("Reading proficiency (or reading assessment score)", "State standardized test (Reading proficiency)")]
    [InlineData("Course performance in English", "Course performance (English and Math)")]
    [InlineData("Course performance in Math", "Course performance (English and Math)")]
    [InlineData("Results on direct child assessment (cognition)", "Direct child assessments (cognition)")]
    [InlineData("Results on direct child assessment (language and literacy)", "Direct child assessments (language and literacy)")]
    [InlineData("Results on kindergarten readiness assessment (cognition)", "Kindergarten readiness assessments (cognition)")]
    [InlineData("Results on teacher-reported kindergarten readiness assessment (language and literacy)", "Teacher-reported kindergarten readiness assessments (language and literacy)")]
    [InlineData("Course ID or course title", "Course identifier or title")]
    [InlineData("Course failure", "Course outcome")]
    [InlineData("Course passage or completion", "Course outcome")]
    [InlineData("Age (or birth date)", "Age")]
    [InlineData("AP enrollment", "Student course enrollment record")]
    [InlineData("IB enrollment", "Student course enrollment record")]
    [InlineData("Dual credit enrollment", "Dual credit course designation")]
    [InlineData("PS institution graduation rate, by race/ethnicity and Pell grant receipt", "PS institution graduation rate; by race/ethnicity and Pell grant receipt")]
    public void Should_MapDirectElements_When_NoSectorNeeded(string ecsName, string expectedFrameworkName)
    {
        var result = EcsElementMapping.MapToFrameworkElement(ecsName, null);
        Assert.Equal(expectedFrameworkName, result);
    }

    [Theory]
    [InlineData("ACT completion")]
    [InlineData("ACT score")]
    [InlineData("SAT completion")]
    [InlineData("SAT score")]
    [InlineData("AP course passage")]
    [InlineData("High school graduation date")]
    [InlineData("FAFSA completion date")]
    [InlineData("Enrollment date")]
    [InlineData("Cohort year")]
    public void Should_MapExactMatchElements(string ecsName)
    {
        var result = EcsElementMapping.MapToFrameworkElement(ecsName, null);
        Assert.Equal(ecsName, result);
    }

    [Fact]
    public void Should_ReturnNull_When_ElementNotMapped()
    {
        var result = EcsElementMapping.MapToFrameworkElement("Some Random Element That Does Not Exist", null);
        Assert.Null(result);
    }

    [Theory]
    [InlineData("ACT completion")]
    [InlineData("ACT score")]
    [InlineData("SAT completion")]
    [InlineData("SAT score")]
    [InlineData("AP course passage")]
    [InlineData("AP credit earned (or AP test scores)")]
    [InlineData("IB course passage")]
    [InlineData("IB credit earned (or IB test scores)")]
    [InlineData("Dual credit course passage")]
    [InlineData("Dual credit earned")]
    [InlineData("High school graduation date")]
    [InlineData("High school graduation indicator")]
    [InlineData("High school diploma type")]
    [InlineData("Diploma or credential award date")]
    [InlineData("FAFSA completion date")]
    [InlineData("Postsecondary applications submitted")]
    [InlineData("Postsecondary enrollment date")]
    [InlineData("Postsecondary Institution ID")]
    [InlineData("Reported intent to enroll in postsecondary education")]
    [InlineData("Enrollment in public pre-K")]
    [InlineData("Pre-K eligbility status")]
    [InlineData("Cohort graduation year")]
    [InlineData("Cohort year")]
    [InlineData("Enrollment date")]
    [InlineData("First-time 9th grade student status")]
    public void Should_MapToValidFrameworkElements(string ecsName)
    {
        var result = EcsElementMapping.MapToFrameworkElement(ecsName, null);
        Assert.NotNull(result);
        Assert.True(
            EwFrameworkDataElements.Elements.ContainsKey(result),
            $"Mapped result '{result}' for ECS element '{ecsName}' is not a valid framework data element"
        );
    }

    [Theory]
    [InlineData("In school suspension", "K-12")]
    [InlineData("Disciplinary use of restraint and seclusion", "K-12")]
    [InlineData("Student attendance rate", "K-12")]
    [InlineData("Student grade level", "K-12")]
    [InlineData("GPA", "K-12")]
    [InlineData("Math proficiency on state standardized test", null)]
    [InlineData("Reading proficiency on state standardized test", null)]
    [InlineData("Course ID or course title", null)]
    [InlineData("Age (or birth date)", null)]
    [InlineData("Results on direct child assessment (cognition)", null)]
    public void Should_MapToValidFrameworkElements_WithSector(string ecsName, string? sector)
    {
        var result = EcsElementMapping.MapToFrameworkElement(ecsName, sector);
        Assert.NotNull(result);
        Assert.True(
            EwFrameworkDataElements.Elements.ContainsKey(result),
            $"Mapped result '{result}' for ECS element '{ecsName}' (sector: {sector}) is not a valid framework data element"
        );
    }
}
