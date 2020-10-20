using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using dotless.Core.Parser.Tree;

namespace dotless.NamedThemes
{
    public class Theme
    {
        private Ruleset rules;

        private string BaseUrl => 
            HttpContext.Current.Request.Url.Scheme + "://" + 
            HttpContext.Current.Request.Url.Authority + 
            HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";

        public void Load(string themeName)
        {
            var themeBaseUrl = ConfigurationManager.AppSettings["dotless.Core.NamedThemes:ThemeBaseUrl"];
			if (string.IsNullOrEmpty(themeBaseUrl))
				themeBaseUrl = ConfigurationManager.AppSettings["dotless.NamedThemes:ThemeBaseUrl"];

            var themeRelBasePath = ConfigurationManager.AppSettings["dotless.Core.NamedThemes:ThemeBasePath"];
            if (string.IsNullOrEmpty(themeRelBasePath))
                themeRelBasePath = ConfigurationManager.AppSettings["dotless.NamedThemes:ThemeBasePath"];

            var themeBasePath = HttpContext.Current.Server.MapPath(themeRelBasePath);
            var themeBaseFile = Path.Combine(themeBasePath, themeName + ".less");
            var themeRelativeUri = themeBaseUrl.TrimStart('~').TrimStart('/') + "?id=" + themeName;

            var cacheKey = "dotless.namedtheme.basefile." + themeName;
            var cache = HttpContext.Current.Cache;
            rules = cache[cacheKey] as Ruleset;

            if (rules == null)
            {
                try
                {
                    var themeUri = new Uri(BaseUrl + themeRelativeUri);
                    rules = GetCachedRulesetFromUri(themeUri);

                    if (rules != null)
                        cache.Insert(cacheKey, rules, new CacheDependency(themeBaseFile));
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString());
                    if (!File.Exists(themeBaseFile))
                        themeBaseFile = Directory.EnumerateFiles(themeBasePath, "*.less").OrderByDescending(t => t.Contains("seattleTheme.less")).FirstOrDefault() ?? Path.Combine(themeBasePath, "seattleTheme.less"); // IGX-dependant hardcode fallback.
                    rules = GetCachedRulesetFromFile(themeBaseFile);
                }
            }
        }

        private Ruleset GetCachedRulesetFromUri(Uri themeUri)
        {
            string themeContent;
            using (WebClient client = new WebClient())
                themeContent = client.DownloadString(themeUri);

            var parser = new dotless.Core.Parser.Parser();
            return parser.Parse(themeContent, themeUri.ToString());
        }

        private Ruleset GetCachedRulesetFromFile(string themeBaseFile)
        {
            var themeFileContent = File.ReadAllText(themeBaseFile);

            var parser = new dotless.Core.Parser.Parser();
            return parser.Parse(themeFileContent, themeBaseFile);
        }

        public Value GetColor(string colorName)
        {
            var rule = rules.Rules
                .OfType<Rule>()
                .SingleOrDefault(a => a.Name == "@" + colorName);

            if (rule == null)
            {
                return null;
            }

            var color = rule.Value as dotless.Core.Parser.Tree.Value;

            return color;
        }
    }
}
