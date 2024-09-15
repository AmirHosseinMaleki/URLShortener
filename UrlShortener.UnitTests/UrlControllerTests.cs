using Microsoft.AspNetCore.Mvc;
using Moq;
using UrlShortener.Data;
using UrlShortener.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using UrlShortener.UnitTests;
using UrlShortener.ViewModels;

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
}
