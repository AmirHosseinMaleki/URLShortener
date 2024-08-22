using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;
using UrlShortener.Models;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UrlController : ControllerBase
{
    private readonly AppDbContext _context;

    public UrlController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Url>> CreateShortUrl([FromBody] Url url)
    {
        // Generate a short URL (for simplicity, this example uses a GUID)
        url.ShortenedUrl = Guid.NewGuid().ToString().Substring(0, 8);

        _context.Urls.Add(url);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUrl), new { id = url.Id }, url);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Url>> GetUrl(int id)
    {
        var url = await _context.Urls.FindAsync(id);

        if (url == null)
        {
            return NotFound();
        }

        return url;
    }

    [HttpGet("redirect/{shortUrl}")]
    public async Task<IActionResult> RedirectToOriginalUrl(string shortUrl)
    {
        var url = _context.Urls.FirstOrDefault(u => u.ShortenedUrl == shortUrl);

        if (url == null)
        {
            return NotFound();
        }

        return Redirect(url.OriginalUrl);
    }
}
