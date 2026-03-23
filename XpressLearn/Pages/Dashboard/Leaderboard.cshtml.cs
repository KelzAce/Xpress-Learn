using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using XpressLearn.Repositories;

namespace XpressLearn.Pages.Dashboard;

public class LeaderboardModel : PageModel
{
    private readonly ILeaderboardRepository _leaderboardRepository;

    public LeaderboardModel(ILeaderboardRepository leaderboardRepository)
    {
        _leaderboardRepository = leaderboardRepository;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnGetLeaderboardDataAsync()
    {
        var data = await _leaderboardRepository.GetLeaderboardAsync();
        return new JsonResult(data);
    }
}
