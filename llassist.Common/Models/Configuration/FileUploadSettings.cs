namespace llassist.Common.Models.Configuration;

public class FileUploadSettings
{
    public int MaxSizeMB { get; set; }
    public string[] AllowedExtensions { get; set; } = [];
    public int MaxSizeBytes => MaxSizeMB * 1024 * 1024;
}
