using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ShowMatic.Server.Application.Catalog.Smss.GetDetails;
public static class ApiHelper
{
    public const string BaseUrl = @"https://dash.brqsms.com/api/v3";
    public const string AuthToken = "4|AsHxMhKoITJYTUa6QgshEg9QULmdj4sJdWAPi3s727e71bc7 ";
    public static async Task<TResponse> GetAsync<TResponse>(string path)
    {
        var url = $"{BaseUrl}{path}";
        try
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TResponse>(content);
                return result == null ? throw new Exception("Error deserializing response") : result;
            }
        }
        catch (Exception)
        {

            throw new Exception("Error during Get Request");
        }
    }

    public static async Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest request)
    {
        var url = new Uri($"{BaseUrl}{path}");
        try
        {
            using var httpClient = new HttpClient();
            using var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);

            var response = await httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {response.StatusCode}");
            }
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();

            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<TResponse>(responseContent);
            if (result == null)
            {
                throw new Exception("Error deserializing response");
            }
            return result;
        }
        catch (Exception)
        {
            throw new Exception("Error during Post Request");
        }
    }
}
