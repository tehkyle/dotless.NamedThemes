dotless.NamedThemes
===================

Allows you to implement switchable themes, each theme stored in a different .less file

Here's what you need to do. First, add it to your projet using [the NuGet package](http://www.nuget.org/packages/dotless.NamedThemes/);

Then enable the plugin by adding it to the `<dotless>` element in web.config;

    <dotless minifyCss="false" cache="false" web="false">
        <plugin name="NamedThemesPlugin" assembly="dotless.NamedThemes" />
    </dotless>

Tell the web.config about the folder that holds your theme files;

    <configuration>
        <appSettings>
            <add key="dotless.NamedThemes:ThemeBaseUrl" value="~/Content/Themes" />
        </appSettings>
    </configuration>

Write files named after your theme;

    // ~/Content/Theme/blueTheme.less
    @theme-base-color:#003d7d;
	@theme-base-secondary-color:#003d7d;
	@theme-font-size-base:14px;
	@theme-header-font-size-base:12px;
	@theme-nav-bar-color:#3a3a3a;
	@theme-nav-bar-bg-color:#FFFFFF;
	@theme-link-color:#003d7d;

Now, the clever bit; in your actual .less file, use the `getThemeColor` function;

    // ~/Content/themeAware.less
    @theme-base-color:getThemeColor(@theme,theme-base-color);
	@theme-base-secondary-color:getThemeColor(@theme,theme-base-secondary-color);

And make sure you pass the @theme variable in the url;

    http://mysite/content/themeAware.less?theme=blueTheme

Voila! The theme name and the theme-specific colour are retrieved by the plugin.