using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Pikcube.Common.Utility;

public static class Predicates
{
    public static Predicate<IRunState> IsModifierPresent<T>() where T : ModifierModel
    {
        return runState => runState.Modifiers.Any(m => m is T);
    }
}