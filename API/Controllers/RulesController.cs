using Application.Core;
using Application.Rules;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    public class RulesController : BaseApiController
    {
        [HttpGet] //api/rules
        public async Task<IActionResult> GetRules([FromQuery] PagingParams param)
        {
            return HandlePagedResult(await Mediator.Send(new List.Query { Params = param }));
        }

        [HttpGet("{id}")] //api/rules/{id}
        public async Task<IActionResult> GetRule(Guid id)
        {
            return HandleResult(await Mediator.Send(new Details.Query { Id = id }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRule(Rule rule)
        {
            return HandleResult(await Mediator.Send(new Create.Command { Rule = rule }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditRule(Guid id, Rule rule)
        {
            rule.Id = id;
            return HandleResult(await Mediator.Send(new Edit.Command { Rule = rule }));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRule(Guid id)
        {
            return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
        }
    }
}