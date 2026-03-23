using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace XpressLearn.Models;

public class CourseCreateViewModel
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int InstructorId { get; set; }

    [Range(0, 9999.99)]
    public decimal Price { get; set; }

    [Required]
    public string Level { get; set; } = "Beginner";

    public IFormFile? Thumbnail { get; set; }

    public List<Category> Categories { get; set; } = new();
    public List<User> Instructors { get; set; } = new();
}
