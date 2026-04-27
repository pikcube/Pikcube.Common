using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Pikcube.Common.Utility;

/// <summary>
/// Class for adding dynamic filtering rules for relics rewards. Useful for preventing specific relics from spawning in Custom Runs. <br/>
/// Only prevents relics from spawning from the RelicGrabBag. Event and Ancient relics are unaffected. <br/>
/// Each RelicSpawnManager instance should be context specific, as each RelicSpawnManager can only register one rule per relic.<br/>
/// If multiple functions are required, then they need to be merged into a single Predicate or multiple RelicSpawnManagers must be used.
/// </summary>
[UsedImplicitly]
public class RelicSpawnManager
{
    private static Dictionary<Type, Dictionary<RelicSpawnManager, Predicate<IRunState>>> RelicRules { get; } = [];

    internal static bool CanRelicSpawn(RelicModel canidate, IRunState runState)
    {
        return !RelicRules.TryGetValue(canidate.GetType(), out Dictionary<RelicSpawnManager, Predicate<IRunState>>? rule) || rule.Values.All(p => p(runState));
    }

    /// <summary>
    /// Sets the rule for when this relic is allowed to spawn from the RelicGrabBag.<br/>
    /// Throws an exception if a rule has already been registered for this relic by this RelicSpawnManager.
    /// </summary>
    /// <param name="rule">The predicate to invoke to determine if this relic can spawn.</param>
    /// <typeparam name="T">The relic this rule applies to.</typeparam>
    [UsedImplicitly]
    public void RegisterRule<T>(Predicate<IRunState> rule) where T : RelicModel
    {
        if (!TryRegisterRule<T>(rule))
        {
            throw new Exception(
                "A rule for this relic has already been registered to the spawn manager. If multiple rules are necessary, either merge them into a single predicate or create a second RelicSpawnManager.");
        }
    }

    /// <summary>
    /// Sets the rule for when this relic is allowed to spawn from the RelicGrabBag if no rule is already set.
    /// </summary>
    /// <param name="rule">The predicate to invoke to determine if this relic can spawn.</param>
    /// <typeparam name="T">The relic this rule applies to.</typeparam>
    public bool TryRegisterRule<T>(Predicate<IRunState> rule) where T : RelicModel
    {
        RelicRules.TryAdd(typeof(T), []);

        Dictionary<RelicSpawnManager, Predicate<IRunState>> ruleDic = RelicRules[typeof(T)];
        if (ruleDic.ContainsKey(this))
        {
            return false;
        }
        ruleDic[this] = rule;
        return true;
    }

    /// <summary>
    /// Removes an existing rule registered by this spawn manager (if present). Does not affect rules registered by other RelicSpawnManagers.
    /// </summary>
    /// <typeparam name="T">The relic whose rule will be cleared.</typeparam>
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