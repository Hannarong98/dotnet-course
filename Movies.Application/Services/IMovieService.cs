using Movies.Application.Model;

namespace Movies.Application.Services;

public interface IMovieService
{
    Task<bool> CreateAsync(Movie movie, CancellationToken token = default);

    Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token = default);
    
    Task<Movie?> GetBySlugAsync(string slug, Guid? userId, CancellationToken token = default);

    Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOption option, CancellationToken token = default);

    Task<Movie?> UpdateAsync(Movie movie, Guid? userId, CancellationToken token = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);

    Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken token = default);
}