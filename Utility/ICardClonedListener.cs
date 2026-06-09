using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines a model that listens for when a CardModel is duplicated.
/// </summary>
public interface ICardClonedListener
{
    /// <summary>
    /// Invoked immediately after a CardModel is duplicated
    /// </summary>
    /// <param name="original">The original card model.</param>
    /// <param name="clone">The copied card model.</param>
    public void AfterCardCloned(CardModel original, CardModel clone);
}