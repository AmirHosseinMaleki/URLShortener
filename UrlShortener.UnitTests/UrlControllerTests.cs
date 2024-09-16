using Microsoft.AspNetCore.Mvc;
using Moq;
using UrlShortener.Data;
using UrlShortener.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using UrlShortener.UnitTests;
using UrlShortener.ViewModels;
using Microsoft.AspNetCore.Http;

namespace UrlShortener.UrlShortener.UnitTests;

public class UrlControllerTests
{
    private readonly DbContextFixture dbContext;
    private readonly Mock<DbSet<Url>> mockSet;
    private readonly UrlController controller;

    public UrlControllerTests()
    {
        dbContext = new DbContextFixture();
        mockSet = new Mock<DbSet<Url>>();
        controller = new UrlController(dbContext.DbContext);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResult_WithListOfUrls()
    {
        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnUrls = Assert.IsType<List<Url>>(okResult.Value);
        Assert.Equal(2, returnUrls.Count);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResult_WhenNoUrlsExist()
    {
        var result = await controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnUrls = Assert.IsType<List<Url>>(okResult.Value);
        Assert.NotNull(returnUrls);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkResult_WithUrl()
    {
        var result = await controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnUrl = Assert.IsType<Url>(okResult.Value);
        Assert.Equal(1, returnUrl.Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenUrlDoesNotExist()
    {
        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedResult_WhenValidUrlIsProvided()
    {
        var urlDto = new UrlCreateDto("https://google.com");

        var mockHttpContext = new Mock<HttpContext>();
        var mockRequest = new Mock<HttpRequest>();

        mockRequest.Setup(r => r.Scheme).Returns("https");
        mockRequest.Setup(r => r.Host).Returns(new HostString("localhost"));

        mockHttpContext.Setup(c => c.Request).Returns(mockRequest.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = mockHttpContext.Object
        };

        var result = await controller.Create(urlDto);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var tt = typeof(CreatedResult);
        var createdUrl = Assert.IsType<Url>(createdResult.Value);
        Assert.Equal("https://google.com", createdUrl.OriginalUrl);
        Assert.NotNull(createdUrl.ShortenedUrl);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenUrlIsNull()
    {
        var urlDto = new UrlCreateDto(null);

        var result = await controller.Create(urlDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
