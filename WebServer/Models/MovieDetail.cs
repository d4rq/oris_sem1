namespace WebServer.Models;

public class MovieDetail
{
    private DateTime release;
    public int Id { get; set; }
    public string Imdb { get; set; } = string.Empty;
    public string Kp { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string Studio { get; set; } = string.Empty;
    public string Age { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;

    public DateTime Release { get; set; }
}