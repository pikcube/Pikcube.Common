using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Pikcube.Common.Utility;

/// <summary>
/// Class for adding dynamic filtering rules for which events can spawn. Useful for preventing specific events from spawning in Custom Runs. <br/>
/// Each EventSpawnManager instance should be context specific, as each EventSpawnManager can only register one rule per event.<br/>
/// If multiple functions are required, then they need to be merged into a single Predicate or multiple EventSpawnManager must be used.
/// </summary>
[UsedImplicitly]
public class EventSpawnManager
{
    private static Dictionary<Type, Dictionary<EventSpawnManager, Predicate<IRunState>>> EventRules { get; } = [];

    internal static bool CanEventSpawn(EventModel canidate, IRunState runState)
    {
        return !EventRules.TryGetValue(canidate.GetType(), out Dictionary<EventSpawnManager, Predicate<IRunState>>? rule) || rule.Values.All(p => p(runState));
    }


    /// <summary>
    /// Sets the rule for when this event is allowed to spawn.<br/>
    /// Throws an exception if a rule has already been registered for this event by this EventSpawnManager.
    /// </summary>
    /// <param name="rule">The predicate to invoke to determine if this event can spawn.</param>
    /// <typeparam name="T">The event this rule applies to.</typeparam>
    [UsedImplicitly]
    public void RegisterRule<T>(Predicate<IRunState> rule) where T : EventModel
    {
        if (!TryRegisterRule<T>(rule))
        {
            throw new Exception(
                "A rule for this Event has already been registered to the spawn manager. If multiple rules are necessary, either merge them into a single predicate or create a second EventSpawnManager.");
        }
    }

    /// <summary>
    /// Sets the rule for when this Event is allowed to spawn if no rule is already set.
    /// </summary>
    /// <param name="rule">The predicate to invoke to determine if this Event can spawn.</param>
    /// <typeparam name="T">The event this rule applies to.</typeparam>
    public bool TryRegisterRule<T>(Predicate<IRunState> rule) where T : EventModel
    {
        EventRules.TryAdd(typeof(T), []);

        Dictionary<EventSpawnManager, Predicate<IRunState>> ruleDic = EventRules[typeof(T)];
        if (ruleDic.ContainsKey(this))
        {
            return false;
        }
        ruleDic[this] = rule;
        return true;
    }

    /// <summary>
    /// Removes an existing rule registered by this spawn manager (if present). Does not affect rules registered by other EventSpawnManagers.
    /// </summary>
    /// <typeparam name="T">The event whose rule will be cleared.</typeparam>
    public void DeregisterRuleIfExist<T>() where T : EventModel
    {
        if (!EventRules.ContainsKey(typeof(T)))
        {
            return;
        }

        Dictionary<EventSpawnManager, Predicate<IRunState>> ruleDic = EventRules[typeof(T)];
        ruleDic.Remove(this);

    }
}