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
        if (__instance.Owner is null || __instance.AllPossibleOptions.Select(option => option.Relic).All(r => r is null || RelicSpawnManager.CanRelicSpawn(r, __instance.Owner.RunState)))
        {
            return true;
        }

        PrivatePropertyWrapper<Orobas, List<EventOption>> optionPool1 = __instance.PrivatePropertyWrapper<Orobas, List<EventOption>>("OptionPool1");

        CharacterModel character = __instance.Owner.Character;
        CharacterModel characterModel = __instance.Rng.NextItem(__instance.Owner.UnlockState.Characters.Where(c => c.Id != character.Id)) ?? character;

        EventOption eventOption;
        if (__instance.Rng.NextFloat() < 0.33333331346511841)
        {
            eventOption = __instance.PrivatePropertyWrapper<Orobas, EventOption>("PrismaticGemOption").Value ?? throw new NoNullAllowedException();
        }
        else
        {
            SeaGlass mutable = (SeaGlass)ModelDb.Relic<SeaGlass>().ToMutable();
            mutable.CharacterId = characterModel.Id;
            eventOption = ReverseRelicOption.RelicOption(__instance, mutable);
        }
        optionPool1.Value!.Add(eventOption);

        optionPool1.Value!.RemoveAll(e => e.Relic is not null && !RelicSpawnManager.CanRelicSpawn(e.Relic, __instance.Owner.RunState));

        List<EventOption> options2 = __instance.PrivatePropertyWrapper<Orobas, List<EventOption>>("OptionPool2").Value ?? throw new NoNullAllowedException();
        options2.RemoveAll(e => e.Relic is not null && !RelicSpawnManager.CanRelicSpawn(e.Relic, __instance.Owner.RunState));

        List<EventOption> options3 = __instance.PrivatePropertyWrapper<Orobas, List<EventOption>>("OptionPool3").Value ?? throw new NoNullAllowedException();
        options3.RemoveAll(e => e.Relic is not null && !RelicSpawnManager.CanRelicSpawn(e.Relic, __instance.Owner.RunState));


        __result =
        [
            __instance.Rng.NextItem(optionPool1.Value!)!,
            __instance.Rng.NextItem(options2)!,
            __instance.Rng.NextItem(options3)!
        ];

        return false;
    }
}