using llassist.ApiService.Services;
using llassist.Common.ViewModels;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace llassist.Tests;

public class FileUploadSettingsServiceTests
{
    private readonly Mock<IAppSettingService> _appSettingServiceMock;
    private readonly FileUploadSettingsService _fileUploadSettingsService;

    public FileUploadSettingsServiceTests()
    {
        _appSettingServiceMock = new Mock<IAppSettingService>();
        _fileUploadSettingsService = new FileUploadSettingsService(_appSettingServiceMock.Object);
    }

    [Fact]
    public async Task GetFileUploadSettingsAsync_WhenSettingsExist_ReturnsCorrectSettings()
    {
        // Arrange
        _appSettingServiceMock.Setup(x => x.GetSettingAsync("FileUpload:MaxSizeMB"))
            .ReturnsAsync(new AppSettingViewModel { Value = "5" });
        _appSettingServiceMock.Setup(x => x.GetSettingAsync("FileUpload:AllowedExtensions"))
            .ReturnsAsync(new AppSettingViewModel { Value = ".csv,.txt,.xlsx" });

        // Act
        var result = await _fileUploadSettingsService.GetFileUploadSettingsAsync();

        // Assert
        Assert.Equal(5, result.MaxSizeMB);
        Assert.Equal(new[] { ".csv", ".txt", ".xlsx" }, result.AllowedExtensions);
    }

    [Fact]
    public async Task GetFileUploadSettingsAsync_WhenSettingsDoNotExist_ReturnsDefaultValues()
    {
        // Arrange
        _appSettingServiceMock.Setup(x => x.GetSettingAsync("FileUpload:MaxSizeMB"))
            .ReturnsAsync((AppSettingViewModel)null);
        _appSettingServiceMock.Setup(x => x.GetSettingAsync("FileUpload:AllowedExtensions"))
            .ReturnsAsync((AppSettingViewModel)null);

        // Act
        var result = await _fileUploadSettingsService.GetFileUploadSettingsAsync();

        // Assert
        Assert.Equal(1, result.MaxSizeMB);
        Assert.Equal(new[] { ".csv" }, result.AllowedExtensions);
    }

    [Fact]
    public async Task GetFileUploadSettingsAsync_WhenOnlyMaxSizeExists_ReturnsCustomSizeWithDefaultExtensions()
    {
        // Arrange
        _appSettingServiceMock.Setup(x => x.GetSettingAsync("FileUpload:MaxSizeMB"))
            .ReturnsAsync(new AppSettingViewModel { Value = "10" });
        _appSettingServiceMock.Setup(x => x.GetSettingAsync("FileUpload:AllowedExtensions"))
            .ReturnsAsync((AppSettingViewModel)null);

        // Act
        var result = await _fileUploadSettingsService.GetFileUploadSettingsAsync();

        // Assert
        Assert.Equal(10, result.MaxSizeMB);
        Assert.Equal(new[] { ".csv" }, result.AllowedExtensions);
    }

    [Fact]
    public async Task GetFileUploadSettingsAsync_WhenOnlyExtensionsExist_ReturnsDefaultSizeWithCustomExtensions()
    {
        // Arrange
        _appSettingServiceMock.Setup(x => x.GetSettingAsync("FileUpload:MaxSizeMB"))
            .ReturnsAsync((AppSettingViewModel)null);
        _appSettingServiceMock.Setup(x => x.GetSettingAsync("FileUpload:AllowedExtensions"))
            .ReturnsAsync(new AppSettingViewModel { Value = ".pdf,.doc" });

        // Act
        var result = await _fileUploadSettingsService.GetFileUploadSettingsAsync();

        // Assert
        Assert.Equal(1, result.MaxSizeMB);
        Assert.Equal(new[] { ".pdf", ".doc" }, result.AllowedExtensions);
    }
}