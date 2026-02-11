namespace LastProject.Models
{


    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Overview { get; set; } = "";
        public string Poster_Path { get; set; } 
        public string Release_Date { get; set; }
        public double Vote_Average { get; set; }
        public List<int> Genre_Ids { get; set; } = new();
        public List<Genre> Genres { get; set; } = new();
    }

}
