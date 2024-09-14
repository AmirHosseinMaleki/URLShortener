using System;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.UnitTests;

public class DbContextFixture : IDisposable
{
    public AppDbContext DbContext { get; private set; }

    public DbContextFixture()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("ApplicationDbContext")
            .Options;

        DbContext = new AppDbContext(options);

        DbContext.Urls.Add(new Url { Id = 1, OriginalUrl = "https://example1.com", ShortenedUrl = "abc123", UserId = "user1" });
        DbContext.Urls.Add(new Url { Id = 2, OriginalUrl = "https://example1.com", ShortenedUrl = "abc123", UserId = "user1" });
        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        DbContext.Dispose();
    }
}
