using Application.Core;
using Domain;
using Newtonsoft.Json.Linq;

namespace Application.Interfaces
{
    public interface IActionStrategy
    {
        string ModificationType { get; }
        Result<JObject> Execute(ActionDto action, Dictionary<string, object> inputData, JObject outputObject);
    }
}