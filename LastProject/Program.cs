using LastProject;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles(); 
app.Map("/", async (HttpContext context) =>
{
    int currentPage = 1;
    if (int.TryParse(context.Request.Query["page"], out int p)) currentPage = p;

    try
    {
        var allCategories = await MovieApi.GetGenreList();
        var popularMovies = await MovieApi.GetPopularMovies(currentPage);
        
        await context.Response.WriteAsync(HtmlParts.GetHtmlPage($"""
        {HtmlParts.GetMoviesSection(popularMovies, allCategories)}
        {HtmlParts.GetHtmlPages(currentPage, popularMovies.Total_Pages, action: "")}
        """, title: "HotFlix - Главная"));
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        await context.Response.WriteAsync("Ошибка при загрузке данных.");
    }
});

app.Map("/category", async (HttpContext context) =>
{
    int id = 28; 
    int currentPage = 1;
    
    if (int.TryParse(context.Request.Query["categoryId"], out int newId)) id = newId;
    if (int.TryParse(context.Request.Query["page"], out int pageNumber)) currentPage = pageNumber;

    try
    {
        var allCategories = await MovieApi.GetGenreList();
        var movies = await MovieApi.GetMoviesByGenre(genreId: id, page: currentPage);
        
        await context.Response.WriteAsync(HtmlParts.GetHtmlPage($"""
        {HtmlParts.GetMoviesSection(movies, allCategories, id)}
        {HtmlParts.GetHtmlPages(currentPage, movies.Total_Pages, action: "category", categoryId: id)}
        """, title: "Категории"));
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
    string query = "";
    int currentPage = 1;

    if (context.Request.Method == "POST")
    {
        var form = await context.Request.ReadFormAsync();
        query = form["searchQuery"];
    }

    else 
    {
        query = context.Request.Query["query"];
        if (int.TryParse(context.Request.Query["page"], out int p)) currentPage = p;
    }

    if (string.IsNullOrEmpty(query))
    {
        context.Response.Redirect("/");
        return;
    }

    try
    {
        var movies = await MovieApi.GetMoviesByName(name: query, page: currentPage);
        await context.Response.WriteAsync(HtmlParts.GetHtmlPage($"""
        {HtmlParts.GetMoviesSection(movies, allCategories)}
        {HtmlParts.GetHtmlPages(currentPage, movies.Total_Pages, action: "search", query: query)}
        """, title: $"Поиск: {query}"));
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        context.Response.Redirect("/");
    }
});

// Детальная страница
app.Map("/details/{movieId}", async (string movieId, HttpContext context) =>
{
    if (int.TryParse(movieId, out int id))
    {
        try
        {
            var currentMovie = await MovieApi.GetMovieById(id);
            var similarMovies = await MovieApi.GetSimilarMovies(id);
            
            if (currentMovie != null)
            {
                await context.Response.WriteAsync(HtmlParts.GetHtmlPage(
                    HtmlParts.GetMoviePage(currentMovie, similarMovies), 
                    title: currentMovie.Title
                ));
            }
            else
            {
                 context.Response.Redirect("/notFound");
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
    await context.Response.WriteAsync(HtmlParts.GetHtmlPage(HtmlParts.GetNotFoundPage()));
});

app.Run();