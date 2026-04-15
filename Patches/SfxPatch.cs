using HarmonyLib;
using MegaCrit.Sts2.Core.Audio.Debug;

namespace Pikcube.Common.Patches;

[HarmonyPatch(typeof(NDebugAudioManager), nameof(NDebugAudioManager.Play), MethodType.Normal)]
public static class SfxPatch
{
    public static List<string> SilenceNext { get; } = [];

    public static void Prefix(string streamName, ref float volume, PitchVariance variance)
    {
        if (!SilenceNext.Contains(streamName))
        {
            return;
        }
        volume = 0f;
        SilenceNext.Remove(streamName);
    }
}