using System.ComponentModel.DataAnnotations;
namespace UrlShortener.Models;

public class UrlDetails
{
    [Key]
    public int Id { get; set; }

    public string IP { get; set; }
    public string OS { get; set; }
    public DateTime ViewedAt { get; set; }

    public int UrlId { get; set; }
    public Url Url { get; set; }
}
