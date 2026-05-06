using System;
using System.IO;
using System.Runtime.Caching;
using dotless.Core;
using dotless.Core.configuration;
using dotless.Core.Plugins;
using dotless.NamedThemes;
using Xunit;

namespace dotless.NamedThemes.Tests
{
    /// <summary>
    /// Integration tests: drives the full dotless compilation pipeline on the current runtime.
    /// Each test simulates the NamedThemesConfig.Options setup that a host application performs
    /// in Startup.cs (ASP.NET Core) or Global.asax Application_Start (.NET Framework).
    /// </summary>
    [Collection("Sequential")]
    public class IntegrationTests : IDisposable
    {
        private readonly string _themesPath =
            Path.Combine(AppContext.BaseDirectory, "Fixtures", "Themes");

        public IntegrationTests() => ClearCache("blueTheme");

        /// <summary>
        /// Simulates ASP.NET Core startup:
        /// <code>
        /// NamedThemesConfig.Options = new NamedThemesOptions
        /// {
        ///     ThemeBasePath      = Path.Combine(env.WebRootPath, "Content", "Themes"),
        ///     ThemeBaseUrl       = "/Content/Themes/",
        ///     ApplicationBaseUrl = "https://myapp.com/"
        /// };
        /// </code>
        /// </summary>
        [Fact]
        public void Compile_CoreStartupPattern_ResolvesThemeColor()
        {
            NamedThemesConfig.Options = new NamedThemesOptions
            {
                ThemeBasePath = _themesPath
                // ApplicationBaseUrl omitted → URI loading skipped, filesystem used
            };

            var css = Compile("@c: getThemeColor(blueTheme, theme-base-color); .test { color: @c; }");

            Assert.Contains("#003d7d", css, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("getThemeColor", css);
        }

        /// <summary>
        /// Simulates ASP.NET Framework startup:
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
        /// </summary>
        [Fact]
        public void Compile_FrameworkGlobalAsaxPattern_ResolvesThemeColor()
        {
            NamedThemesConfig.Options = new NamedThemesOptions
            {
                ThemeBasePath = _themesPath
            };

            var css = Compile("@c: getThemeColor(blueTheme, theme-base-color); .test { color: @c; }");

            Assert.Contains("#003d7d", css, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("getThemeColor", css);
        }

        [Fact]
        public void Compile_MultipleThemeVariables_AllResolvedInOutput()
        {
            NamedThemesConfig.Options = new NamedThemesOptions { ThemeBasePath = _themesPath };

            const string less = @"
                @primary:   getThemeColor(blueTheme, theme-base-color);
                @bg:        getThemeColor(blueTheme, theme-nav-bar-bg-color);
                @link:      getThemeColor(blueTheme, theme-link-color);
                body { color: @primary; background: @bg; }
                a    { color: @link; }
            ";

            var css = Compile(less);

            Assert.Contains("#003d7d", css, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("#ffffff", css, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("getThemeColor", css);
        }

        public void Dispose() => ClearCache("blueTheme");

        private static string Compile(string less)
        {
            var config = new DotlessConfiguration { Web = false };
            config.Plugins.Add(new GenericPluginConfigurator<NamedThemesPlugin>());
            return Less.Parse(less, config);
        }

        private static void ClearCache(string themeName) =>
            MemoryCache.Default.Remove("dotless.namedtheme.basefile." + themeName);
    }
}
