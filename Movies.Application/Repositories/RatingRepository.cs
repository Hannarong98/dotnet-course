using Dapper;
using Movies.Application.Database;
using Movies.Application.Model;

namespace Movies.Application.Repositories;

public class RatingRepository(IDbConnectionFactory dbConnectionFactory) : IRatingRepository
{
    public async Task<bool> RateMovieAsync(Guid movieId, Guid userId, int rating, CancellationToken token = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

        const string sql = """
                           insert into ratings(userid, movieid, rating)
                           values (@userId, @movieId, @rating)
                           on conflict (userid, movieid) do update
                           set rating = @rating
                           """;
        
        var result =  await connection.ExecuteAsync(new CommandDefinition(sql, new { userId, movieId, rating }, cancellationToken: token));

        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken token = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

        const string sql = """
                           select round(avg(r.rating), 1) from ratings r
                           where movieid = @movieId
                           """;

        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition(sql,
            new { movieId },
            cancellationToken: token
        ));
    }

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

        const string sql = """
                           delete from ratings
                           where movieid = @movieId
                           and userid = @userId
                           """;

        var result =
            await connection.ExecuteAsync(new CommandDefinition(sql, new { movieId, userId },
                cancellationToken: token));

        return result > 0;
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken token = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

        const string sql = """
                           select round(avg(rating), 1),
                               (select rating from ratings where movieid = @movieId and userid = @userId limit 1)
                           from ratings
                           where movieid = @movieId
                           """;
        
        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition(sql,
            new { movieId, userId },
            cancellationToken: token
        ));
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken token = default)
    {
        using var connection = await dbConnectionFactory.CreateConnectionAsync(token);

        const string sql = """
                           select r.rating, r.movieid, m.slug
                           from ratings r
                           inner join movies m on r.movieid = m.id
                           where userid = @userId
                           """;

        return await connection.QueryAsync<MovieRating>(new CommandDefinition(sql, new { userId },
            cancellationToken: token));
    }
}