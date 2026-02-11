var builder = WebApplication.CreateBuilder(args);
var allServices = builder.Services;

builder.Services.AddSingleton<IUserRepository, UserRepository>();

var app = builder.Build();

app.Map("/category", async (HttpContext context) =>
{
    int id = 1, currentPage = 1;
    string categoryId = context.Request.Query["categoryId"];
    string page = context.Request.Query["page"];

    if (int.TryParse(categoryId, out int newId))
    {
        id = newId;
    }

    if (int.TryParse(page, out int pageNumber))
    {
        currentPage = pageNumber;
    }

    try
    {
        var allCategories = await MovieApi.GetGenreList();
        var movies = await MovieApi.GetMoviesByGenre(genreId: id, page: currentPage);
        await context.Response.WriteAsync(HtmlParts.GetHtmlPage($"""
        {HtmlParts.GetMoviesSection(movies, allCategories, id)}
        {HtmlParts.GetHtmlPages(movies.Page, movies.Total_Pages, action: "/category", categoryId: id)}
        """));
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        context.Response.Redirect("/");
    }
});

app.Map("/search", async (HttpContext context) =>
{
    var allCategories = await MovieApi.GetGenreList();

    if (context.Request.Method == "POST")
    {
        var form = context.Request.Form;
        string query = form["searchQuery"];
        try
        {
            var movies = await MovieApi.GetMoviesByName(name: query, page: 1);
            await context.Response.WriteAsync(HtmlParts.GetHtmlPage($"""
            {HtmlParts.GetMoviesSection(movies, allCategories)}
            {HtmlParts.GetHtmlPages(movies.Page, movies.Total_Pages, action: "search", query: query)}
            """, query: query));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            context.Response.Redirect("/");
        }
    }
    else
    {
        int currentPage = 1;
        string page = context.Request.Query["page"];
        string query = context.Request.Query["query"];

        if (int.TryParse(page, out int pageNumber))
        {
            currentPage = pageNumber;
        }

        try
        {
            // Этот блок 'try' был обрезан на скриншотах, но исходя из логики POST-запроса, 
            // код должен выглядеть так:
            var movies = await MovieApi.GetMoviesByName(name: query, page: currentPage);
            await context.Response.WriteAsync(HtmlParts.GetHtmlPage($"""
            {HtmlParts.GetMoviesSection(movies, allCategories)}
            {HtmlParts.GetHtmlPages(movies.Page, movies.Total_Pages, action: "search", query: query)}
            """, query: query));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            context.Response.Redirect("/");
        }
    }
});

app.Map("/details/{movieId}", async (string movieId, HttpContext context) =>
{
    if (int.TryParse(movieId, out int id))
    {
        try
        {
            var currentMovie = await MovieApi.GetMovieById(id);
            if (currentMovie != null)
            {
                await context.Response.WriteAsync(HtmlParts.GetHtmlPage($"""
                {HtmlParts.GetMoviePage(currentMovie)}
                """));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            context.Response.Redirect("/");
        }
    }
    else
    {
        context.Response.Redirect("/");
    }
});

app.Map("/notFound", async (HttpContext context) =>
{
    await context.Response.WriteAsync(HtmlParts.GetHtmlPage($"""
    {HtmlParts.GetNotFoundPage()}
    """));
});

app.Run();


static async Task<T> SendApi<T>(string query)
{
    string apiKey = GetMovieApiKey();
    HttpClient httpClient = new HttpClient();
    // Обратите внимание: в реальном коде HttpClient лучше создавать через IHttpClientFactory
    HttpResponseMessage responseMessage = await httpClient.GetAsync(query + $"&api_key={apiKey}");

    if (responseMessage.IsSuccessStatusCode)
    {
        var result = await responseMessage.Content.ReadFromJsonAsync<T>();
        return result;
    }
    else
    {
        throw new Exception("Error. Try again later.");
    }
}

static string GetMovieApiKey()
{
    var builder = new ConfigurationBuilder();
    // установка пути к текущему каталогу
    builder.SetBasePath(Directory.GetCurrentDirectory());
    // получаем конфигурацию из файла appsettings.json
    builder.AddJsonFile("appsettings.json");
    // создаем конфигурацию
    var config = builder.Build();
    // получаем строку подключения
    var connectionString = config.GetSection("MovieApi:ApiKey");
    return connectionString.Value;
}

public static async Task<Movies> GetPopularMovies(int page = 1)
{
    return await SendApi<Movies>($"https://api.themoviedb.org/3/movie/popular?language=ru-ru&page={page}&include_adult=true");
}

public static async Task<Movies> GetMoviesByName(string name, int page = 1)
{
    return await SendApi<Movies>($"https://api.themoviedb.org/3/search/movie?language=ru-ru&query={name}&page={page}&include_adult=true");
}

public static async Task<Movies> GetMoviesByGenre(int genreId, int page = 1)
{
    return await SendApi<Movies>($"https://api.themoviedb.org/3/discover/movie?with_genres={genreId}&page={page}&include_adult=true&language=ru-ru");
}

public static async Task<Movie> GetMovieById(int id)
{
    return await SendApi<Movie>($"https://api.themoviedb.org/3/movie/{id}?language=ru-ru");
}

public static async Task<Movies> GetSimilarMovies(int movieId, int page = 1)
{
    return await SendApi<Movies>($"https://api.themoviedb.org/3/movie/{movieId}/similar?language=ru-ru&page={page}&include_adult=true");
}

public static async Task<Movies> GetMoviesByYearAndName(string name, int year, int page = 1)
{
    return await SendApi<Movies>($"https://api.themoviedb.org/3/search/movie?language=ru-ru&query={name}&page={page}&include_adult=false&year={year}");
}
        
        // Метод GetGenreList упоминается в Program.cs, но его реализация отсутствовала на скриншотах.
        // Вероятно, он выглядит похоже на остальные:
        // public static async Task<Genres> GetGenreList() { ... }
    }
}