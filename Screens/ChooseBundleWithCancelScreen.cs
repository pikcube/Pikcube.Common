using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;

namespace Pikcube.Common.Screens;

public partial class ChooseBundleWithCancelScreen : NChooseABundleSelectionScreen
{
    private static string ScenePath => SceneHelper.GetScenePath("/screens/card_selection/choose_a_bundle_selection_screen");

    public new static ChooseBundleWithCancelScreen ShowScreen(
        IReadOnlyList<IReadOnlyList<CardModel>> bundles)
    {
        ChooseBundleWithCancelScreen screen = PreloadManager.Cache.GetScene(ScenePath).Instantiate<ChooseBundleWithCancelScreen>();
        screen.Name = nameof(ChooseBundleWithCancelScreen);
        AccessTools.DeclaredField(typeof(NChooseABundleSelectionScreen), "_bundles").SetValue(screen, bundles);
        NOverlayStack.Instance?.Push(screen);
        return screen;
    }
}