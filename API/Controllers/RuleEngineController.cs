using Application.RuleEngine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace API.Controllers
{
    public class RuleEngineController : BaseApiController
    {
        [Authorize(Policy = "RuleExecutionPolicy")]
        [HttpPost("{id}/execute")]
        public async Task<IActionResult> ExecuteRule(Guid id, [FromBody] JObject data)
        {
            return HandleResult(await Mediator.Send(new Execute.Command { Id = id, Data = data }));
        }

        [Authorize(Policy = "RuleExecutionPolicy")]
        [HttpPost("{id}/executeTable")]
        public async Task<IActionResult> ExecuteTable(Guid id, [FromBody] JObject data)
        {
            return HandleResult(await Mediator.Send(new ExecuteTable.Command { Id = id, Data = data }));
        }
    }
}