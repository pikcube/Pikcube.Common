using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Pikcube.Common.Utility;

public static class ComparisonHelper
{
    public static bool IsEqual(SavedProperties? leftProps, SavedProperties? rightProps)
    {
        return (leftCardProps: leftProps, rightCardProps: rightProps) switch
        {
            (null, null) => true,
            (null, not null) or (not null, null) => false,
            (not null, not null) =>
                IsEqual(leftProps.cardArrays, rightProps.cardArrays,
                    (list1, list2) => list1.Length == list2.Length &&
                                      list1.Zip(list2).All(tuple => IsEqual(tuple.First, tuple.Second))) &&
                IsEqual(leftProps.cards, rightProps.cards, IsEqual) &&
                IsEqual(leftProps.bools, rightProps.bools, (b1, b2) => b1 == b2) &&
                IsEqual(leftProps.ints, rightProps.ints, (i1, i2) => i1 == i2) &&
                IsEqual(leftProps.intArrays, rightProps.intArrays, (int1, ints2) => int1.SequenceEqual(ints2)) &&
                IsEqual(leftProps.strings, rightProps.strings, (s1, s2) => s1 == s2) &&
                IsEqual(leftProps.modelIds, leftProps.modelIds, (m1, m2) => m1 == m2)
        };
    }

    private static bool IsEqual<T>(IList<SavedProperties.SavedProperty<T>>? propsListL,
        IList<SavedProperties.SavedProperty<T>>? propsListR, Func<T, T, bool> comp)
    {
        return (propsListL, propsListR) switch
        {
            (null, null) => true,
            (null, not null) or (not null, null) => false,
            (not null, not null) => propsListL.Count == propsListR.Count && 
                                    propsListL
                                        .Select(propL => 
                                            (propL, propR: propsListR.FirstOrDefault(propR => propR.name == propL.name)))
                                        .All(tuple => tuple.propL.name == tuple.propR.name && 
                                                      comp(tuple.propL.value, tuple.propR.value))
        };
    }

    public static bool IsEqual(SerializableCard leftCard, SerializableCard rightCard)
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

    public static bool IsEqual(CardModel leftCard, SerializableCard rightCard)
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
}