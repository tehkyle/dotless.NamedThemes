using System;
using System.Web;

namespace dotless.NamedThemes.SampleSite
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            NamedThemesConfig.Options = new NamedThemesOptions
            {
                ThemeBasePath = Server.MapPath("~/Content/Themes/"),
                ThemeBaseUrl  = "/Content/Themes/"
                // ApplicationBaseUrl: set this to enable URI-based theme loading,
                // e.g. $"{Request.Url.Scheme}://{Request.Url.Authority}/"
                // Leave unset to use filesystem loading only.
            };
        }
    }
}
