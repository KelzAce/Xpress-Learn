using XpressLearn.Models;

namespace XpressLearn.Repositories;

public interface IAttemptRepository
{
    Task<IEnumerable<Attempt>> GetAttemptsByCourseAsync(int courseId);
    Task<IEnumerable<Attempt>> GetAttemptsByUserAsync(int userId);
    Task<int> CreateAttemptAsync(Attempt attempt);
}
