using Microsoft.AspNetCore.Mvc;
using XpressLearn.Models;
using XpressLearn.Repositories;
using System.Data;
using Dapper;

namespace XpressLearn.Controllers;

public class CoursesController : Controller
{
    private static readonly HashSet<string> AllowedImageExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    private readonly ICourseRepository _courseRepository;
    private readonly IDbConnection _db;
    private readonly IWebHostEnvironment _env;

    public CoursesController(ICourseRepository courseRepository, IDbConnection db, IWebHostEnvironment env)
    {
        _courseRepository = courseRepository;
        _db = db;
        _env = env;
    }

    private async Task<(List<Category> categories, List<User> instructors)> GetFormDataAsync()
    {
        var categories = (await _db.QueryAsync<Category>("usp_GetAllCategories", commandType: CommandType.StoredProcedure)).ToList();
        var instructors = (await _db.QueryAsync<User>("usp_GetInstructors", commandType: CommandType.StoredProcedure)).ToList();
        return (categories, instructors);
    }

    private async Task<string?> SaveThumbnailAsync(IFormFile? thumbnail, string? existingUrl = null)
    {
        if (thumbnail == null || thumbnail.Length == 0) return existingUrl;

        var ext = Path.GetExtension(thumbnail.FileName);
        if (!AllowedImageExtensions.Contains(ext))
        {
            ModelState.AddModelError("Thumbnail", "Only image files (.jpg, .jpeg, .png, .gif, .webp) are allowed.");
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

    public async Task<IActionResult> Index()
    {
        var courses = await _courseRepository.GetAllCoursesAsync();
        return View(courses);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var (categories, instructors) = await GetFormDataAsync();
        var vm = new CourseCreateViewModel { Categories = categories, Instructors = instructors };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseCreateViewModel vm)
    {
        var thumbnailUrl = await SaveThumbnailAsync(vm.Thumbnail);
        if (!ModelState.IsValid)
        {
            var (categories, instructors) = await GetFormDataAsync();
            vm.Categories = categories;
            vm.Instructors = instructors;
            return View(vm);
        }

        var course = new Course
        {
            Title = vm.Title,
            Description = vm.Description,
            CategoryId = vm.CategoryId,
            InstructorId = vm.InstructorId,
            Price = vm.Price,
            Level = vm.Level,
            ThumbnailUrl = thumbnailUrl
        };

        await _courseRepository.CreateCourseAsync(course);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _courseRepository.GetCourseByIdAsync(id);
        if (course == null) return NotFound();

        var (categories, instructors) = await GetFormDataAsync();
        var vm = new CourseCreateViewModel
        {
            Title = course.Title,
            Description = course.Description,
            CategoryId = course.CategoryId,
            InstructorId = course.InstructorId,
            Price = course.Price,
            Level = course.Level,
            Categories = categories,
            Instructors = instructors
        };
        ViewBag.CourseId = id;
        ViewBag.ExistingThumbnail = course.ThumbnailUrl;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CourseCreateViewModel vm)
    {
        var existing = await _courseRepository.GetCourseByIdAsync(id);
        if (existing == null) return NotFound();

        var thumbnailUrl = await SaveThumbnailAsync(vm.Thumbnail, existing.ThumbnailUrl);
        if (!ModelState.IsValid)
        {
            var (categories, instructors) = await GetFormDataAsync();
            vm.Categories = categories;
            vm.Instructors = instructors;
            ViewBag.CourseId = id;
            return View(vm);
        }

        var course = new Course
        {
            CourseId = id,
            Title = vm.Title,
            Description = vm.Description,
            CategoryId = vm.CategoryId,
            InstructorId = vm.InstructorId,
            Price = vm.Price,
            Level = vm.Level,
            ThumbnailUrl = thumbnailUrl,
            IsPublished = existing.IsPublished
        };

        await _courseRepository.UpdateCourseAsync(course);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _courseRepository.DeleteCourseAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
