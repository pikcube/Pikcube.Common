using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Code.Utility;

namespace Pikcube.Common.Code.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.IsAllowedInRun))]
internal static class IsCardAllowedInRunPatches
{
    [HarmonyPostfix]
    internal static bool ModifyIsAllowed(bool __result, CardModel __instance, IRunState runState)
    {
        return __result && BetterHooks.OnModifyIsCardAllowed(runState, __instance);
    }
}