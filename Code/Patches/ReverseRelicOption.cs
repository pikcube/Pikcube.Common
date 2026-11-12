using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;

namespace Pikcube.Common.Code.Patches;

[HarmonyPatch]
internal static class ReverseRelicOption
{
    [HarmonyReversePatch]
    [HarmonyPatch(typeof(AncientEventModel), "RelicOption", typeof(RelicModel), typeof(string))]
    internal static EventOption RelicOption(object instance, RelicModel relic, string pageName = "INITIAL")
    {
        throw new NotImplementedException();
    }
}