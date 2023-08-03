using Domain;
using FluentValidation;

namespace Application.Conditions
{
    public class ConditionValidator : AbstractValidator<Condition>
    {
        public ConditionValidator()
        {
            RuleFor(x => x.Field).NotEmpty();
            RuleFor(x => x.Operator).NotEmpty();
            RuleFor(x => x.Actions).NotEmpty();
        }
    }
}