using XpressLearn.Domain.Entities;

namespace XpressLearn.Application.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int userId);
    Task<IEnumerable<User>> GetInstructorsAsync();
}
