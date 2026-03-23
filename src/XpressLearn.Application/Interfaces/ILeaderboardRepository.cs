using XpressLearn.Domain.Entities;

namespace XpressLearn.Application.Interfaces;

public interface ILeaderboardRepository
{
    Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync();
}
