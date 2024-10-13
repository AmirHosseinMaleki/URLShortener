using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Models;
using UrlShortener.ViewModels;

public class ReportService : IReportService
{
    private readonly AppDbContext context;

    public ReportService(AppDbContext context)
    {
        context = context;
    }

    public async Task<List<MostViewedLinkReportViewModel>> GetMostViewedLinksReport(int month, int year)
    {
        var report = await context.Urls
            .Where(url => url.UrlDetails.Any(u => u.ViewedAt.Month == month && u.ViewedAt.Year == year))
            .Select(url => new MostViewedLinkReportViewModel
            {
                OriginalUrl = u.OriginalUrl,
                ShortenedUrl = u.ShortenedUrl,
                Views = url.UrlDetails.Count(u => u.ViewedAt.Month == month && u.ViewedAt.Year == year)
            })
            .OrderByDescending(u => u.Views)
            .ToListAsync();

        return report;
    }

    public async Task<List<UsageByOSAndIPReportViewModel>> GetLinkUsageByOSAndIPReport()
    {
        var report = await context.UrlDetails
            .GroupBy(u => new { u.OS, u.IP, u.Url.ShortenedUrl })
            .Select(group => new UsageByOSAndIPReportViewModel
            {
                OS = group.Key.OS,
                IP = group.Key.IP,
                Link = group.Key.ShortenedUrl,
                Views = group.Count()
            })
            .OrderByDescending(u => u.Views)
            .ToListAsync();

        return report;
    }
}
