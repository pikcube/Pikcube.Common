using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines a model that adds keywords using the <see cref="AbstractModel.TryModifyKeywordsInCombat"/>, for which some or all should be purple./>
/// </summary>
public interface IPurpleKeywordModifier
{
    /// <summary>
    /// The set of CardKeywords that should render purple if added by any model.
    /// </summary>
    public ISet<CardKeyword> PurpleKeywords { get; }

    /// <summary>
    /// Add or remove keywords on a card during combat. This is the global counterpart to the local keywords stored on
    /// the card itself (see <see cref="T:MegaCrit.Sts2.Core.Entities.Cards.KeywordSources" />), and works just like <see cref="MegaCrit.Sts2.Core.Models.AbstractModel.TryModifyEnergyCostInCombat" />
    /// does for energy cost: a model contributes keywords for as long as it exists, with no explicit cleanup needed when
    /// it's removed.
    /// This will never be called while outside of combat.
    /// Mutate <paramref name="keywords" /> in place (add or remove).
    /// </summary>
    /// <param name="card">Card whose keywords we're modifying.</param>
    /// <param name="keywords">The card's current keyword set, to be mutated in place.</param>
    /// <returns>Whether the keyword set was modified.</returns>
    public bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords);
}