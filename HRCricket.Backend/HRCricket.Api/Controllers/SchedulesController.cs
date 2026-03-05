using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using HRCricket.Api.Models;
using HRCricket.Api.Data;

namespace HRCricket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchedulesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey = "339e0aaa-5592-419f-af0b-790f753a25f0";

        public SchedulesController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetSchedules()
        {
            try {
                return await _context.Schedules.OrderBy(s => s.MatchTime).ToListAsync();
            } catch {
                return Ok(new List<Schedule>());
            }
        }

        [HttpGet("upcoming")]
        public async Task<ActionResult> GetUpcomingSchedules()
        {
            try 
            {
                var schedules = await _context.Schedules
                    .Where(s => s.MatchTime >= DateTime.UtcNow)
                    .OrderBy(s => s.MatchTime)
                    .Take(10)
                    .ToListAsync();
                
                if (schedules.Count > 0) return Ok(schedules);
                
                return await GetFallbackSchedules();
            }
            catch (Exception)
            {
                return await GetFallbackSchedules();
            }
        }

        private async Task<ActionResult> GetFallbackSchedules()
        {
            try 
            {
                var client = _httpClientFactory.CreateClient();
                // Use currentMatches as a proxy for upcoming/live schedules
                var url = $"https://api.cricapi.com/v1/currentMatches?apikey={_apiKey}&offset=0";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<CricApiResponse>(json);
                    if (apiResponse?.Data != null)
                    {
                        var dynamicSchedules = apiResponse.Data.Select(m => new {
                            MatchName = m.Name,
                            Series = m.MatchType?.ToUpper() ?? "International",
                            Venue = m.Venue ?? "TBD",
                            MatchTime = DateTime.TryParse(m.Date, out var dt) ? dt : DateTime.UtcNow.AddHours(2),
                            Category = m.Status ?? "Upcoming",
                            ApiId = m.Id
                        }).ToList();
                        return Ok(dynamicSchedules);
                    }
                }
            } catch {}
            return Ok(new List<object>());
        }

        [HttpGet("series/{id}")]
        public async Task<ActionResult<CricApiSeriesInfo>> GetSeriesInfo(string id)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://api.cricapi.com/v1/series_info?apikey={_apiKey}&id={id}";
            
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return BadRequest("External API error");

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<CricApiSeriesInfoResponse>(json);

            if (apiResponse?.Data != null)
            {
                return Ok(apiResponse.Data);
            }

            return NotFound();
        }
    }
}
