using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Conditions
{
    public class EditCondition
    {
        public class Command : IRequest<Result<Unit>>
        {
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
                var condition = await _context.Conditions
                    .Include(p => p.SubConditions)
                    .FirstOrDefaultAsync(r => r.Id == request.Condition.Id);

                if (condition == null) return null;

                condition.Field = request.Condition.Field;
                condition.Operator = request.Condition.Operator;
                condition.Value = request.Condition.Value;
                condition.LogicalOperator = request.Condition.LogicalOperator;

                if (condition.SubConditions != null) _context.Conditions.RemoveRange(condition.SubConditions);

                foreach (Condition subCondition in request.Condition.SubConditions)
                {
                    subCondition.Field = request.Condition.Field;
                    subCondition.Operator = request.Condition.Operator;
                    subCondition.Value = request.Condition.Value;
                    subCondition.ParentConditionId = condition.Id;
                    _context.Conditions.Add(subCondition);
                }

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to update condition");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}