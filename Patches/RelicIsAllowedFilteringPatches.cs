using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Utility;
using System.Reflection;

namespace Pikcube.Common.Patches;

[HarmonyPatch]
internal class RelicIsAllowedFilteringPatches
{
    static IEnumerable<MethodBase> TargetMethods()
    {
        MethodInfo? onPlayMethod = typeof(RelicModel).GetMethod(nameof(RelicModel.IsAllowed));
        if (onPlayMethod is not null)
        {
            yield return onPlayMethod;
        }

        foreach (Type? type in AccessTools.AllTypes()
                     .Where(t => t.IsSubclassOf(typeof(CardModel)) && !t.IsAbstract))
        {
            MethodInfo? method = type.GetMethod(nameof(RelicModel.IsAllowed),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (method != null && method.DeclaringType != typeof(CardModel))
            {
                yield return method;
            }
        }
    }

    static bool Postfix(bool __result, RelicModel __instance, IRunState runState)
    {
        return __result && RelicSpawnManager.CanRelicSpawn(__instance, runState);
    }
}