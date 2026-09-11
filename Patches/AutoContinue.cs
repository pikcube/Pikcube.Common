using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.Saves;

namespace Pikcube.Common.Patches;

[HarmonyPatch(typeof(NMainMenu), "CheckCommandLineArgs")]
internal class AutoContinue
{
    public static bool IsFirst { get; set; } = true;

    public static void Postfix(NMainMenu __instance)
    {
        if (CommandLineHelper.HasArg("continue") && SaveManager.Instance.HasRunSave && IsFirst)
        {
            IsFirst = false;
            typeof(NMainMenu).DeclaredMethod("OnContinueButtonPressed").Invoke(__instance, [null]);
        }
    }
}