using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Pikcube.Common.Utility;

/// <summary>
/// A collection of commonly used Predicates used with the RelicSpawnManager
/// </summary>
public static class Predicates
{
    /// <summary>
    /// True if the modifier is currently loaded, false otherwise.
    /// </summary>
    /// <param name="runState">The current runstate</param>
    /// <typeparam name="T">The modifier we are checking for.</typeparam>
    /// <returns>True if the modifier is present, false otherwise.</returns>
    public static bool IsModifierPresent<T>(IRunState runState) where T : ModifierModel
    {
        return runState.Modifiers.Any(m => m is T);
    }

    /// <summary>
    /// True unless the modifier is currently loaded, otherwise false.
    /// </summary>
    /// <param name="runState">The current runstate</param>
    /// <typeparam name="T">The modifier we are checking for.</typeparam>
    /// <returns>True if the modifier is not present, false otherwise.</returns>
    public static bool UnlessModifierPresent<T>(IRunState runState) where T : ModifierModel
    {
        return !IsModifierPresent<T>(runState);
    }
}