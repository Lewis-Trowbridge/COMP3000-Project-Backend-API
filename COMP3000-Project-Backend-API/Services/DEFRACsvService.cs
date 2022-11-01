namespace COMP3000_Project_Backend_API.Services
{
    public class DEFRACsvService
    {
        private readonly HttpClient _httpClient;

        public DEFRACsvService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
