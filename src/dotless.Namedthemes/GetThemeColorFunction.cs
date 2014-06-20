using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using dotless.Core.Parser.Functions;
using dotless.Core.Parser.Infrastructure;
using dotless.Core.Parser.Infrastructure.Nodes;
using dotless.Core.Parser.Tree;
using dotless.Core.Utils;

namespace dotless.NamedThemes
{
    public class GetThemeColorFunction : Function
    {
        protected override Node Evaluate(Env env)
        {
            Guard.ExpectNumArguments(2, Arguments.Count(), this, Location);
            Guard.ExpectNode<Keyword>(Arguments[0], this, Arguments[0].Location);
            Guard.ExpectNode<Keyword>(Arguments[1], this, Arguments[0].Location);

            var themeName = Arguments[0] as Keyword;
            var colorName = Arguments[1] as Keyword;

            var theme = new Theme();
            theme.Load(themeName);

            return theme.GetColor(colorName);
        }        
    }
}
