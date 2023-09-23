using Application.Core;
using Domain;
using FluentValidation;
using MathNet.Symbolics;
using MediatR;
using Persistence;

namespace Application.Conditions
{
    public class AddConditions
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid RuleId { get; set; }
            public string Predicate { get; set; }
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
                    AddConditionToContext(condition, request.RuleId, request.Predicate);
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

            private void AddConditionToContext(Condition condition, Guid ruleId, string requestPredicate)
            {
                if (requestPredicate == "Rule")
                {
                    condition.RuleId = ruleId;
                }
                else if (requestPredicate == "Table")
                {
                    condition.TableId = ruleId;
                }

                _context.Conditions.Add(condition);

                if (condition.SubConditions != null)
                {
                    foreach (var subCondition in condition.SubConditions)
                    {
                        subCondition.ParentConditionId = condition.Id;
                        AddConditionToContext(subCondition, ruleId, requestPredicate);
                    }
                }

                AddDefaultValueForNewCondition(condition);
            }

            private void AddDefaultValueForNewCondition(Condition condition)
            {
                var rows = _context.DecisionRows.Where(r => r.TableId == condition.TableId).ToList();

                foreach (var row in rows)
                {
                    if (row.Values == null)
                    {
                        row.Values = new List<ConditionValue>();
                    }
                    
                    row.Values.Add(new ConditionValue
                    {
                        ConditionId = condition.Id,
                        DecisionRowId = row.Id,
                        Value = ""
                    });
                }
            }
        }
    }
}