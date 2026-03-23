using XpressLearn.Domain.Entities;

namespace XpressLearn.Application.Interfaces;

public interface IAttemptRepository
{
    Task<IEnumerable<Attempt>> GetAttemptsByCourseAsync(int courseId);
    Task<IEnumerable<Attempt>> GetAttemptsByUserAsync(int userId);
    Task<int> CreateAttemptAsync(Attempt attempt);
}
