using FluentValidation;
using Movies.Application.Model;

namespace Movies.Application.Validators;

public class GetAllMoviesOptionValidator : AbstractValidator<GetAllMoviesOption>
{
    public GetAllMoviesOptionValidator()
    {
        RuleFor(x => x.YearOfRelease)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}