using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Audio.Debug;

namespace Pikcube.Common.Patches;

[HarmonyPatch(typeof(NDebugAudioManager), nameof(NDebugAudioManager.Play), typeof(string), typeof(float), typeof(PitchVariance))]
internal static class SfxPatch
{
    internal static List<string> SilenceNext { get; } = [];

    [UsedImplicitly]
    internal static void Prefix(string streamName, ref float volume)
    {
        if (!SilenceNext.Contains(streamName))
        {
            return;
        }
        volume = 0f;
        SilenceNext.Remove(streamName);
    }
}