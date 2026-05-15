using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.Models.Project;
using EwFrameworkAnalysis.Common.Services;
using FluentAssertions;

namespace EwFrameworkAnalysis.Common.UnitTests.Assessors.EdFi;

public class DemographicAssessorTests
{
    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri("https://api.test.com/") };
    private readonly DataSource _dataSource = new() { Name = "Test", Type = DataSourceType.EdFiApi };
    private readonly AssessorContext _context = new((_, _) => { }, _ => { });

    private static readonly EdFiEducationOrganizationReference _orgRef = new(1);
    private static readonly EdFiStudentReference _studentRef = new("student1");

    private static EdFiStudentDemographicsProvider CreateProviderWithData(
        List<EdFiStudentEducationOrganizationAssociation> data)
    {
        var provider = new EdFiStudentDemographicsProvider();
        var field = typeof(EdFiStudentDemographicsProvider)
            .GetField("_cachedData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        field.SetValue(provider, data);
        return provider;
    }

    private static EdFiStudentEducationOrganizationAssociation CreateAssociation(
        string? sexDescriptor = null,
        List<EdFiStudentEducationOrganizationAssociationRace>? races = null,
        List<EdFiStudentEducationOrganizationAssociationDisability>? disabilities = null,
        string? limitedEnglishProficiencyDescriptor = null,
        List<EdFiStudentEducationOrganizationAssociationLanguage>? languages = null,
        List<EdFiStudentEducationOrganizationAssociationStudentCharacteristic>? studentCharacteristics = null,
        List<EdFiStudentEducationOrganizationAssociationStudentIndicator>? studentIndicators = null)
    {
        return new EdFiStudentEducationOrganizationAssociation(
            id: null!,
            educationOrganizationReference: _orgRef,
            studentReference: _studentRef,
            sexDescriptor: sexDescriptor ?? "uri://ed-fi.org/SexDescriptor#Not Selected",
            races: races!,
            disabilities: disabilities!,
            limitedEnglishProficiencyDescriptor: limitedEnglishProficiencyDescriptor!,
            languages: languages!,
            studentCharacteristics: studentCharacteristics!,
            studentIndicators: studentIndicators!);
    }

    [Fact]
    public async Task Should_ProduceGenderDistribution_When_DataHasSexDescriptors()
    {
        var data = new List<EdFiStudentEducationOrganizationAssociation>
        {
            CreateAssociation(sexDescriptor: "uri://ed-fi.org/SexDescriptor#Female"),
            CreateAssociation(sexDescriptor: "uri://ed-fi.org/SexDescriptor#Male"),
            CreateAssociation(sexDescriptor: "uri://ed-fi.org/SexDescriptor#Female")
        };

        var provider = CreateProviderWithData(data);
        var assessor = new StudentGenderEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Gender");
        result.Characteristics.Should().Contain(c => c is RecordCount);
        result.Characteristics.Should().Contain(c => c is Distribution);

        var distribution = result.Characteristics.OfType<Distribution>().First();
        distribution.Counts["Female"].Should().Be(2);
        distribution.Counts["Male"].Should().Be(1);
    }

    [Fact]
    public async Task Should_ProduceRaceDistribution_When_DataHasRaces()
    {
        var data = new List<EdFiStudentEducationOrganizationAssociation>
        {
            CreateAssociation(races: [new EdFiStudentEducationOrganizationAssociationRace("uri://ed-fi.org/RaceDescriptor#White")]),
            CreateAssociation(races: [new EdFiStudentEducationOrganizationAssociationRace("uri://ed-fi.org/RaceDescriptor#Black - African American")]),
            CreateAssociation(races:
            [
                new EdFiStudentEducationOrganizationAssociationRace("uri://ed-fi.org/RaceDescriptor#White"),
                new EdFiStudentEducationOrganizationAssociationRace("uri://ed-fi.org/RaceDescriptor#Asian")
            ])
        };

        var provider = CreateProviderWithData(data);
        var assessor = new RaceAndEthnicityIndividualEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Race and ethnicity (individual)");
        var distribution = result.Characteristics.OfType<Distribution>().First();
        distribution.Counts["White"].Should().Be(2);
        distribution.Counts["Black - African American"].Should().Be(1);
        distribution.Counts["Asian"].Should().Be(1);
    }

    [Fact]
    public async Task Should_ProduceDisabilityDistribution_When_DataHasDisabilities()
    {
        var data = new List<EdFiStudentEducationOrganizationAssociation>
        {
            CreateAssociation(disabilities:
            [
                new EdFiStudentEducationOrganizationAssociationDisability("uri://ed-fi.org/DisabilityDescriptor#Specific Learning Disability")
            ]),
            CreateAssociation(disabilities: [])
        };

        var provider = CreateProviderWithData(data);
        var assessor = new StudentDisabilityStatusEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Disability status");
        var distribution = result.Characteristics.OfType<Distribution>().First();
        distribution.Counts["Has Disability"].Should().Be(1);
        distribution.Counts["No Disability"].Should().Be(1);
    }

    [Fact]
    public async Task Should_ProduceEnglishLearnerDistribution_When_DataHasLepDescriptors()
    {
        var data = new List<EdFiStudentEducationOrganizationAssociation>
        {
            CreateAssociation(limitedEnglishProficiencyDescriptor: "uri://ed-fi.org/LimitedEnglishProficiencyDescriptor#Limited"),
            CreateAssociation(limitedEnglishProficiencyDescriptor: "uri://ed-fi.org/LimitedEnglishProficiencyDescriptor#NotLimited"),
            CreateAssociation()
        };

        var provider = CreateProviderWithData(data);
        var assessor = new StudentEnglishLearnerEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("English learner status");
        var distribution = result.Characteristics.OfType<Distribution>().First();
        distribution.Counts["Limited"].Should().Be(1);
        distribution.Counts["NotLimited"].Should().Be(1);
    }

    [Fact]
    public async Task Should_ProduceHomeLanguageDistribution_When_DataHasLanguages()
    {
        var data = new List<EdFiStudentEducationOrganizationAssociation>
        {
            CreateAssociation(languages:
            [
                new EdFiStudentEducationOrganizationAssociationLanguage(
                    languageDescriptor: "uri://ed-fi.org/LanguageDescriptor#Spanish",
                    uses: [new EdFiStudentEducationOrganizationAssociationLanguageUse("uri://ed-fi.org/LanguageUseDescriptor#Home language")])
            ]),
            CreateAssociation(languages:
            [
                new EdFiStudentEducationOrganizationAssociationLanguage(
                    languageDescriptor: "uri://ed-fi.org/LanguageDescriptor#English")
            ])
        };

        var provider = CreateProviderWithData(data);
        var assessor = new StudentHomeLanguageEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Home language");
        var distribution = result.Characteristics.OfType<Distribution>().First();
        distribution.Counts["Spanish"].Should().Be(1);
        distribution.Counts["English"].Should().Be(1);
    }

    [Fact]
    public async Task Should_ProduceHomelessnessDistribution_When_DataHasCharacteristics()
    {
        var data = new List<EdFiStudentEducationOrganizationAssociation>
        {
            CreateAssociation(studentCharacteristics:
            [
                new EdFiStudentEducationOrganizationAssociationStudentCharacteristic("uri://ed-fi.org/StudentCharacteristicDescriptor#Homeless")
            ]),
            CreateAssociation(studentCharacteristics:
            [
                new EdFiStudentEducationOrganizationAssociationStudentCharacteristic("uri://ed-fi.org/StudentCharacteristicDescriptor#Other")
            ])
        };

        var provider = CreateProviderWithData(data);
        var assessor = new StudentHomelessnessEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Individuals experiencing homelessness");
        var distribution = result.Characteristics.OfType<Distribution>().First();
        distribution.Counts["Homeless"].Should().Be(1);
        distribution.Counts["Not Homeless"].Should().Be(1);
    }

    [Fact]
    public async Task Should_ProduceMilitaryDistribution_When_DataHasCharacteristics()
    {
        var data = new List<EdFiStudentEducationOrganizationAssociation>
        {
            CreateAssociation(studentCharacteristics:
            [
                new EdFiStudentEducationOrganizationAssociationStudentCharacteristic("uri://ed-fi.org/StudentCharacteristicDescriptor#Military Connected")
            ]),
            CreateAssociation()
        };

        var provider = CreateProviderWithData(data);
        var assessor = new MilitaryStatusIndividualFamilyEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Military status (individual/family)");
        var distribution = result.Characteristics.OfType<Distribution>().First();
        distribution.Counts["Military Connected"].Should().Be(1);
        distribution.Counts["Not Reported"].Should().Be(1);
    }

    [Fact]
    public async Task Should_ProduceMigrantDistribution_When_DataHasIndicators()
    {
        var data = new List<EdFiStudentEducationOrganizationAssociation>
        {
            CreateAssociation(studentIndicators:
            [
                new EdFiStudentEducationOrganizationAssociationStudentIndicator(indicatorName: "Migrant", indicator: "true")
            ]),
            CreateAssociation(studentIndicators:
            [
                new EdFiStudentEducationOrganizationAssociationStudentIndicator(indicatorName: "Migrant", indicator: "false")
            ])
        };

        var provider = CreateProviderWithData(data);
        var assessor = new StudentMigrantStatusEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Student from migrant family household");
        var distribution = result.Characteristics.OfType<Distribution>().First();
        distribution.Counts["Migrant"].Should().Be(1);
        distribution.Counts["Not Migrant"].Should().Be(1);
    }

    [Fact]
    public async Task Should_ProduceIncomeLevelDistribution_When_DataHasCharacteristics()
    {
        var data = new List<EdFiStudentEducationOrganizationAssociation>
        {
            CreateAssociation(studentCharacteristics:
            [
                new EdFiStudentEducationOrganizationAssociationStudentCharacteristic("uri://ed-fi.org/StudentCharacteristicDescriptor#Economic Disadvantaged")
            ]),
            CreateAssociation(studentCharacteristics:
            [
                new EdFiStudentEducationOrganizationAssociationStudentCharacteristic("uri://ed-fi.org/StudentCharacteristicDescriptor#Other")
            ]),
            CreateAssociation()
        };

        var provider = CreateProviderWithData(data);
        var assessor = new IncomeLevelIndividualFamilyEdFiAssessor(provider);

        var result = await assessor.AssessAsync(_httpClient, _dataSource, _context);

        result.DataElementName.Should().Be("Income level (individual/family)");
        var distribution = result.Characteristics.OfType<Distribution>().First();
        distribution.Counts["Economic Disadvantaged"].Should().Be(1);
        distribution.Counts["Not Economic Disadvantaged"].Should().Be(1);
        distribution.Counts["Not Reported"].Should().Be(1);
    }

    [Fact]
    public void Should_MatchDataElementNames_When_ComparedToFrameworkData()
    {
        var emptyProvider = CreateProviderWithData([]);

        var assessors = new IEdFiAssessor[]
        {
            new RaceAndEthnicityIndividualEdFiAssessor(emptyProvider),
            new StudentGenderEdFiAssessor(emptyProvider),
            new StudentDisabilityStatusEdFiAssessor(emptyProvider),
            new IncomeLevelIndividualFamilyEdFiAssessor(emptyProvider),
            new StudentEnglishLearnerEdFiAssessor(emptyProvider),
            new StudentHomeLanguageEdFiAssessor(emptyProvider),
            new StudentMigrantStatusEdFiAssessor(emptyProvider),
            new StudentHomelessnessEdFiAssessor(emptyProvider),
            new MilitaryStatusIndividualFamilyEdFiAssessor(emptyProvider),
            new SchoolTypeEdFiAssessor(),
            new SchoolUrbanicityEdFiAssessor()
        };

        var frameworkElements = EwFrameworkAnalysis.Common.FrameworkReferenceData.EwFrameworkDataElements.Elements;

        foreach (var assessor in assessors)
        {
            frameworkElements.Should().ContainKey(assessor.DataElementName,
                $"Assessor '{assessor.GetType().Name}' references data element '{assessor.DataElementName}' which must exist in the framework");
        }
    }
}
