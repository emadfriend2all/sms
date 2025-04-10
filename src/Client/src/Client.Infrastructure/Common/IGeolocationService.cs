using BrowserInterop.Geolocation;

namespace Showmatics.Blazor.Client.Infrastructure.Common;
public interface IGeolocationService
{
    Task<bool> CheckGeolocationPermission();
    Task<GeolocationPosition?> GetCurrentLocation();
    Task InitAsync();
    Task<bool> RequestGeolocationPermission();
    Task StopWatch();
    Task<GeolocationPosition?> WatchCurrentLocation();
}