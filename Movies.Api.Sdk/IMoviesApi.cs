using Movies.Contracts.Requests;
using Movies.Contracts.Responses;
using Refit;

namespace Movies.Api.Sdk;

public interface IMoviesApi
{
    [Get(ApiEndpoints.V1.Movies.GetByIdOrSlug)]
    Task<MovieResponse> GetMovieAsync(string idOrSlug);

    [Get(ApiEndpoints.V1.Movies.GetAll)]
    Task<MoviesResponse> GetMoviesAsync(GetAllMoviesRequest request);

    [Post(ApiEndpoints.V1.Movies.Create)]
    Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request);
}