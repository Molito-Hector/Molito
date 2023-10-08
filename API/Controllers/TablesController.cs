using Application.Actions;
using Application.Conditions;
using Application.Core;
using Application.DecisionTables;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class TablesController : BaseApiController
    {
        [Authorize]
        [HttpGet] //api/tables
        public async Task<IActionResult> GetTables([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpGet("{id}")] //api/tables/{id}
        public async Task<IActionResult> GetTable(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTable(DecisionTable table)
        {
            return HandleResult(await Mediator.Send(new Create.Command { DecisionTable = table }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditTable(Guid id, DecisionTable table)
        {
            table.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { DecisionTable = table }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpPost("{id}/populate")]
        public async Task<IActionResult> PopulateTable(DecisionTable table)
        {
            return HandleResult(await Mediator.Send(new Populate.Command { DecisionTable = table }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpPost("{id}/addColumn")]
        public async Task<IActionResult> addColumn(Condition condition, string predicate, Guid id)
        {
            ICollection<Condition> conditions = new List<Condition>
            {
                condition
            };
            return HandleResult(await Mediator.Send(new AddConditions.Command { Conditions = conditions, Predicate = predicate, RuleId = id }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpPost("{id}/addActionColumn")]
        public async Task<IActionResult> addActionColumn(Domain.Action action, string predicate, Guid id)
        {
            ICollection<Domain.Action> actions = new List<Domain.Action>
            {
                action
            };
            return HandleResult(await Mediator.Send(new AddActions.Command { Actions = actions, Predicate = predicate, RuleId = id }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpDelete("{id}/removeColumn")]
        public async Task<IActionResult> removeColumn(Guid id)
        {
            return HandleResult(await Mediator.Send(new RemoveCondition.Command { Id = id }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpDelete("{id}/removeActionColumn")]
        public async Task<IActionResult> removeActionColumn(Guid id)
        {
            return HandleResult(await Mediator.Send(new RemoveAction.Command { Id = id }));
        }

        [Authorize(Policy = "IsRuleProjectMember")]
        [HttpPut("{tableId}/actions/column")]
        public async Task<IActionResult> EditActionColumn(Guid tableId, Domain.Action action)
        {
            return HandleResult(await Mediator.Send(new EditActionColumn.Command { Action = action, TableId = tableId }));
        }
    }
}