using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Utility;
using System.Reflection;

namespace Pikcube.Common.Patches;

[HarmonyPatch(typeof(MerchantPotionEntry), "FillSlot", typeof(IEnumerable<PotionModel>))]
internal static class MerchantPotionEntryPatches
{
    internal static void Prefix(MerchantPotionEntry __instance, ref IEnumerable<PotionModel> blacklist, Player ____player)
    {
        blacklist = BetterHooks.OnModifyMerchantPotionBlacklist(__instance, blacklist, ____player);
    }
}

[HarmonyPatch(typeof(MerchantPotionEntry), MethodType.Constructor, typeof(Player))]
internal static class MerchantPointEntryCtorPatches1
{
    internal static void Postfix(MerchantPotionEntry __instance, Player player)
    {
        PropertyInfo modelProperty = AccessTools.DeclaredProperty(typeof(MerchantPotionEntry), nameof(MerchantPotionEntry.Model));
        modelProperty.SetValue(__instance, BetterHooks.OnModifyMerchantPotionResult(__instance, __instance.Model, player));
    }
}

[HarmonyPatch(typeof(MerchantPotionEntry), MethodType.Constructor, typeof(PotionModel), typeof(Player))]
internal static class MerchantPointEntryCtorPatches2
{
    internal static void Postfix(MerchantPotionEntry __instance, Player player)
    {
        PropertyInfo modelProperty = AccessTools.DeclaredProperty(typeof(MerchantPotionEntry), nameof(MerchantPotionEntry.Model));
        modelProperty.SetValue(__instance, BetterHooks.OnModifyMerchantPotionResult(__instance, __instance.Model, player));
    }
}