using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OrchardCore.Localization;
using ShowMatic.Server.Infrastructure.Localization;
using Showmatics.Infrastructure.Middleware;

namespace Showmatics.Infrastructure.Localization;

internal static class Startup
{
    internal static IServiceCollection AddPOLocalization(this IServiceCollection services, IConfiguration config)
    {
        var localizationSettings = config.GetSection(nameof(LocalizationSettings)).Get<LocalizationSettings>();

        if (localizationSettings?.EnableLocalization is true
            && localizationSettings.ResourcesPath is not null)
        {
            services.AddPortableObjectLocalization(options => options.ResourcesPath = localizationSettings.ResourcesPath);
            services.Configure<RequestLocalizationOptions>(options =>
            {
                if (localizationSettings.SupportedCultures != null)
                {
                    var supportedCultures = localizationSettings.SupportedCultures.Select(x => new CultureInfo(x)).ToList();
                    foreach (var culture in supportedCultures)
                    {
                        culture.DateTimeFormat.ShortDatePattern = "dd-MM-yy";
                        culture.DateTimeFormat.FullDateTimePattern = "dd-MM-yy HH:mm:ss";
                        culture.NumberFormat.NumberDecimalSeparator = ".";
                    }

                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                }

                options.DefaultRequestCulture = new RequestCulture(localizationSettings.DefaultRequestCulture ?? "ar-SA");
                options.FallBackToParentCultures = localizationSettings.FallbackToParent ?? true;
                options.FallBackToParentUICultures = localizationSettings.FallbackToParent ?? true;
            });

            services.AddSingleton<ILocalizationFileLocationProvider, FSHPoFileLocationProvider>();

        }

        return services;
    }

    internal static IServiceCollection AddJsonLocalization(this IServiceCollection services, IConfiguration config)
    {
        var localizationSettings = config.GetSection(nameof(LocalizationSettings)).Get<LocalizationSettings>();

        if (localizationSettings?.EnableLocalization is true
            && localizationSettings.ResourcesPath is not null)
        {
            services.AddLocalization(options => options.ResourcesPath = localizationSettings.ResourcesPath);
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                if (localizationSettings.SupportedCultures != null)
                {
                    var supportedCultures = localizationSettings.SupportedCultures.Select(x => new CultureInfo(x)).ToList();

                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                }

                options.DefaultRequestCulture = new RequestCulture(localizationSettings.DefaultRequestCulture ?? "ar-SA");
                options.FallBackToParentCultures = localizationSettings.FallbackToParent ?? true;
                options.FallBackToParentUICultures = localizationSettings.FallbackToParent ?? true;
            });

            services.AddSingleton<LocalizationMiddleware>();
        }

        return services;
    }

    internal static IApplicationBuilder UseLocalization(this IApplicationBuilder app, IConfiguration config)
    {
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(new CultureInfo("en-US"))
        });

        var middlewareSettings = config.GetSection(nameof(MiddlewareSettings)).Get<MiddlewareSettings>();
        if (middlewareSettings.EnableLocalization)
        {
            app.UseMiddleware<LocalizationMiddleware>();
        }

        return app;
    }
}