using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.DecisionTables
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public DecisionTable DecisionTable { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.DecisionTable).SetValidator(new DTValidator());
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
                var table = await _context.DecisionTables
                    .Include(r => r.Rows)
                    .FirstOrDefaultAsync(r => r.Id == request.DecisionTable.Id);

                if (table == null) return null;

                table.Name = request.DecisionTable.Name;
                table.Description = request.DecisionTable.Description;
                table.EvaluationType = request.DecisionTable.EvaluationType;


                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to update rule");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}