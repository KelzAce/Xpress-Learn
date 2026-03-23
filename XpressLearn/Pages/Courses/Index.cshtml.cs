using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using XpressLearn.Models;
using XpressLearn.Repositories;

namespace XpressLearn.Pages.Courses;

public class IndexModel : PageModel
{
    private readonly ICourseRepository _courseRepository;

    public IndexModel(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public IEnumerable<Course> Courses { get; set; } = Enumerable.Empty<Course>();

    public async Task OnGetAsync()
    {
        Courses = await _courseRepository.GetAllCoursesAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _courseRepository.DeleteCourseAsync(id);
        return RedirectToPage();
    }
}
