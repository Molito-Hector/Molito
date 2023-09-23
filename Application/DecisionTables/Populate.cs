using Application.Conditions;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                using var transaction = _context.Database.BeginTransaction();

                try
                {
                    foreach (DecisionRow row in request.DecisionTable.Rows)
                    {
                        var existingRow = await _context.DecisionRows.Include(r => r.Actions).Include(r => r.Values).FirstOrDefaultAsync(r => r.Id == row.Id);
                        if (existingRow != null)
                        {
                            // Update scalar properties
                            _context.Entry(existingRow).CurrentValues.SetValues(row);

                            // Update Actions
                            foreach (var incomingAction in row.Actions)
                            {
                                var existingAction = existingRow.Actions.FirstOrDefault(a => a.Id == incomingAction.Id);
                                if (existingAction != null)
                                {
                                    // Update existing action
                                    _context.Entry(existingAction).CurrentValues.SetValues(incomingAction);
                                }
                                else
                                {
                                    // Add new action
                                    existingRow.Actions.Add(incomingAction);
                                }
                            }

                            // Remove actions that no longer exist
                            foreach (var existingAction in existingRow.Actions.ToList())
                            {
                                if (!row.Actions.Any(a => a.Id == existingAction.Id))
                                {
                                    existingRow.Actions.Remove(existingAction);
                                }
                            }

                            // Update Values
                            foreach (var incomingValue in row.Values)
                            {
                                var existingValue = existingRow.Values.FirstOrDefault(v => v.Id == incomingValue.Id);
                                if (existingValue != null)
                                {
                                    // Update existing value
                                    _context.Entry(existingValue).CurrentValues.SetValues(incomingValue);
                                }
                                else
                                {
                                    // Add new value
                                    existingRow.Values.Add(incomingValue);
                                }
                            }

                            // Remove values that no longer exist
                            foreach (var existingValue in existingRow.Values.ToList())
                            {
                                if (!row.Values.Any(v => v.Id == existingValue.Id))
                                {
                                    existingRow.Values.Remove(existingValue);
                                }
                            }
                        }
                        else
                        {
                            DecisionRow newRow = new DecisionRow
                            {
                                Id = row.Id,
                                TableId = row.TableId,
                                Actions = row.Actions,
                                Values = row.Values
                            };
                            _context.DecisionRows.Add(newRow);
                        }
                    }

                    var log = _context.ChangeTracker.DebugView.LongView;

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