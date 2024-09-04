using System;

namespace UrlShortener.Models;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }

    public ICollection<Url> Urls { get; set; }
}

