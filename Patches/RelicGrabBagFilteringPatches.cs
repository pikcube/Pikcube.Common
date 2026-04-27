using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches;

internal static class RelicGrabBagFilteringPatches
{
    [HarmonyPatch(typeof(RelicGrabBag), nameof(RelicGrabBag.PullFromFront), typeof(RelicRarity), typeof(Func<RelicModel, bool>), typeof(IRunState))]
    internal static class PullFrontPatches
    {
        public static void Prefix(RelicRarity rarity, ref Func<RelicModel, bool> filter, IRunState runState)
        {
            Func<RelicModel, bool> original = filter;
            filter = model => original(model) && RelicSpawnManager.CanRelicSpawn(model, runState);
        }
    }

    [HarmonyPatch(typeof(RelicGrabBag), nameof(RelicGrabBag.PullFromBack), typeof(RelicRarity), typeof(Func<RelicModel, bool>), typeof(IRunState))]
    internal static class PullBackPatches
    {
        public static void Prefix(RelicRarity rarity, ref Func<RelicModel, bool> filter, IRunState runState)
        {
            Func<RelicModel, bool> original = filter;
            filter = model => original(model) && RelicSpawnManager.CanRelicSpawn(model, runState);
        }
    }
}