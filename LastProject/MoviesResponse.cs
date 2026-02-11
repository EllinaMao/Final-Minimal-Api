namespace LastProject
{
    public class MoviesResponse
    {
        public int Page { get; set; }
        public List<Movie> Results { get; set; } = new();
        public int Total_Pages { get; set; }
        public int Total_Results { get; set; }
    }

    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Overview { get; set; } = "";
        public string Poster_Path { get; set; } // Путь к картинке
        public string Release_Date { get; set; }
        public double Vote_Average { get; set; }
        public List<int> Genre_Ids { get; set; } = new();

        // Для детального просмотра жанры приходят объектами
        public List<Genre> Genres { get; set; } = new();
    }

    public class GenreResponse
    {
        public List<Genre> Genres { get; set; } = new();
    }

    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}
