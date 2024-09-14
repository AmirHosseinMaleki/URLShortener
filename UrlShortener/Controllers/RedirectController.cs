using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;

// New Controller to handle root-level shortened URLs
[ApiController]
public class RedirectController(AppDbContext context) : ControllerBase
{
    [HttpGet("{shortUrl}")]
    public async Task<IActionResult> RedirectToOriginalUrl(string shortUrl)
    {
        var url = await context.Urls.FirstOrDefaultAsync(u => u.ShortenedUrl == shortUrl);
        if (url == null)
        {
            return NotFound();
        }

        url.ViewCount++;

        await context.SaveChangesAsync();

        return Redirect(url.OriginalUrl);
    }
}
