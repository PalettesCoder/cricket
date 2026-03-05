using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using HRCricket.Api.Models;

namespace HRCricket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey = "339e0aaa-5592-419f-af0b-790f753a25f0";

        public PlayersController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            return await _context.Players.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetPlayer(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();
            return player;
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Player>>> SearchPlayers(string name)
        {
            return await _context.Players
                .Where(p => p.Name.Contains(name))
                .ToListAsync();
        }

        [HttpGet("search-api")]
        public async Task<ActionResult<IEnumerable<CricApiPlayer>>> SearchApiPlayers(string name)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://api.cricapi.com/v1/players?apikey={_apiKey}&offset=0&search={name}";
            
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return BadRequest("External API error");

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<CricApiPlayerResponse>(json);

            if (apiResponse?.Data != null)
            {
                // Auto-import new players into local DB
                foreach (var p in apiResponse.Data)
                {
                    if (!await _context.Players.AnyAsync(lp => lp.Name == p.Name))
                    {
                        _context.Players.Add(new Player { 
                            Name = p.Name ?? "Unknown", 
                            Team = p.Country ?? "TBD",
                            Role = "Player"
                        });
                    }
                }
                await _context.SaveChangesAsync();
                return Ok(apiResponse.Data);
            }

            return Ok(new List<CricApiPlayer>());
        }
    }
}
