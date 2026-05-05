using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches;

internal static class CustomRunModifierPatches
{

    [HarmonyPatch(typeof(ModelDb), nameof(ModelDb.GoodModifiers), MethodType.Getter)]
    internal static class GoodModifierPatches
    {
        private static IReadOnlyList<ModifierModel>? _cached;
        [UsedImplicitly]
        public static IReadOnlyList<ModifierModel> Postfix(IReadOnlyList<ModifierModel> __result)
        {
            if (_cached is not null)
            {
                return _cached;
            }
            List<ModifierModel> allGood = CustomRunManager.GetGoodModifiers(__result);
            _cached = allGood.AsReadOnly();

            return _cached;
        }
    }

    [HarmonyPatch(typeof(ModelDb), nameof(ModelDb.BadModifiers), MethodType.Getter)]
    internal static class BadModifierPatches
    {
        private static IReadOnlyList<ModifierModel>? _cached;
        [UsedImplicitly]
        public static IReadOnlyList<ModifierModel> Postfix(IReadOnlyList<ModifierModel> __result)
        {
            if (_cached is not null)
            {
                return _cached;
            }
            List<ModifierModel> allBad = CustomRunManager.GetBadModifiers(__result);

            _cached = allBad.AsReadOnly();
            return _cached;
        }
    }
}