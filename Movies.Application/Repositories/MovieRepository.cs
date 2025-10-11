using Dapper;
using Movies.Application.Database;
using Movies.Application.Model;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{

    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory connectionFactory)
    {
        _dbConnectionFactory = connectionFactory;
    }
    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         insert into movies (id, slug, title, year_of_release)
                                                                         values (@Id, @Slug, @Title, @YearOfRelease)
                                                                         """, movie, cancellationToken: token));

        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                                                                    insert into genres (movieId, name)
                                                                    values (@MovieId, @Name)
                                                                    """, new { MovieId = movie.Id, Name = genre }, cancellationToken: token));
            }
        }
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        const string query = """
                             select * from movies where id = @id;
                             """;
        
        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition(query, new { id }, cancellationToken: token));
        
        if (movie is null)
        {
            return null;
        }
        
        const string genreQuery = """
                                  select name from genres where movieid = @id
                                  """;

        var genres = await connection.QueryAsync<string>(new CommandDefinition(genreQuery, new { id }, cancellationToken: token));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        const string query = """
                             select * from movies where slug = @slug
                             """;

        var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
            new CommandDefinition(query, new { slug }, cancellationToken: token));
        if (movie == null)
        {
            return null;
        }

        var genres = await connection.QueryAsync<string>(new CommandDefinition(@"select name from genres where movieid = @id;", new { id = movie.Id }, cancellationToken: token));

        foreach (var genre in genres)
        {
            movie.Genres.Add(genre);
        }

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        const string query = """
                             select m.*, string_agg(g.name, ',') as genres
                             from movies m left join genres g on m.id = g.movieid
                             group by id
                             """;

        var results = await connection.QueryAsync(new CommandDefinition(query, cancellationToken: token));
        
        return results.Select(item => new Movie
        {
            Id = item.id,
            Title = item.title,
            YearOfRelease = item.year_of_release,
            Genres = Enumerable.ToList(item.genres.Split(','))
        });

    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        using var transaction = connection.BeginTransaction();

        const string deleteQuery = """
                             delete from genres where movieid = @id
                             """;

        await connection.ExecuteAsync(new CommandDefinition(deleteQuery, new { id = movie.Id }, cancellationToken: token));

        const string updateQuery = """
                                   update movies set slug = @Slug, title = @Title, year_of_release = @YearOfRelease
                                   where id = @Id
                                   """;
        var result = await connection.ExecuteAsync(new CommandDefinition(updateQuery, movie, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        using var transaction = connection.BeginTransaction();
        
        const string deleteGenresQuery = """
                                   delete from genres where movieid = @id
                                   """;
        await connection.ExecuteAsync(new CommandDefinition(deleteGenresQuery, new { id }, cancellationToken: token));

        const string deleteMovieQuery = """
                                        delete from movies where id = @id
                                        """;

        var result = await connection.ExecuteAsync(new CommandDefinition(deleteMovieQuery, new { id }, cancellationToken: token));
        
        transaction.Commit();

        return result > 0;
    }

    public async Task<bool> ExistById(Guid id, CancellationToken token = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();

        const string query = """
                             select count(1) from movies where id = @id
                             """;

        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(query, new { id }, cancellationToken: token));
    }
}