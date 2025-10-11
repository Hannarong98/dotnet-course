using Movies.Application.Model;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    Task<bool> CreateAsync(Movie movie, CancellationToken token = default);

    Task<Movie?> GetByIdAsync(Guid id, Guid? userId, CancellationToken token = default);
    
    Task<Movie?> GetBySlugAsync(string slug, Guid? userId, CancellationToken token = default);

    Task<IEnumerable<Movie>> GetAllAsync(Guid? userId, CancellationToken token = default);

    Task<bool> UpdateAsync(Movie movie, Guid? userId, CancellationToken token = default);

    Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default);
    
    Task<bool> ExistById(Guid id, CancellationToken token = default);
}