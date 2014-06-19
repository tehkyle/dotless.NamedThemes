using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotless.Core.Plugins;

namespace dotless.NamedThemes
{
    [DisplayName("NamedThemesPlugin")]
    public class NamedThemesPlugin : IFunctionPlugin
    {
        public Dictionary<string, Type> GetFunctions()
        {
            return new Dictionary<string, Type> 
            {
                { "getThemeColor", typeof(GetThemeColorFunction) },
            };
        }
    }

}
