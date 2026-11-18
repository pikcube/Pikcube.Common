using MegaCrit.Sts2.Core.Runs;

namespace Pikcube.Common.Code.Utility;

/// <summary>
/// Modify whether or not a card is allowed to spawn given the runstate.
/// </summary>
public interface IModifyCardIsAllowedListener
{
    /// <summary>
    /// Modifies whether or not a card is allowed to spawn given the runstate.
    /// </summary>
    /// <param name="runState">The current runstate.</param>
    /// <returns>True if the card is allowed, false otherwise.</returns>
    public bool IsAllowed(IRunState runState);
}