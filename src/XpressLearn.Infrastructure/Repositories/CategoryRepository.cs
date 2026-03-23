using System.Data;
using Dapper;
using XpressLearn.Application.Interfaces;
using XpressLearn.Domain.Entities;

namespace XpressLearn.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly IDbConnection _db;

    public CategoryRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _db.QueryAsync<Category>(
            "usp_GetAllCategories",
            commandType: CommandType.StoredProcedure);
    }
}
