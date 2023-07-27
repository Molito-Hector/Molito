using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Persistence;

namespace Application.RuleEngine
{
    public class Execute
    {
        public class Command : IRequest<Result<JObject>>
        {
            public Guid Id { get; set; }
            public JObject Data { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<JObject>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly IRuleEngine _ruleEngine;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor, IRuleEngine ruleEngine)
            {
                _ruleEngine = ruleEngine;
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<JObject>> Handle(Command request, CancellationToken cancellationToken)
            {
                var rule = await _context.Rules
                    .ProjectTo<RuleDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() })
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (rule == null) return null;

                var result = _ruleEngine.ExecuteRule(rule, request.Data);

                return result;
            }
        }
    }
}