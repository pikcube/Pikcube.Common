using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches;

[HarmonyPatch(typeof(OneTimeInitialization), "ExecuteDeferred")]
internal class OneTimeInitPatches
{
    internal static void Postfix()
    {
        BetterHooks.OnOneTimeInitializationFinished();
    }
}