using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches;

public static class CustomRunModifierPatches
{

    [HarmonyPatch(typeof(ModelDb), nameof(ModelDb.GoodModifiers), MethodType.Getter)]
    public static class GoodModifierPatches
    {
        [UsedImplicitly]
        public static IReadOnlyList<ModifierModel> Postfix(IReadOnlyList<ModifierModel> __result)
        {
            List<ModifierModel> allGood = [..__result, ..CustomRunManager.GetGoodModifiers()];

            return allGood.AsReadOnly();
        }
    }

    [HarmonyPatch(typeof(ModelDb), nameof(ModelDb.BadModifiers), MethodType.Getter)]
    public static class BadModifierPatches
    {
        [UsedImplicitly]
        public static IReadOnlyList<ModifierModel> Postfix(IReadOnlyList<ModifierModel> __result)
        {
            List<ModifierModel> allBad = [.. __result, .. CustomRunManager.GetBadModifiers()];

            return allBad.AsReadOnly();
        }
    }
}