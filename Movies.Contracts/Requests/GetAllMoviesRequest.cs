using System.ComponentModel;

namespace Movies.Contracts.Requests;

public class GetAllMoviesRequest : PagedRequest
{
    [Description("Title of the movie")] public required string? Title { get; init; }

    [Description("Release year of the movie")]
    public required int? Year { get; init; }

    [Description("Sort field prefixed by -(descending) or +(ascending)")]
    public required string? SortBy { get; set; }
}