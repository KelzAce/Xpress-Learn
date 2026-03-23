using System.Data;
using Dapper;
using XpressLearn.Application.Interfaces;
using XpressLearn.Domain.Entities;

namespace XpressLearn.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _db;

    public UserRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _db.QueryAsync<User>(
            "usp_GetAllUsers",
            commandType: CommandType.StoredProcedure);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _db.QueryFirstOrDefaultAsync<User>(
            "usp_GetUserById",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<User>> GetInstructorsAsync()
    {
        return await _db.QueryAsync<User>(
            "usp_GetInstructors",
            commandType: CommandType.StoredProcedure);
    }
}
