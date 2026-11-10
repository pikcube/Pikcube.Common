using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches;

[HarmonyPatch]
internal class AfterPowerRemovedPatch
{
    [UsedImplicitly]
    static IEnumerable<MethodBase> TargetMethods() =>
        PatchingUtilities.TargetMethodOverrides(typeof(PowerModel), nameof(PowerModel.AfterRemoved));

    [UsedImplicitly]
    static Task Postfix(Task __result, PowerModel __instance, Creature? oldOwner)
    {
        return BetterHooks.OnPowerRemvoedAsync(__result, __instance, oldOwner);
    }
}