using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Persistence;

namespace Application.DecisionTables
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<DecisionTableListDto>>>
        {
            public PagingParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<DecisionTableListDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<PagedList<DecisionTableListDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = _context.DecisionTables
                    .OrderBy(d => d.Name)
                    .ProjectTo<DecisionTableListDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() })
                    .AsQueryable();

                return Result<PagedList<DecisionTableListDto>>.Success(
                    await PagedList<DecisionTableListDto>.CreateAsync(query, request.Params.PageNumber, request.Params.PageSize)
                );
            }
        }
    }
}