using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace COMP3000_Project_Backend_API.Models.Request
{
    public class AirQualityRequest
    {
        [Required]
        [FromQuery(Name = "bbox")]
        public BoundingBox? Bbox { get; set; }
        [FromQuery(Name = "timestamp")]
        public DateTime? Timestamp { get; set; }
    }
}
