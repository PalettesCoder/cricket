using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using HRCricket.Api.Models;

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
            return await _context.Schedules
                .OrderBy(s => s.MatchTime)
                .ToListAsync();
        }

        [HttpGet("upcoming")]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetUpcomingSchedules()
        {
            return await _context.Schedules
                .Where(s => s.MatchTime >= DateTime.UtcNow)
                .OrderBy(s => s.MatchTime)
                .Take(10)
                .ToListAsync();
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
