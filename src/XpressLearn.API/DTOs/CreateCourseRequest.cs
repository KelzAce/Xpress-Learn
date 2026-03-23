using System.ComponentModel.DataAnnotations;

namespace XpressLearn.API.DTOs;

public class CreateCourseRequest
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
}
