using Domain;
using FluentValidation;

namespace Application.Rules
{
    public class RuleValidator : AbstractValidator<Rule>
    {
        public RuleValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.RuleProjectId).NotEmpty();
        }
    }
}