using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Movies.Contracts.Requests;

public class PagedRequest
{
    [Description("Page number")] public required int Page { get; init; } = 1;

    [Description("Page size")]
    [Range(1, 20)]
    public required int PageSize { get; set; } = 10;
}