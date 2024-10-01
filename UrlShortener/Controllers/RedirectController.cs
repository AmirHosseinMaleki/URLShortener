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
        var originalUrl = await context.Urls
                                    .Where(u => u.ShortenedUrl == shortUrl)
                                    .Select(p => p.OriginalUrl)
                                    .SingleOrDefaultAsync();
        if (originalUrl == null)
        {
            return NotFound();
        }

        // url.ViewCount++;

        // put count as of of the column of url db (computed column in sql server)

        // await context.SaveChangesAsync();

        return Redirect(originalUrl);
    }
}
