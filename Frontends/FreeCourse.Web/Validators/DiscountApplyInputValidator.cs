using FluentValidation;
using FreeCourse.Web.Models.DiscountDtos;

namespace FreeCourse.Web.Validators;

public class DiscountApplyInputValidator : AbstractValidator<DiscountApplyInput>
{
    public DiscountApplyInputValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("Discount Code can not be empty!");
    }
}