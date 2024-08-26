using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models
{
    public class UrlCreateDto
    {
        [Required]
        public required string OriginalUrl { get; set; }
    }
}
