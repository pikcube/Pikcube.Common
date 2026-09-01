using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Extensions;

namespace Pikcube.Common.Utility;

/// <summary>
/// Basic implementation of an End Turn Ping State Machine.<br/>
/// Entries take the form of `banter.alive.endTurnPing.X`, where X is the index.
/// </summary>
public class SimpleEndTurnPingMachine : ICustomEndTurnPingMachine
{
    private Dictionary<string, int> IndexDict { get; set; } = [];

    private DynamicVarSet DynamicVars { get; init; }

    /// <summary>
    /// Basic implementation of an End Turn Ping State Machine.<br/>
    /// Entries take the form of `banter.alive.endTurnPing.X`, where X is the index.
    /// </summary>
    /// <param name="maintainStateBetweenFloors">True if dialog should reset to entry 0 at the start of the floor.</param>
    /// <param name="canonicalVars">Any dynamic vars that are required for the loc string.</param>
    public SimpleEndTurnPingMachine(bool maintainStateBetweenFloors, params IEnumerable<DynamicVar> canonicalVars)
    {
        if (!maintainStateBetweenFloors)
        {
            RunManager.Instance.RoomEntered += Instance_RoomEntered;
        }

        DynamicVars = new DynamicVarSet(canonicalVars);
    }

    /// <summary>
    /// Cleans up RoomEntered hook that handles resetting state.
    /// </summary>
    ~SimpleEndTurnPingMachine()
    {
        RunManager.Instance.RoomEntered -= Instance_RoomEntered;
    }

    private void Instance_RoomEntered()
    {
        foreach (string key in IndexDict.Keys.ToArray())
        {
            IndexDict[key] = -1;
        }
    }

    /// <inheritdoc />
    public LocString GetNext(Player _, string table, string key)
    {
        IndexDict.TryAdd(key, -1);
        ++IndexDict[key];

        LocString? current = LocString.GetIfExists(table, $"{key}.{IndexDict[key]}");
        if (current is not null)
        {
            return current.WithDynamicVars(DynamicVars);
        }

        IndexDict[key] = 0;
        return (LocString.GetIfExists(table, $"{key}.{IndexDict[key]}") ?? new LocString(table, key))
            .WithDynamicVars(DynamicVars);
    }
}