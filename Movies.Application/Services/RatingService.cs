using FluentValidation;
using FluentValidation.Results;
using Movies.Application.Model;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class RatingService(IRatingRepository ratingRepository, IMovieRepository movieRepository) : IRatingService
{
    public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken token = default)
    {
        if (rating is <= 0 or > 5)
            throw new ValidationException(new[]
            {
                new ValidationFailure
                {
                    PropertyName = "Rating",
                    ErrorMessage = "Rating must be between 1 and 5"
                }
            });

        var movieExists = await movieRepository.ExistByIdAsync(movieId, token);

        if (!movieExists) return false;

        return await ratingRepository.RateMovieAsync(movieId, userId, rating, token);
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        return await ratingRepository.DeleteRatingAsync(movieId, userId, token);
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        return await ratingRepository.GetRatingsForUserAsync(userId, token);
    }
}