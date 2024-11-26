using llassist.ApiService.Services;
using llassist.Common.Models.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace llassist.ApiService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadSettingsController : ControllerBase
{
    private readonly IFileUploadSettingsService _fileUploadSettingsService;

    public FileUploadSettingsController(IFileUploadSettingsService fileUploadSettingsService)
    {
        _fileUploadSettingsService = fileUploadSettingsService;
    }

    [HttpGet]
    public async Task<ActionResult<FileUploadSettings>> Get()
    {
        var settings = await _fileUploadSettingsService.GetFileUploadSettingsAsync();
        if (settings == null) return NotFound();
        return Ok(settings);
    }
}
