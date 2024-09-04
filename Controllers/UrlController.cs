using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Models;
using UrlShortener.ViewModels;

[ApiController]
[Route("api/[controller]/[action]")]
public class UrlController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Url>>> GetAll()
    {
        var urls = await context.Urls.ToListAsync();
        return Ok(urls);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var url = await context.Urls.FindAsync(id);
        return url == null ? NotFound() : Ok(url);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UrlCreateDto urlDto)
    {
        if (string.IsNullOrEmpty(urlDto.OriginalUrl))
        {
            return BadRequest("Original URL is required.");
        }

        var shortCode = Guid.NewGuid().ToString()[..8];
        var url = new Url
        {
            OriginalUrl = urlDto.OriginalUrl,
            ShortenedUrl = shortCode,
            CreatedAt = DateTime.UtcNow
        };

        context.Urls.Add(url);
        await context.SaveChangesAsync();

        var fullShortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}";
        return CreatedAtAction(nameof(GetById), new { id = url.Id }, new { fullShortUrl });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var url = await context.Urls.FindAsync(id);

        if (url == null)
        {
            return NotFound();
        }

        context.Urls.Remove(url);
        await context.SaveChangesAsync();

        return NoContent();
    }
}
