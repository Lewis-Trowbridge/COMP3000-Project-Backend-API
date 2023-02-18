namespace COMP3000_Project_Backend_API.Models.External.Predictions
{
    public class PredictionRequest
    {
        // In format [timestamp, lat, long]
        public List<List<double>> Inputs { get; set; } = new List<List<double>>();
    }
}
