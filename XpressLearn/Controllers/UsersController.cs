using System.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using XpressLearn.Models;

namespace XpressLearn.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IDbConnection _db;

    public UsersController(IDbConnection db)
    {
        _db = db;
    }

    /// <summary>
    /// Get all users.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _db.QueryAsync<User>(
            "usp_GetAllUsers",
            commandType: CommandType.StoredProcedure);
        return Ok(users);
    }

    /// <summary>
    /// Get a user by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _db.QueryFirstOrDefaultAsync<User>(
            "usp_GetUserById",
            new { UserId = id },
            commandType: CommandType.StoredProcedure);
        if (user == null) return NotFound();
        return Ok(user);
    }

    /// <summary>
    /// Get all instructors.
    /// </summary>
    [HttpGet("instructors")]
    public async Task<IActionResult> GetInstructors()
    {
        var instructors = await _db.QueryAsync<User>(
            "usp_GetInstructors",
            commandType: CommandType.StoredProcedure);
        return Ok(instructors);
    }
}
