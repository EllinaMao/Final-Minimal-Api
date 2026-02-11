namespace LastProject
{
    {
        public static class HtmlParts
    {
        public static string GetHtmlPage(string body, string query = "")
        {
            return $"""
            <!DOCTYPE html>
            <html lang="ru">
            <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
                {GetStyleSection()}
                {GetMetaSection()}
            </head>
            <body class="body">
            {GetHeaderSection(query)} 
            {body}
            {GetFooterSection()}
            {GetScriptSection()}
            </body>
            </html>
            """;
        }

        public static string GetNotFoundPage()
        {
            return """
                <div class="not-found">
                    <h1 class="not-found__title">404</h1>
                    <p class="not-found__text">Страница не найдена</p>
                    <a href="/" class="not-found__link">Вернуться на главную</a>
                </div>
                """;
        }

        public static string GetHeaderSection(string query = "")
        {


        }
        // Метод GetHeaderSection вызывается выше, но его код отсутствовал на скриншотах.

        public static string GetHtmlPages(int currentPage, int totalPages, string action = "/", string query = "", int? categoryId = null)
        {
            // Код на скриншоте обрезан, воспроизведено только видимое начало
            string pages = $"""
                <li class="paginator__item paginator__item--prev">
                <a href="{action}?query={query}&categoryId={categoryId}&page={currentPage - 1}"><i class="icon ion-ios-arrow-back"></i></a>
            """;
            // ... продолжение логики пагинации
            return pages;
        }

        static string GetStyleSection()
        {
            return """
                <link rel="stylesheet" href="../css/bootstrap-reboot.min.css">
                <link rel="stylesheet" href="../css/bootstrap-grid.min.css">
                <link rel="stylesheet" href="../css/owl.carousel.min.css">
                <link rel="stylesheet" href="../css/jquery.mCustomScrollbar.min.css">
                <link rel="stylesheet" href="../css/nouislider.min.css">
                <link rel="stylesheet" href="../css/ionicons.min.css">
                <link rel="stylesheet" href="../css/magnific-popup.css">
                <link rel="stylesheet" href="../css/plyr.css">
                <link rel="stylesheet" href="../css/photoswipe.css">
                <link rel="stylesheet" href="../css/default-skin.css">
                <link rel="stylesheet" href="../css/main.css">
            """;
        }

        static string GetMetaSection(string title = "HotFlix")
        {
            return $"""
                <link rel="icon" type="image/png" href="icon/favicon-32x32.png" sizes="32x32">
                <link rel="apple-touch-icon" href="icon/favicon-32x32.png">

                <meta name="description" content="">
                <meta name="keywords" content="">
                <meta name="author" content="">
                <title>{title}</title>
            """;
        }

        static string GetScriptSection()
        {
            return """
                <script src="../js/jquery-3.5.1.min.js"></script>
                <script src="../js/bootstrap.bundle.min.js"></script>
                <script src="../js/owl.carousel.min.js"></script>
                <script src="../js/jquery.magnific-popup.min.js"></script>
                <script src="../js/jquery.mousewheel.min.js"></script>
                <script src="../js/jquery.mCustomScrollbar.min.js"></script>
                <script src="../js/wNumb.js"></script>
                <script src="../js/nouislider.min.js"></script>
                <script src="../js/plyr.min.js"></script>
                <script src="../js/photoswipe.min.js"></script>
                <script src="../js/photoswipe-ui-default.min.js"></script>
                <script src="../js/main.js"></script>
            """;
        }

        static string GetFooterSection()
        {
            return """
                <footer class="footer">
                    <div class="container">
                        <div class="row">
                            <div class="col-12">
                                <div class="footer__content">
                                    <a href="index" class="footer__logo">
                                        <img src="img/logo.svg" alt="">
                                    </a>

                                    <span class="footer__copyright">©HotFlix 2023</span>

                                    <nav class="footer__nav">
                                        <a href="/">Главная</a>
                                        <a href="#">О нас</a>
                                        <a href="#">Контакты</a>
                                    </nav>

                                    <button class="footer__back" type="button">
                                        <i class="icon ion-ios-arrow-round-up"></i>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </footer>
            """;
        }
    }
}

