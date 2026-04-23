using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Pikcube.Common.Utility;

[UsedImplicitly]
public class RelicSpawnManager
{
    private static Dictionary<Type, Dictionary<RelicSpawnManager, Predicate<IRunState>>> RelicRules { get; } = [];

    internal static bool CanRelicSpawn(RelicModel canidate, IRunState runState)
    {
        if (RelicRules.TryGetValue(canidate.GetType(), out Dictionary<RelicSpawnManager, Predicate<IRunState>>? rule))
        {
            return rule.Values.All(p => p(runState));
        }

        return true;
    }

    public void RegisterRule<T>(Predicate<IRunState> rule) where T : RelicModel
    {
        RelicRules.TryAdd(typeof(T), []);

        Dictionary<RelicSpawnManager, Predicate<IRunState>> ruleDic = RelicRules[typeof(T)];
        ruleDic[this] = rule;
    }

    public void DeregisterRuleIfExist<T>() where T : RelicModel
    {
        if (!RelicRules.ContainsKey(typeof(T)))
        {
            return;
        }

        Dictionary<RelicSpawnManager, Predicate<IRunState>> ruleDic = RelicRules[typeof(T)];
        ruleDic.Remove(this);

    }
}