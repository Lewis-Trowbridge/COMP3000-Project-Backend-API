using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace COMP3000_Project_Backend_API.Models
{
    public class BoundingBox
    {
        public BoundingBox()
        {

        }

        public BoundingBox(double bottomLeftX, double bottomLeftY, double topRightX, double topRightY)
        {
            BottomLeftX = bottomLeftX;
            BottomLeftY = bottomLeftY;
            TopRightX = topRightX;
            TopRightY = topRightY;
        }

        [Required]
        [FromQuery(Name = "bottomLeftX")]
        public double BottomLeftX { get; set; }
        [Required]
        [FromQuery(Name = "bottomLeftY")]
        public double BottomLeftY { get; set; }
        [Required]
        [FromQuery(Name = "topRightX")]
        public double TopRightX { get; set; }
        [Required]
        [FromQuery(Name = "topRightY")]
        public double TopRightY { get; set; }
    }
}
