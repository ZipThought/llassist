using llassist.Common.Models.Configuration;

namespace llassist.ApiService.Services;

public interface IFileUploadSettingsService
{
    Task<FileUploadSettings> GetFileUploadSettingsAsync();
}

public class FileUploadSettingsService : IFileUploadSettingsService
{
    private const string FileUploadSettingsKey = "FileUpload";
    private const string MaxFileSizeKey = $"{FileUploadSettingsKey}:MaxSizeMB";
    private const string AllowedExtensionsKey = $"{FileUploadSettingsKey}:AllowedExtensions";
    private const int DefaultMaxFileSizeMB = 1;
    private static readonly string[] DefaultAllowedExtensions = [".csv"];

    private readonly IAppSettingService _appSettingService;

    public FileUploadSettingsService(IAppSettingService appSettingService)
    {
        _appSettingService = appSettingService;
    }

    public async Task<FileUploadSettings> GetFileUploadSettingsAsync()
    {
        var maxFileSizeMBSetting = await _appSettingService.GetSettingAsync(MaxFileSizeKey);
        var allowedExtensionsSetting = await _appSettingService.GetSettingAsync(AllowedExtensionsKey);

        return new FileUploadSettings
        {
            MaxSizeMB = maxFileSizeMBSetting != null 
                ? int.Parse(maxFileSizeMBSetting.Value)
                : DefaultMaxFileSizeMB,

            AllowedExtensions = allowedExtensionsSetting?.Value?.Split(',') ?? DefaultAllowedExtensions
        };
    }
}