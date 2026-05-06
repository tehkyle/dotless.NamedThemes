using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using dotless.Core.Parser.Tree;

namespace dotless.NamedThemes
{
    public class Theme
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        private Ruleset rules;

        public void Load(string themeName)
        {
            var options = NamedThemesConfig.Options;
            var themeBasePath = options.ThemeBasePath;
            var themeBaseUrl = options.ThemeBaseUrl;

            var themeBaseFile = !string.IsNullOrEmpty(themeBasePath)
                ? Path.Combine(themeBasePath, themeName + ".less")
                : null;

            var cacheKey = "dotless.namedtheme.basefile." + themeName;
            var cache = MemoryCache.Default;
            rules = cache[cacheKey] as Ruleset;

            if (rules != null)
                return;

            var applicationBaseUrl = options.ApplicationBaseUrl?.Invoke();
            if (!string.IsNullOrEmpty(themeBaseUrl) && !string.IsNullOrEmpty(applicationBaseUrl))
            {
                try
                {
                    var relativeUri = themeBaseUrl.TrimStart('~').TrimStart('/') + "?id=" + themeName;
                    var themeUri = new Uri(applicationBaseUrl.TrimEnd('/') + "/" + relativeUri);
                    rules = GetRulesetFromUri(themeUri);

                    if (rules != null)
                    {
                        var policy = BuildCachePolicy(themeBaseFile);
                        cache.Set(cacheKey, rules, policy);
                        return;
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                }
            }

            // File fallback
            if (themeBaseFile != null && !string.IsNullOrEmpty(themeBasePath))
            {
                if (!File.Exists(themeBaseFile))
                    themeBaseFile = Directory.EnumerateFiles(themeBasePath, "*.less")
                        .OrderByDescending(t => t.Contains("seattleTheme.less"))
                        .FirstOrDefault()
                        ?? Path.Combine(themeBasePath, "seattleTheme.less");

                rules = GetRulesetFromFile(themeBaseFile);

                if (rules != null)
                    cache.Set(cacheKey, rules, BuildCachePolicy(themeBaseFile));
            }
        }

        private static CacheItemPolicy BuildCachePolicy(string filePath)
        {
            var policy = new CacheItemPolicy();
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                policy.ChangeMonitors.Add(new HostFileChangeMonitor(new[] { filePath }));
            return policy;
        }

        private static Ruleset GetRulesetFromUri(Uri themeUri)
        {
            var themeContent = HttpClient.GetStringAsync(themeUri).GetAwaiter().GetResult();
            var parser = new dotless.Core.Parser.Parser();
            return parser.Parse(themeContent, themeUri.ToString());
        }

        private static Ruleset GetRulesetFromFile(string themeBaseFile)
        {
            var themeFileContent = File.ReadAllText(themeBaseFile);
            var parser = new dotless.Core.Parser.Parser();
            return parser.Parse(themeFileContent, themeBaseFile);
        }

        public Value GetColor(string colorName)
        {
            var rule = rules?.Rules
                .OfType<Rule>()
                .SingleOrDefault(a => a.Name == "@" + colorName);

            return rule?.Value as dotless.Core.Parser.Tree.Value;
        }
    }
}
