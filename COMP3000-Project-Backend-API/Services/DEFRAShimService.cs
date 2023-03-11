using System.Net.Http;
using COMP3000_Project_Backend_API.Models;
using COMP3000_Project_Backend_API.Models.External.Shim;
using COMP3000_Project_Backend_API.Models.MongoDB;
using COMP3000_Project_Backend_API.Utils;

namespace COMP3000_Project_Backend_API.Services
{
    public class DEFRAShimService : IDEFRAShimService
    {
        public static string BaseAddress { get; } = "https://defra-shim-xsji6nno4q-ew.a.run.app";

        private readonly HttpClient _httpClient;
        public DEFRAShimService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ShimResponse?> GetDataFromShim(DEFRAMetadata metadata, DateTime? timestamp)
        {
            var queryValues = new Dictionary<string, string?>
            {
                { "site", metadata.Id },
            };

            if (timestamp is not null)
            {
                queryValues.Add("date", timestamp.Value.ToIsoTimestamp());
            }

            var response = await _httpClient.GetAsync("/data" + QueryString.Create(queryValues).ToString());
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ShimResponse>();
            }
            return null;
        }
    }
}
