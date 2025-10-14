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
            Rating = movie.Rating,
            UserRating = movie.UserRating,
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
    
    public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> ratings)
    {
        return ratings.Select(x => new MovieRatingResponse
        {
            Rating = x.Rating,
            Slug = x.Slug,
            MovieId = x.MovieId
        });
    }

    public static GetAllMoviesOption MapToOptions(this GetAllMoviesRequest request)
    {
        return new GetAllMoviesOption
        {
            Title = request.Title,
            YearOfRelease = request.Year
        };
    }
    
    public static GetAllMoviesOption WithUser(this GetAllMoviesOption option, Guid? userId)
    {
        option.UserId = userId;
        return option;
    }
}