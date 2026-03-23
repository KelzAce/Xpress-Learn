namespace XpressLearn.Domain.Entities;

public class LeaderboardEntry
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public decimal BestScore { get; set; }
}
