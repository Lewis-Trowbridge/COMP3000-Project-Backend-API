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

        public double BottomLeftX { get; set; }
        public double BottomLeftY { get; set; }
        public double TopRightX { get; set; }
        public double TopRightY { get; set; }
    }
}
