using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Pikcube.Common.Utility;

[UsedImplicitly]
public static class SerializableCardExtensions
{
    private static bool IsEqual(SavedProperties? leftProps, SavedProperties? rightProps)
    {
        switch (leftCardProps: leftProps, rightCardProps: rightProps)
        {
            case (null, null):
                return true;
            case (null, not null):
            case (not null, null):
                return false;
            case (not null, not null):
                break;
        }

        return IsEqual(leftProps.cardArrays ?? [], rightProps.cardArrays ?? [], (list1, list2) =>
                   list1.Length == list2.Length && list1.Zip(list2).All(tuple => IsEqual(tuple.First, tuple.Second))) &&
               IsEqual(leftProps.cards ?? [], rightProps.cards ?? [], IsEqual) &&
               IsEqual(leftProps.bools ?? [], rightProps.bools ?? [], (b1, b2) => b1 == b2) &&
               IsEqual(leftProps.ints ?? [], rightProps.ints ?? [], (i1, i2) => i1 == i2) &&
               IsEqual(leftProps.intArrays ?? [], rightProps.intArrays ?? [], (int1, ints2) => int1.SequenceEqual(ints2)) &&
               IsEqual(leftProps.strings ?? [], rightProps.strings ?? [], (s1, s2) => s1 == s2) &&
               IsEqual(leftProps.modelIds ?? [], leftProps.modelIds ?? [], (m1, m2) => m1 == m2);
    }

    private static bool IsEqual<T>(IList<SavedProperties.SavedProperty<T>> propsListL,
        IList<SavedProperties.SavedProperty<T>> propsListR, Func<T, T, bool> comp)
    {
        return propsListL.Select(propL => (propL, propR: propsListR.FirstOrDefault(propR => propR.name == propL.name)))
            .All(tuple => tuple.propL.name == tuple.propR.name && comp(tuple.propL.value, tuple.propR.value));
    }

    private static bool IsEqual(SerializableCard leftCard, SerializableCard rightCard)
    {
        return leftCard.Id == rightCard.Id &&
               IsEqual(leftCard.Enchantment, rightCard.Enchantment) &&
               leftCard.CurrentUpgradeLevel == rightCard.CurrentUpgradeLevel &&
               leftCard.FloorAddedToDeck == rightCard.FloorAddedToDeck &&
               IsEqual(leftCard.Props, rightCard.Props);
    }


    private static bool IsEqual(SerializableEnchantment? leftCardEnchantment, SerializableEnchantment? rightCardEnchantment)
    {
        return leftCardEnchantment?.Id == rightCardEnchantment?.Id &&
               leftCardEnchantment?.Amount == rightCardEnchantment?.Amount &&
               IsEqual(leftCardEnchantment?.Props, rightCardEnchantment?.Props);
    }

    private static bool IsEqual(CardModel leftCard, SerializableCard rightCard)
    {
        return rightCard.Id == leftCard.Id &&
               rightCard.CurrentUpgradeLevel == leftCard.CurrentUpgradeLevel &&
               rightCard.FloorAddedToDeck == leftCard.FloorAddedToDeck &&
               IsEqual(rightCard.Enchantment, leftCard.Enchantment) &&
               IsEqual(rightCard.Props, SavedProperties.From(leftCard));
    }

    private static bool IsEqual(EnchantmentModel? leftCardEnchantment, SerializableEnchantment? rightCardEnchantment)
    {
        return leftCardEnchantment switch
        {
            null when rightCardEnchantment is null => true,
            null => false,
            _ => leftCardEnchantment.Id == rightCardEnchantment?.Id &&
                 leftCardEnchantment.Amount == rightCardEnchantment.Amount &&
                 IsEqual(SavedProperties.From(leftCardEnchantment), rightCardEnchantment.Props)
        };
    }

    private static bool IsEqual(SerializableEnchantment? leftCardEnchantment, EnchantmentModel? rightCardEnchantment) =>
        IsEqual(rightCardEnchantment, leftCardEnchantment);

    public static bool IsEqual(SerializableCard leftCard, CardModel rightCard) => IsEqual(rightCard, leftCard);

    extension<T>(T cardBlueprint) where T : SerializableCard
    {
        public CardModel CreateNewInstance(Player p)
        {
            if (cardBlueprint.Id is null)
            {
                throw new ArgumentException("Id cannot be null");
            }

            CardModel card = p.RunState.CreateCard(ModelDb.GetById<CardModel>(cardBlueprint.Id).CanonicalInstance, p);

            for (int n = 0; n < cardBlueprint.CurrentUpgradeLevel; ++n)
            {
                card.UpgradeInternal();
                card.FinalizeUpgradeInternal();
            }

            cardBlueprint.Props?.Fill(card);

            if (cardBlueprint.Enchantment is null)
            {
                return card;
            }

            card.EnchantInternal(EnchantmentModel.FromSerializable(cardBlueprint.Enchantment), cardBlueprint.Enchantment.Amount);
            card.Enchantment?.ModifyCard();
            card.FinalizeUpgradeInternal();

            return card;
        }

        [UsedImplicitly]
        public CardModel LoadCardFrom(CardPile pile)
        {
            return pile.Cards.First(c => ComparisonHelper.IsEqual(c, cardBlueprint));
        }

        [UsedImplicitly]
        public CardModel? LoadCardFromOrDefault(CardPile pile)
        {
            return pile.Cards.FirstOrDefault(c => ComparisonHelper.IsEqual(c, cardBlueprint));
        }

        [UsedImplicitly]
        public static List<CardModel> LoadCardsFromPile(IEnumerable<SerializableCard> cardBlueprints, CardPile pile)
        {
            List<CardModel> cards = [];

            foreach (SerializableCard blueprint in cardBlueprints)
            {
                cards.Add(pile.Cards.Where(c => !cards.Contains(c))
                    .First(c => ComparisonHelper.IsEqual(c, blueprint)));
            }

            return cards;
        }

        [UsedImplicitly]
        public static List<CardModel> LoadCardsFromPileIfExist(IEnumerable<SerializableCard> cardBlueprints, CardPile pile)
        {
            List<CardModel> cards = [];

            foreach (SerializableCard blueprint in cardBlueprints)
            {
                CardModel? cardModel = pile.Cards.Where(c => !cards.Contains(c))
                    .FirstOrDefault(c => ComparisonHelper.IsEqual(c, blueprint));
                if (cardModel is null)
                {
                    continue;
                }
                cards.Add(cardModel);
            }

            return cards;
        }

    }
}