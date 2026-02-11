using System.Text;

namespace LastProject
{
    public static class HtmlParts
    {
        public static string GetHtmlPage(string body, string title = "HotFlix")
        {
            return $"""
            <!DOCTYPE html>
            <html lang="ru">
            <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
                {GetStyleSection()}
                <title>{title}</title>
            </head>
            <body class="body">
                {GetHeaderSection()} 
                {body}
                {GetFooterSection()}
                {GetScriptSection()}
            </body>
            </html>
            """;
        }

        // Хедер с формой поиска
        public static string GetHeaderSection()
        {
            return """
            <header class="header">
                <div class="container">
                    <div class="row">
                        <div class="col-12">
                            <div class="header__content">
                                <a href="/" class="header__logo">
                                    <img src="/img/logo.svg" alt="">
                                </a>
                                <ul class="header__nav">
                                    <li class="header__nav-item"><a href="/" class="header__nav-link">Главная</a></li>
                                    <li class="header__nav-item"><a href="/category" class="header__nav-link">Каталог</a></li>
                                </ul>
                                <div class="header__auth">
                                    <form action="/search" method="POST" class="header__search">
                                        <input class="header__search-input" name="searchQuery" type="text" placeholder="Поиск...">
                                        <button class="header__search-button" type="button"><i class="icon ion-ios-search"></i></button>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </header>
            """;
        }

        // Генерация сетки фильмов
        public static string GetMoviesSection(MoviesResponse moviesData, List<Genre> allGenres, int? currentGenreId = null)
        {
            var sb = new StringBuilder();

            // Фильтр категорий (простой dropdown)
            sb.Append("""
            <section class="content">
                <div class="content__head">
                    <div class="container">
                        <div class="row">
                            <div class="col-12">
                                <h2 class="content__title">Фильмы</h2>
                                <div class="content__tabs">
                                    <a href="/" class="active">Все</a>
            """);

            foreach (var g in allGenres.Take(5))
            {
                string colorStyle = (currentGenreId == g.Id) ? "color:#ff55a5;" : "";

                sb.Append($"<a href='/category?categoryId={g.Id}' style='margin-left:15px; {colorStyle}'>{g.Name}</a>");
            }

            sb.Append("""
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="container">
                    <div class="row">
            """);

            foreach (var movie in moviesData.Results)
            {
                string imagePath = string.IsNullOrEmpty(movie.Poster_Path)
                    ? "img/covers/cover.jpg"
                    : $"{MovieApi.ImageBaseUrl}{movie.Poster_Path}";

                string genreName = "Unknown";
                if (movie.Genre_Ids.Count > 0)
                {
                    var g = allGenres.FirstOrDefault(x => x.Id == movie.Genre_Ids[0]);
                    if (g != null) genreName = g.Name;
                }

                sb.Append($"""
                 <div class="col-6 col-sm-4 col-lg-3 col-xl-2">
                    <div class="item">
                        <div class="item__cover">
                            <img src="{imagePath}" alt="{movie.Title}">
                            <a href="/details/{movie.Id}" class="item__play">
                                <i class="icon ion-ios-play"></i>
                            </a>
                            <span class="item__rate item__rate--green">{movie.Vote_Average:F1}</span>
                        </div>
                        <div class="item__content">
                            <h3 class="item__title"><a href="/details/{movie.Id}">{movie.Title}</a></h3>
                            <span class="item__category">
                                <a href="#">{genreName}</a>
                            </span>
                        </div>
                    </div>
                </div>
                """);
            }

            sb.Append("</div></div></section>");
            return sb.ToString();
        }


        public static string GetHtmlPages(int currentPage, int totalPages, string action, string query = "", int? categoryId = null)
        {
            if (totalPages <= 1) return "";

            var sb = new StringBuilder();
            sb.Append("<div class='container'><div class='row'><div class='col-12'><ul class='paginator'>");

            if (currentPage > 1)
            {
                string prevLink = BuildLink(action, currentPage - 1, query, categoryId);
                sb.Append($"<li class='paginator__item paginator__item--prev'><a href='{prevLink}'><</a></li>");
            }

            int range = 2;
            int startPage = Math.Max(1, currentPage - range);
            int endPage = Math.Min(totalPages, currentPage + range);

            if (endPage - startPage < range * 2)
            {
                if (startPage == 1)
                    endPage = Math.Min(totalPages, startPage + range * 2);
                else if (endPage == totalPages)
                    startPage = Math.Max(1, endPage - range * 2);
            }

            for (int i = startPage; i <= endPage; i++)
            {
                if (i == currentPage)
                {
                    sb.Append($"<li class='paginator__item paginator__item--active'><a href='#'>{i}</a></li>");
                }
                else
                {
                    string link = BuildLink(action, i, query, categoryId);
                    sb.Append($"<li class='paginator__item'><a href='{link}'>{i}</a></li>");
                }
            }

            if (currentPage < totalPages)
            {
                string nextLink = BuildLink(action, currentPage + 1, query, categoryId);
                sb.Append($"<li class='paginator__item paginator__item--next'><a href='{nextLink}'>></a></li>");
            }

            sb.Append("</ul></div></div></div>");
            return sb.ToString();
        }

