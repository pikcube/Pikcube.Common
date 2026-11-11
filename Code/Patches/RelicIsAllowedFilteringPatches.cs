using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Code.Utility;

namespace Pikcube.Common.Code.Patches;

[HarmonyPatch]
internal static class RelicIsAllowedFilteringPatches
{
    [UsedImplicitly]
    static IEnumerable<MethodBase> TargetMethods() => PatchingUtilities.TargetMethodOverrides(typeof(RelicModel), nameof(RelicModel.IsAllowed));

    [UsedImplicitly]
    static bool Postfix(bool __result, RelicModel __instance, IRunState runState)
    {
        return __result && RelicSpawnManager.CanRelicSpawn(__instance, runState);
    }
}