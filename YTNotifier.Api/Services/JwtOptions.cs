using System.ComponentModel.DataAnnotations;

namespace YTNotifier.Api.Services;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [MinLength(30)]
    public string SecretKey { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Range(300, 3600)]
    public int ExpiresIn { get; init; }
}
