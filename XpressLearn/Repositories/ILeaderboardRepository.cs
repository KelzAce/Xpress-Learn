using XpressLearn.Models;

namespace XpressLearn.Repositories;

public interface ILeaderboardRepository
{
    Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync();
}
