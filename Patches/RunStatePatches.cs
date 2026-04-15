using HarmonyLib;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using JetBrains.Annotations;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Pikcube.Common.Utility;

namespace Pikcube.Common.Patches;


public static class RunStatePatches
{
    [HarmonyPatch(typeof(RunState), "FromSerializable", MethodType.Normal)]
    public static class RunStateLoadPatches
    {
        [UsedImplicitly]
        public static RunState Postfix(RunState __result, SerializableRun save)
        {
            BetterHooks.OnAfterRunInitialized(__result);
            BetterHooks.OnAfterRunLoadedFromSave(__result, save);
            return __result;
        }
    }

    [HarmonyPatch(typeof(RunState), "CreateForNewRun", MethodType.Normal)]
    public static class RunStateCreatePatches
    {
        [UsedImplicitly]
        public static RunState Postfix(RunState __result, IReadOnlyList<Player> players,
            IReadOnlyList<ActModel> acts,
            IReadOnlyList<ModifierModel> modifiers,
            GameMode gameMode,
            int ascensionLevel,
            string seed)
        {
            BetterHooks.OnAfterRunInitialized(__result);
            BetterHooks.OnAfterCreatingNewRun(__result, players, acts, modifiers, gameMode, ascensionLevel, seed);
            return __result;
        }
    }
}