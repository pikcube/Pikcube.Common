using BaseLib.Abstracts;
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

    /// <summary>
    /// Add a modifier to the Custom Run Menu.
    /// </summary>
    /// <param name="runType">The type of run modifier (Good for Green, Bad for Red).</param>
    /// <typeparam name="T">The modifier to add.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the CustomRunType is out is not one of Good or Bad.</exception>
    public static void Register<T>(CustomRunType runType) where T : CustomRunModifierModel
    {
        switch (runType)
        {
            case CustomRunType.Good:
                AdditionalGoodModifiers.Add(typeof(T));
                break;
            case CustomRunType.Bad:
                AdditionalBadModifiers.Add(typeof(T));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(runType), runType, null);
        }
    }

    internal static void RegisterInteranl(CustomRunModifierModel modifier)
    {
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
                throw new ArgumentOutOfRangeException(nameof(modifier), modifier.RunType, null);
        }
    }

    internal static IEnumerable<ModifierModel> GetGoodModifiers(IReadOnlyList<ModifierModel> baseGameModifiers)
    {
        bool isBaseGameItterated = false;
        foreach (ModifierModel m in AdditionalGoodModifiers.Select(t => ModelDb.GetById<ModifierModel>(ModelDb.GetId(t))))
        {
            if (m is not CustomRunModifierModel custom)
            {
                continue;
            }

            if (isBaseGameItterated || (int)custom.Info.Priority < 3)
            {
                yield return m;
                continue;
            }

            foreach (ModifierModel baseModel in baseGameModifiers)
            {
                yield return baseModel;
            }

            isBaseGameItterated = true;

            yield return m;
        }
    }

    internal static IEnumerable<ModifierModel> GetBadModifiers(IReadOnlyList<ModifierModel> baseGameModifiers)
    {
        bool isBaseGameItterated = false;
        foreach (ModifierModel m in AdditionalBadModifiers.Select(t => ModelDb.GetById<ModifierModel>(ModelDb.GetId(t))))
        {
            if (m is not CustomRunModifierModel custom)
            {
                continue;
            }

            if (isBaseGameItterated || custom.Info.Priority is not ModifierPriority.PostfixGeneric and ModifierPriority.PostfixSegmented)
            {
                yield return m;
                continue;
            }

            foreach (ModifierModel baseModel in baseGameModifiers)
            {
                yield return baseModel;
            }

            isBaseGameItterated = true;

            yield return m;
        }
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