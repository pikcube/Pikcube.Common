using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches;

[HarmonyPatch]
internal static class CardModelClonePatch
{
    [UsedImplicitly]
    static IEnumerable<MethodBase> TargetMethods() =>
        PatchingUtilities.TargetMethodOverrides(typeof(CardModel), nameof(CardModel.MutableClone));

    [UsedImplicitly]
    internal static void Postfix(AbstractModel __result, AbstractModel __instance)
    {
        if (__instance is not CardModel original || __result is not CardModel result)
        {
            return;
        }

        BetterHooks.OnCardCloned(original, result);
    }

}