        private static string BuildLink(string action, int page, string query, int? categoryId)
        {
            string url = $"/{action}?page={page}";
            if (!string.IsNullOrEmpty(query)) url += $"&query={query}"; 
            if (categoryId.HasValue) url += $"&categoryId={categoryId}";
            return url;
        }

        // Страница одного фильма
        public static string GetMoviePage(Movie movie, MoviesResponse similar)
        {
            string imagePath = string.IsNullOrEmpty(movie.Poster_Path) ? "" : $"{MovieApi.ImageBaseUrl}{movie.Poster_Path}";
            string genres = string.Join(", ", movie.Genres.Select(g => g.Name));

            var sb = new StringBuilder();

            // Блок деталей
            sb.Append($"""
            <section class="section section--details section--bg" style="margin-top: 70px;">
                <div class="container">
                    <div class="row">
                        <div class="col-12">
                            <h1 class="section__title section__title--head">{movie.Title}</h1>
                        </div>
                        <div class="col-12 col-xl-6">
                            <div class="item item--details">
                                <div class="row">
                                    <div class="col-12 col-sm-5 col-md-5 col-lg-4 col-xl-6 col-xxl-5">
                                        <div class="item__cover">
                                            <img src="{imagePath}" alt="">
                                            <span class="item__rate item__rate--green">{movie.Vote_Average:F1}</span>
                                        </div>
                                    </div>
                                    <div class="col-12 col-md-7 col-lg-8 col-xl-6 col-xxl-7">
                                        <div class="item__content">
                                            <ul class="item__meta">
                                                <li><span>Жанры:</span> {genres}</li>
                                                <li><span>Дата выхода:</span> {movie.Release_Date}</li>
                                            </ul>
                                            <div class="item__description">
                                                <p>{movie.Overview}</p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
            """);

            if (similar != null && similar.Results.Count > 0)
            {
                sb.Append("""
                <section class="section section--border">
                    <div class="container">
                        <div class="row">
                            <div class="col-12">
                                <h2 class="section__title">Похожие фильмы</h2>
                            </div>
                """);

                foreach (var sim in similar.Results.Take(5))
                {
                    string simImg = string.IsNullOrEmpty(sim.Poster_Path) ? "img/covers/cover.jpg" : $"{MovieApi.ImageBaseUrl}{sim.Poster_Path}";
                    sb.Append($"""
                    <div class="col-6 col-sm-4 col-lg-2">
                        <div class="item">
                            <div class="item__cover">
                                <img src="{simImg}" alt="">
                                <a href="/details/{sim.Id}" class="item__play"><i class="icon ion-ios-play"></i></a>
                            </div>
                            <div class="item__content">
                                <h3 class="item__title"><a href="/details/{sim.Id}">{sim.Title}</a></h3>
                            </div>
                        </div>
                    </div>
                    """);
                }
                sb.Append("</div></div></section>");
            }

            return sb.ToString();
        }

        public static string GetNotFoundPage()
        {
            return """
                <div class="container" style="padding: 100px 0; text-align: center;">
                    <h1 style="color: #fff; font-size: 60px;">404</h1>
                    <p style="color: #fff;">Страница не найдена</p>
                    <a href="/" style="color: #ff55a5;">Вернуться на главную</a>
                </div>
                """;
        }

       
        static string GetStyleSection()
        {
            return """
                <link rel="stylesheet" href="/css/bootstrap.min.css">
                <link rel="stylesheet" href="/css/main.css">
                <link rel="stylesheet" href="/webfont/tabler-icons.min.css">
                <style>
                    /* Дополнительный CSS для правильного отображения на белом фоне, если нужно, 
                       но ваш шаблон кажется темным */
                    .body { background-color: #2b2b31; color: #fff; }
                </style>
            """;
        }

        static string GetFooterSection()
        {
            return """
            <footer class="footer">
                <div class="container">
                    <span class="footer__copyright">© HotFlix 2026</span>
                </div>
            </footer>
            """;
        }

        static string GetScriptSection()
        {
            return """
                <script src="/js/bootstrap.bundle.min.js"></script>
                <script src="/js/main.js"></script>
            """;
        }
    }
}