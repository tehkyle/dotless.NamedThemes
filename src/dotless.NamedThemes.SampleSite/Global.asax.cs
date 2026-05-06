using System;
using System.Web;
using System.Web.Hosting;

namespace dotless.NamedThemes.SampleSite
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            NamedThemesConfig.Options = new NamedThemesOptions
            {
                ThemeBasePath    = HostingEnvironment.MapPath("~/Content/Themes/"),
                ThemeBaseUrl     = "/Content/Themes/",
                ApplicationBaseUrl = () =>
                {
                    var req = HttpContext.Current?.Request;
                    return req != null
                        ? $"{req.Url.Scheme}://{req.Url.Authority}{req.ApplicationPath.TrimEnd('/')}/"
                        : null;
                }
            };
        }
    }
}
