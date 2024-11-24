using System.ComponentModel.DataAnnotations;

namespace llassist.Common.Models.Library;

public record DataField
{
    [Required]
    [StringLength(100)]
    public string Key { get; init; } = string.Empty;
    
    [Required]
    [StringLength(2000)]
    public string Value { get; init; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string DataType { get; init; } = "string";
    
    [Required]
    [StringLength(100)]
    public string Schema { get; init; } = string.Empty;
    
    public int Order { get; init; }
    
    public bool Required { get; init; }
    
    public string? ValidationPattern { get; init; }
} 