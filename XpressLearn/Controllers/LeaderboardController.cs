using Microsoft.AspNetCore.Mvc;
using XpressLearn.Repositories;

namespace XpressLearn.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardRepository _leaderboardRepository;

    public LeaderboardController(ILeaderboardRepository leaderboardRepository)
    {
        _leaderboardRepository = leaderboardRepository;
    }

    /// <summary>
    /// Get the leaderboard showing top scorer per course category.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetLeaderboard()
    {
        var data = await _leaderboardRepository.GetLeaderboardAsync();
        return Ok(data);
    }
}
