﻿using COMP3000_Project_Backend_API.IntegrationTests.Support;
using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Models.Request;
using COMP3000_Project_Backend_API.Services;
using COMP3000_Project_Backend_API.TestUtilities.Support;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace COMP3000_Project_Backend_API.FunctionalTests.Controllers
{
    public class AirQualityControllerTest : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<MongoDBFixture>, IDisposable
    {
        private HttpClient _client;
        private MongoDBFixture _mongoDBFixture;

        public AirQualityControllerTest(WebApplicationFactory<Program> webApplicationFactory, MongoDBFixture mongoDBFixture)
        {
            _mongoDBFixture = mongoDBFixture;
            mongoDBFixture.runner.Import("metadata", "metadata", "Assets/mongo.json", "--jsonArray");
            _client = webApplicationFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.Configure<MongoDBSettings>(options =>
                    {
                        options.ConnectionString = mongoDBFixture.runner.ConnectionString;
                        options.CollectionName = "metadata";
                        options.DatabaseName = "metadata";
                    });

                    services.AddHttpClient<IAirQualityService, DEFRACsvService>()
                    .ConfigureHttpMessageHandlerBuilder(builder => builder.PrimaryHandler = DEFRAUCsvUtilities.GetHttpMessageHandler());
                });
            })
                .CreateClient();
        }

        [Fact]
        public async void AirQualityController_GetAirQuality_Returns200AndValidData()
        {
            var request = new AirQualityRequest()
            {
                Timestamp = DateTime.Parse("2022-01-04T01:00:00.000Z"),
                Bbox = new BoundingBox()
                {
                    BottomLeftX = 49.70890434294886,
                    BottomLeftY = -13.168850856210385,
                    TopRightX = 59.6111417069173,
                    TopRightY = 1.9483351626784629
                }
            };

            var response = await _client.PostAsJsonAsync("api/airquality", request);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var stringActual = await response.Content.ReadAsStringAsync();

            var expected = JToken.Parse(ValidJSON);
            var actual = JToken.Parse(stringActual);

            actual.Should().BeEquivalentTo(expected);

        }

        private static string ValidJSON { get; } = @"[{""value"":2.263,""unit"":""PM2.5"",""timestamp"":""2022-01-04T01:00:00+00:00"",""licenseInfo"":""© Crown copyright 2021 Defra via uk-air.defra.gov.uk, licensed under the Open Government Licence."",""station"":{""name"":""Aberdeen Erroll Park"",""coordinates"":{""lat"":57.1574,""lng"":-2.09477}}},{""value"":1.263,""unit"":""PM2.5"",""timestamp"":""2022-01-04T01:00:00+00:00"",""licenseInfo"":""© Crown copyright 2021 Defra via uk-air.defra.gov.uk, licensed under the Open Government Licence."",""station"":{""name"":""Aberdeen"",""coordinates"":{""lat"":57.15736,""lng"":-2.094278}}}]";

        public void Dispose()
        {
            _mongoDBFixture.mongoClient.DropDatabase("metadata");
        }
    }
}
