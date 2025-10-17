using System.ComponentModel;

namespace Movies.Contracts.Requests;

public class CreateMovieRequest
{
    [Description("Title of the movie")] public required string Title { get; set; }

    [Description("Release year of the movie")]
    public required int YearOfRelease { get; set; }

    [Description("Genres of the movie")]
    public required IEnumerable<string> Genres { get; init; } = Enumerable.Empty<string>();
}