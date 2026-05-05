using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Pikcube.Common.Utility;

/// <summary>
/// Interface to allow models to listen for when a new run is created
/// </summary>
public interface ICreatingNewRunListener
{
    /// <summary>
    /// Invoked immediately after creating a new run. Used to perform additional initialization after a new run is created but not when it is loaded from a save.
    /// </summary>
    /// <param name="runState">The current RunState</param>
    /// <param name="players">All players</param>
    /// <param name="acts">All acts to play</param>
    /// <param name="modifiers">All modifiers</param>
    /// <param name="gameMode">The current game modes</param>
    /// <param name="ascensionLevel">The current ascension</param>
    /// <param name="seed">The seed</param>
    public void AfterCreatingNewRun(RunState runState, IReadOnlyList<Player> players, IReadOnlyList<ActModel> acts, IReadOnlyList<ModifierModel> modifiers, GameMode gameMode, int ascensionLevel, string seed);
}