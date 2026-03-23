using System.Data;
using Dapper;
using XpressLearn.Models;

namespace XpressLearn.Repositories;

public class AttemptRepository : IAttemptRepository
{
    private readonly IDbConnection _db;

    public AttemptRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Attempt>> GetAttemptsByCourseAsync(int courseId)
    {
        return await _db.QueryAsync<Attempt>(
            "usp_GetAttemptsByCourse",
            new { CourseId = courseId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Attempt>> GetAttemptsByUserAsync(int userId)
    {
        return await _db.QueryAsync<Attempt>(
            "usp_GetAttemptsByUser",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> CreateAttemptAsync(Attempt attempt)
    {
        return await _db.QueryFirstOrDefaultAsync<int>(
            "usp_CreateAttempt",
            new
            {
                attempt.UserId,
                attempt.CourseId,
                attempt.Score,
                attempt.MaxScore,
                attempt.Status,
                attempt.Notes
            },
            commandType: CommandType.StoredProcedure);
    }
}
