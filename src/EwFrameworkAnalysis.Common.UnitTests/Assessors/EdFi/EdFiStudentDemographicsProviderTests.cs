using System.Net;
using System.Text;
using EdFi.OdsApi.Sdk.Models.Ed_Fi;
using EwFrameworkAnalysis.Common.Assessors.EdFi;
using EwFrameworkAnalysis.Common.Services;
using FluentAssertions;
using Newtonsoft.Json;

namespace EwFrameworkAnalysis.Common.UnitTests.Assessors.EdFi;

public class EdFiStudentDemographicsProviderTests
{
    [Fact]
    public async Task Should_CacheData_When_CalledMultipleTimes()
    {
        var callCount = 0;
        var handler = new FakeHttpMessageHandler(request =>
        {
            callCount++;
            if (request.RequestUri!.Query.Contains("totalCount=true"))
            {
                var countResponse = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]", Encoding.UTF8, "application/json")
                };
                countResponse.Headers.Add("total-count", "0");
                return countResponse;
            }
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            };
        });

        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var provider = new EdFiStudentDemographicsProvider();
        var context = new AssessorContext((_, _) => { }, _ => { });

        var result1 = await provider.GetDataAsync(httpClient, context);
        var result2 = await provider.GetDataAsync(httpClient, context);

        result1.Should().BeSameAs(result2);
    }

    [Fact]
    public async Task Should_ReturnData_When_ApiReturnsRecords()
    {
        var orgRef = new EdFiEducationOrganizationReference(1);
        var studentRef = new EdFiStudentReference("student1");
        var testData = new List<EdFiStudentEducationOrganizationAssociation>
        {
            new(educationOrganizationReference: orgRef, studentReference: studentRef, sexDescriptor: "uri://ed-fi.org/SexDescriptor#Female"),
            new(educationOrganizationReference: orgRef, studentReference: studentRef, sexDescriptor: "uri://ed-fi.org/SexDescriptor#Male")
        };

        var handler = new FakeHttpMessageHandler(request =>
        {
            if (request.RequestUri!.Query.Contains("totalCount=true"))
            {
                var countResponse = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("[]", Encoding.UTF8, "application/json")
                };
                countResponse.Headers.Add("total-count", "2");
                return countResponse;
            }
            if (request.RequestUri.Query.Contains("offset=0") || !request.RequestUri.Query.Contains("offset"))
            {
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(testData), Encoding.UTF8, "application/json")
                };
            }
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            };
        });

        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.test.com/") };
        var provider = new EdFiStudentDemographicsProvider();
        var context = new AssessorContext((_, _) => { }, _ => { });

        var result = await provider.GetDataAsync(httpClient, context);

        result.Should().HaveCount(2);
    }

    private class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_handler(request));
        }
    }
}
