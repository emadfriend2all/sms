using Showmatics.Blazor.Client.Infrastructure.Theme;

namespace Showmatics.Blazor.Client.Infrastructure.Preferences;

public class ClientPreference : IPreference
{
    public bool IsDarkMode { get; set; }
    public bool IsRTL { get; set; }
    public bool IsDrawerOpen { get; set; } = true;
    public string PrimaryColor { get; set; } = CustomColors.Light.Primary;
    public string SecondaryColor { get; set; } = CustomColors.Light.Secondary;
    public string LogoUrl { get; set; } = "Files/showmatics-logo.png";
    public double BorderRadius { get; set; } = 5;
    public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "ar-SA";
    public FshTablePreference TablePreference { get; set; } = new FshTablePreference();
}
