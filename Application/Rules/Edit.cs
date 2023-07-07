using Application.Core;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Rules
{
    public class Edit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Rule Rule { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.Rule).SetValidator(new RuleValidator());
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var rule = await _context.Rules.FindAsync(request.Rule.Id);

                if (rule == null) return null;

                _mapper.Map(request.Rule, rule);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to update rule");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}