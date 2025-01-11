using HttpServerLibrary;
using HttpServerLibrary.Attributes;
using HttpServerLibrary.Configuration;
using HttpServerLibrary.HttpResponse;
using OrmLibrary;
using TemplateEngine;
using WebServer.Models;
using WebServer.Repositories;
using WebServer.services;

namespace WebServer.Endpoints;

public class MoviesEndpoint : EndpointBase
{
    [Get("movies")]
    public IHttpResponseResult GetMovies()
    {
        var repository = new MovieRepository();
        return Json(new { movies = repository.GetMovies() });
    }

    [Get("movie")]
    public IHttpResponseResult GetMovie(int id)
    {
        var repository = new MovieRepository();
        var html = File.ReadAllText("./zonafilm/html/movie.html");
        var engine = new HtmlTemplateEngine();
        var movie = repository.GetMovies().FirstOrDefault(x => x.Id == id);
        if (movie == null) return Json(new { error = "Movie not found" });
        html = engine.Render(html, movie!);
        var details = repository.GetMovieDetail(id);
        var actor = repository.GetActor(id);
        html = engine.Render(html, actor);
        return Html(engine.Render(html, details));
    }
    
    [Get("getInfo")]
    public IHttpResponseResult GetInfo(int id)
    {
        var repository = new MovieRepository();
        return Json(new { movie = repository.GetMovieDetail(id) });
    }

    [Post("add")]
    public IHttpResponseResult AddMovie(int id, string name, string rating, string preview, string year, string imdb,
        string kp, string title, string time, string genre, string studio, string age, DateTime release, string actor, string pic)
    {
        var repository = new MovieRepository();
        if (id == 0)
        {
            return Json(new
            {
                result = repository.AddMovie(
                    new Movie()
                    {
                        Name = name,
                        Rating = rating,
                        Year = year,
                        Preview = preview
                    },
                    new MovieDetail()
                    {
                        Imdb = imdb,
                        Title = title,
                        Kp = kp,
                        Age = age,
                        Studio = studio,
                        Release = release,
                        Genre = genre,
                        Time = time
                    },
                    new Actor()
                    {
                        ActorName = actor,
                        Pic = pic
                    }
                )
            });
        }
        if (repository.GetMovies().FirstOrDefault(x => x.Id == id) == null) return Json(new { result = "Movie not found" });
        return Json(false);
    }

    [Post("delete")]
    public IHttpResponseResult DeleteMovie(int id)
    {
        var repository = new MovieRepository();
        return Json(new { success = repository.DeleteMovie(id) });
    }
    
    [Get("admin")]
    public IHttpResponseResult GetPage()
    {
        if (!IsAuthorized(Context)) // Используем метод проверки авторизации
        {
            return Redirect("/auth/login");
        }
        
        var repository = new UserRepository();
        var id = int.Parse((ReadOnlySpan<char>)SessionStorage.GetUserId(Context.Request.Cookies["session-token"]!.Value));
        var user = repository.GetUserById(id);
        return Html(File.ReadAllText("./zonafilm/html/admin.html"));
    }
        
    private bool IsAuthorized(HttpRequestContext context)
    {
        // Проверка наличия Cookie с session-token
        //var cookies = context.Request.Cookies.First();
        if (context.Request.Cookies.Any(c=> c.Name == "session-token"))
        {
            var cookie = context.Request.Cookies["session-token"];
            return SessionStorage.ValidateToken(cookie.Value);
        }
         
        return false;
    }
}