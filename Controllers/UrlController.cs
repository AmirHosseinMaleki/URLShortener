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
    public async Task<IActionResult> CreateShortUrl([FromBody] UrlCreateDto urlDto)
    {
        if (string.IsNullOrEmpty(urlDto.OriginalUrl))
        {
            return BadRequest("Original URL is required.");
        }

        var shortCode = Guid.NewGuid().ToString().Substring(0, 8);

        var url = new Url
        {
            OriginalUrl = urlDto.OriginalUrl,
            ShortenedUrl = shortCode,
            CreatedAt = DateTime.UtcNow
        };

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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUrl(int id)
    {
        var url = await _context.Urls.FindAsync(id);

        if (url == null)
        {
            return NotFound();
        }

        _context.Urls.Remove(url);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Url>>> GetAllUrls()
    {
        var urls = await _context.Urls.ToListAsync();
        return Ok(urls);
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
        if (url == null)
        {
            return NotFound();
        }

        url.ViewCount++;
        await _context.SaveChangesAsync();

        return Redirect(url.OriginalUrl);
    }
}
