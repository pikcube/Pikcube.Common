using System.Data;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches;

[HarmonyPatch(typeof(NChooseACardSelectionScreen), nameof(NChooseACardSelectionScreen.ShowScreen))]
internal static class ChooseACardTitlePatches
{
    public static void Postfix(NChooseACardSelectionScreen __result)
    {
        FieldInfo? bannerInfo = AccessTools.DeclaredField(typeof(NChooseACardSelectionScreen), "_banner");

        NCommonBanner banner = (NCommonBanner?)bannerInfo.GetValue(__result) ?? throw new NoNullAllowedException();

        string defaultText = new LocString("gameplay_ui", "CHOOSE_CARD_HEADER").GetRawText();

        banner.label.SetTextAutoSize(BetterHooks.OnModifyCardSelectionScreenTitle(__result, defaultText));
    }
}