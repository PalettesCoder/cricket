using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRCricket.Api.Models
{
    public class Blog
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Excerpt { get; set; }
        public string? Content { get; set; }
        public string? Author { get; set; }
        public string? ImageUrl { get; set; }
        public int ReadingTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Player
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Team { get; set; }
        public string? Role { get; set; }
        public string? ImageUrl { get; set; }
        public int Matches { get; set; }
        public int Innings { get; set; }
        public int TotalRuns { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Average { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal StrikeRate { get; set; }
        public int Hundreds { get; set; }
        public int Fifties { get; set; }
        public int HighScore { get; set; }
    }

    public class Match
    {
        public int Id { get; set; }
        public string? SeriesName { get; set; }
        public string? TeamA { get; set; }
        public string? TeamB { get; set; }
        public string? ScoreA { get; set; }
        public string? ScoreB { get; set; }
        public string? Status { get; set; }
        public string? MatchType { get; set; }
        public bool IsLive { get; set; }
        public DateTime MatchDate { get; set; }
    }

    public class Schedule
    {
        public int Id { get; set; }
        public string? MatchName { get; set; }
        public string? Series { get; set; }
        public string? Venue { get; set; }
        public DateTime MatchTime { get; set; }
        public string? Category { get; set; }
    }
}
