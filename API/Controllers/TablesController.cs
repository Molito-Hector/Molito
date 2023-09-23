using Application.Conditions;
using Application.Core;
using Application.DecisionTables;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    public class TablesController : BaseApiController
    {
        [HttpGet] //api/tables
        public async Task<IActionResult> GetTables([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }

        [HttpGet("{id}")] //api/tables/{id}
        public async Task<IActionResult> GetTable(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTable(DecisionTable table)
        {
            return HandleResult(await Mediator.Send(new Create.Command { DecisionTable = table }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditTable(Guid id, DecisionTable table)
        {
            table.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { DecisionTable = table }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        [HttpPost("{id}/populate")]
        public async Task<IActionResult> PopulateTable(DecisionTable table)
        {
            return HandleResult(await Mediator.Send(new Populate.Command { DecisionTable = table }));
        }

        [HttpPost("{id}/addColumn")]
        public async Task<IActionResult> addColumn(Condition condition, string predicate, Guid id)
        {
            ICollection<Condition> conditions = new List<Condition>();
            conditions.Add(condition);
            return HandleResult(await Mediator.Send(new AddConditions.Command { Conditions = conditions, Predicate = predicate, RuleId = id }));
        }
    }
}