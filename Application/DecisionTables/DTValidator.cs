using Domain;
using FluentValidation;

namespace Application.DecisionTables
{
    public class DTValidator : AbstractValidator<DecisionTable>
    {
        public DTValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.RuleProjectId).NotEmpty();
            RuleFor(x => x.EvaluationType).NotEmpty();
        }
    }
}