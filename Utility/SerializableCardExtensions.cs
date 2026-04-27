using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Pikcube.Common.Utility;

/// <summary>
/// Defines an extension block that adds methods to SerializableCards
/// </summary>
[UsedImplicitly]
public static class SerializableCardExtensions
{
    extension<T>(T cardBlueprint) where T : SerializableCard
    {
        /// <summary>
        /// Create an instance of the SerializableCard owned by the player.
        /// </summary>
        /// <param name="player">The player who will own this card.</param>
        /// <returns>A CardModel instance owned by the player with all properties set.</returns>
        /// <exception cref="ArgumentException">Tried to pass a SerializableCard without an Id</exception>
        public CardModel CreateNewInstance(Player player)
        {
            if (cardBlueprint.Id is null)
            {
                throw new ArgumentException("Id cannot be null");
            }

            CardModel card = player.RunState.CreateCard(ModelDb.GetById<CardModel>(cardBlueprint.Id).CanonicalInstance, player);

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

        /// <summary>
        /// Find the first CardModel in a pile that is equal by value to the SerializeableCard. Throws an exception if the card is not found.
        /// </summary>
        /// <param name="pile">The pile containing the cards.</param>
        /// <returns>A CardModel instance within the specified pile.</returns>
        [UsedImplicitly]
        public CardModel LoadCardFrom(CardPile pile)
        {
            return pile.Cards.First(c => IsEqual(c, cardBlueprint));
        }

        /// <summary>
        /// Find the first CardModel in a pile that is equal by value to the SerializeableCard. Returns null if not found.
        /// </summary>
        /// <param name="pile">The pile containing the cards.</param>
        /// <returns>A CardModel instance within the specified pile or null if none was found.</returns>
        [UsedImplicitly]
        public CardModel? LoadCardFromOrDefault(CardPile pile)
        {
            return pile.Cards.FirstOrDefault(c => IsEqual(c, cardBlueprint));
        }

        /// <summary>
        /// Find the first CardModel instances in a pile that are equal by value to the SerializableCard instances. Throws an exception if not all cards are found.
        /// </summary>
        /// <param name="serializableCards">The cards to find.</param>
        /// <param name="pile">The pile containing the cards.</param>
        /// <returns>A list of CardModels containing the matching cards in the pile.</returns>
        [UsedImplicitly]
        public static List<CardModel> LoadCardsFromPile(IEnumerable<SerializableCard> serializableCards, CardPile pile)
        {
            List<CardModel> cards = [];

            foreach (SerializableCard blueprint in serializableCards)
            {
                cards.Add(pile.Cards.Where(c => !cards.Contains(c))
                    .First(c => IsEqual(c, blueprint)));
            }

            return cards;
        }

        /// <summary>
        /// Find the first CardModel instances in a pile that are equal by value to the SerializableCard instances if they exist.
        /// </summary>
        /// <param name="serializableCards">The cards to find.</param>
        /// <param name="pile">The pile containing the cards.</param>
        /// <returns>A list of CardModels containing the matching cards in the pile. Cards not found are omitted.</returns>
        [UsedImplicitly]
        public static List<CardModel> LoadCardsFromPileIfExist(IEnumerable<SerializableCard> serializableCards, CardPile pile)
        {
            List<CardModel> cards = [];

            foreach (SerializableCard blueprint in serializableCards)
            {
                CardModel? cardModel = pile.Cards.Where(c => !cards.Contains(c))
                    .FirstOrDefault(c => IsEqual(c, blueprint));
                if (cardModel is null)
                {
                    continue;
                }
                cards.Add(cardModel);
            }

            return cards;
        }

    }

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

    private static bool IsEqual(SerializableCard leftCard, CardModel rightCard) => IsEqual(rightCard, leftCard);
}