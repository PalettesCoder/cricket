using HRCricket.Api.Data;
using HRCricket.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRCricket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BlogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> GetBlogs()
        {
            return await _context.Blogs.OrderByDescending(b => b.CreatedAt).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> GetBlog(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null) return NotFound();
            return blog;
        }

        [HttpPost]
        public async Task<ActionResult<Blog>> CreateBlog(Blog blog)
        {
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBlog), new { id = blog.Id }, blog);
        }
    }
}
