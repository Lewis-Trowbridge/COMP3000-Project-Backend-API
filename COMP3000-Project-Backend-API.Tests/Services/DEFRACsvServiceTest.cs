using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Services;
using SimpleDateTimeProvider;
using System.Globalization;

namespace COMP3000_Project_Backend_API.Tests.Services
{
    public class DEFRACsvServiceTest
    {
        [Fact]
        public async void DEFRACsvService_Get_MakesCorrectHttpRequest()
        {
            var testStationId = "test";
            var testDateTime = DateTime.Parse("01-01-2022 04:00:00");
            var testAddress = DEFRACsvService.DEFRABaseAddress + $"{testStationId}_PM25_{testDateTime.Year}.csv";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK, ValidCSV);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRACsvService.DEFRABaseAddress);
            var service = new DEFRACsvService(client, new SystemDateTimeProvider());

            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationId,
                Coords = new double[] { 0, 0 }
            };

            await service.GetAirQualityInfo(testMetadata, testDateTime);

            handler.VerifyRequest(testAddress, Times.Once());
        }

        [Fact]
        public async void DEFRACsvService_Get_ReturnsEmptyObjectOn404()
        {
            var testStationId = "test";
            var testDateTime = DateTime.MinValue;
            var testAddress = DEFRACsvService.DEFRABaseAddress + $"{testStationId}_PM25_{testDateTime.Year}.csv";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.NotFound);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRACsvService.DEFRABaseAddress);
            var service = new DEFRACsvService(client, new SystemDateTimeProvider());

            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId
            };

            var expected = new AirQualityInfo();

            var actual = await service.GetAirQualityInfo(testMetadata, testDateTime);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void DEFRACsvService_Get_ReturnsValidResultWithExactTime()
        {
            var testStationId = "WREX";
            var testStationName = "Wrexham";
            var testStationCoords = new double[] { 51.5106748, -0.1355159 };
            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationName,
                Coords = testStationCoords
            };

            var testDateTime = DateTime.Parse("07-01-2022 04:00:00", CultureInfo.GetCultureInfo("en-GB"));
            var testAddress = DEFRACsvService.DEFRABaseAddress + $"{testStationId}_PM25_{testDateTime.Year}.csv";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK, ValidCSV);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRACsvService.DEFRABaseAddress);
            var service = new DEFRACsvService(client, new SystemDateTimeProvider());

            var expected = new AirQualityInfo()
            {
                Value = 3.396F,
                Unit = DEFRACsvService.PM25Unit,
                Timestamp = testDateTime,
                LicenseInfo = DEFRACsvService.LicenseString,
                Station = new Station()
                {
                    Name = testStationName,
                    Coordinates = new LatLong()
                    {
                        Lat = testStationCoords[1],
                        Lng = testStationCoords[0]
                    }
                }
            };

            var actual = await service.GetAirQualityInfo(testMetadata, testDateTime);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void DEFRACsvService_Get_ReturnsValidResultWithBeforeHalfPast()
        {
            var testStationId = "WREX";
            var testStationName = "Wrexham";
            var testStationCoords = new double[] { 51.5106748, -0.1355159 };
            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationName,
                Coords = testStationCoords
            };

            var testDateTime = DateTime.Parse("07-01-2022 04:15:00", CultureInfo.GetCultureInfo("en-GB"));
            var testAddress = DEFRACsvService.DEFRABaseAddress + $"{testStationId}_PM25_{testDateTime.Year}.csv";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK, ValidCSV);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRACsvService.DEFRABaseAddress);
            var service = new DEFRACsvService(client, new SystemDateTimeProvider());

            var expected = new AirQualityInfo()
            {
                Value = 3.396F,
                Unit = DEFRACsvService.PM25Unit,
                Timestamp = testDateTime,
                LicenseInfo = DEFRACsvService.LicenseString,
                Station = new Station()
                {
                    Name = testStationName,
                    Coordinates = new LatLong()
                    {
                        Lat = testStationCoords[1],
                        Lng = testStationCoords[0]
                    }
                }
            };

            var actual = await service.GetAirQualityInfo(testMetadata, testDateTime);

            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async void DEFRACsvService_Get_ReturnsValidResultAtHalfPast()
        {
            var testStationId = "WREX";
            var testStationName = "Wrexham";
            var testStationCoords = new double[] { 51.5106748, -0.1355159 };
            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationName,
                Coords = testStationCoords
            };

            var testDateTime = DateTime.Parse("07-01-2022 03:30:00", CultureInfo.GetCultureInfo("en-GB"));
            var testAddress = DEFRACsvService.DEFRABaseAddress + $"{testStationId}_PM25_{testDateTime.Year}.csv";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK, ValidCSV);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRACsvService.DEFRABaseAddress);
            var service = new DEFRACsvService(client, new SystemDateTimeProvider());

            var expected = new AirQualityInfo()
            {
                Value = 3.396F,
                Unit = DEFRACsvService.PM25Unit,
                Timestamp = testDateTime,
                LicenseInfo = DEFRACsvService.LicenseString,
                Station = new Station()
                {
                    Name = testStationName,
                    Coordinates = new LatLong()
                    {
                        Lat = testStationCoords[1],
                        Lng = testStationCoords[0]
                    }
                }
            };

            var actual = await service.GetAirQualityInfo(testMetadata, testDateTime);

            actual.Should().BeEquivalentTo(expected);

        }

        [Fact]
        public async void DEFRACsvService_Get_ReturnsValidResultWithAfterHalfPast()
        {
            var testStationId = "WREX";
            var testStationName = "Wrexham";
            var testStationCoords = new double[] { 51.5106748, -0.1355159 };
            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationName,
                Coords = testStationCoords
            };

            var testDateTime = DateTime.Parse("07-01-2022 03:45:00", CultureInfo.GetCultureInfo("en-GB"));
            var testAddress = DEFRACsvService.DEFRABaseAddress + $"{testStationId}_PM25_{testDateTime.Year}.csv";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK, ValidCSV);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRACsvService.DEFRABaseAddress);
            var service = new DEFRACsvService(client, new SystemDateTimeProvider());

            var expected = new AirQualityInfo()
            {
                Value = 3.396F,
                Unit = DEFRACsvService.PM25Unit,
                Timestamp = testDateTime,
                LicenseInfo = DEFRACsvService.LicenseString,
                Station = new Station()
                {
                    Name = testStationName,
                    Coordinates = new LatLong()
                    {
                        Lat = testStationCoords[1],
                        Lng = testStationCoords[0]
                    }
                }
            };

            var actual = await service.GetAirQualityInfo(testMetadata, testDateTime);

            actual.Should().BeEquivalentTo(expected);

        }

        [Fact]
        public async void DEFRACsvService_Get_ReturnsNullIfNotFoundInCSV()
        {
            var testStationId = "WREX";
            var testStationName = "Wrexham";
            var testStationCoords = new double[] { 51.5106748, -0.1355159 };
            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationName,
                Coords = testStationCoords
            };

            var testDateTime = DateTime.Parse("07-01-2011 04:00:00", CultureInfo.GetCultureInfo("en-GB"));
            var testAddress = DEFRACsvService.DEFRABaseAddress + $"{testStationId}_PM25_{testDateTime.Year}.csv";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK, ValidCSV);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRACsvService.DEFRABaseAddress);
            var service = new DEFRACsvService(client, new SystemDateTimeProvider());

            var actual = await service.GetAirQualityInfo(testMetadata, testDateTime);

            actual.Should().BeNull();
        }

        [Fact]
        public async void DEFRACsvService_Get_ReturnsNullIfEmptyStringFoundInCSV()
        {
            var testStationId = "WREX";
            var testStationName = "Wrexham";
            var testStationCoords = new double[] { 51.5106748, -0.1355159 };
            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationName,
                Coords = testStationCoords
            };

            var testDateTime = DateTime.Parse("01-01-2022 06:00:00", CultureInfo.GetCultureInfo("en-GB"));
            var testAddress = DEFRACsvService.DEFRABaseAddress + $"{testStationId}_PM25_{testDateTime.Year}.csv";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK, CSVWithEmptyValues);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRACsvService.DEFRABaseAddress);
            var service = new DEFRACsvService(client, new SystemDateTimeProvider());

            var actual = await service.GetAirQualityInfo(testMetadata, testDateTime);

            actual.Should().BeNull();
        }

        [Theory]
        [InlineData("11-01-2022 00:00:00")]
        [InlineData("11-01-2022 04:00:00")]
        [InlineData("11-01-2022 04:30:00")]
        [InlineData("11-01-2022 04:15:00")]
        [InlineData("11-01-2022 04:45:00")]
        public async void DEFRACsvService_Get_ReturnsLastResultWithNullTimestamp(string currentTime)
        {
            var testStationId = "WREX";
            var testStationName = "Wrexham";
            var testStationCoords = new double[] { 51.5106748, -0.1355159 };
            var testMetadata = new DEFRAMetadata()
            {
                Id = testStationId,
                SiteName = testStationName,
                Coords = testStationCoords
            };

            var mockCurrentDateTime = DateTime.Parse(currentTime, CultureInfo.GetCultureInfo("en-GB"));
            var mockDateTimeProvider = Mock.Of<IDateTimeProvider>(x => x.UtcNow == mockCurrentDateTime);
            var testAddress = DEFRACsvService.DEFRABaseAddress + $"{testStationId}_PM25_{mockCurrentDateTime.Year}.csv";
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, testAddress).ReturnsResponse(System.Net.HttpStatusCode.OK, ValidCSV);

            var client = handler.CreateClient();
            client.BaseAddress = new Uri(DEFRACsvService.DEFRABaseAddress);
            var service = new DEFRACsvService(client, mockDateTimeProvider);

            var expectedDatetime = DateTime.Parse("10-01-2022 00:00:00", CultureInfo.GetCultureInfo("en-GB"));
            var expected = new AirQualityInfo()
            {
                Value = 1.792F,
                Unit = DEFRACsvService.PM25Unit,
                Timestamp = expectedDatetime,
                LicenseInfo = DEFRACsvService.LicenseString,
                Station = new Station()
                {
                    Name = testStationName,
                    Coordinates = new LatLong()
                    {
                        Lat = testStationCoords[1],
                        Lng = testStationCoords[0]
                    }
                }
            };

            var actual = await service.GetAirQualityInfo(testMetadata, null);

            actual.Should().BeEquivalentTo(expected);

        }



        private static readonly string ValidCSV = @"
