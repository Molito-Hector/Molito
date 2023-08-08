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
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    var tableId = request.DecisionTable.Id;

                    foreach (var condition in request.DecisionTable.Conditions)
                    {
                        AddConditions(condition, tableId);
                    }
                    _context.Conditions.AddRange(request.DecisionTable.Conditions);

                    foreach (var row in request.DecisionTable.Rows)
                    {
                        row.TableId = tableId;

                        foreach (var action in row.Actions)
                        {
                            action.RowId = row.Id;
                        }

                        for (var i = 0; i < request.DecisionTable.Conditions.Count; i++)
                        {
                            row.Values.ElementAt(i).ConditionId = request.DecisionTable.Conditions.ElementAt(i).Id;
                            row.Values.ElementAt(i).DecisionRowId = row.Id;
                        }
                    }
                    _context.DecisionRows.AddRange(request.DecisionTable.Rows);

                    var result = await _context.SaveChangesAsync() > 0;

                    if (!result) throw new Exception("Failed to populate decision table");

                    await transaction.CommitAsync();

                    return Result<Unit>.Success(Unit.Value);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Result<Unit>.Failure(ex.Message);
                }
            }

            private void AddConditions(Condition condition, Guid tableId)
            {
                condition.Id = Guid.NewGuid();
                condition.TableId = tableId;

                if (condition.SubConditions != null)
                {
                    foreach (var subCondition in condition.SubConditions)
                    {
                        subCondition.ParentConditionId = condition.Id;
                        AddConditions(subCondition, tableId);
                    }
                }
            }
        }
    }
}