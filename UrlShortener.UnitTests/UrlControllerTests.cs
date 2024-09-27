using Microsoft.AspNetCore.Mvc;
using Moq;
using UrlShortener.Data;
using UrlShortener.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using UrlShortener.UnitTests;
using UrlShortener.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;

namespace UrlShortener.UrlShortener.UnitTests;

public class UrlControllerTests
{
    private readonly DbContextFixture dbContext = new();
    private readonly Mock<DbSet<Url>> mockSet = new();
    private readonly UrlController controller;

    public UrlControllerTests()
    {
        controller = new UrlController(dbContext.DbContext)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity([new(ClaimTypes.NameIdentifier, "some user id"),]))
                }
            }
        };

        // var mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
        // mockClaimsPrincipal.Setup(p => p.FindFirstValue(It.IsAny<string>())).Returns(It.IsAny<string>);

        // controller.ControllerContext.HttpContext.User = mockClaimsPrincipal.Object;
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
        var result = await controller.Create(urlDto);

        Assert.True(((IStatusCodeActionResult)result).StatusCode == (int)HttpStatusCode.Created);

        Assert.True(((CreatedAtActionResult)result).RouteValues["id"] is not null);

        Assert.True(((CreatedAtActionResult)result).Value is not null);

        // var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        // var createdUrl = Assert.IsType<Url>(createdResult.Value);
        // Assert.Equal("https://google.com", createdUrl.OriginalUrl);
        // Assert.NotNull(createdUrl.ShortenedUrl);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenUrlIsNull()
    {
        var urlDto = new UrlCreateDto(null);

        var result = await controller.Create(urlDto);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenUrlIsDeleted()
    {
        var result = await controller.Delete(1);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenUrlDoesNotExist()
    {
        var result = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ShouldReturnUnauthorized_IfUserIsNotOwner()
    {
        var url = dbContext.DbContext.Urls.First();
        url.UserId = "anotherUser";
        dbContext.DbContext.SaveChanges();

        var result = await controller.Delete(1);

        Assert.IsType<UnauthorizedResult>(result);
    }

}
