using System.Net;
using llassist.Common.Models.Configuration;

namespace llassist.Web;

public class FileUploadSettingsApiClient
{
    private readonly HttpClient _httpClient;

    public FileUploadSettingsApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<FileUploadSettings?> GetFileUploadSettingsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/FileUploadSettings");
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
                
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FileUploadSettings>();
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
}