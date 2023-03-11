using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.MongoDB;

namespace COMP3000_Project_Backend_API.Services
{
    public class DEFRAShimTemperatureService : ITemperatureService
    {
        public static string Unit { get; } = "°C";
        public static string LicenseString { get; } = "\u00A9 Crown copyright 2021 Defra via uk-air.defra.gov.uk, licensed under the Open Government Licence.";

        private readonly IDEFRAShimService _shimService;

        public DEFRAShimTemperatureService(IDEFRAShimService shimService)
        {
            _shimService = shimService;
        }

        public async Task<ReadingInfo?> GetTemperatureInfo(DEFRAMetadata metadata, DateTime? timestamp)
        {
            var data = await _shimService.GetDataFromShim(metadata, timestamp);
            if (data is not null && data.Temperature is not null)
            {
                return new ReadingInfo()
                {
                    LicenseInfo = LicenseString,
                    Unit = Unit,
                    Timestamp = data.Timestamp,
                    Station = metadata.ToStation(),
                    Value = data.Temperature.Value
                };
            }
            return null;
        }
    }
}
