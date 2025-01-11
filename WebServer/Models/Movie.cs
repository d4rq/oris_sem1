namespace WebServer.Models;

public class Movie
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string Rating { get; set; } = string.Empty;
    public string Preview { get; set; } = string.Empty;
}