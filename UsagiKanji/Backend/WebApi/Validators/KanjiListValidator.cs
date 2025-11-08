using Application.Dtos;
using FluentValidation;
namespace WebApi.Validators;

public class KanjiListValidator : AbstractValidator<KanjiListParams>
{
    public KanjiListValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0).WithMessage("PageIndex must be greater than 0.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50).WithMessage("PageSize must be between 1 and 50.");
        RuleFor(x => x.SortBy)
                .Must(IsAValidSortOption)
                .When(x => !string.IsNullOrWhiteSpace(x.SortBy))
                .WithMessage("Invalid sort field.");
        RuleFor(x => x)
                .Must(x => x.PageIndex <= x.TotalPages || x.TotalPages == 0)
                .WithMessage("PageIndex cannot exceed TotalPages.");
    }
    private bool IsAValidSortOption(string sortBy)
    {
        var validOptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "grade",
        "jlptlevel",
        "frequency",
        "heisig",
        "heisig6"
    };

        return validOptions.Contains(sortBy);
    }
}
