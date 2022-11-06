using System.ComponentModel.DataAnnotations;

namespace COMP3000_Project_Backend_API.Models.Request
{
    public class AirQualityRequest
    {
        [Required]
        public BoundingBox? Bbox { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
