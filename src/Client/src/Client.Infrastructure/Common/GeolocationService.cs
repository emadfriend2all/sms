using BrowserInterop.Extensions;
using BrowserInterop.Geolocation;
using Microsoft.JSInterop;

namespace Showmatics.Blazor.Client.Infrastructure.Common;
public class GeolocationService : IGeolocationService
{
    private readonly IJSRuntime _jsRuntime;
    private WindowNavigatorGeolocation _geolocationWrapper = default!;
    private GeolocationPosition _currentLocation = default!;
    private List<GeolocationPosition> _locationHistory = new();
    private IAsyncDisposable? _watcher;
    public Func<GeolocationPosition?, Task>? _onPositionChanged;
    public GeolocationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<GeolocationPosition?> GetCurrentLocation()
    {
        try
        {
            var geolocation = await _geolocationWrapper.GetCurrentPosition(new PositionOptions()
            {
                EnableHighAccuracy = true,
                MaximumAgeTimeSpan = TimeSpan.FromHours(1),
                TimeoutTimeSpan = TimeSpan.FromMinutes(1)
            });

            return geolocation.Location;
        }
        catch (Exception ex)
        {
            return null!;
        }
    }

    public async Task<GeolocationPosition?> WatchCurrentLocation()
    {
        await InitAsync();
        try
        {
            _watcher = await _geolocationWrapper.WatchPosition(async (position) =>
            {
                _currentLocation = position.Location;
                _locationHistory.Add(position.Location);
                if (position.Location != null && _onPositionChanged != null)
                {
                    await _onPositionChanged.Invoke(position?.Location);
                }
            });

            return _currentLocation;
        }
        catch (Exception ex)
        {
            return null!;
        }
    }

    public async Task StopWatch()
    {
        if (_watcher != null)
        {
            await _watcher.DisposeAsync();
            _watcher = null;
        }
    }

    public async Task<bool> CheckGeolocationPermission()
    {
        try
        {
            string permissionState = await _jsRuntime.InvokeAsync<string>("navigator.permissions.query", "geolocation");

            // Check if the permission state is 'granted'
            return permissionState == "granted";
        }
        catch (Exception ex)
        {
            // Handle exceptions or errors when checking geolocation permission
            Console.WriteLine(ex.Message);
            return false; // Return false if unable to determine permission status
        }
    }

    public async Task<bool> RequestGeolocationPermission()
    {
        try
        {
            string permissionStatus = await _jsRuntime.InvokeAsync<string>("navigator.permissions.query", new { name = "geolocation" });

            if (permissionStatus != "granted") // Request permission if not already granted
            {
                string permissionResult = await _jsRuntime.InvokeAsync<string>("navigator.geolocation.requestPermission");

                return permissionResult == "granted";
            }

            return true; // Permission already granted
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task InitAsync()
    {
        var window = await _jsRuntime.Window();
        var navigator = await window.Navigator();
        _geolocationWrapper = navigator.Geolocation;
    }
}