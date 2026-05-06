namespace dotless.NamedThemes
{
    public class NamedThemesOptions
    {
        /// <summary>
        /// Relative URL path to the themes directory or handler, e.g. "/Content/Themes/".
        /// Combined with <see cref="ApplicationBaseUrl"/> to form the absolute URI used for HTTP-based theme loading.
        /// </summary>
        public string ThemeBaseUrl { get; set; }

        /// <summary>
        /// Absolute filesystem path to the directory containing .less theme files,
        /// e.g. "C:\web\Content\Themes\". Used as the fallback when URI loading fails.
        /// </summary>
        public string ThemeBasePath { get; set; }

        /// <summary>
        /// Base URL of the host application, e.g. "https://myapp.com/".
        /// Required for URI-based theme loading. If not set, loading falls back to <see cref="ThemeBasePath"/>.
        /// </summary>
        public string ApplicationBaseUrl { get; set; }
    }
}
