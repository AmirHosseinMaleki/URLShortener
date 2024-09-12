using Microsoft.AspNetCore.Mvc;
using Moq;
using UrlShortener.Data;
using UrlShortener.Models;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.UrlShortener.UnitTests;

public class UrlControllerTests
{
    private readonly Mock<AppDbContext> mockContext;
    private readonly Mock<DbSet<Url>> mockSet;
    private readonly UrlController controller;

    public UrlControllerTests()
    {
        mockContext = new Mock<AppDbContext>();
        mockSet = new Mock<DbSet<Url>>();
        controller = new UrlController(mockContext.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResult_WithListOfUrls()
    {
        // Arrange
        var mockUrls = new List<Url>
        {
            new Url
            {
                Id = 1,
                OriginalUrl = "https://example1.com",
                ShortenedUrl = "abc123",
                UserId = "user1"
            },
            new Url
            {
                Id = 2,
                OriginalUrl = "https://example2.com",
                ShortenedUrl = "def456",
                UserId = "user2"
            }
        }.AsQueryable();

        mockSet.As<IQueryable<Url>>().Setup(m => m.Provider).Returns(mockUrls.Provider);
        mockSet.As<IQueryable<Url>>().Setup(m => m.Expression).Returns(mockUrls.Expression);
        mockSet.As<IQueryable<Url>>().Setup(m => m.ElementType).Returns(mockUrls.ElementType);
        mockSet.As<IQueryable<Url>>().Setup(m => m.GetEnumerator()).Returns(mockUrls.GetEnumerator());

        mockContext.Setup(c => c.Urls).Returns(mockSet.Object);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnUrls = Assert.IsType<List<Url>>(okResult.Value);
        Assert.Equal(2, returnUrls.Count);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResult_WhenNoUrlsExist()
    {
        var emptyUrls = new List<Url>().AsQueryable();

        mockSet.As<IQueryable<Url>>().Setup(m => m.Provider).Returns(emptyUrls.Provider);
        mockSet.As<IQueryable<Url>>().Setup(m => m.Expression).Returns(emptyUrls.Expression);
        mockSet.As<IQueryable<Url>>().Setup(m => m.ElementType).Returns(emptyUrls.ElementType);
        mockSet.As<IQueryable<Url>>().Setup(m => m.GetEnumerator()).Returns(emptyUrls.GetEnumerator());

        mockContext.Setup(c => c.Urls).Returns(mockSet.Object);

        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnUrls = Assert.IsType<List<Url>>(okResult.Value);
        Assert.Empty(returnUrls);
    }


}
