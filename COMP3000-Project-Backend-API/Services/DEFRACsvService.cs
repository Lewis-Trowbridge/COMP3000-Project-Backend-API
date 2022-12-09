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
        public static string LicenseString { get; } = "\u00A9 Crown copyright 2021 Defra via uk-air.defra.gov.uk, licensed under the Open Government Licence.";
        public static string DEFRABaseAddress { get; } = "https://uk-air.defra.gov.uk/datastore/data_files/site_pol_data/";

        public DEFRACsvService(HttpClient httpClient, IDateTimeProvider dateTimeProvider)
        {
            _httpClient = httpClient;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<AirQualityInfo?> GetAirQualityInfo(DEFRAMetadata metadata, DateTime? timestamp)
        {
            DateTime updatedTimestamp = timestamp ?? _dateTimeProvider.UtcNow.Date.AddDays(-1);

            var request = await _httpClient.GetAsync($"{metadata.Id}_PM25_{updatedTimestamp.Year}.csv");

            if (!request.IsSuccessStatusCode)
            {
                return null;
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
                var floatRecord = GetFloatValue(record, timeString);

                return AssembleAirQualityInfo(metadata, updatedTimestamp, floatRecord);
            }
            else
            {
                var record = records.Last() as IDictionary<string, object>;
                var date = record?["   Date   "] as string;
                var time = record?.Last(x => !string.IsNullOrWhiteSpace(x.Value as string)).Key;
                // Need to get the most recent value dynamically
                var floatRecord = GetFloatValue(record!, time);

                return AssembleAirQualityInfo(metadata, date, time, floatRecord);
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

        private static float GetFloatValue(IDictionary<string, object> record, string timeString)
        {
            return record!.TryGetValue(timeString, out var objectRecord)
                    && objectRecord is string stringRecord
                    && !string.IsNullOrWhiteSpace(stringRecord)
                    ? float.Parse(stringRecord) : -1f;
        }

        private static AirQualityInfo? AssembleAirQualityInfo(DEFRAMetadata metadata, string dateString, string hourAndMinuteString, float value)
        {
            var timestamp = hourAndMinuteString != " 24:00" ?
                DateTime.Parse($"{dateString} {hourAndMinuteString.Trim()}:00", CultureInfo.GetCultureInfo("en-GB")) :
                // 24:00 is the beginning of tomorrow
                DateTime.Parse($"{dateString} 00:00:00", CultureInfo.GetCultureInfo("en-GB")).AddDays(1);
            return AssembleAirQualityInfo(metadata, timestamp, value);
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
