using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches;

[HarmonyPatch]
internal static class ModifyCardDescriptionPatch
{ 
    public static MethodBase TargetMethod()
    {
        List<MethodInfo>? methods = AccessTools.GetDeclaredMethods(typeof(CardModel));
        return methods.Single(m => m is { Name: "GetDescriptionForPile", IsPrivate: true });
    }

#pragma warning disable CA1806
    [UsedImplicitly]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher matcher = new CodeMatcher(instructions);

        matcher.MatchStartForward(CodeMatch.Calls(() => string.Join('n', Enumerable.Empty<string>())));
        matcher.InsertAndAdvance(CodeInstruction.LoadArgument(0));
        matcher.InsertAndAdvance(CodeInstruction.Call(() => ModifyListOfStrings(null!, null!)));

        return matcher.InstructionEnumeration();
    }
#pragma warning restore CA1806


    //Note that arguments will be passed in whatever order they are placed on the evaluation stack (leftmost being the bottom)
    private static IEnumerable<string> ModifyListOfStrings(IEnumerable<string> lines, CardModel instance)
    {
        List<string> newLines = [.. lines];
        BetterHooks.OnModifyCardText(instance, ref newLines);
        return newLines;
    }
}