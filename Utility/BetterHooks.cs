using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace Pikcube.Common.Utility;

public static class BetterHooks
{
    public delegate void AfterRunInitializedHandeler(RunState runState);
    public static event AfterRunInitializedHandeler? AfterRunInitialized;

    internal static void OnAfterRunInitialized(RunState runState)
    {
        AfterRunInitialized?.Invoke(runState);
    }

    public delegate void AfterRunLoadedFromSaveHandler(RunState runState, SerializableRun save);
    public static event AfterRunLoadedFromSaveHandler? AfterRunLoadedFromSave;

    internal static void OnAfterRunLoadedFromSave(RunState runState, SerializableRun save)
    {
        AfterRunLoadedFromSave?.Invoke(runState, save);
    }
    public delegate void AfterCreatingNewRunHandler(
        RunState runState,
        IReadOnlyList<Player> players,
        IReadOnlyList<ActModel> acts,
        IReadOnlyList<ModifierModel> modifiers,
        GameMode gameMode,
        int ascensionLevel,
        string seed
    );
    public static event AfterCreatingNewRunHandler? AfterCreatingNewRun;

    internal static void OnAfterCreatingNewRun(RunState runState, IReadOnlyList<Player> players, IReadOnlyList<ActModel> acts, IReadOnlyList<ModifierModel> modifiers, GameMode gameMode, int ascensionLevel, string seed)
    {
        AfterCreatingNewRun?.Invoke(runState, players, acts, modifiers, gameMode, ascensionLevel, seed);
    }
}