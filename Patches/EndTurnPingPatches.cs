using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using System.Reflection.Emit;
using MegaCrit.Sts2.Core.Localization;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches;

[HarmonyPatch(typeof(FlavorSynchronizer), "CreateEndTurnPingDialogueIfNecessary")]
internal static class CreateEndTurnPingDialogueIfNecessaryPatch
{
    internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Newobj && //Opcode is correct
                instruction.operand is ConstructorInfo info && //Operand type is correct
                info.DeclaringType == typeof(LocString)) //ctor is correct type
            {
                yield return CodeInstruction.LoadArgument(1);

                //CustomEndTurnPingManager.GetEndTurnPingLocString(string table, string key, Player player)
                yield return CodeInstruction.Call(() => CustomEndTurnPingManager.GetEndTurnPingLocString(null!, null!, null!));
            }
            else if (instruction.opcode == OpCodes.Ldc_R8)
            {
                yield return CodeInstruction.LoadArgument(1);
                yield return instruction;
                yield return CodeInstruction.Call(() => CustomEndTurnPingManager.GetLength(null!, 0));
            }
            else
            {
                yield return instruction; //Do not modify instruction
            }


            //Add argument 1 (the current player) to the evalucation stack to it gets passed to the manager
        }
    }
}
