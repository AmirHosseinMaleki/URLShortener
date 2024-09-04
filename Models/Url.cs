using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models
{
    public class Url
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string OriginalUrl { get; set; }

        [Required]
        public required string ShortenedUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ViewCount { get; set; } = 0;

        public string UserId { get; set; }

    }
}
