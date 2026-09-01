using MegaCrit.Sts2.Core.Entities.Players;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines a character that has an associated ICustomEndTurnCharacter
/// </summary>
public interface ICustomEndTurnCharacter
{
    /// <summary>
    /// Initialize a <see cref="ICustomEndTurnCharacter"/> for the run.
    /// </summary>
    /// <param name="player">The player we are creating the machine for.</param>
    /// <returns>An instance of <see cref="ICustomEndTurnCharacter"/></returns>
    public ICustomEndTurnPingMachine Create(Player player);
}