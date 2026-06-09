using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using Pikcube.Common.Extensions;
using Pikcube.Common.Utility;
using System.Data;

namespace Pikcube.Common.Patches;

[HarmonyPatch(typeof(Orobas), "GenerateInitialOptions")]
internal static class OrobasPatches
{
    public static bool Prefix(ref IReadOnlyList<EventOption> __result, Orobas __instance)
    {
        if (__instance.Owner is null)
        {
            return true;
        }

        CharacterModel character = __instance.Owner.Character;
        CharacterModel characterModel = __instance.Rng.NextItem(__instance.Owner.UnlockState.Characters.Where(c => c.Id != character.Id)) ?? character;

        List<EventOption> options1 = __instance.GetPrivateProperty<Orobas, List<EventOption>>("OptionPool1") ?? throw new NoNullAllowedException();

        EventOption eventOption;
        if (__instance.Rng.NextFloat() < 0.33333331346511841)
        {
            eventOption = __instance.GetPrivateProperty<Orobas, EventOption>("PrismaticGemOption") ?? throw new NoNullAllowedException();
        }
        else
        {
            SeaGlass mutable = (SeaGlass)ModelDb.Relic<SeaGlass>().ToMutable();
            mutable.CharacterId = characterModel.Id;
            eventOption = ReverseRelicOption.RelicOption(__instance, mutable);
        }
        options1.Add(eventOption);

        options1.RemoveAll(e => e.Relic is not null && !RelicSpawnManager.CanRelicSpawn(e.Relic, __instance.Owner.RunState));

        List<EventOption> options2 = __instance.GetPrivateProperty<Orobas, List<EventOption>>("OptionPool2") ?? throw new NoNullAllowedException();
        options2.RemoveAll(e => e.Relic is not null && !RelicSpawnManager.CanRelicSpawn(e.Relic, __instance.Owner.RunState));

        List<EventOption> options3 = __instance.GetPrivateProperty<Orobas, List<EventOption>>("OptionPool3") ?? throw new NoNullAllowedException();
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