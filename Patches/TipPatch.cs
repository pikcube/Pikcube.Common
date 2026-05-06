using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using System.Collections.ObjectModel;
using MegaCrit.Sts2.Core.Hooks;
using Pikcube.Common.Patches;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches
{
    /// <summary>
    /// Harmony patch for adding additional HoverTips to existing cards.
    /// </summary>
    [HarmonyPatch(typeof(CardModel), "HoverTips", MethodType.Getter)]
    internal static class TipPatch
    {

        [UsedImplicitly]
        internal static IEnumerable<IHoverTip> Postfix(IEnumerable<IHoverTip> __result, CardModel __instance)
        {
            return BetterHooks.OnModifyCardHoverTips(__instance, __result);
        }
    }
}