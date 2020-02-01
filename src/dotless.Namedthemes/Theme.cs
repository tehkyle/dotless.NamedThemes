using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using dotless.Core.Parser.Tree;

namespace dotless.NamedThemes
{
    public class Theme
    {
        private Ruleset rules;

        public void Load(string themeName)
        {
            var themeBaseUrl = ConfigurationManager.AppSettings["dotless.Core.NamedThemes:ThemeBaseUrl"];
			if (string.IsNullOrEmpty(themeBaseUrl))
				themeBaseUrl = ConfigurationManager.AppSettings["dotless.NamedThemes:ThemeBaseUrl"];
			var themeBasePath = HttpContext.Current.Server.MapPath(themeBaseUrl);
            var themeBaseFile = Path.Combine(themeBasePath, themeName + ".less");
            var themeRelativeUri = themeBaseUrl + "?id=" + themeName;
            if (File.Exists(themeBaseFile))
                this.rules = GetCachedRulesetFromFile(themeBaseFile);
            else
            {
                var themeUri = new Uri(HttpContext.Current.Request.Url, themeRelativeUri);
                this.rules = GetCachedRulesetFromUri(themeUri);
            }
        }

        private Ruleset GetCachedRulesetFromUri(Uri themeUri)
        {
            string themeContent;
            using (WebClient client = new WebClient())
                themeContent = client.DownloadString(themeUri);

            var parser = new dotless.Core.Parser.Parser();
            Ruleset ruleset = parser.Parse(themeContent, themeUri.ToString());

            return ruleset;
        }

        private Ruleset GetCachedRulesetFromFile(string themeBaseFile)
        {
            var cacheKey = "dotless.namedtheme.basefile." + themeBaseFile;
            var cache = HttpContext.Current.Cache;

            var ruleset = cache[cacheKey] as Ruleset;
            if (ruleset == null)
            {
                var themeFileContent = File.ReadAllText(themeBaseFile);

                var parser = new dotless.Core.Parser.Parser();
                ruleset = parser.Parse(themeFileContent, themeBaseFile);

                cache.Insert(cacheKey, ruleset, new CacheDependency(themeBaseFile));
            }

            return ruleset;
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
