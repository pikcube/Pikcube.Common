using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace Pikcube.Common.Utility;

/// <summary>
/// Additional hooks that can be subscribed to.
/// </summary>
public static class BetterHooks
{
    /// <summary>
    /// Defines a void method that accepts the current RunState as an argument.
    /// </summary>
    public delegate void AfterRunInitializedHandeler(RunState runState);
    /// <summary>
    /// Invoked immediately after a new run is either created or loaded from a save file. Used to perform custom initialization any time a game starts.
    /// </summary>
    public static event AfterRunInitializedHandeler? AfterRunInitialized;

    internal static void OnAfterRunInitialized(RunState runState)
    {
        AfterRunInitialized?.Invoke(runState);
    }

    /// <summary>
    /// Defines a void method that accepts the current RunState and a SerializableRun
    /// </summary>
    public delegate void AfterRunLoadedFromSaveHandler(RunState runState, SerializableRun save);
    /// <summary>
    /// Invoked immediately after a saved run is loaded. Used to perform additional initialization after loading a saved game but not when a new game is created.
    /// </summary>
    public static event AfterRunLoadedFromSaveHandler? AfterRunLoadedFromSave;

    internal static void OnAfterRunLoadedFromSave(RunState runState, SerializableRun save)
    {
        AfterRunLoadedFromSave?.Invoke(runState, save);
    }
    /// <summary>
    /// Defines a void method that accepts a RunState, a list of Players, a list of Acts, a list of Modifiers, the current GameMode, the current ascension, and the current seed.
    /// </summary>
    public delegate void AfterCreatingNewRunHandler(
        RunState runState,
        IReadOnlyList<Player> players,
        IReadOnlyList<ActModel> acts,
        IReadOnlyList<ModifierModel> modifiers,
        GameMode gameMode,
        int ascensionLevel,
        string seed
    );
    /// <summary>
    /// Invoked immediately after creating a new run. Used to perform additional initialization after a new run is created but not when it is loaded from a save.
    /// </summary>
    public static event AfterCreatingNewRunHandler? AfterCreatingNewRun;

    internal static void OnAfterCreatingNewRun(RunState runState, IReadOnlyList<Player> players, IReadOnlyList<ActModel> acts, IReadOnlyList<ModifierModel> modifiers, GameMode gameMode, int ascensionLevel, string seed)
    {
        AfterCreatingNewRun?.Invoke(runState, players, acts, modifiers, gameMode, ascensionLevel, seed);
    }
}