using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using Pikcube.Common.Code.Utility;

namespace Pikcube.Common.Code.Patches;

[HarmonyPatch(typeof(OneTimeInitialization), "ExecuteEssential")]
internal class OneTimeInitPatches
{
    internal static void Postfix()
    {
        BetterHooks.OnOneTimeInitializationFinished();
    }
}