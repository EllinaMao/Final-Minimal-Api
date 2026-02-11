namespace LastProject.Models
{
    public class MoviesResponse
    {
        public int Page { get; set; }
        public List<Movie> Results { get; set; } = new();
        public int Total_Pages { get; set; }
        public int Total_Results { get; set; }
    }

}