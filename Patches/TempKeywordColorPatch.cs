using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using System.Reflection;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches;

[HarmonyPatch]
internal static class TempKeywordColorPatch
{
    internal static Dictionary<CardKeyword, string> KeywordFormats = [];

    public static MethodBase TargetMethod()
    {
        List<MethodInfo>? methods = AccessTools.GetDeclaredMethods(typeof(CardModel));
        return methods.Single(m => m is { Name: "GetDescriptionForPile", IsPrivate: true });
    }

    internal static void Prefix()
    {
        KeywordFormats.Clear();
    }

    internal static string Postfix(string __result, CardModel __instance)
    { 
        foreach (KeyValuePair<CardKeyword, string> pair in KeywordFormats)
        {
            if (!TempKeywordManager.IsTempKeyword(pair.Key, __instance))
            {
                continue;
            }

            string newString = pair.Value.Replace("gold]", "purple]");
            //string newString = pair.Value.Replace("[gold]", "[color=#aaaaee]").Replace("[/gold]", "[/color]");



            if (CardKeywordOrder.beforeDescription.Contains(pair.Key))
            {
                int pos = __result.IndexOf(pair.Value, StringComparison.Ordinal);
                if (pos < 0)
                {
                    continue;
                }
                __result = $"{__result[..pos]}{newString}{__result[(pos + pair.Value.Length)..]}";
            }
            else
            {
                int pos = __result.LastIndexOf(pair.Value, StringComparison.Ordinal);
                if (pos < 0)
                {
                    continue;
                }
                __result = $"{__result[..pos]}{newString}{__result[(pos + pair.Value.Length)..]}";
            }
        }

        KeywordFormats.Clear();
        return __result;
    }
}

[HarmonyPatch]
internal static class KeywordExtensionPatch
{
    public static MethodBase TargetMethod()
    {
        Type? type = AccessTools.TypeByName("CardKeywordExtensions");
        return AccessTools.DeclaredMethod(type, "GetCardText");
    }

    public static void Postfix(CardKeyword keyword, string __result)
    {
        TempKeywordColorPatch.KeywordFormats[keyword] = __result;
    }
}