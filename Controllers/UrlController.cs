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
        url.ShortenedUrl = Guid.NewGuid().ToString()[..8];
        _context.Urls.Add(url);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUrl), new { id = url.Id }, url);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUrl(int id)
    {
        var url = await _context.Urls.FindAsync(id);
        return url == null ? NotFound() : Ok(url);
    }

    [HttpGet("redirect/{shortUrl}")]
    public async Task<IActionResult> RedirectToOriginalUrl(string shortUrl)
    {
        var url = await _context.Urls.FirstOrDefaultAsync(u => u.ShortenedUrl == shortUrl);
        return url == null ? NotFound() : Redirect(url.OriginalUrl);
    }
}
