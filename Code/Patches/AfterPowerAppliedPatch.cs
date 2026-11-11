using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Code.Utility;

namespace Pikcube.Common.Code.Patches;

[HarmonyPatch]
internal class AfterPowerAppliedPatch
{
    [UsedImplicitly]
    static IEnumerable<MethodBase> TargetMethods() =>
        PatchingUtilities.TargetMethodOverrides(typeof(PowerModel), nameof(PowerModel.AfterApplied));

    [UsedImplicitly]
    static Task Postfix(Task __result, PowerModel __instance, Creature? applier, CardModel? cardSource)
    {
        return BetterHooks.OnPowerAppliedAsync(__result, __instance, applier, cardSource);
    }
}