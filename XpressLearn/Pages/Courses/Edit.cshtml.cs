using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using XpressLearn.Models;
using XpressLearn.Repositories;
using System.Data;
using Dapper;

namespace XpressLearn.Pages.Courses;

public class EditModel : PageModel
{
    private static readonly HashSet<string> AllowedImageExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    private readonly ICourseRepository _courseRepository;
    private readonly IDbConnection _db;
    private readonly IWebHostEnvironment _env;

    public EditModel(ICourseRepository courseRepository, IDbConnection db, IWebHostEnvironment env)
    {
        _courseRepository = courseRepository;
        _db = db;
        _env = env;
    }

    [BindProperty]
    public CourseCreateViewModel Input { get; set; } = new();

    public List<Category> Categories { get; set; } = new();
    public List<User> Instructors { get; set; } = new();
    public string? ExistingThumbnail { get; set; }

    private async Task LoadFormDataAsync()
    {
        Categories = (await _db.QueryAsync<Category>("usp_GetAllCategories", commandType: CommandType.StoredProcedure)).ToList();
        Instructors = (await _db.QueryAsync<User>("usp_GetInstructors", commandType: CommandType.StoredProcedure)).ToList();
    }

    private async Task<string?> SaveThumbnailAsync(IFormFile? thumbnail, string? existingUrl)
    {
        if (thumbnail == null || thumbnail.Length == 0) return existingUrl;

        var ext = Path.GetExtension(thumbnail.FileName);
        if (!AllowedImageExtensions.Contains(ext))
        {
            ModelState.AddModelError("Input.Thumbnail", "Only image files (.jpg, .jpeg, .png, .gif, .webp) are allowed.");
            return existingUrl;
        }

        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsDir))
            Directory.CreateDirectory(uploadsDir);

        var fileName = Guid.NewGuid().ToString() + ext;
        var filePath = Path.Combine(uploadsDir, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await thumbnail.CopyToAsync(stream);
        return "/uploads/" + fileName;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var course = await _courseRepository.GetCourseByIdAsync(id);
        if (course == null) return NotFound();

        await LoadFormDataAsync();

        Input = new CourseCreateViewModel
        {
            Title = course.Title,
            Description = course.Description,
            CategoryId = course.CategoryId,
            InstructorId = course.InstructorId,
            Price = course.Price,
            Level = course.Level
        };
        ExistingThumbnail = course.ThumbnailUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var existing = await _courseRepository.GetCourseByIdAsync(id);
        if (existing == null) return NotFound();

        var thumbnailUrl = await SaveThumbnailAsync(Input.Thumbnail, existing.ThumbnailUrl);
        if (!ModelState.IsValid)
        {
            await LoadFormDataAsync();
            ExistingThumbnail = existing.ThumbnailUrl;
            return Page();
        }

        var course = new Course
        {
            CourseId = id,
            Title = Input.Title,
            Description = Input.Description,
            CategoryId = Input.CategoryId,
            InstructorId = Input.InstructorId,
            Price = Input.Price,
            Level = Input.Level,
            ThumbnailUrl = thumbnailUrl,
            IsPublished = existing.IsPublished
        };

        await _courseRepository.UpdateCourseAsync(course);
        return RedirectToPage("Index");
    }
}