Data supplied by UK-AIR on 1/11/2022
All Data GMT hour ending  
Rows begining ## are Provisional
Wrexham PM<sub>2.5</sub> particulate matter (Hourly measured) ug/m-3
   Date   , 01:00, 02:00, 03:00, 04:00, 05:00, 06:00, 07:00, 08:00, 09:00, 10:00, 11:00, 12:00, 13:00, 14:00, 15:00, 16:00, 17:00, 18:00, 19:00, 20:00, 21:00, 22:00, 23:00, 24:00
01-01-2022,19.198, 5.212, 6.109, 5.873, 6.863, 8.844, 8.137, 8.443, 9.009,11.533,11.863,11.651,12.453,12.783,12.736,10.920,10.873,12.948,11.981, 9.623, 9.811, 9.127,10.354, 9.009
02-01-2022, 8.915, 8.373, 8.019, 5.165, 2.217, 2.547, 1.675, 2.948, 3.892, 4.222, 4.599, 5.519, 7.453, 6.887, 2.642, 3.491, 5.472, 6.250, 7.807, 8.160, 9.481, 9.104, 8.750, 8.184
03-01-2022, 8.160, 8.467, 9.670, 9.104, 9.293, 7.264, 7.170, 7.217, 6.792, 7.453, 8.113, 9.976,11.014, 8.373, 7.406, 7.547, 9.293,16.604,10.047, 9.151, 4.835, 5.354, 6.014, 6.132
04-01-2022, 1.863, 2.335, 1.887, 2.052, 1.344, 1.344, 1.816, 8.019, 4.976, 5.212, 5.920, 6.580, 6.344, 7.241, 6.580, 6.557, 6.344, 4.009, 2.972, 3.443, 4.222, 4.151, 6.014, 6.179
05-01-2022, 6.792, 6.392, 6.392, 7.170, 6.392, 5.873, 6.509, 6.816, 4.387, 5.259, 5.943, 5.684, 5.684, 7.288, 6.439, 7.642, 9.552,10.330,18.538,25.212,21.816,18.208,14.906,14.670
06-01-2022,15.590, 6.509, 4.741, 5.071, 4.976, 4.717, 4.599, 4.009, 4.033, 4.057, 3.538, 2.854, 2.429, 2.241, 2.052, 3.278, 5.401,10.212, 4.859, 8.844, 6.415, 8.561, 3.514, 3.396
07-01-2022, 3.656, 4.528, 3.609, 3.396, 2.972, 2.052, 2.453, 2.241, 2.453, 2.382, 2.995, 3.561, 4.552, 4.623, 3.561, 3.726, 4.340, 3.750, 3.892, 3.821, 3.255, 4.245, 5.377, 5.165
08-01-2022, 5.236, 4.741, 3.939, 3.420, 2.429, 3.774, 3.137, 1.321, 1.250, 0.708, 1.792, 8.585,10.425, 9.175,10.613,11.910,12.830, 7.712, 5.920, 6.179, 5.000, 4.693, 5.472, 5.755
09-01-2022, 5.495, 4.976, 4.788, 3.892, 2.807, 3.278, 3.797, 4.222, 5.024, 5.637, 5.472, 5.519, 4.835, 6.321, 6.132, 9.009,10.826,12.901,16.132,14.127, 8.915, 7.807, 7.642, 7.642
10-01-2022, 8.679, 7.948, 8.113, 8.255, 8.750, 8.821, 8.184, 6.675, 5.118, 4.764, 4.552, 4.410, 3.750, 3.373, 3.561, 5.896,10.000,15.566, 8.326, 8.726, 7.948, 9.104, 5.401, 1.792";

        private static readonly string CSVWithEmptyValues = @"
Data supplied by UK-AIR on 1/11/2022
All Data GMT hour ending  
Rows begining ## are Provisional
Wrexham PM<sub>2.5</sub> particulate matter (Hourly measured) ug/m-3
   Date   , 01:00, 02:00, 03:00, 04:00, 05:00, 06:00, 07:00, 08:00, 09:00, 10:00, 11:00, 12:00, 13:00, 14:00, 15:00, 16:00, 17:00, 18:00, 19:00, 20:00, 21:00, 22:00, 23:00, 24:00
01-01-2022,19.198, 5.212, 6.109, 5.873, 6.863,      ,      , 8.443, 9.009,11.533,11.863,11.651,12.453,12.783,12.736,10.920,10.873,12.948,11.981, 9.623, 9.811, 9.127,10.354, 9.009
";
    }
}
