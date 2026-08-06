using System.ComponentModel.DataAnnotations;

namespace YouTubeNotifier.Api.Services;

public class SmtpOptions
{
    public static readonly string SectionName = "SmtpOptions";

    public string DisplayName { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }
}
