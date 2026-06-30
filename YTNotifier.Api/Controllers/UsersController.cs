using Microsoft.AspNetCore.Authorization;

using YTNotifier.Api.Contracts.Users;

namespace YTNotifier.Api.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("update-delivery-time")]
    public async Task<IActionResult> UpdateDeliveryTime([FromBody] UpdateDeliveryTimeRequest request)
    {
        var result = await _userService
            .SetDeliveryTimeAsync(UserId, request.PreferredDeliveryDay, request.PreferredDeliveryHour);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var result = await _userService
            .ChangePasswordAsync(UserId, request);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpPost("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var result = await _userService
            .UpdateProfileAsync(UserId, request);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpGet("get-profile-info")]
    public async Task<IActionResult> GetProfileInfo()
    {
        var result = await _userService
            .GetProfileInfoAsync(UserId);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }
}
