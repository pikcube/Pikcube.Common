using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Patches;

[HarmonyPatch]
internal static class ReverseRelicOption
{
    [HarmonyReversePatch]
    [HarmonyPatch(typeof(AncientEventModel), "RelicOption", typeof(RelicModel), typeof(string), typeof(string))]
    internal static EventOption RelicOption(object instance, RelicModel relic, string pageName = "INITIAL", string? customDonePage = null)
    {
        throw new NotImplementedException();
    }
}