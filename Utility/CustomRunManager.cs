using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Utility;

/// <summary>
/// Static class for adding custom run modifiers to the modifier list.
/// </summary>
public static class CustomRunManager
{
    private static List<Type> AdditionalGoodModifiers { get; } = [];
    private static List<Type> AdditionalBadModifiers { get; } = [];

    /// <summary>
    /// Add a modifier to the Custom Run Menu.
    /// </summary>
    /// <param name="runType">The type of run modifier (Good for Green, Bad for Red).</param>
    /// <typeparam name="T">The modifier to add.</typeparam>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the CustomRunType is out is not one of Good or Bad.</exception>
    public static void Register<T>(CustomRunType runType) where T : ModifierModel
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

    internal static IEnumerable<ModifierModel> GetGoodModifiers()
    {
        return AdditionalGoodModifiers.Select(t => ModelDb.GetById<ModifierModel>(ModelDb.GetId(t)));
    }

    internal static IEnumerable<ModifierModel> GetBadModifiers()
    {
        return AdditionalBadModifiers.Select(t => ModelDb.GetById<ModifierModel>(ModelDb.GetId(t)));
    }
}

/// <summary>
/// Defines whether a CustomRunModifier should be in the Green Modifier list or the Red Modifier list.
/// </summary>
public enum CustomRunType
{
    /// <summary>
    /// Green Modifier.
    /// </summary>
    Good = 1,
    /// <summary>
    /// Red Modifier.
    /// </summary>
    Bad = 2,
}