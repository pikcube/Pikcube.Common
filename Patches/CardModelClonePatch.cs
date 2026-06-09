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
    static IEnumerable<MethodBase> TargetMethods()
    {
        MethodInfo? onPlayMethod = typeof(CardModel).GetMethod(nameof(CardModel.MutableClone));
        if (onPlayMethod is not null)
        {
            yield return onPlayMethod;
        }

        foreach (Type? type in AccessTools.AllTypes()
                     .Where(t => t.IsSubclassOf(typeof(CardModel)) && !t.IsAbstract))
        {
            MethodInfo? method = type.GetMethod(nameof(CardModel.MutableClone),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (method != null && method.DeclaringType != typeof(CardModel))
            {
                yield return method;
            }
        }
    }

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