namespace Application.Interfaces.Strategies
{
    public class ActionStrategyFactory
    {
        private readonly Dictionary<string, IActionStrategy> _strategies;
        public ActionStrategyFactory(IEnumerable<IActionStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(
                s => s.ModificationType,
                s => s,
                StringComparer.OrdinalIgnoreCase);

        }
        public IActionStrategy CreateStrategy(string modificationType)
        {
            if (!_strategies.TryGetValue(modificationType, out var strategy))
            {
                throw new ArgumentException($"Invalid modification type {modificationType}.");
            }

            return strategy;
        }
    }
}