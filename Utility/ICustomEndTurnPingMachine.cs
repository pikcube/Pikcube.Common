using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines a state machine that determines which end of turn ping to display in combat.
/// </summary>
public interface ICustomEndTurnPingMachine
{
    /// <summary>
    /// Get the next end of turn string.
    /// </summary>
    /// <param name="player">The player to generate dialog for.</param>
    /// <param name="table">The table the string is stored in.</param>
    /// <param name="key">The key being fetched.</param>
    /// <returns>The <see cref="LocString"/> to display to the player.</returns>
    public LocString GetNext(Player player, string table, string key);
}