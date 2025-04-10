using System.Globalization;
using Showmatics.Blazor.Client;
using Showmatics.Blazor.Client.Infrastructure;
using Showmatics.Blazor.Client.Infrastructure.Common;
using Showmatics.Blazor.Client.Infrastructure.Preferences;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddClientServices(builder.Configuration);

var host = builder.Build();

var storageService = host.Services.GetRequiredService<IClientPreferenceManager>();
if (storageService != null)
{
    CultureInfo culture;
    if (await storageService.GetPreference() is ClientPreference preference)
    {
        culture = new CultureInfo(preference.LanguageCode);
    }
    else
    {
        culture = new CultureInfo(LocalizationConstants.SupportedLanguages.FirstOrDefault(x => x.Code.Contains("ar-SA"))?.Code ?? "en-US");
    }

    culture.DateTimeFormat.ShortDatePattern = "dd-MM-yy";
    culture.DateTimeFormat.FullDateTimePattern = "dd-MM-yy HH:mm:ss";
    culture.NumberFormat.NumberDecimalSeparator = ".";
    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}

await host.RunAsync();