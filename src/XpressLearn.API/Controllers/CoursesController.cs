using Microsoft.AspNetCore.Mvc;
using XpressLearn.API.DTOs;
using XpressLearn.Application.Interfaces;
using XpressLearn.Domain.Entities;

namespace XpressLearn.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private static readonly HashSet<string> AllowedImageExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    private readonly ICourseRepository _courseRepository;
    private readonly IWebHostEnvironment _env;

    public CoursesController(ICourseRepository courseRepository, IWebHostEnvironment env)
    {
        _courseRepository = courseRepository;
        _env = env;
    }

    private async Task<string?> SaveThumbnailAsync(IFormFile? thumbnail)
    {
        if (thumbnail == null || thumbnail.Length == 0) return null;

        var ext = Path.GetExtension(thumbnail.FileName);
        if (!AllowedImageExtensions.Contains(ext))
            return null;

        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsDir))
            Directory.CreateDirectory(uploadsDir);

        var fileName = Guid.NewGuid().ToString() + ext;
        var filePath = Path.Combine(uploadsDir, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await thumbnail.CopyToAsync(stream);
        return "/uploads/" + fileName;
    }

    /// <summary>
    /// Get all courses.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var courses = await _courseRepository.GetAllCoursesAsync();
        return Ok(courses);
    }

    /// <summary>
    /// Get a course by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var course = await _courseRepository.GetCourseByIdAsync(id);
        if (course == null) return NotFound();
        return Ok(course);
    }

    /// <summary>
    /// Create a new course with optional thumbnail upload.
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Create([FromForm] CreateCourseRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var thumbnailUrl = await SaveThumbnailAsync(request.Thumbnail);

        var course = new Course
        {
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            InstructorId = request.InstructorId,
            Price = request.Price,
            Level = request.Level,
            ThumbnailUrl = thumbnailUrl
        };

        var id = await _courseRepository.CreateCourseAsync(course);
        course.CourseId = id;
        return CreatedAtAction(nameof(GetById), new { id }, course);
    }

    /// <summary>
    /// Update an existing course with optional thumbnail upload.
    /// </summary>
    [HttpPut("{id}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Update(int id, [FromForm] UpdateCourseRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existing = await _courseRepository.GetCourseByIdAsync(id);
        if (existing == null) return NotFound();

        var thumbnailUrl = await SaveThumbnailAsync(request.Thumbnail) ?? existing.ThumbnailUrl;

        var course = new Course
        {
            CourseId = id,
            Title = request.Title,
            Description = request.Description,
            CategoryId = request.CategoryId,
            InstructorId = request.InstructorId,
            Price = request.Price,
            Level = request.Level,
            ThumbnailUrl = thumbnailUrl,
            IsPublished = request.IsPublished
        };

        await _courseRepository.UpdateCourseAsync(course);
        return NoContent();
    }

    /// <summary>
    /// Delete a course by ID.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _courseRepository.GetCourseByIdAsync(id);
        if (existing == null) return NotFound();

        await _courseRepository.DeleteCourseAsync(id);
        return NoContent();
    }
}
