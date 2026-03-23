using Microsoft.AspNetCore.Mvc;
using XpressLearn.DTOs;
using XpressLearn.Models;
using XpressLearn.Repositories;

namespace XpressLearn.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttemptsController : ControllerBase
{
    private readonly IAttemptRepository _attemptRepository;

    public AttemptsController(IAttemptRepository attemptRepository)
    {
        _attemptRepository = attemptRepository;
    }

    /// <summary>
    /// Get all attempts for a specific course.
    /// </summary>
    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetByCourse(int courseId)
    {
        var attempts = await _attemptRepository.GetAttemptsByCourseAsync(courseId);
        return Ok(attempts);
    }

    /// <summary>
    /// Get all attempts for a specific user.
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var attempts = await _attemptRepository.GetAttemptsByUserAsync(userId);
        return Ok(attempts);
    }

    /// <summary>
    /// Create a new attempt.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAttemptRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var attempt = new Attempt
        {
            UserId = request.UserId,
            CourseId = request.CourseId,
            Score = request.Score,
            MaxScore = request.MaxScore,
            Status = request.Status,
            Notes = request.Notes
        };

        var id = await _attemptRepository.CreateAttemptAsync(attempt);
        attempt.AttemptId = id;
        return Created($"/api/attempts/{id}", attempt);
    }
}
