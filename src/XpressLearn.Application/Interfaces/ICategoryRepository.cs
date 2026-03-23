using XpressLearn.Domain.Entities;

namespace XpressLearn.Application.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
}
