using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using Pikcube.Common.Patches;

namespace Pikcube.Common.Vfx;

internal static class SilentExhaustVfx
{
    internal static NExhaustVfx? Create(NCard card)
    {
        SfxPatch.SilenceNext.Add(TmpSfx.cardExhaust);
        return NExhaustVfx.Create(card);
    }
}