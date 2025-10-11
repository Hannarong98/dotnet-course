using Movies.Application.Model;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping;

public static class ContractMapping
{
    public static Movie MapToMovie(this CreateMovieRequest request)
    {
        return new Movie()
        {
            Id = Guid.NewGuid(),
            Genres = request.Genres.ToList(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease
        };
    }
    
    public static Movie MapToMovie(this UpdateMovieRequest request, Guid id)
    {
        return new Movie()
        {
            Id = id,
            Genres = request.Genres.ToList(),
            Title = request.Title,
            YearOfRelease = request.YearOfRelease
        };
    }



    public static MovieResponse MapToResponse(this Movie movie)
    {
        return new MovieResponse()
        {
            Genres = movie.Genres,
            Id = movie.Id,
            Title = movie.Title,
            YearOfRelease = movie.YearOfRelease,
            Slug = movie.Slug
        };
    }
    
    public static MoviesResponse MapToResponse(this IEnumerable<Movie> movies)
    {
        return new MoviesResponse()
        {
           Items = movies.Select(MapToResponse)
        };
    }
}