using YTNotifier.Api.Abstractions;
using YTNotifier.Api.Contracts.Authentication;

namespace YTNotifier.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.GetTokenAsync(request.Email, request.Password);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }
}
