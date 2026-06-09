using System.Data;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Events;
using Pikcube.Common.Extensions;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches;

[HarmonyPatch(typeof(Tezcatara), "GenerateInitialOptions")]
internal static class TezcataraPatches
{
    public static bool Prefix(ref IReadOnlyList<EventOption> __result, Tezcatara __instance)
    {
        if (__instance.Owner is null || __instance.AllPossibleOptions.Select(option => option.Relic).All(r => r is null || RelicSpawnManager.CanRelicSpawn(r, __instance.Owner.RunState)))
        {
            return true;
        }

        List<EventOption> options1 = __instance.GetPrivateProperty<Tezcatara, List<EventOption>>("OptionPool1") ?? throw new NoNullAllowedException();
        if (__instance.Owner.Deck.Cards.Any(c => c.Tags.Contains(CardTag.Strike) && c.Rarity == CardRarity.Basic))
        {
            options1.Add(__instance.GetPrivateProperty<Tezcatara, EventOption>("NutritiousSoupOption") ?? throw new NoNullAllowedException());
        }

        options1.RemoveAll(e => e.Relic is not null && !RelicSpawnManager.CanRelicSpawn(e.Relic, __instance.Owner.RunState));

        List<EventOption> options2 = __instance.GetPrivateProperty<Tezcatara, List<EventOption>>("OptionPool2") ?? throw new NoNullAllowedException();
        options2.RemoveAll(e => e.Relic is not null && !RelicSpawnManager.CanRelicSpawn(e.Relic, __instance.Owner.RunState));
        
        List<EventOption> options3 = __instance.GetPrivateProperty<Tezcatara, List<EventOption>>("OptionPool3") ?? throw new NoNullAllowedException();
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