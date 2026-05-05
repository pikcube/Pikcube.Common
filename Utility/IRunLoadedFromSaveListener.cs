using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;

namespace Pikcube.Common.Utility;

/// <summary>
/// Interface that allows models to listen for when a run is loaded from a save
/// </summary>
public interface IRunLoadedFromSaveListener
{
    /// <summary>
    /// Invoked immediately after a saved run is loaded. Used to perform additional initialization after loading a saved game but not when a new game is created.
    /// </summary>
    /// <param name="runState">The current RunState</param>
    /// <param name="save">The Seralized Save</param>
    public void AfterRunLoadedFromSave(RunState runState, SerializableRun save);
}