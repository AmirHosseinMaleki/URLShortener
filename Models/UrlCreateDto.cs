using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models
{
    public record UrlCreateDto(
        [Required] string OriginalUrl
    );
}
