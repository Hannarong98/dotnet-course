using FluentValidation;
using Movies.Application.Model;

namespace Movies.Application.Validators;

public class GetAllMoviesOptionValidator : AbstractValidator<GetAllMoviesOption>
{
    private static readonly string[] ValidSortFields =
    {
        "title", "yearofrelease"
    };

    public GetAllMoviesOptionValidator()
    {
        RuleFor(x => x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);

        RuleFor(x => x.SortField)
            .Must(x => x is null || ValidSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Only following field are sortable: {string.Join(", ", ValidSortFields)}");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 20)
            .WithMessage("You can get between 1 and 20 movies page");
    }
}