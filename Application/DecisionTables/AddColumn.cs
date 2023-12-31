using Application.Conditions;
using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.DecisionTables
{
    public class AddColumn
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid TableId { get; set; }
            public Condition Condition { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Condition).SetValidator(new ConditionValidator());
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
                    var currentTable = await _context.DecisionTables
                    .Include(t => t.Rows).ThenInclude(r => r.Values)
                    .FirstOrDefaultAsync(t => t.Id == request.TableId);

                    _context.Conditions.Add(request.Condition);

                    foreach (var row in currentTable.Rows)
                    {
                        var decisionValue = new ConditionValue
                        {
                            ConditionId = request.Condition.Id,
                            DecisionRowId = row.Id,
                            Value = ""
                        };

                        row.Values.Add(decisionValue);
                    }

                    var result = await _context.SaveChangesAsync() > 0;

                    if (!result) throw new Exception("Failed to add condition to decision table");

                    await transaction.CommitAsync();

                    return Result<Unit>.Success(Unit.Value);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Result<Unit>.Failure(ex.Message);
                }
            }
        }
    }
}