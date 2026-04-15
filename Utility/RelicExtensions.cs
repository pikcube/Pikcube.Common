using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using JetBrains.Annotations;

namespace Pikcube.Common.Utility;

public static class RelicExtensions
{
    extension<T>(T relic) where T : RelicModel
    {
        [UsedImplicitly]
        public void UpgradeValidCards(IEnumerable<CardCreationResult> cards, Predicate<CardModel> filter)
        {
            foreach (CardCreationResult cardCreationResult in cards.Where(c => c.Card.IsUpgradable && filter(c.Card)))
            {
                CardModel card = relic.Owner.RunState.CloneCard(cardCreationResult.Card);
                CardCmd.Upgrade(card);
                cardCreationResult.ModifyCard(card, relic);
            }
        }

        public static T Canonical => (T)ModelDb.Relic<T>().CanonicalInstance;
    }
}