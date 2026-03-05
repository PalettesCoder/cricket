using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using HRCricket.Api.Models;
using HRCricket.Api.Data;

namespace HRCricket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey = "339e0aaa-5592-419f-af0b-790f753a25f0";

        public MatchesController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult> GetMatches()
        {
            try
            {
                var matches = await _context.Matches.OrderByDescending(m => m.MatchDate).ToListAsync();
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Database Error", details = ex.Message });
            }
        }

        [HttpGet("live")]
        public async Task<ActionResult> GetLiveMatches()
        {
            try
            {
                var matches = await _context.Matches.Where(m => m.IsLive).ToListAsync();
                return Ok(matches);
            }
            catch (Exception)
            {
                // Fallback to External API directly if DB is down
                try {
                    var client = _httpClientFactory.CreateClient();
                    var url = $"https://api.cricapi.com/v1/cricScore?apikey={_apiKey}";
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode) {
                        var json = await response.Content.ReadAsStringAsync();
                        var apiResponse = JsonSerializer.Deserialize<CricScoreResponse>(json);
                        if (apiResponse?.Data != null) return Ok(apiResponse.Data);
                    }
                } catch {}
                
                return Ok(new List<object>()); // Return empty instead of misleading mock data
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CricApiMatch>> GetMatchInfo(string id)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://api.cricapi.com/v1/match_info?apikey={_apiKey}&id={id}";
            
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return BadRequest("External API error");

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<CricApiMatchInfoResponse>(json);

            if (apiResponse?.Data != null)
            {
                return Ok(apiResponse.Data);
            }

            return NotFound();
        }
        [HttpGet("scores")]
        public async Task<ActionResult> GetCricScores()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var url = $"https://api.cricapi.com/v1/cricScore?apikey={_apiKey}";
                
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode) return BadRequest(new { error = "External API error" });

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<CricScoreResponse>(json);

                if (apiResponse?.Data != null)
                {
                    return Ok(apiResponse.Data);
                }

                return NotFound(new { message = "No score data found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
            }
        }
        [HttpGet("current")]
        public async Task<ActionResult> GetCurrentMatches()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var url = $"https://api.cricapi.com/v1/currentMatches?apikey={_apiKey}&offset=0";
                
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode) return BadRequest(new { error = "External API error" });

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<CricApiResponse>(json);

                if (apiResponse?.Data != null)
                {
                    return Ok(apiResponse.Data);
                }

                return NotFound(new { message = "No current matches found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
            }
        }
    }
}
