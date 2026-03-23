using System.ComponentModel.DataAnnotations;

namespace XpressLearn.DTOs;

public class CreateAttemptRequest
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Range(0, 100)]
    public decimal? Score { get; set; }

    [Range(0, 100)]
    public decimal MaxScore { get; set; } = 100;

    [Required]
    public string Status { get; set; } = "InProgress";

    [StringLength(1000)]
    public string? Notes { get; set; }
}
