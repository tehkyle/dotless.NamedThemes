namespace dotless.NamedThemes
{
    /// <summary>
    /// Static configuration holder for the NamedThemes plugin.
    /// Set <see cref="Options"/> once at application startup before dotless processes any requests.
    /// </summary>
    /// <example>
    /// .NET Core / ASP.NET Core (in Program.cs or Startup.cs):
    /// <code>
    /// NamedThemesConfig.Options = new NamedThemesOptions
    /// {
    ///     ThemeBasePath      = Path.Combine(env.WebRootPath, "Content", "Themes"),
    ///     ThemeBaseUrl       = "/Content/Themes/",
    ///     ApplicationBaseUrl = () => "https://myapp.com/"
    /// };
    /// </code>
    ///
    /// .NET Framework (in Global.asax Application_Start):
    /// <code>
    /// NamedThemesConfig.Options = new NamedThemesOptions
    /// {
    ///     ThemeBasePath      = HostingEnvironment.MapPath("~/Content/Themes/"),
    ///     ThemeBaseUrl       = "/Content/Themes/",
    ///     ApplicationBaseUrl = () =>
    ///     {
    ///         var req = HttpContext.Current?.Request;
    ///         return req != null
    ///             ? $"{req.Url.Scheme}://{req.Url.Authority}{req.ApplicationPath.TrimEnd('/')}/"
    ///             : null;
    ///     }
    /// };
    /// </code>
    /// </example>
    public static class NamedThemesConfig
    {
        public static NamedThemesOptions Options { get; set; } = new NamedThemesOptions();
    }
}
