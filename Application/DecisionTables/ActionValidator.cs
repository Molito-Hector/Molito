using FluentValidation;

namespace Application.DecisionTables
{
    public class ActionValidator : AbstractValidator<Domain.Action>
    {
        public ActionValidator()
        {
            RuleFor(x => x.TargetProperty).NotEmpty();
            RuleFor(x => x.ModificationType).NotEmpty();
        }
    }
}