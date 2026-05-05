using MegaCrit.Sts2.Core.Runs;

namespace Pikcube.Common.Utility;

/// <summary>
/// Interface that allows models to listen for when a new run is initialized
/// </summary>
public interface IRunInitializedListener
{
    /// <summary>
    /// Invoked immediately after a new run is either created or loaded from a save file. Used to perform custom initialization any time a game starts.
    /// </summary>
    /// <param name="runState">The current RunState</param>
    public void AfterRunInitialized(RunState runState);
}