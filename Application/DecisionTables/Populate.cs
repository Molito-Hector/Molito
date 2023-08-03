using Application.Conditions;
using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.DecisionTables
{
    public class Populate
    {
        public class Command : IRequest<Result<Unit>>
        {
            public DecisionTable DecisionTable { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleForEach(x => x.DecisionTable.Conditions).SetValidator(new ConditionValidator());
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
                var tableId = request.DecisionTable.Id;

                foreach (var condition in request.DecisionTable.Conditions)
                {
                    condition.Id = new Guid();
                    AddConditionToContext(condition, tableId);
                }

                foreach (var row in request.DecisionTable.Rows)
                {
                    row.TableId = tableId;
                    _context.DecisionRows.Add(row);

                    foreach (var action in row.Actions)
                    {
                        action.RowId = row.Id;
                        _context.Actions.Add(action);
                    }
                }

                for (var i = 0; i < request.DecisionTable.Conditions.Count; i++)
                {
                    foreach (var row in request.DecisionTable.Rows)
                    {
                        row.Values.ElementAt(i).ConditionId = request.DecisionTable.Conditions.ElementAt(i).Id;
                        row.Values.ElementAt(i).DecisionRowId = row.Id;
                        _context.ConditionValues.Add(row.Values.ElementAt(i));
                    }
                }

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to populate decision table");

                return Result<Unit>.Success(Unit.Value);
            }

            private void AddConditionToContext(Condition condition, Guid tableId)
            {
                condition.TableId = tableId;
                _context.Conditions.Add(condition);

                if (condition.SubConditions != null)
                {
                    foreach (var subCondition in condition.SubConditions)
                    {
                        subCondition.ParentConditionId = condition.Id;
                        AddConditionToContext(subCondition, tableId);
                    }
                }
            }
        }
    }
}