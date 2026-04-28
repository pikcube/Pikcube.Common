using System.Data;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using System.Reflection;

namespace Pikcube.Common.Patches;

[HarmonyPatch(typeof(Darv), "GenerateInitialOptions")]
internal class DarvOptionPatches
{
    internal static bool Prefix(Darv __instance, ref IReadOnlyList<EventOption> __result)
    {
        if (__instance.Owner is null)
        {
            return true;
        }

        List<BetterDarvRelicSet> sets = MainFile.RelicSetCache.ToList();
        foreach (BetterDarvRelicSet set in sets)
        {
            set.relics = [..set.relics.Where(r => r.IsAllowed(__instance.Owner.RunState))];
            Func<Player, bool> oldFilter = set.filter;
            set.filter = p => oldFilter(p) && set.relics.Length > 0;
        }

        List<EventOption> source = sets.Where(rs => rs.filter(__instance.Owner))
            .Select(rs => ReverseRelicOption.RelicOption(__instance, (__instance.Rng.NextItem(rs.relics) ?? throw new NoNullAllowedException()).ToMutable())).ToList()
            .UnstableShuffle(__instance.Rng);
        DustyTome mutable = (DustyTome)ModelDb.Relic<DustyTome>().ToMutable();
        List<EventOption> list;
        if (__instance.Rng.NextBool() && mutable.IsAllowed(__instance.Owner.RunState))
        {
            list = source.Take(2).ToList<EventOption>();

            if (__instance.Owner != null)
                mutable.SetupForPlayer(__instance.Owner);
            list.Add(ReverseRelicOption.RelicOption(__instance, mutable));
        }
        else
        {
            list = source.Take(3).ToList();
        }

        __result = list;



        return false;
    }

    internal class BetterDarvRelicSet
    {
        public Func<Player, bool> filter { get; set; }
        public RelicModel[] relics { get; set; }
    }
}

internal class ReverseRelicOption
{
    [HarmonyReversePatch]
    [HarmonyPatch(typeof(AncientEventModel), "RelicOption")]
    internal static EventOption RelicOption(object instance, RelicModel relic, string pageName = "INITIAL", string? customDonePage = null)
    {
        throw new NotImplementedException();
    }
}