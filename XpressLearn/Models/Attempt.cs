namespace XpressLearn.Models;

public class Attempt
{
    public int AttemptId { get; set; }
    public int UserId { get; set; }
    public string? Username { get; set; }
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public decimal? Score { get; set; }
    public decimal MaxScore { get; set; } = 100;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Status { get; set; } = "InProgress";
    public string? Notes { get; set; }
}
