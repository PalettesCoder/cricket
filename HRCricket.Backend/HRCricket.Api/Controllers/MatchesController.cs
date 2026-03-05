using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using HRCricket.Api.Models;

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
        public async Task<ActionResult<IEnumerable<Match>>> GetMatches()
        {
            return await _context.Matches.OrderByDescending(m => m.MatchDate).ToListAsync();
        }

        [HttpGet("live")]
        public async Task<ActionResult<IEnumerable<Match>>> GetLiveMatches()
        {
            return await _context.Matches.Where(m => m.IsLive).ToListAsync();
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
    }
}
