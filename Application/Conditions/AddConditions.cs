using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Conditions
{
    public class AddConditions
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid RuleId { get; set; }
            public ICollection<Condition> Conditions { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleForEach(x => x.Conditions).SetValidator(new ConditionValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                foreach (Condition condition in request.Conditions)
                {
                    AddConditionToContext(condition, request.RuleId);
                    foreach (var action in condition.Actions)
                    {
                        action.ConditionId = condition.Id;
                        _context.Actions.Add(action);
                    }
                }

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to create conditions");

                return Result<Unit>.Success(Unit.Value);
            }

            private void AddConditionToContext(Condition condition, Guid ruleId)
            {
                condition.RuleId = ruleId;
                _context.Conditions.Add(condition);

                if (condition.SubConditions != null)
                {
                    foreach (var subCondition in condition.SubConditions)
                    {
                        subCondition.ParentConditionId = condition.Id;
                        AddConditionToContext(subCondition, ruleId);
                    }
                }
            }
        }
    }
}