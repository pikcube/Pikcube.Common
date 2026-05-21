using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Utility;
using System.Reflection;

namespace Pikcube.Common.Patches;

[HarmonyPatch]
internal static class EventIsAllowedFilteringPatches
{
    [UsedImplicitly]
    static IEnumerable<MethodBase> TargetMethods()
    {
        MethodInfo? onPlayMethod = typeof(EventModel).GetMethod(nameof(EventModel.IsAllowed));
        if (onPlayMethod is not null)
        {
            yield return onPlayMethod;
        }

        foreach (Type? type in AccessTools.AllTypes()
                     .Where(t => t.IsSubclassOf(typeof(EventModel)) && !t.IsAbstract))
        {
            MethodInfo? method = type.GetMethod(nameof(EventModel.IsAllowed),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (method != null && method.DeclaringType != typeof(EventModel))
            {
                yield return method;
            }
        }
    }

    [UsedImplicitly]
    static bool Postfix(bool __result, EventModel __instance, IRunState runState)
    {
        return __result && EventSpawnManager.CanEventSpawn(__instance, runState);
    }
}