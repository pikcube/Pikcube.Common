using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches;

[HarmonyPatch]
internal static class EventIsAllowedFilteringPatches
{
    [UsedImplicitly]
    static IEnumerable<MethodBase> TargetMethods() =>
        PatchingUtilities.TargetMethodOverrides(typeof(EventModel), nameof(EventModel.IsAllowed));

    [UsedImplicitly]
    static bool Postfix(bool __result, EventModel __instance, IRunState runState)
    {
        return __result && EventSpawnManager.CanEventSpawn(__instance, runState);
    }
}