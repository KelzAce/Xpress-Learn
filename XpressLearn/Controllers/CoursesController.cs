using Microsoft.AspNetCore.Mvc;
using XpressLearn.Models;
using XpressLearn.Repositories;
using System.Data;
using Dapper;

namespace XpressLearn.Controllers;

public class CoursesController : Controller
{
    private readonly ICourseRepository _courseRepository;
    private readonly IDbConnection _db;
    private readonly IWebHostEnvironment _env;

    public CoursesController(ICourseRepository courseRepository, IDbConnection db, IWebHostEnvironment env)
    {
        _courseRepository = courseRepository;
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var courses = await _courseRepository.GetAllCoursesAsync();
        return View(courses);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new CourseCreateViewModel
        {
            Categories = (await _db.QueryAsync<Category>("usp_GetAllCategories", commandType: CommandType.StoredProcedure)).ToList(),
            Instructors = (await _db.QueryAsync<User>("SELECT UserId, Username, FirstName, LastName FROM Users WHERE Role = 'Instructor'")).ToList()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Categories = (await _db.QueryAsync<Category>("usp_GetAllCategories", commandType: CommandType.StoredProcedure)).ToList();
            vm.Instructors = (await _db.QueryAsync<User>("SELECT UserId, Username, FirstName, LastName FROM Users WHERE Role = 'Instructor'")).ToList();
            return View(vm);
        }

        string? thumbnailUrl = null;
        if (vm.Thumbnail != null && vm.Thumbnail.Length > 0)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.Thumbnail.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await vm.Thumbnail.CopyToAsync(stream);
            thumbnailUrl = "/uploads/" + fileName;
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

        var vm = new CourseCreateViewModel
        {
            Title = course.Title,
            Description = course.Description,
            CategoryId = course.CategoryId,
            InstructorId = course.InstructorId,
            Price = course.Price,
            Level = course.Level,
            Categories = (await _db.QueryAsync<Category>("usp_GetAllCategories", commandType: CommandType.StoredProcedure)).ToList(),
            Instructors = (await _db.QueryAsync<User>("SELECT UserId, Username, FirstName, LastName FROM Users WHERE Role = 'Instructor'")).ToList()
        };
        ViewBag.CourseId = id;
        ViewBag.ExistingThumbnail = course.ThumbnailUrl;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CourseCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Categories = (await _db.QueryAsync<Category>("usp_GetAllCategories", commandType: CommandType.StoredProcedure)).ToList();
            vm.Instructors = (await _db.QueryAsync<User>("SELECT UserId, Username, FirstName, LastName FROM Users WHERE Role = 'Instructor'")).ToList();
            ViewBag.CourseId = id;
            return View(vm);
        }

        var existing = await _courseRepository.GetCourseByIdAsync(id);
        if (existing == null) return NotFound();

        string? thumbnailUrl = existing.ThumbnailUrl;
        if (vm.Thumbnail != null && vm.Thumbnail.Length > 0)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.Thumbnail.FileName);
            var filePath = Path.Combine(uploadsDir, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await vm.Thumbnail.CopyToAsync(stream);
            thumbnailUrl = "/uploads/" + fileName;
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
