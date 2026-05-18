using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using Pikcube.Common.Utility;
using FileAccess = Godot.FileAccess;

namespace Pikcube.Common.Patches;

internal static class CustomLocTablePatches
{
    [HarmonyPatch(typeof(LocManager), "ListLocalizationFiles")]
    public static class ListLocalizationFilesPatch
    {
        public static IEnumerable<string> Postfix(IEnumerable<string> __result)
        {
            return CustomLocManager.GetCustomLocTables(__result);
        }
    }

    [HarmonyPatch(typeof(LocManager), "LoadTable")]
    public static class LoadTablePatch
    {
        public static bool Prefix(string path, ref Dictionary<string, string> __result)
        {
            if (FileAccess.FileExists(path))
            {
                return true;
            }

            __result = [];
            return false;
        }
    }
}