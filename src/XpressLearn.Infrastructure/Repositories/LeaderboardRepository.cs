using System.Data;
using Dapper;
using XpressLearn.Application.Interfaces;
using XpressLearn.Domain.Entities;

namespace XpressLearn.Infrastructure.Repositories;

public class LeaderboardRepository : ILeaderboardRepository
{
    private readonly IDbConnection _db;

    public LeaderboardRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync()
    {
        return await _db.QueryAsync<LeaderboardEntry>(
            "usp_GetLeaderboard",
            commandType: CommandType.StoredProcedure);
    }
}
