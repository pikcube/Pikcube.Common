using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using JetBrains.Annotations;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines an extension block on all types inheriting from RelicModel
/// </summary>
public static class RelicExtensions
{
    extension<T>(T relic) where T : RelicModel
    {
        /// <summary>
        /// Upgrade all cards in a card reward if the provided predicate is true. Flashes the relic when the card is revealed.
        /// </summary>
        /// <param name="cards">The cards to upgrade.</param>
        /// <param name="filter">A function that returns true if the card should be upgraded.</param>
        [UsedImplicitly]
        public void UpgradeValidCards(IEnumerable<CardCreationResult> cards, Predicate<CardModel>? filter = null)
        {
            filter ??= _ => true;
            foreach (CardCreationResult cardCreationResult in cards.Where(c => c.Card.IsUpgradable && filter(c.Card)))
            {
                CardModel card = relic.Owner.RunState.CloneCard(cardCreationResult.Card);
                CardCmd.Upgrade(card);
                cardCreationResult.ModifyCard(card, relic);
            }
        }

        /// <summary>
        /// Get the canonical instance of a relic.
        /// </summary>
        public static T Canonical => (T)ModelDb.Relic<T>().CanonicalInstance;
    }
}