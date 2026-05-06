using System;
using System.IO;
using System.Runtime.Caching;
using dotless.NamedThemes;
using Xunit;

namespace dotless.NamedThemes.Tests
{
    /// <summary>
    /// Unit tests for the Theme class in isolation (no dotless compilation pipeline).
    /// </summary>
    [Collection("Sequential")]
    public class ThemeTests : IDisposable
    {
        private readonly string _themesPath =
            Path.Combine(AppContext.BaseDirectory, "Fixtures", "Themes");

        public ThemeTests()
        {
            ClearCache("blueTheme");
            ClearCache("nonexistentTheme");
        }

        [Fact]
        public void Load_WithValidTheme_ParsesColorVariables()
        {
            NamedThemesConfig.Options = new NamedThemesOptions { ThemeBasePath = _themesPath };

            var theme = new Theme();
            theme.Load("blueTheme");

            Assert.NotNull(theme.GetColor("theme-base-color"));
        }

        [Fact]
        public void GetColor_ReturnsExpectedHexValue()
        {
            NamedThemesConfig.Options = new NamedThemesOptions { ThemeBasePath = _themesPath };

            var theme = new Theme();
            theme.Load("blueTheme");
            var color = theme.GetColor("theme-base-color");

            Assert.Contains("#003d7d", color.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetColor_WithMissingVariable_ReturnsNull()
        {
            NamedThemesConfig.Options = new NamedThemesOptions { ThemeBasePath = _themesPath };

            var theme = new Theme();
            theme.Load("blueTheme");

            Assert.Null(theme.GetColor("nonexistent-variable"));
        }

        [Fact]
        public void Load_WithMissingThemeFile_FallsBackToFirstAvailableLessFile()
        {
            NamedThemesConfig.Options = new NamedThemesOptions { ThemeBasePath = _themesPath };

            // "nonexistentTheme.less" doesn't exist — should fall back to blueTheme.less
            var theme = new Theme();
            theme.Load("nonexistentTheme");

            Assert.NotNull(theme.GetColor("theme-base-color"));
        }

        public void Dispose()
        {
            ClearCache("blueTheme");
            ClearCache("nonexistentTheme");
        }

        private static void ClearCache(string themeName) =>
            MemoryCache.Default.Remove("dotless.namedtheme.basefile." + themeName);
    }
}
