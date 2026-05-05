using System.Data;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Abstracts;

namespace Pikcube.Common.Utility;

/// <summary>
/// Static class for adding custom run modifiers to the modifier list.
/// </summary>
public static class CustomRunManager
{
    private static HashSet<Type> AdditionalGoodModifiers { get; } = [];
    private static HashSet<Type> AdditionalBadModifiers { get; } = [];

    internal static bool IsLocked { get; set; } = false;

    /// <summary>
    /// Add a modifier to the Custom Run Menu.
    /// </summary>
    /// <param name="runType">The type of run modifier (Good for Green, Bad for Red).</param>
    /// <typeparam name="T">The modifier to add.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the CustomRunType is out is not one of Good or Bad.</exception>
    /// <exception cref="ReadOnlyException">Throw if a modifier is registered after the cache is initialized.</exception>
    [Obsolete($"Inheriting modifiers from {nameof(CustomRunModifierModel)} is preferred due to auto add. Manual registration will be remove in the next major version.", false)]
    public static void Register<T>(CustomRunType runType) where T : ModifierModel
    {
        if (IsLocked)
        {
            throw new ReadOnlyException(
                "Modifier cache has been initialized, no additional modifiers may be registered");
        }
        switch (runType)
        {
            case CustomRunType.Good:
                AdditionalGoodModifiers.Add(typeof(T));
                break;
            case CustomRunType.Bad:
                AdditionalBadModifiers.Add(typeof(T));
                break;
            case CustomRunType.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(runType), runType, null);
        }
    }

    internal static void RegisterInternal(CustomRunModifierModel modifier)
    {
        if (IsLocked)
        {
            throw new ReadOnlyException(
                "Modifier cache has been initialized, no additional modifiers may be registered");
        }

        switch (modifier.RunType)
        {
            case CustomRunType.Good:
                AdditionalGoodModifiers.Add(modifier.GetType());
                break;
            case CustomRunType.Bad:
                AdditionalBadModifiers.Add(modifier.GetType());
                break;
            case CustomRunType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(modifier), modifier.RunType, "CustomRunType must be a valid value");
        }
    }

    internal static List<ModifierModel> GetGoodModifiers(IReadOnlyList<ModifierModel> baseGameModifiers)
    {
        return GetModifiers(baseGameModifiers, AdditionalGoodModifiers);
    }

    internal static List<ModifierModel> GetBadModifiers(IReadOnlyList<ModifierModel> baseGameModifiers)
    {
        return GetModifiers(baseGameModifiers, AdditionalBadModifiers);
    }

    private static List<ModifierModel> GetModifiers(IReadOnlyList<ModifierModel> baseGameModifiers, HashSet<Type> additionalModifiers)
    {
        IsLocked = true;
        List<ModifierModel> result = [];
        IEnumerable<ModifierModel> legacy = [];
        Dictionary<ModifierPriority, List<CustomRunModifierModel>> customs = [];

        foreach (IGrouping<bool, ModifierModel> group in additionalModifiers.Select(ModelDb.GetModel<ModifierModel>)
                     .GroupBy(modifier => modifier is CustomRunModifierModel))
        {
            if (!group.Key)
            {
                legacy = group;
            }

            customs = group
                .OfType<CustomRunModifierModel>()
                .GroupBy(custom => custom.Info.Priority)
                .OrderBy(subGroup => subGroup.Key)
                .ToDictionary(g => g.Key, 
                    g => g
                        .OrderBy(c => c.Info.Primary)
                        .ThenBy(c => c.Info.Secondary)
                        .ToList());
        }

        if (customs.TryGetValue(ModifierPriority.Immediate, out List<CustomRunModifierModel>? list1))
        {
            result.AddRange(list1);
        }

        if (customs.TryGetValue(ModifierPriority.PrefixSegmented, out List<CustomRunModifierModel>? list2))
        {
            result.AddRange(list2);
        }

        if (customs.TryGetValue(ModifierPriority.PrefixGeneric, out List<CustomRunModifierModel>? list3))
        {
            result.AddRange(list3);
        }

        result.AddRange(baseGameModifiers);

        if (customs.TryGetValue(ModifierPriority.PostfixSegmented, out List<CustomRunModifierModel>? list4))
        {
            result.AddRange(list4);
        }

        if (customs.TryGetValue(ModifierPriority.PostfixGeneric, out List<CustomRunModifierModel>? list5))
        {
            result.AddRange(list5);
        }

        result.AddRange(legacy);

        return result;
    }
}

/// <summary>
/// Defines whether a CustomRunModifier should be in the Green Modifier list or the Red Modifier list.
/// </summary>
public enum CustomRunType
{
    /// <summary>
    /// Do not include modifier in list
    /// </summary>
    None = 0,
    /// <summary>
    /// Green Modifier.
    /// </summary>
    Good = 1,
    /// <summary>
    /// Red Modifier.
    /// </summary>
    Bad = 2,
}