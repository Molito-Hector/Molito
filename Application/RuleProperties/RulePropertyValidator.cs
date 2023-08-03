using Domain;
using FluentValidation;

namespace Application.RuleProperties
{
    public class RulePropertyValidator : AbstractValidator<RuleProperty>
    {
        public RulePropertyValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Type).NotEmpty();
            RuleFor(x => x.Direction).NotEmpty();
        }
    }
}