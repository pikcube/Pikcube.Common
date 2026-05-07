using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
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
        foreach (IRunInitializedListener listener in runState.IterateHookListeners(null).OfType<IRunInitializedListener>())
        {
            listener.AfterRunInitialized(runState);
        }
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
        foreach (IRunLoadedFromSaveListener listener in runState.IterateHookListeners(null).OfType<IRunLoadedFromSaveListener>())
        {
            listener.AfterRunLoadedFromSave(runState, save);
        }
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
        foreach (ICreatingNewRunListener listener in runState.IterateHookListeners(null).OfType<ICreatingNewRunListener>())
        {
            listener.AfterCreatingNewRun(runState, players, acts, modifiers, gameMode, ascensionLevel, seed);
        }
    }

    /// <summary>
    /// Defines a void method that acccept a card model and a HoverTipEventArgs.
    /// </summary>
    public delegate void ModifyCardHoverTipsHandler(CardModel sender, HoverTipEventArgs e);

    /// <summary>
    /// Invoked immediately after getting the hover tips for a card. Modify HoverTipEventArgs.NewHovetTips to change the return value.
    /// </summary>
    public static event ModifyCardHoverTipsHandler? ModifyCardHoverTips;

    internal static IEnumerable<IHoverTip> OnModifyCardHoverTips(CardModel cardModel, IEnumerable<IHoverTip> original)
    {
        IModifyHoverTipsListener[] listeners = cardModel.RunState?.IterateHookListeners(cardModel.CombatState).OfType<IModifyHoverTipsListener>().ToArray() ?? [];
        if (ModifyCardHoverTips is null && listeners.Length == 0)
        {
            return original;
        }

        HoverTipEventArgs args = new(original.ToArray().AsReadOnly());
        ModifyCardHoverTips?.Invoke(cardModel, args);
        foreach (IModifyHoverTipsListener listener in listeners)
        {
            listener.ModifyCardHoverTips(cardModel, args);
        }

        return args.NewHoverTips;
    }


    /// <summary>
    /// Defines a named void method
    /// </summary>
    public delegate void AfterOneTimeInitializationHandler();

    /// <summary>
    /// Invoked immediately after OneTimeInitialization finishes. Useful for code that would be in your mod initialize method, but can't be because ModelDB hasn't been initialized yet.
    /// </summary>
    public static event AfterOneTimeInitializationHandler? AfterOneTimeInitialization;

    internal static void OnOneTimeInitializationFinished()
    {
        AfterOneTimeInitialization?.Invoke();
    }

    /// <summary>
    /// Defines a named void method that accepts a NChooseACardSelectionScreen and a ModifyCardSelectionScreenTitleArgs
    /// </summary>
    public delegate void ModifyCardSelectionScreenTitleHandler(NChooseACardSelectionScreen sender, ModifyCardSelectionScreenTitleArgs e);

    /// <summary>
    /// Triggered immediately before showing a card selection screen to the player, allowing you to modify the text on the banner.
    /// </summary>
    public static event ModifyCardSelectionScreenTitleHandler? ModifyCardSelectionScreenTitle;


    internal static string OnModifyCardSelectionScreenTitle(NChooseACardSelectionScreen nChooseACardSelectionScreen, string defaultText)
    {
        ModifyCardSelectionScreenTitleArgs args = new(defaultText);
        ModifyCardSelectionScreenTitle?.Invoke(nChooseACardSelectionScreen, args);

        return args.NewText;
    }
}