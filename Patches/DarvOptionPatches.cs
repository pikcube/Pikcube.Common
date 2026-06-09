using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using Pikcube.Common.Utility;
using System.Data;

namespace Pikcube.Common.Patches;

[HarmonyPatch(typeof(Darv), "GenerateInitialOptions")]
internal static class DarvOptionPatches
{
    internal static bool Prefix(Darv __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (__instance.Owner is null || __instance.AllPossibleOptions.Select(option => option.Relic).All(r => r is null || RelicSpawnManager.CanRelicSpawn(r, __instance.Owner.RunState)))
        {
            return true;
        }

        List<BetterDarvRelicSet> sets = [.. MainFile.RelicSetCache.Select(set => new BetterDarvRelicSet
        {
            Relics = [.. set.Relics.Where(r => r.IsAllowed(__instance.Owner.RunState))], 
            Filter = p => set.Filter(p) && set.Relics.Length > 0
        })];
        

        List<EventOption> source = sets.Where(rs => rs.Filter(__instance.Owner))
            .Select(rs => ReverseRelicOption.RelicOption(__instance, __instance.Rng.NextItem(rs.Relics)?.ToMutable() ?? throw new NoNullAllowedException()))
            .ToList()
            .UnstableShuffle(__instance.Rng);
        DustyTome mutable = (DustyTome)ModelDb.Relic<DustyTome>().ToMutable();
        List<EventOption> list;
        if (__instance.Rng.NextBool() && mutable.IsAllowed(__instance.Owner.RunState))
        {
            list = [.. source.Take(2)];

            if (__instance.Owner != null)
            {
                mutable.SetupForPlayer(__instance.Owner);
            }

            list.Add(ReverseRelicOption.RelicOption(__instance, mutable));
        }
        else
        {
            list = [.. source.Take(3)];
        }

        __result = list;



        return false;
    }

    internal readonly struct BetterDarvRelicSet
    {
        public required Func<Player, bool> Filter { get; init; }
        public required RelicModel[] Relics { get; init; }
    }
}