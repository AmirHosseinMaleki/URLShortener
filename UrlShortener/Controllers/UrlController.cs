using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Models;
using UrlShortener.ViewModels;

[ApiController]
[Authorize]
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
        var userId = GetUserId(User) ?? throw new UnauthorizedAccessException();
        var url = new Url
        {
            OriginalUrl = urlDto.OriginalUrl,
            ShortenedUrl = shortCode,
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };

        context.Urls.Add(url);
        await context.SaveChangesAsync();

        var fullShortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}";
        return CreatedAtAction(nameof(GetById), new { id = url.Id }, new { NewUrl = fullShortUrl });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var url = await context.Urls.FindAsync(id);
        var userId = GetUserId(User) ?? throw new UnauthorizedAccessException();

        if (url == null)
        {
            return NotFound();
        }

        if (url.UserId != userId)
        {
            return Unauthorized();
        }


        context.Urls.Remove(url);
        await context.SaveChangesAsync();

        return NoContent();
    }

    internal static string? GetUserId(ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    [HttpGet("{id:int}/details")]
    public async Task<IActionResult> GetUrlDetails(int id)
    {
        var urlDetails = await context.UrlDetails.Where(ud => ud.UrlId == id).ToListAsync();
        return Ok(urlDetails);
    }

}
