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

    [UsedImplicitly]
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> code = [.. instructions];
        for (int n = code.Count - 1; n >= 0; --n)
        {
            CodeInstruction ilCode = code[n];
            if (ilCode.opcode != OpCodes.Ldloc_S)
            {
                continue;
            }

            object operand = ilCode.operand;
            int? localIndex = AccessTools.DeclaredProperty(operand.GetType(), "LocalIndex").GetValue(operand) as int?;
            if (localIndex is not 5)
            {
                throw new Exception("Your patch broke, index of local variable `List<string> sources` is no longer 5.");
            }

            code.Insert(n + 1, new CodeInstruction(OpCodes.Ldarg_0));
            code.Insert(n + 2, new CodeInstruction(OpCodes.Ldloc_S, operand));
            code.Insert(n + 3, CodeInstruction.Call(() => ModifyListOfStrings(null!, null!)));
            
            break;
        }

        return code;
    }

    //Note that arguments will be passed in whatever order they are placed on the evaluation stack (leftmost being the bottom)
    private static void ModifyListOfStrings(CardModel instance, List<string> lines)
    {
        List<string> newLines = [.. lines];
        BetterHooks.OnModifyCardText(instance, ref newLines);
        lines.Clear();
        lines.AddRange(newLines);
    }
}