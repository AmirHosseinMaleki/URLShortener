using System.ComponentModel.DataAnnotations;

namespace UrlShortener.ViewModels;

public record UrlCreateDto([Required] string OriginalUrl);
