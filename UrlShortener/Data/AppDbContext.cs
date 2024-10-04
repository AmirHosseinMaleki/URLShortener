using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Models;

namespace UrlShortener.Data;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Url> Urls { get; set; }
    public DbSet<UrlDetails> UrlDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Url>()
            .HasMany(u => u.UrlDetails)
            .WithOne(ud => ud.Url)
            .HasForeignKey(ud => ud.UrlId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
