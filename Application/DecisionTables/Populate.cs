using Application.Conditions;
using Application.Core;
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
                RuleForEach(x => x.DecisionTable.Actions).SetValidator(new ActionValidator());
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
                    var existingRows = await _context.DecisionRows
                        .Include(r => r.Values)
                        .Include(r => r.ActionValues)
                        .Where(r => r.TableId == request.DecisionTable.Id)
                        .ToListAsync();

                    foreach (DecisionRow row in request.DecisionTable.Rows)
                    {
                        var existingRow = existingRows.Where(r => r.Id == row.Id).FirstOrDefault();
                        if (existingRow != null)
                        {
                            // Update scalar properties
                            _context.Entry(existingRow).CurrentValues.SetValues(row);

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

                            // Update ActionValues
                            foreach (var incomingValue in row.ActionValues)
                            {
                                var existingValue = existingRow.ActionValues.FirstOrDefault(v => v.Id == incomingValue.Id);
                                if (existingValue != null)
                                {
                                    // Update existing value
                                    _context.Entry(existingValue).CurrentValues.SetValues(incomingValue);
                                }
                                else
                                {
                                    // Add new action value
                                    existingRow.ActionValues.Add(incomingValue);
                                }
                            }

                            // Remove values that no longer exist
                            foreach (var existingValue in existingRow.ActionValues.ToList())
                            {
                                if (!row.ActionValues.Any(v => v.Id == existingValue.Id))
                                {
                                    existingRow.ActionValues.Remove(existingValue);
                                }
                            }
                        }
                        else
                        {
                            DecisionRow newRow = new DecisionRow
                            {
                                Id = row.Id,
                                TableId = row.TableId,
                                ActionValues = row.ActionValues,
                                Values = row.Values
                            };
                            _context.DecisionRows.Add(newRow);
                        }
                    }

                    var incomingRowIds = request.DecisionTable.Rows.Select(r => r.Id).ToList();
                    var existingRowIds = existingRows
                        .Where(r => r.TableId == request.DecisionTable.Id)
                        .ToList();

                    _context.DecisionRows.RemoveRange(
                        existingRowIds.Where(r => !incomingRowIds.Contains(r.Id))
                    );

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
        }
    }
}