using FluentValidation;
using Movies.Application.Model;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService(IMovieRepository movieRepository, IRatingRepository ratingRepository, IValidator<Movie> movieValidator, IValidator<GetAllMoviesOption> optionValidator)
    : IMovieService
{
    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await movieValidator.ValidateAndThrowAsync(movie, cancellationToken: token);
        return await movieRepository.CreateAsync(movie, token);
    }

    public Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token = default)
    {
        return movieRepository.GetByIdAsync(id, userId, token);
    }

    public Task<Movie?> GetBySlugAsync(string slug, Guid? userId, CancellationToken token = default)
    {
        return movieRepository.GetBySlugAsync(slug, userId, token);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOption option, CancellationToken token = default)
    {

        await optionValidator.ValidateAndThrowAsync(option, token);
        
        return await movieRepository.GetAllAsync(option, token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId, CancellationToken token = default)
    {

        await movieValidator.ValidateAndThrowAsync(movie, cancellationToken: token);
        var movieExists = await movieRepository.ExistByIdAsync(movie.Id);
        if (!movieExists)
        {
            return null;
        }

        await movieRepository.UpdateAsync(movie, userId, token);

        if (!userId.HasValue)
        {
            var rating = await ratingRepository.GetRatingAsync(movie.Id, token);
            movie.Rating = rating;
            return movie;
        }

        var ratings = await ratingRepository.GetRatingAsync(movie.Id, userId.Value, token);
        movie.Rating = ratings.Rating;
        movie.UserRating = ratings.UserRating;
        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return movieRepository.DeleteByIdAsync(id, token);
    }
}