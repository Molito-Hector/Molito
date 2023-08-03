using Domain;
using FluentValidation;

namespace Application.RuleProjects
{
    public class RuleProjectValidator : AbstractValidator<RuleProject>
    {
        public RuleProjectValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}