using Application.Core;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.DecisionTables
{
    public class Create
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
                var projectId = request.DecisionTable.RuleProjectId;

                var decisionTable = new DecisionTable
                {
                    Id = request.DecisionTable.Id,
                    RuleProjectId = projectId,
                    Name = request.DecisionTable.Name,
                    Description = request.DecisionTable.Description,
                    EvaluationType = request.DecisionTable.EvaluationType
                };

                _context.DecisionTables.Add(decisionTable);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to create decision table");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}