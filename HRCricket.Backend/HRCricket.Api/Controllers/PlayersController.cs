using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using HRCricket.Api.Models;
using HRCricket.Api.Data;

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
        public async Task<ActionResult> GetPlayers()
        {
            try
            {
                var players = await _context.Players.ToListAsync();
                return Ok(players);
            }
            catch (Exception)
            {
                return Ok(new List<object>()); 
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetPlayer(int id)
        {
            try
            {
                var player = await _context.Players.FindAsync(id);
                if (player == null) return NotFound(new { message = "Player not found" });
                return Ok(player);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Database Error", details = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult> SearchPlayers(string name)
        {
            try
            {
                var players = await _context.Players
                    .Where(p => p.Name.Contains(name))
                    .ToListAsync();
                return Ok(players);
            }
            catch (Exception)
            {
                return Ok(new List<object>());
            }
        }

        [HttpGet("search-api")]
        public async Task<ActionResult> SearchApiPlayers(string name)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var url = $"https://api.cricapi.com/v1/players?apikey={_apiKey}&offset=0&search={name}";
                
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode) return BadRequest(new { error = "External API error" });

                var json = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<CricApiPlayerResponse>(json);

                if (apiResponse?.Data != null)
                {
                    // Auto-import new players into local DB (wrapped in its own try-catch or handled carefully)
                    try 
                    {
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
                    }
                    catch (Exception dbEx)
                    {
                        // Log DB error but still return API data
                        Console.WriteLine($"Failed to auto-import players: {dbEx.Message}");
                    }
                    
                    return Ok(apiResponse.Data);
                }

                return Ok(new List<CricApiPlayer>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
            }
        }
    }
}
