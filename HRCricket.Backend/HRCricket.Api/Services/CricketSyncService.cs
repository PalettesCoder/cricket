using HRCricket.Api.Data;
using HRCricket.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HRCricket.Api.Services
{
    public class CricketSyncService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey = "339e0aaa-5592-419f-af0b-790f753a25f0";
        private readonly string _apiUrl = "https://api.cricapi.com/v1/currentMatches";
        private readonly string _seriesUrl = "https://api.cricapi.com/v1/series";
        private readonly string _allMatchesUrl = "https://api.cricapi.com/v1/matches";
        private readonly string _playersUrl = "https://api.cricapi.com/v1/players";

        public CricketSyncService(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
        {
            _serviceProvider = serviceProvider;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SyncMatches();
                    await SyncSeries();
                    await SyncAllMatches();
                    await SyncPlayers();
                }
                catch (Exception)
                {
                    // Log error here
                }

                // Wait for 5 minutes before next match sync
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task SyncMatches()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync($"{_apiUrl}?apikey={_apiKey}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<CricApiResponse>(json);

                if (apiResponse?.Data != null)
                {
                    foreach (var apiMatch in apiResponse.Data)
                    {
                        var teamA = apiMatch.Teams?.Count > 0 ? apiMatch.Teams[0] : "TBD";
                        var teamB = apiMatch.Teams?.Count > 1 ? apiMatch.Teams[1] : "TBD";

                        var existingMatch = await dbContext.Matches
                            .FirstOrDefaultAsync(m => m.MatchType == apiMatch.MatchType && m.TeamA == teamA && m.TeamB == teamB);

                        if (existingMatch == null)
                        {
                            existingMatch = new Match
                            {
                                SeriesName = apiMatch.Name,
                                TeamA = apiMatch.Teams?.Count > 0 ? apiMatch.Teams[0] : "TBD",
                                TeamB = apiMatch.Teams?.Count > 1 ? apiMatch.Teams[1] : "TBD",
                                MatchType = apiMatch.MatchType,
                                MatchDate = DateTime.TryParse(apiMatch.Date, out var dt) ? dt : DateTime.UtcNow
                            };
                            dbContext.Matches.Add(existingMatch);
                        }

                        // Update scores and status
                        existingMatch.Status = apiMatch.Status;
                        existingMatch.IsLive = apiMatch.MatchStatus == "live";
                        
                        if (apiMatch.Score != null && apiMatch.Score.Count > 0)
                        {
                            existingMatch.ScoreA = $"{apiMatch.Score[0].Runs}/{apiMatch.Score[0].Wickets} ({apiMatch.Score[0].Overs})";
                            if (apiMatch.Score.Count > 1)
                            {
                                existingMatch.ScoreB = $"{apiMatch.Score[1].Runs}/{apiMatch.Score[1].Wickets} ({apiMatch.Score[1].Overs})";
                            }
                        }
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task SyncSeries()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync($"{_seriesUrl}?apikey={_apiKey}&offset=0");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<CricApiSeriesResponse>(json);

                if (apiResponse?.Data != null)
                {
                    foreach (var apiSeries in apiResponse.Data)
                    {
                        var matchDate = DateTime.TryParse(apiSeries.StartDate, out var dt) ? dt : DateTime.UtcNow;
                        
                        // Check if already in schedule or matches
                        var existingSchedule = await dbContext.Schedules
                            .FirstOrDefaultAsync(s => s.MatchName == apiSeries.Name);

                        if (existingSchedule == null)
                        {
                            dbContext.Schedules.Add(new Schedule
                            {
                                MatchName = apiSeries.Name,
                                Series = apiSeries.Name,
                                Venue = "TBD",
                                MatchTime = matchDate,
                                Category = "Series"
                            });
                        }
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task SyncAllMatches()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync($"{_allMatchesUrl}?apikey={_apiKey}&offset=0");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<CricApiResponse>(json);

                if (apiResponse?.Data != null)
                {
                    foreach (var apiMatch in apiResponse.Data)
                    {
                        var matchDate = DateTime.TryParse(apiMatch.Date, out var dt) ? dt : DateTime.UtcNow;

                        // Add to schedule if not exists
                        var existingSchedule = await dbContext.Schedules
                            .FirstOrDefaultAsync(s => s.MatchName == apiMatch.Name && s.MatchTime == matchDate);

                        if (existingSchedule == null)
                        {
                            dbContext.Schedules.Add(new Schedule
                            {
                                MatchName = apiMatch.Name,
                                Series = apiMatch.Name, // Or extract series if available
                                Venue = apiMatch.Venue ?? "TBD",
                                MatchTime = matchDate,
                                Category = apiMatch.MatchType ?? "International"
                            });
                        }

                        // Also update match results if match is over
                        if (apiMatch.Status == "Match Ended")
                        {
                            var existingMatch = await dbContext.Matches
                                .FirstOrDefaultAsync(m => m.SeriesName == apiMatch.Name && m.MatchDate.Date == matchDate.Date);

                            if (existingMatch == null)
                            {
                                dbContext.Matches.Add(new Match
                                {
                                    SeriesName = apiMatch.Name,
                                    TeamA = apiMatch.Teams?.Count > 0 ? apiMatch.Teams[0] : "TBD",
                                    TeamB = apiMatch.Teams?.Count > 1 ? apiMatch.Teams[1] : "TBD",
                                    Status = apiMatch.Status,
                                    MatchType = apiMatch.MatchType,
                                    IsLive = false,
                                    MatchDate = matchDate
                                });
                            }
                        }
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task SyncPlayers()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync($"{_playersUrl}?apikey={_apiKey}&offset=0");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<CricApiPlayerResponse>(json);

                if (apiResponse?.Data != null)
                {
                    foreach (var apiPlayer in apiResponse.Data)
                    {
                        var existingPlayer = await dbContext.Players
                            .FirstOrDefaultAsync(p => p.Name == apiPlayer.Name);

                        if (existingPlayer == null)
                        {
                            dbContext.Players.Add(new Player
                            {
                                Name = apiPlayer.Name ?? "Unknown",
                                Team = apiPlayer.Country ?? "TBD",
                                Role = "Player",
                                Matches = 0,
                                Innings = 0,
                                TotalRuns = 0,
                                Average = 0,
                                StrikeRate = 0,
                                Hundreds = 0,
                                Fifties = 0,
                                HighScore = 0
                            });
                        }
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
