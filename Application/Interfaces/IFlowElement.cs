using Application.Core;
using Newtonsoft.Json.Linq;

namespace Application.Interfaces
{
    public interface IFlowElement
    {
        Result<JObject> Execute(JObject data);
    }
}