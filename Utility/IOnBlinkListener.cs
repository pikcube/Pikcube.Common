using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines a model that listens for when a card is Blinked.
/// </summary>
public interface IOnBlinkListener
{
    /// <summary>
    /// Invoked immediately after a card is Blinked.
    /// </summary>
    /// <param name="choiceContext">The choice context</param>
    /// <param name="card">The card that was blinked.</param>
    public Task AfterCardBlinkedAsync(PlayerChoiceContext choiceContext, CardModel card);
}