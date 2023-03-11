using System.Web;
using COMP3000_Project_Backend_API.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Moq.Contrib.HttpClient;

namespace COMP3000_Project_Backend_API.TestUtilities.Support
{
    public static class DEFRATemperatureShimUtilities
    {
        public static HttpMessageHandler GetHttpMessageHandler()
        {
            var handler = GetMockHttpMessageHandler();
            return handler.Object;
        }

        public static HttpClient GetMockHttpClient()
        {
            var handler = GetMockHttpMessageHandler();
            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRAShimService.BaseAddress);
            return client;
        }

        private static Mock<HttpMessageHandler> GetMockHttpMessageHandler()
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest("https://defra-shim-xsji6nno4q-ew.a.run.app/data?site=ABD&date=2022-01-04T01%3A00%3A00Z").ReturnsResponse(System.Net.HttpStatusCode.OK, ABDResponse);
            handler.SetupRequest("https://defra-shim-xsji6nno4q-ew.a.run.app/data?site=ABD9&date=2022-01-04T01%3A00%3A00Z").ReturnsResponse(System.Net.HttpStatusCode.OK, ABD9Response);

            return handler;
        }

        private static string ABDResponse = @"{""temp"":1.0, ""timestamp"": ""2022-01-04T01:00:00.000""}";
        private static string ABD9Response = @"{""temp"":2.0, ""timestamp"": ""2022-01-04T01:00:00.000""}";
    }
}
