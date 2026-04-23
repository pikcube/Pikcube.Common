using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Pikcube.Common.Utility;

public static class Predicates
{
    public static bool IsModifierPresent<T>(IRunState runState) where T : ModifierModel
    {
        return runState.Modifiers.Any(m => m is T);
    }

    public static bool UnlessModifierPresent<T>(IRunState runState) where T : ModifierModel
    {
        return !IsModifierPresent<T>(runState);
    }
}