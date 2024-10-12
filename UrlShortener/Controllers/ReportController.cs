using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UrlShortener.Services;

[ApiController]
[Route("api/[controller]")]
public class ReportController(AppDbContext context) : ControllerBase
{
    private readonly IReportService reportService;

    public ReportController(IReportService reportService)
    {
        reportService = reportService;
    }

    [HttpGet("most-viewed-links")]
    public async Task<IActionResult> GetMostViewedLinksReport([FromQuery] int month, [FromQuery] int year)
    {
        var report = await reportService.GetMostViewedLinksReport(month, year);
        return Ok(report);
    }

    [HttpGet("usage-by-os-ip")]
    public async Task<IActionResult> GetUsageByOSAndIPReport()
    {
        var report = await reportService.GetLinkUsageByOSAndIPReport();
        return Ok(report);
    }
}
