using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;
using UrlShortener.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UrlController : ControllerBase
{
    private readonly AppDbContext _context;

    public UrlController(AppDbContext context) => _context = context;

    [HttpPost]
    public async Task<IActionResult> CreateShortUrl([FromBody] Url url)
    {
        if (string.IsNullOrEmpty(url.OriginalUrl))
        {
            return BadRequest("Original URL is required.");
        }

        // Generate a short code (e.g., "1a0566ee")
        var shortCode = Guid.NewGuid().ToString().Substring(0, 8);

        // Set the properties on the Url model
        url.ShortenedUrl = shortCode;
        url.CreatedAt = DateTime.UtcNow;

        _context.Urls.Add(url);
        await _context.SaveChangesAsync();

        var fullShortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}";
        return CreatedAtAction(nameof(GetUrl), new { id = url.Id }, new { fullShortUrl });
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUrl(int id)
    {
        var url = await _context.Urls.FindAsync(id);
        return url == null ? NotFound() : Ok(url);
    }
}

// New Controller to handle root-level shortened URLs
[ApiController]
public class RedirectController : ControllerBase
{
    private readonly AppDbContext _context;

    public RedirectController(AppDbContext context) => _context = context;

    [HttpGet("{shortUrl}")]
    public async Task<IActionResult> RedirectToOriginalUrl(string shortUrl)
    {
        var url = await _context.Urls.FirstOrDefaultAsync(u => u.ShortenedUrl == shortUrl);
        return url == null ? NotFound() : Redirect(url.OriginalUrl);
    }
}
