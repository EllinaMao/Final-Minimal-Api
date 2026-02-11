using System.Text.Json;

namespace LastProject
{
    public static class MovieApi
    {
        private static readonly HttpClient httpClient = new HttpClient();
      
        public const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";

        static string GetMovieApiKey()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            return config.GetSection("TheMovieDb:ApiKey").Value;
        }

        private static async Task<T> SendApi<T>(string url)
        {
            string apiKey = GetMovieApiKey();
            string separator = url.Contains("?") ? "&" : "?";
            string requestUrl = $"{url}{separator}api_key={apiKey}&language=ru-RU";

            var response = await httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return await response.Content.ReadFromJsonAsync<T>(options);
            }
            else
            {
                throw new Exception($"Error accessing API: {response.StatusCode}");
            }
        }

        public static async Task<List<Genre>> GetGenreList()
        {
            var response = await SendApi<GenreResponse>("https://api.themoviedb.org/3/genre/movie/list");
            return response?.Genres ?? new List<Genre>();
        }

        public static async Task<MoviesResponse> GetPopularMovies(int page = 1)
        {
            return await SendApi<MoviesResponse>($"https://api.themoviedb.org/3/movie/popular?page={page}");
        }

        public static async Task<MoviesResponse> GetMoviesByName(string name, int page = 1)
        {
            return await SendApi<MoviesResponse>($"https://api.themoviedb.org/3/search/movie?query={name}&page={page}&include_adult=false");
        }

        public static async Task<MoviesResponse> GetMoviesByGenre(int genreId, int page = 1)
        {
            return await SendApi<MoviesResponse>($"https://api.themoviedb.org/3/discover/movie?with_genres={genreId}&page={page}");
        }

        public static async Task<Movie> GetMovieById(int id)
        {
            return await SendApi<Movie>($"https://api.themoviedb.org/3/movie/{id}");
        }

        
        public static async Task<MoviesResponse> GetSimilarMovies(int movieId)
        {
            return await SendApi<MoviesResponse>($"https://api.themoviedb.org/3/movie/{movieId}/similar");
        }
    }
}