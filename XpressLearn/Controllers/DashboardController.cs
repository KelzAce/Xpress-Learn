using Microsoft.AspNetCore.Mvc;
using XpressLearn.Repositories;

namespace XpressLearn.Controllers;

public class DashboardController : Controller
{
    private readonly ILeaderboardRepository _leaderboardRepository;

    public DashboardController(ILeaderboardRepository leaderboardRepository)
    {
        _leaderboardRepository = leaderboardRepository;
    }

    public IActionResult Leaderboard()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetLeaderboardData()
    {
        var data = await _leaderboardRepository.GetLeaderboardAsync();
        return Json(data);
    }
}
