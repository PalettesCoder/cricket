using HRCricket.Api.Data;
using HRCricket.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HRCricket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey = "339e0aaa-5592-419f-af0b-790f753a25f0";

        public BlogsController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult> GetBlogs()
        {
            try
            {
                var blogs = await _context.Blogs.OrderByDescending(b => b.CreatedAt).ToListAsync();
                if (blogs.Count > 0) return Ok(blogs);
                
                // Fallback to Live News if DB is empty
                return await GetFallbackNews();
            }
            catch (Exception)
            {
                return await GetFallbackNews();
            }
        }

        private async Task<ActionResult> GetFallbackNews()
        {
            try 
            {
                var client = _httpClientFactory.CreateClient();
                var url = $"https://api.cricapi.com/v1/news?apikey={_apiKey}";
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<CricApiNewsResponse>(json);
                    if (apiResponse?.Data != null)
                    {
                        var news = apiResponse.Data.Select(n => new {
                            Id = n.Id,
                            Title = n.Title,
                            Category = "LIVE NEWS",
                            Excerpt = n.Story,
                            Author = "CricAPI",
                            ImageUrl = n.Image ?? "https://images.unsplash.com/photo-1540747913346-19e32dc3e97e?auto=format&fit=crop&q=80&w=800",
                            ReadingTime = 5,
                            CreatedAt = DateTime.TryParse(n.PubTime, out var dt) ? dt : DateTime.UtcNow
                        }).ToList();
                        return Ok(news);
                    }
                }
            } catch {}

            return Ok(new List<object>());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> GetBlog(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null) return NotFound();
            return blog;
        }
    }
}
