using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using XpressLearn.Models;

namespace XpressLearn.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IDbConnection _db;

    public CategoriesController(IDbConnection db)
    {
        _db = db;
    }

    /// <summary>
    /// Get all course categories.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _db.QueryAsync<Category>(
            "usp_GetAllCategories",
            commandType: CommandType.StoredProcedure);
        return Ok(categories);
    }
}
