using System.ComponentModel.DataAnnotations;

namespace YTNotifier.Api.Services;

public class YouTubeOptions
{
    public const string SectionName = "YouTube";

    [Required]
    public string ApiKey { get; init; } = string.Empty;

    [MinLength(3)]
    public string ApplicationName { get; init; } = string.Empty;
}
