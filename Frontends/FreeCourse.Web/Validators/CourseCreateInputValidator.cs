using FluentValidation;
using FreeCourse.Web.Models.CatalogDtos;

namespace FreeCourse.Web.Validators;

public class CourseCreateInputValidator:AbstractValidator<CourseCreateInput>
{
    public CourseCreateInputValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name can not be empty!");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description can not be empty!");
        RuleFor(x => x.Feature.Duration).InclusiveBetween(1, int.MaxValue).WithMessage("Duration can not be empty!");
        RuleFor(x => x.Price).NotEmpty().WithMessage("Price can not be empty!").PrecisionScale(2, 1, true)
            .WithMessage("Price format is invalid!");
    }
}