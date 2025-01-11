using HttpServerLibrary.Configuration;
using Microsoft.Data.SqlClient;
using OrmLibrary;
using WebServer.Models;

namespace WebServer.Repositories;

public class MovieRepository
{
    private readonly OrmContext<Movie> _movieContext = new(new SqlConnection(AppConfig.GetInstance().ConnectionString));
    private readonly OrmContext<MovieDetail> _detailsContext = new(new SqlConnection(AppConfig.GetInstance().ConnectionString));
    private readonly OrmContext<Actor> _actorsContext = new(new SqlConnection(AppConfig.GetInstance().ConnectionString));

    public List<Movie> GetMovies()
    {
        return _movieContext.ReadAll();
    }

    public MovieDetail GetMovieDetail(int id)
    {
        return _detailsContext.ReadAll().FirstOrDefault(x => x.Id == id)!;
    }

    public bool AddMovie(Movie movie, MovieDetail movieDetail, Actor actor)
    {
        return _movieContext.Create(movie)
            && _detailsContext.Create(movieDetail)
            && _actorsContext.Create(actor);
    }

    public bool DeleteMovie(int id)
    {
        return _movieContext.Delete(id)
               && _detailsContext.Delete(id);
    }

    public Actor GetActor(int id)
    {
        return _actorsContext.ReadAll().FirstOrDefault(x => x.Id == id)!;
    }
}