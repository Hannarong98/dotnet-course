using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Movies.Contracts.Requests;

public class RateMovieRequest
{
    [Description("Rate score")]
    [Range(1, 5)]
    public required int Rating { get; init; }
}