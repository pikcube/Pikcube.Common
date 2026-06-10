using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Events;
using Pikcube.Common.Extensions;
using Pikcube.Common.Utility;
using System.Data;

namespace Pikcube.Common.Patches;

[HarmonyPatch(typeof(Pael), "GenerateInitialOptions")]
internal static class PaelPatches
{
    public static bool Prefix(ref IReadOnlyList<EventOption> __result, Pael __instance)
    {
        if (__instance.Owner is null || __instance.AllPossibleOptions.Select(option => option.Relic).All(r => r is null || RelicSpawnManager.CanRelicSpawn(r, __instance.Owner.RunState)))
        {
            return true;
        }

        List<EventOption> options1 = __instance.GetPrivateProperty<Pael, IList<EventOption>>("OptionPool1")?.ToList() ?? throw new NoNullAllowedException();
        options1.RemoveAll(e => e.Relic is not null && !RelicSpawnManager.CanRelicSpawn(e.Relic, __instance.Owner.RunState));

        List<EventOption> options2 = __instance.GetPrivateProperty<Pael, List<EventOption>>("OptionPool2") ?? throw new NoNullAllowedException();

        if (__instance.Owner.Deck.Cards.Count(c => ModelDb.Enchantment<Goopy>().CanEnchant(c)) >= 3)
        {
            options2.Add(__instance.GetPrivateProperty<Pael, EventOption>("PaelsClawOption") ?? throw new NoNullAllowedException());
        }

        if (__instance.Owner.Deck.Cards.Count(c => c.IsRemovable) >= 5)
        {
            options2.Add(__instance.GetPrivateProperty<Pael, EventOption>("PaelsToothOption") ?? throw new NoNullAllowedException());
        }

        options2.Add(__instance.GetPrivateProperty<Pael, EventOption>("PaelsGrowthOption") ?? throw new NoNullAllowedException());
        options2.RemoveAll(e => e.Relic is not null && !RelicSpawnManager.CanRelicSpawn(e.Relic, __instance.Owner.RunState));

        List<EventOption> options3 = __instance.GetPrivateProperty<Pael, List<EventOption>>("OptionPool3") ?? throw new NoNullAllowedException();
        if (!__instance.Owner.HasEventPet())
        {
            options3.Add(__instance.GetPrivateProperty<Pael, EventOption>("PaelsLegionOption") ?? throw new NoNullAllowedException());
        }
        options3.RemoveAll(e => e.Relic is not null && !RelicSpawnManager.CanRelicSpawn(e.Relic, __instance.Owner.RunState));

        __result =
        [
            __instance.Rng.NextItem(options1)!,
            __instance.Rng.NextItem(options2)!,
            __instance.Rng.NextItem(options3)!
        ];

        return false;
    }
}