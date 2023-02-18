namespace COMP3000_Project_Backend_API.Utils
{
    public static class DateTimeExtensions
    {
        public static string ToIsoTimestamp(this DateTime datetime)
        {
            return datetime.ToUniversalTime().ToString("u").Replace(" ", "T");
        }
    }
}
