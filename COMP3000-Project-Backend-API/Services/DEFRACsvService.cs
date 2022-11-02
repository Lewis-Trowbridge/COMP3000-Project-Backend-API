using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using CsvHelper;
using CsvHelper.Configuration;
using System.Dynamic;
using System.Globalization;

namespace COMP3000_Project_Backend_API.Services
{
    public class DEFRACsvService
    {
        private readonly HttpClient _httpClient;

        public static string PM25Unit { get; } = "PM2.5";
        public static string LicenseString { get; } = "© Crown copyright 2021 Defra via uk-air.defra.gov.uk, licensed under the Open Government Licence.";

        public DEFRACsvService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AirQualityInfo> GetAirQualityInfo(DEFRAMetadata metadata, DateTime timestamp)
        {
            var request = await _httpClient.GetAsync($"{metadata.Id}_PM25_{timestamp.Year}.csv");

            if (!request.IsSuccessStatusCode)
            {
                return new AirQualityInfo();
            }
            
            var contentString = await request.Content.ReadAsStringAsync();
            contentString = RemoveHeaderLines(contentString);
            contentString = RemoveProvisionalTags(contentString);
            var dateString = timestamp.ToString("dd-MM-yyyy");
            var timeString = GetTimeString(timestamp);

            using var contentStringReader = new StringReader(contentString);
            using var csv = new CsvReader(contentStringReader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<dynamic>();
            var record = records
                .Select(x => x as IDictionary<string, object> ?? new Dictionary<string, object>() { { "   Date   ", "" } })
                .Single(x => x is not null && x["   Date   "].Equals(dateString))
                [timeString] ?? 0f;
            var floatRecord = (record is string stringRecord) ? float.Parse(stringRecord) : (float)record;

            return AssembleAirQualityInfo(metadata, timestamp, floatRecord);

        }

        private static string RemoveHeaderLines(string csvString)
        {
            return csvString.Split("ug/m-3")[1];
        }

        private static string RemoveProvisionalTags(string csvString)
        {
            return csvString.Replace("##", "");
        }

        private static string GetTimeString(DateTime timestamp)
        {
            // Round up the hour if we are past half past
            var hour = timestamp.Minute < 30 ? timestamp.Hour : timestamp.Hour + 1;
            // Adjust for CSV's usage of 24th hour
            if (hour == 0)
            {
                hour = 24;
            }
            return string.Format(" {0:00}:00", hour);
        }

        private static AirQualityInfo AssembleAirQualityInfo(DEFRAMetadata metadata, DateTime timestamp, float value)
        {
            return new AirQualityInfo()
            {
                Value = value,
                Timestamp = timestamp,
                Unit = PM25Unit,
                LicenseInfo = LicenseString,
                Station = new Station()
                {
                    Name = metadata.SiteName,
                    Coordinates = new LatLong()
                    {
                        Lat = metadata.Coords[1],
                        Lng = metadata.Coords[0]
                    }
                }
            };
        }
    }
}
