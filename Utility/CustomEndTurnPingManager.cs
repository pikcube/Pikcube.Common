using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;

namespace Pikcube.Common.Utility;

internal static class CustomEndTurnPingManager
{ 
    private static ConditionalWeakTable<Player, ICustomEndTurnPingMachine> Machines { get; set; } = [];

    internal static LocString GetEndTurnPingLocString(string table, string key, Player player)
    {
        if (player.Character is not ICustomEndTurnCharacter customEndTurnCharacter)
        {
            return new LocString(table, key);
        }

        if (Machines.TryGetValue(player, out ICustomEndTurnPingMachine? machine))
        {
            return machine.GetNext(table, key);
        }

        ICustomEndTurnPingMachine newMachine = customEndTurnCharacter.Create(player);
        Machines.Add(player, newMachine);
        return newMachine.GetNext(table, key);
    }
}