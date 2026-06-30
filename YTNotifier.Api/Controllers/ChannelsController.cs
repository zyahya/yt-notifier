using Microsoft.AspNetCore.Authorization;

using YTNotifier.Api.Contracts.Channels;
using YTNotifier.Api.Contracts.Users;

namespace YTNotifier.Api.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class ChannelsController : ControllerBase
{
    private readonly IChannelsService _channelsService;
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    public ChannelsController(IChannelsService channelsService)
    {
        _channelsService = channelsService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll()
    {
        var result = await _channelsService.GetAllAsync(UserId);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost()]
    public async Task<IActionResult> Add([FromBody] AddChannelRequest request)
    {
        var result = await _channelsService.AddAsync(UserId, request.ChannelId);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }

    [HttpDelete()]
    public async Task<IActionResult> Delete([FromBody] string channelId)
    {
        var result = await _channelsService.DeleteAsync(UserId, channelId);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}
