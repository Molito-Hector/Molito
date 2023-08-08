using Application.Core;
using Application.Interfaces;
using Application.RuleFlows;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Persistence;

namespace Application.RuleEngine
{
    public class ExecuteTable
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
            private readonly IEngineFunctions _engineFunctions;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor, IEngineFunctions engineFunctions)
            {
                _engineFunctions = engineFunctions;
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<JObject>> Handle(Command request, CancellationToken cancellationToken)
            {
                var table = await _context.DecisionTables
                    .ProjectTo<DTWithProjectDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() })
                    .FirstOrDefaultAsync(x => x.Id == request.Id);

                if (table == null) return null;

                var tib = new DecisionTableFlowElement(_engineFunctions) { DecisionTable = table };

                var result = tib.Execute(request.Data);

                return result;
            }
        }
    }
}