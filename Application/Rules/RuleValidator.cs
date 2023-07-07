using Domain;
using FluentValidation;

namespace Application.Rules
{
    public class RuleValidator : AbstractValidator<Rule>
    {
        public RuleValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}