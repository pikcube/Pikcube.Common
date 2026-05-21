using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines a model that listens for when a card is Entranced.
/// </summary>
public interface IOnEntrancedListener
{
    /// <summary>
    /// Invoked immediately after a card is Entranced.
    /// </summary>
    /// <param name="choiceContext">The choice context</param>
    /// <param name="card">The card that was entranced.</param>
    Task AfterCardEntrancedAsync(PlayerChoiceContext choiceContext, CardModel card);
}