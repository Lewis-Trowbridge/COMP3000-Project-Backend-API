using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;
using CsvHelper;
using SimpleDateTimeProvider;
using System.Globalization;

namespace COMP3000_Project_Backend_API.Services
{
    public class DEFRACsvService : IAirQualityService
    {
        private readonly HttpClient _httpClient;
        private readonly IDateTimeProvider _dateTimeProvider;

        public static string PM25Unit { get; } = "PM2.5";
        public static string LicenseString { get; } = "© Crown copyright 2021 Defra via uk-air.defra.gov.uk, licensed under the Open Government Licence.";
        public static string DEFRABaseAddress { get; } = "https://uk-air.defra.gov.uk/datastore/data_files/site_pol_data/";

        public DEFRACsvService(HttpClient httpClient, IDateTimeProvider dateTimeProvider)
        {
            _httpClient = httpClient;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<AirQualityInfo?> GetAirQualityInfo(DEFRAMetadata metadata, DateTime? timestamp)
        {
            DateTime updatedTimestamp = timestamp ?? _dateTimeProvider.UtcNow.AddDays(-1);

            var request = await _httpClient.GetAsync($"{metadata.Id}_PM25_{updatedTimestamp.Year}.csv");

            if (!request.IsSuccessStatusCode)
            {
                return new AirQualityInfo();
            }
            
            var contentString = await request.Content.ReadAsStringAsync();
            contentString = RemoveHeaderLines(contentString);
            contentString = RemoveProvisionalTags(contentString);

            using var contentStringReader = new StringReader(contentString);
            using var csv = new CsvReader(contentStringReader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<dynamic>();
            if (timestamp.HasValue)
            {
                var dateString = updatedTimestamp.ToString("dd-MM-yyyy");
                var timeString = GetTimeString(updatedTimestamp);
                var record = records
                .Select(x => x as IDictionary<string, object>)
                .SingleOrDefault(x => x is not null && x["   Date   "].Equals(dateString), new Dictionary<string, object>())!;
                var floatRecord = (record.TryGetValue(timeString, out var stringRecord)) ? float.Parse((string)stringRecord) : -1f;

                return AssembleAirQualityInfo(metadata, updatedTimestamp, floatRecord);
            }
            else
            {
                var record = records.Last() as IDictionary<string, object>;
                var floatRecord = record!.TryGetValue(" 24:00", out var stringRecord) ? float.Parse((string)stringRecord) : -1f;

                return AssembleAirQualityInfo(metadata, updatedTimestamp, floatRecord);
            }

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

        private static AirQualityInfo? AssembleAirQualityInfo(DEFRAMetadata metadata, DateTime timestamp, float value)
        {
            if (value == -1)
            {
                return null;
            }

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
